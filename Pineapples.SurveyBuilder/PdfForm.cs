﻿using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Layout;
using iText.Kernel.Pdf;
using System.Windows;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Geom;
using iText.IO.Font.Constants;

namespace surveybuilder
{

	public class FormValue
	{
		public FormValue(string path, string value)
		{
			string[] parts = path.Split(new char[] { '.', '!' });
			Name = parts[parts.Length - 1];
			ParentPath = String.Join(".", parts, 0, parts.Length - 1);
			Path = path;
			Value = value;
		}
		public FormValue(string path) : this(path, null)
		{

		}

		public string Name;                // the truncted part of the name
		public string Value;               // the form field value
		public string Path;                // the full path through the hierarchy - this is uniqu
		public string ParentPath;
	}

	/// <summary>
	/// Read a form within a PDF document
	/// </summary>
	public class PdfForm
	{
		public PdfForm(string fileName)
		{
			FileName = fileName;
			try
			{

				if (System.IO.Path.GetExtension(fileName).ToLower() == ".xfdf")
				{
					importXfdf(fileName);
				}
				else
				{
					PdfDocument pdfDoc = new PdfDocument(new PdfReader(fileName));
					PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, false);
					// test for empty
					// prep the itext fields into the internal, simpler format.
					// note that this what what itext tries to do in the Xfdf classes
					// but there are problems...
					IDictionary<String, PdfFormField> FieldsIn = form.GetAllFormFields();

					if (FieldsIn == null)
					{
						throw new Exception("PDF File contains no interactive form fields - it may have been \"flattened\"");
					}
					foreach (var fld in FieldsIn.Values)
					{
						// this unicode char may appear on fields that have been pushed in to the pdf
						// it makes a numeric non-convertible if we do not remove
						//fld.SetValue(fld.GetValue()?.ToString()?.Replace("\uFEFF", ""));
					}
					Fields = new Dictionary<String, FormValue>();
					foreach (var fld in FieldsIn)
					{
						string value = fld.Value.GetValueAsString();
						String key = fld.Key;
						while (!String.IsNullOrEmpty(key) && !Fields.ContainsKey(key))
						{
							FormValue formValue = new FormValue(key, value);
							Fields.Add(key, formValue);
							key = formValue.ParentPath;
							value = "";
						}
					}
				}

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private PdfForm(Dictionary<string, FormValue> fields, string PdfFileName)
		{
			Fields = fields;
			FileName = PdfFileName;

		}
		/// <summary>
		/// File name of the loaded pdf
		/// </summary>
		public string FileName { get; private set; }

		public Dictionary<string, FormValue> Fields { get => fields; private set => fields = value; }

		/// <summary>
		/// get a field from the form, using the fully qualified 'path'
		/// </summary>
		/// <param name="path"></param>
		public FormValue GetField(string path)
		{
			return Fields[path];
		}

		private FormValue[] GetChildren(string parentPath)
		{
			return Fields.Values.Where(fld => fld.ParentPath == parentPath).ToArray();
		}

		/// <summary>
		/// return true if the loaded pdf contains form fields
		/// </summary>
		/// <returns></returns>
		public Boolean IsForm()
		{
			if (Fields == null)
			{
				return false;
			}
			return (Fields.Count > 0);
		}

		#region xfdf export
		static XNamespace xfdfnamespace = "http://ns.adobe.com/xfdf/";
		private Dictionary<string, FormValue> fields;

		// Generate the Xfdf file from the Fields collection
		// This assumes that the fields collection is propertly hierarchical ; ie the field names do not implicitly 
		// imply a hierarchy by having embedded dots.
		// If the source pdf or source xfdf has problem, it is sorted out on loading, when the Feidls collection is created.
		public XDocument Xfdf()
		{

			XDocument xfdf = XDocument.Parse($"<xfdf xmlns=\"{xfdfnamespace}\"/>");
			XElement fieldselement = new XElement(xfdfnamespace + "fields");
			xfdf.Root.Add(fieldselement);

			AddField(null, Fields, fieldselement);

			// exit
			return xfdf;
		}

		// recursively add a <field> node to the Xfdf
		private static void AddField(string currentPath, IDictionary<string, FormValue> fields, XElement parentnode)
		{
			if (String.IsNullOrEmpty(currentPath))
			{
				// top level elements
				foreach (var ff in fields.Where(xx => String.IsNullOrEmpty(xx.Value.ParentPath)))
				{
					AddField(ff.Value.Name, fields, parentnode);
				}
				return;
			}

			FormValue currentField = fields[currentPath];
			XElement fieldnode = new XElement(xfdfnamespace + "field");

			fieldnode.SetAttributeValue("name", currentField.Name);

			if (!String.IsNullOrEmpty(currentField.Value))
			{
				string value = currentField.Value;
				XElement valuenode = new XElement(xfdfnamespace + "value", value);
				fieldnode.Add(valuenode);
			}
			else
			{

				foreach (var ff in fields.Where(xx => xx.Value.ParentPath == currentPath))
				{
					AddField(ff.Key, fields, fieldnode);
				}
			}
			// only branches ending with a <value> will get exported
			//if (fieldnode.DescendantNodes().Count() > 0)
			{
				parentnode.Add(fieldnode);
			}

		}
		#endregion

		#region xfdf import

		private void importXfdf(string filename)
		{
			XfdfDocument xfdf = new XfdfDocument(XDocument.Load(filename));
			importXfdf(xfdf);

		}
		private void importXfdf(XfdfDocument xfdf)
		{
			var fields = new Dictionary<string, FormValue>();

			foreach (var child in xfdf.FieldsNode.Elements(XfdfDocument.xfdfNamespace + "field"))
			{
				addNode(child, String.Empty, fields);
			}
			Fields = fields;

		}
		// these 2 routines will recursevely move through field nodes of the xfdf being impported
		// to create the Fields collection, correctly setting the parentIndex.
		// As well to cater for files that where the field collection is flat - and the fields hierarchy is represented
		// by dots in the file name (e.g. Enrol.Data.R08.C05.M) such names are split up to create fields at each level
		// As a workaround for hyperref problem with flat names, we now split either by . or ! See issue 1396

		private void addNode(XElement node, string parentPath, Dictionary<string, FormValue> fields)
		{
			string name = node.Attribute("name").Value;
			string value = node.Elements(XfdfDocument.xfdfNamespace + "value").FirstOrDefault()?.Value;


			string[] parts = name.Split(new[] { '.', '!' }, 2);

			string path = (!String.IsNullOrEmpty(parentPath) ? $"{parentPath}.{parts[0]}" : parts[0]);

			parentPath = addNode(name, value, parentPath, fields);
			foreach (var child in node.Elements(XfdfDocument.xfdfNamespace + "field"))
			{
				addNode(child, parentPath, fields);
			}
		}
		private string addNode(string name, string value, string parentPath, Dictionary<string, FormValue> fields)
		{

			string[] parts = name.Split(new[] { '.', '!' }, 2);
			string path = (!String.IsNullOrEmpty(parentPath) ? $"{parentPath}.{parts[0]}" : parts[0]);

			var currentfld = fields.Where(x => x.Value.Path == path).Count();
			if (currentfld > 0)
			{
				// use it as the parent from here
				parentPath = path;
			}
			else
			{
				var fld = new FormValue(path);
				fld.Value = (parts.Length == 1) ? value : null; // only put the value on the terminating node
				fields.Add(path, fld);

				parentPath = path;
			}
			if (parts.Length > 1)
			{
				// if there is a '.' in name, process the part after the first dot
				parentPath = addNode(parts[1], value, parentPath, fields);
			}
			return parentPath;
		}

		#endregion

		#region Appearance creation utilities

		/// <summary>
		///  to be determined best place to put all this.....
		/// </summary>
		/// <returns></returns>
		private static Color DefaultBackColor()
		{
			return new DeviceRgb((float)253 / 256, (float)245 / 256, (float)230 / 256);
		}
		private static Color DefaultDownBackColor()
		{
			return new DeviceRgb((float)160 / 256, (float)160 / 256, (float)160 / 256);
		}

		/// <summary>
		/// Create a bordered checkbox, containing a symbol in a given font (usually Dingbats)
		/// </summary>
		/// <param name="pdfDoc">the host pdfDocument</param>
		/// <param name="charFont">the font (usually Dingbats</param>
		/// <param name="symbol">symbol to centre in the checkbox</param>
		/// <param name="color">color of the symbol</param>
		/// <returns></returns>
		public static PdfFormXObject CheckSymbol(PdfDocument pdfDoc, PdfFont charFont, string symbol, Color color)
		{
			return DrawCheckAppearance(pdfDoc, charFont, symbol, color, DefaultBackColor());
		}
		public static PdfFormXObject CheckSymbolDown(PdfDocument pdfDoc, PdfFont charFont, string symbol)
		{
			return DrawCheckAppearance(pdfDoc, charFont, symbol, ColorConstants.BLACK, DefaultDownBackColor());
		}

		/// <summary>
		/// Create a bordered checkbox, containing a solid colored rectangle
		/// </summary>
		/// <param name="pdfDoc">the host pdfDocument</param>
		/// <param name="color">color of the rectangular block</param>
		/// <returns></returns>
		public static PdfFormXObject CheckBlock(PdfDocument pdfDoc, Color blockcolor)
		{

			return DrawBlockAppearance(pdfDoc, blockcolor, DefaultBackColor());
		}
		public static PdfFormXObject CheckBlockDown(PdfDocument pdfDoc)
		{

			return DrawBlockAppearance(pdfDoc, ColorConstants.BLACK, DefaultDownBackColor());
		}

		public static PdfFormXObject CheckEmpty(PdfDocument pdfDoc)
		{

			return DrawCheckAppearance(pdfDoc, null, String.Empty, ColorConstants.BLACK, DefaultBackColor());
		}
		public static PdfFormXObject CheckEmptyDown(PdfDocument pdfDoc)
		{

			return DrawCheckAppearance(pdfDoc, null, String.Empty, ColorConstants.BLACK, DefaultDownBackColor());
		}


		private static PdfFormXObject DrawCheckAppearance(PdfDocument pdfDoc, PdfFont charFont, string symbol, Color color, Color backcolor)
		{
			// Create the form XObject (appearance stream)
			PdfFormXObject appearance = new PdfFormXObject(new Rectangle(0, 0, 20, 20));
			PdfCanvas canvas = new PdfCanvas(appearance, pdfDoc);

			iText.Kernel.Colors.Color borderColor = ColorConstants.GRAY;


			// Set the border color and draw the border (rectangle)
			canvas.SetLineWidth(3)                    // Set border thickness
				  .SetStrokeColor(borderColor)         // Set the color of the border
				  .SetFillColor(backcolor)
				  .Rectangle(0, 0, 20, 20)             // Draw a rectangle (border)
				  .FillStroke();                           // Apply the stroke (border)

			// Now draw the dingbat symbol
			// Set the font, size, and color for the symbol
			if (symbol.Length > 0)
			{

				canvas.BeginText()
					  .SetFontAndSize(charFont, 18)       // Set font size
					  .SetColor(color, true)                 // Set the color of the text
					  .MoveText(3, 4)                        // Adjust positioning as needed
					  .ShowText(symbol)                      // Write the dingbat symbol
					  .EndText();
			}
			return appearance;
		}
		private static PdfFormXObject DrawBlockAppearance(PdfDocument pdfDoc, Color color, Color backcolor)
		{
			// Create the form XObject (appearance stream)
			PdfFormXObject appearance = new PdfFormXObject(new Rectangle(0, 0, 20, 20));
			PdfCanvas canvas = new PdfCanvas(appearance, pdfDoc);

			iText.Kernel.Colors.Color borderColor = ColorConstants.GRAY;


			// Set the border color and draw the border (rectangle)
			canvas.SetLineWidth(3)                    // Set border thickness
				  .SetStrokeColor(borderColor)         // Set the color of the border
				  .SetFillColor(backcolor)       //old lace
				  .Rectangle(0, 0, 20, 20)             // Draw a rectangle (border)
				  .FillStroke()
				  .SetFillColor(color)
				  .Rectangle(5, 5, 10, 10)             // Draw a rectangle (border)
				  .Fill();                           // Apply the stroke (border)
			return appearance;
		}

		public static PdfDictionary CheckMK(string symbol)
		{
			iText.Kernel.Colors.Color borderColor = ColorConstants.GRAY;
			PdfDictionary mk = new PdfDictionary();
			mk.Put(PdfName.CA, new PdfString(symbol));
			mk.Put(PdfName.BC, new PdfArray(borderColor.GetColorValue()));
			return mk;
		}

		#endregion

		#region Fix up utilities

		/// <summary>
		/// After importing an Xfdf file, the checkboxes appear to lose their custom 
		/// styling. This routing replaces it. This is copied from the routine used 
		/// in pdfSurveyController.Generate for the same purpose.
		/// There, it is critical to maintain the desired visual presention
		/// after populating a form. Here, it is used only by the loadXfdf
		/// routine in Toolbox.
		/// </summary>
		/// <param name="pdfDocument"></param>
		/// <returns></returns>
		public static PdfDocument StandardizeCheckboxes(PdfDocument pdfDocument)
		{
			// make the default Y and N appearances
			// Load ZapfDingbats font for the symbols
			PdfFont dingbatFont = PdfFontFactory.CreateFont(StandardFonts.ZAPFDINGBATS);


			var tickAppearance = PdfForm.CheckSymbol(pdfDocument, dingbatFont
									, PdfDingbat.Tick, new DeviceRgb(0, 128, 0)); // Tick (✓) - green
			var crossAppearance = PdfForm.CheckSymbol(pdfDocument, dingbatFont
									, PdfDingbat.Cross, new DeviceRgb(128, 0, 0));   // Cross (✗) - red
			var uncheckedAppearance = PdfForm.CheckEmpty(pdfDocument); // Empty square

			// Down (/D) versions
			var tickAppearanceDown = PdfForm.CheckSymbolDown(pdfDocument, dingbatFont, PdfDingbat.Tick); // Tick (✓) - green
			var crossAppearanceDown = PdfForm.CheckSymbolDown(pdfDocument, dingbatFont, PdfDingbat.Cross);
			var uncheckedAppearanceDown = PdfForm.CheckEmptyDown(pdfDocument); // Empty squa

			var tickMK = PdfForm.CheckMK(PdfDingbat.Tick);
			var crossMK = PdfForm.CheckMK(PdfDingbat.Cross);
			var blockMK = PdfForm.CheckMK(PdfDingbat.Square);

			tickAppearance.MakeIndirect(pdfDocument);
			crossAppearance.MakeIndirect(pdfDocument);
			uncheckedAppearance.MakeIndirect(pdfDocument);
			tickAppearanceDown.MakeIndirect(pdfDocument);
			crossAppearanceDown.MakeIndirect(pdfDocument);
			uncheckedAppearanceDown.MakeIndirect(pdfDocument);

			// BLOCKS

			PdfFormXObject GoldBlockAppearance = PdfForm.CheckBlock(pdfDocument, new DeviceRgb(255, 215, 0));   // 
			PdfFormXObject BlackBlockAppearance = PdfForm.CheckBlock(pdfDocument, new DeviceRgb(0, 0, 0));
			PdfFormXObject GreyBlockAppearance = PdfForm.CheckBlock(pdfDocument, new DeviceRgb(96, 96, 96));
			PdfFormXObject RedBlockAppearance = PdfForm.CheckBlock(pdfDocument, new DeviceRgb(128, 0, 0));
			PdfFormXObject BlockAppearanceDown = PdfForm.CheckBlockDown(pdfDocument);

			GoldBlockAppearance.MakeIndirect(pdfDocument);
			BlackBlockAppearance.MakeIndirect(pdfDocument);
			GreyBlockAppearance.MakeIndirect(pdfDocument);
			RedBlockAppearance.MakeIndirect(pdfDocument);
			BlockAppearanceDown.MakeIndirect(pdfDocument);


			PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDocument, true);
			var ifielddic = form.GetAllFormFields();
			foreach (string n in ifielddic.Keys)
			{
				PdfFormField f = ifielddic[n];

				if (f is PdfButtonFormField btn)
				{
					if ((btn.GetFieldFlags() & 0x10000) == 0) // not a push button
					{
						string[] apstates = btn.GetAppearanceStates();

						foreach (var widg in btn.GetWidgets())
						{
							//SizeWidget(widg, 13, 13);

							PdfDictionary appearanceDict = widg.GetAppearanceDictionary();
							// Check if the appearance dictionary has normal appearances (default visual state)
							PdfDictionary currentNormal = widg.GetNormalAppearanceObject();
							PdfDictionary normalAppearance = new PdfDictionary();
							PdfDictionary downAppearance = new PdfDictionary();

							PdfDictionary mk = new PdfDictionary();


							{
								//	some GOOD FAIR POOR appear as G F P
								//	Resource provision NONE SOME..... ALL
								//	Make sure the blocks are all correcllty colored
								//	

								foreach (PdfName key in currentNormal.KeySet())
								{
									string keyValue = key.GetValue();

									switch (keyValue)
									{
										case "Off":
											normalAppearance.Put(key, uncheckedAppearance.GetPdfObject());
											break;
										case "Y":
											normalAppearance.Put(key, tickAppearance.GetPdfObject());
											break;
										case "N":
											normalAppearance.Put(key, crossAppearance.GetPdfObject());
											break;

										// GOOD FAIR POOR // G F P // NONE SOME ALLL...
										// note FAIR, SOME etc are all Grey Blocks (default)
										case "G":
										case "GOOD":
										case "ALL":
											normalAppearance.Put(key, GoldBlockAppearance.GetPdfObject());
											break;
										case "P":
										case "POOR":
										case "NONE":
											normalAppearance.Put(key, RedBlockAppearance.GetPdfObject());
											break;
										case "F":
										case "FAIR":
											normalAppearance.Put(key, GreyBlockAppearance.GetPdfObject());
											break;
										default:
											//everything else is a grey block
											normalAppearance.Put(key, GreyBlockAppearance.GetPdfObject());
											break;
									}
									// treat down appearance
									switch (keyValue)
									{
										case "Off":
											downAppearance.Put(key, uncheckedAppearanceDown.GetPdfObject());
											break;

										case "Y":
											downAppearance.Put(key, tickAppearanceDown.GetPdfObject());
											mk = tickMK;
											break;
										case "N":
											downAppearance.Put(key, crossAppearanceDown.GetPdfObject());
											mk = crossMK;
											break;

										default:
											// all of these blocks, whatever colour, use the same down state
											downAppearance.Put(key, BlockAppearanceDown.GetPdfObject());
											mk = blockMK;
											break;
									}

								}
								// appply the new appearances to the widget
								appearanceDict.Put(PdfName.N, normalAppearance);
								appearanceDict.Put(PdfName.D, downAppearance);      // for now make the Down appearance the same
								widg.SetAppearanceCharacteristics(mk);
							}
						}
					}
				}
			}
			return pdfDocument;
		}

		#endregion
	}
}

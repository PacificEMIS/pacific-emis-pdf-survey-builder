using iText.Forms.Fields.Properties;
using iText.Forms.Fields;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Forms;
using iText.Forms.Form.Element;
using static iText.IO.Codec.TiffWriter;
using static surveybuilder.CellMakers;
using surveybuilder.Utilities;
using Org.BouncyCastle.Tls.Crypto.Impl;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;

namespace surveybuilder
{
	public class ExpectedStaff
	{
		PdfStylesheet ss;
		PdfStyle ThinHeader;
		PdfStyle ThinHeaderLeft;
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Expected Staff");
			// Import common table styles
			ss = builder.stylesheet;
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			ss.Add("colheaderthin", ThinHeader = new PdfStyle(ss["colheader"])
			{
				FontBold = false,
				FontSize = 8,
				Height = 12
			});

			// left text
			ss.Add("tleft", ThinHeaderLeft = new PdfStyle(ss["colheaderthin"])
			{
				TextAlignment = TextAlignment.LEFT
			}
			);

			document.Add(new Paragraph()
				.Add(@"This list shows all the teachers recorded at your school in the last survey you submitted. "
				+ @"As well, it includes all teachers appointed to teach at your school in the current year. "
				+ @"To complete this table, answer Y in the On Staff? column to confirm that the teacher is working at your school. "
				+ @"Answer N otherwise. For teachers who are On Staff, review the remaining fields, and make any corrections required.")
			);

			document.Add(new Paragraph()
				.Add(@"If there are teachers at your school who are not in this list, add their details in the next section – New Staff.")
			);

			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model12 = CellStyleFactory.TwoColumn;
			var model13 = CellStyleFactory.ThreeColumn;
			var model15 = CellStyleFactory.FiveColumn;
			var model21 = CellStyleFactory.TwoRowOneColumn;

			var colwidths = new float[] { 40, 60, 60, 10, 10, 30, 10, 10 };
			var subcolwidths = new float[] { 40, 60, 40, 60 };

			PdfAction onOnStaffChange = PdfAction.CreateJavaScript("actions.onOnStaffChange(event);");

			const int MaxTeachers = 80;
			for (int i = 0; i <= MaxTeachers; i++)
			{
				builder.NewPageIf(document, (i % 4 == 3));

				Table table = CellStyleFactory.DefaultTable(colwidths);
				table.AddHeaderRow(ThinHeader,
					TextCell(model, "")
					, TextCell(model, "")
					, TextCell(model, "")
					, TextCell(model12, "Gender")
					, TextCell(model, "")
					, TextCell(model12, "On Staff")
				);
				table.AddHeaderRow(ThinHeader,
					TextCell(model, "Payroll No").Style(ThinHeaderLeft)
					, TextCell(model, "Family Name").Style(ThinHeaderLeft)
					, TextCell(model, "Given Name").Style(ThinHeaderLeft)

					, TextCell(model, "M")
					, TextCell(model, "F")
					, TextCell(model, "DoB")
					, TextCell(model, "Y")
					, TextCell(model, "N")
				);

				var grpOnStaff = new RadioFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.OnStaff")
					.CreateRadioGroup();

				var grpGender = new RadioFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.Gender")
					.CreateRadioGroup();
				table.AddRow(
					InputCell(model, $"TL.{i:00}.PayrollNo", 10),
					InputCell(model, $"TL.{i:00}.FamilyName", 30),
					InputCell(model, $"TL.{i:00}.FirstName", 30),
					SelectCell(model, grpGender, "M"),
					SelectCell(model, grpGender, "F"),
					InputCell(model, $"TL.{i:00}.DoB", 10),
					YesCell(model, grpOnStaff),
					NoCell(model, grpOnStaff)
				);

				Table subtable = CellStyleFactory.DefaultTable(subcolwidths)
					.SetMarginBottom(0);

				subtable.AddRow(
					TextCell(model, "Citizenship"),
					ComboCell(model, $"TL.{i:00}.Citizenship", builder.lookups.Opt("nationalities")),
					TextCell(model, "Home Island"),
					ComboCell(model, $"TL.{i:00}.HomeIsland", builder.lookups.Opt("islands"))
					);

				subtable.AddRow(
					TextCell(model, "Qualification"),
					ComboCell(model, $"TL.{i:00}.Qual", builder.lookups.Opt("qualN")),
					TextCell(model, "Ed Qual"),
					ComboCell(model, $"TL.{i:00}.QualEd", builder.lookups.Opt("qualEd"))
					);

				subtable.AddRow(
					TextCell(model, "Class Taught (Min):"),
					ComboCell(model, $"TL.{i:00}.Class.Min", builder.lookups.Opt("classLevels")),
					TextCell(model, "Class Taught (Max):"),
					ComboCell(model, $"TL.{i:00}.Class.Max", builder.lookups.Opt("classLevels"))
					);
				subtable.AddRow(
					TextCell(model, "Role:"),
					ComboCell(model, $"TL.{i:00}.Role", builder.lookups.Opt("filteredRoles")),
					TextCell(model, "Duties:"),
					ComboCell(model, $"TL.{i:00}.Duties", builder.lookups.Opt("classLevels"))
					);

				table.AddRow(ElementCell(model13, subtable).Style(ss["abstractcell"]));
				document.Add(table);

				foreach (var w in grpOnStaff.GetWidgets())
				{
					w.SetAdditionalAction(PdfName.U, onOnStaffChange);
				}
				// now we also have to create the hidden field for tID on every teacher instance
				// like the Grid, if we create a form field and don;t place it on the page, it will
				// exists somewhere in the ether not visible
			}
			var form = builder.GetPdfAcroForm();
			for (int i = 0; i <= MaxTeachers; i++)
			{
				var txt = new TextFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.tID")
				.SetWidgetRectangle(new Rectangle(10,10,10,10))
				.CreateText();

				txt.GetWidgets()[0].SetFlag(PdfAnnotation.HIDDEN);
				form.AddField(txt);
			}


			return document;
		}
	}
}

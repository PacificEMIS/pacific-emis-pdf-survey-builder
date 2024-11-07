using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Geom;
using iText.Layout.Renderer;
using Colors = iText.Kernel.Colors;
using Borders = iText.Layout.Borders;
using iText.Kernel.Pdf.Action;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Forms.Form.Element;
using System.Reflection;
using System.CodeDom;

namespace surveybuilder
{
	public static class PdfBuilders
	{

		/// <summary>
		/// Return a non-terminal named from all but the last part of the field name
		/// e.g. Enrol.D.00.05.M will return nonterminal Enrol.D.00.05
		/// supported by the full hierarchy:
		/// Enrol.D.00
		/// Enrol.D
		/// Enrol
		/// If the result is applied as the parent to a field named M, its name after will be
		/// Enrol.D.00.00.M
		/// HP = HierarchyPrepare
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parent"></param>
		/// <param name="pdfDoc"></param>
		/// <returns></returns>
		/// not needed or used
		public static PdfFormField HP(string name, PdfDocument pdfDoc)
		{
			if (name == String.Empty)
			{
				return null;
			}

			PdfFormField parent;
			PdfAcroForm form = PdfFormCreator.GetAcroForm(pdfDoc, true);

			if (form.GetAllFormFields().TryGetValue(name, out parent))
			{
				if (parent != null)
				{
					return parent;
				}
			}
			string[] parts = name.Split(new[] { '.', '!' });

			string parentName = String.Join(".", parts, 0, parts.Length - 1);
			string currentPartial = parts[parts.Length - 1];
			PdfFormField tmp = new NonTerminalFormFieldBuilder(pdfDoc, currentPartial)
					.CreateNonTerminalFormField();
			parent = HP(parentName, pdfDoc);
			if (parent != null)
			{
				tmp.SetParent(parent);
			}
			form.AddField(tmp);
			return tmp;
		}

		// rename a field - not needed
		public static PdfFormField RF(string oldName, string newName, PdfDocument pdfDoc)
		{
			if (oldName == String.Empty)
			{
				return null;
			}
			var form = PdfFormCreator.GetAcroForm(pdfDoc, true);
			var fields = form.GetAllFormFields();
			var oldfield = form.GetField(oldName);
			PdfFormField newfield = oldfield;
			newfield.SetFieldName(newName);
			form.RemoveField(oldName);
			form.AddField(newfield);
			return newfield;
		}
	}
}

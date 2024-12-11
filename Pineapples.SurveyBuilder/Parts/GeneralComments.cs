using iText.Forms.Fields;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;
using iText.Forms;
using iText.Forms.Form.Element;
using static iText.IO.Codec.TiffWriter;
using iText.Forms.Fields.Properties;
using static surveybuilder.CellMakers;
using surveybuilder.Utilities;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Commons.Utils;

namespace surveybuilder
{

	public class GeneralComments
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public GeneralComments() { }

		public Document Build(PdfBuilder builder, Document document, PdfOutline generalOutline)
		{
			Console.WriteLine("Part: General Comments");

			// Cell layout/styling models
			var model = CellStyleFactory.Default;

			builder.AddOutline(document, generalOutline, "Final Comments");
			builder.Heading_2("Final Comments", document);

			document.Add(new Paragraph(@"If you would like to make any comments or provide additional information about your "
			+ @"school please record these below."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1 }))
						.UseAllAvailableWidth();

			table.AddCell(ts.TableHeaderStyle(InputCell(new Cell().SetHeight(400), "Survey.Comment")));

			document.Add(table);
			builder.NewPage(document);


			builder.AddOutline(document, generalOutline, "Certification");
			builder.Heading_2("Certification", document);

			table = new Table(UnitValue.CreatePercentArray(new float[] { 100 }))
				.UseAllAvailableWidth();

			string jsCode = "v.doAllValidations();";
			Cell abs = ts.AbstractCell(new Cell().SetHeight(50));
			table.AddRow(
				PushButtonCell(abs, "CheckBtn", "Verify Responses", jsCode)
			);

			document.Add(table);

			document.Add(new Paragraph(@"All information presented in this survey must be complete and accurate and "
			+ @"you must certify to that it is complete and accurate to the best of your knowledge and belief."));



			Table tableCertification = new Table(UnitValue.CreatePercentArray(new float[] { 60, 40 }))
						.UseAllAvailableWidth();

			PdfButtonFormField grp = new RadioFormFieldBuilder(builder.pdfDoc, "Survey.PrincipalCertification")
				.CreateRadioGroup();



			tableCertification.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Principal’s Name:")),
				InputCell(model, "Survey.PrincipalName")
			);
			tableCertification.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Signature:")),
				InputCell(model, "Survey.PrincipalSignature")
			);

			tableCertification.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("I certify information presented is accurate and complete:")),
				YesCell(model, grp)
			);
			tableCertification.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Date Completed:")),
				DateCell(model, "Survey.DateCompleted")
			);

			document.Add(tableCertification);



			return document;
		}
	}
}

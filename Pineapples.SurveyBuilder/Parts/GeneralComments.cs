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

namespace surveybuilder
{

	public class GeneralComments
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public GeneralComments() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{

			var model = new Cell()
				.SetHeight(20)
				.SetVerticalAlignment(VerticalAlignment.MIDDLE)
				.SetHorizontalAlignment(HorizontalAlignment.CENTER);

			builder.Heading_2("Final Comments", document);
			document.Add(new Paragraph(@"If you would like to make any comments or provide additional information about your "
			+ @"school please record these below."));

			Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1 }))
						.UseAllAvailableWidth();

			table.AddCell(ts.TableHeaderStyle(InputCell(new Cell().SetHeight(400), "Survey.Comment")));

			document.Add(table);
						
			builder.Heading_2("Certification", document);

			Table tableCertification = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 }))
						.UseAllAvailableWidth();

			PdfButtonFormField grp = new RadioFormFieldBuilder(builder.pdfDoc, "Survey.PrincipalCertification")
				.CreateRadioGroup();

			tableCertification.AddRow(
				ts.TableHeaderStyle(TextCell(model, "Certification Details")),
				ts.TableHeaderStyle(TextCell(model, "Answer"))
			);
			tableCertification.AddRow(
				TextCell(model, "Principal’s Name:"),
				InputCell(model, "Survey.PrincipalName")
			);
			tableCertification.AddRow(
				TextCell(model, "Signature:"),
				InputCell(model, "Survey.PrincipalSignature")
			);

			tableCertification.AddRow(
				TextCell(model, "I certify that the information presented is accurate and complete*:"),
				YesCell(model, grp)
			);
			tableCertification.AddRow(
				TextCell(model, "Date Completed:"),
				DateCell(model, "Survey.DateCompleted")
			);

			document.Add(tableCertification);

			document.Add(new Paragraph(@"* All information presented in this survey is accurate and complete to the best "
			+ @"of my knowledge and belief."));

			return document;
		}

	}
}

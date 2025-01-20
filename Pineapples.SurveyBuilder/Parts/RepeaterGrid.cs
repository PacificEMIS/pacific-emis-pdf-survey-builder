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
using iText.Kernel.Colors;
using static System.Net.Mime.MediaTypeNames;
using surveybuilder.Utilities;
using iText.Kernel.Pdf;


namespace surveybuilder
{
	public class RepeaterGrid
	{
		public RepeaterGrid()
		{

		}
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Repeater Grid");
			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			Table tbl = CellStyleFactory.DefaultTable(160, 20, 20, 20, 20);

			PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, "Rep.HasData")
				.CreateRadioGroup();

			var model = CellStyleFactory.Default;
			tbl.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Are any pupils at your school repeating last year's class level?")),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Yes"))),
				YesCell(model, rgrp),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("No"))),
				NoCell(model, rgrp)
			);
			document.Add(tbl);

			document.Add(new Paragraph(@"If YES, For each class, record the number of pupils who were enrolled in the same class in the previous school year. "
			+ @"Record the repeating students by their age as at 31 March this year."));

			GenderedGridmaker grd = new GenderedGridmaker();
			grd.Rows = builder.lookups["ages"];
			grd.Columns = builder.lookups["classLevels"];

			grd.Tag = "Rep";
			document.Add(grd.Make(builder, document));

			RequiredFields flds = new RequiredFields("Repeaters",
				"Repeater certification not set."
			);
			flds.Add("Rep.HasData");
			ValidationManager.AddRequiredFields(document.GetPdfDocument(), flds);
			ConditionalFields cflds = new ConditionalFields("Repeaters",
				"Repeater data is not complete."
			);
			cflds.Add(ConditionalField.IfYes("Rep.HasData", "Rep.T.T.T.T"));
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), cflds); 
			return document;
		}
	}
}

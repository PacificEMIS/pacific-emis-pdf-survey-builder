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
using System.Runtime.InteropServices;


namespace surveybuilder
{
	public class BoarderGrid
	{
		public BoarderGrid()
		{

		}
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Boarder Grid");
			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			Table tbl = CellStyleFactory.DefaultTable(160, 20, 20, 20, 20);

			PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, "Brd.HasData")
				.CreateRadioGroup();

			var model = CellStyleFactory.Default;
			tbl.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Does your school have boarders?")),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Yes"))),
				YesCell(model, rgrp),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("No"))),
				NoCell(model, rgrp)
			);
			document.Add(tbl);

			document.Add(new Paragraph(@"If YES, for each class, record the number of boarding pupils:"));

			GenderedGridmaker grd = new GenderedGridmaker();

			var rows = new LookupList();
			rows.Add(new LookupEntry("BRD", "Boarders"));
			grd.Rows = rows;

			grd.Columns = builder.lookups["classLevels"];

			grd.Tag = "Brd";
			document.Add(grd.Make(builder, document));

			//validation
			RequiredFields flds = new RequiredFields("Boarders",
				"Boarder certification not set."
			);
			flds.Add("Brd.HasData");
			ValidationManager.AddRequiredFields(document.GetPdfDocument(), flds);
			ConditionalFields cflds = new ConditionalFields("Boarders",
				"Boarder data is not complete."
			);
			cflds.Add(ConditionalField.IfYes("Brd.HasData", "Brd.T.T.T.T"));
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), cflds); 

			return document;
		}
	}
}

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
	public class TransfersInGrid
	{
		public TransfersInGrid()
		{

		}
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Transfers In Grid");
			// Import common table styles
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			Table tbl = CellStyleFactory.DefaultTable(160, 20, 20, 20, 20);

			PdfButtonFormField rgrp = new RadioFormFieldBuilder(builder.pdfDoc, "TRIN.HasData")
				.CreateRadioGroup();

			var model = CellStyleFactory.Default;
			tbl.AddRow(
				TextCell(model, ts.TableRowHeaderStyle("Did any pupils transfer to your school from other islands?")),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("Yes"))),
				YesCell(model, rgrp),
				ts.TableHeaderStyle(TextCell(model, ts.TableHeaderStyle("No"))),
				NoCell(model, rgrp)
			);
			document.Add(tbl);

			document.Add(new Paragraph(@"If YES, Record the number of pupils that transferred in from another island"
				+ @" at the beginning of the school year."));
			GenderedGridmaker grd = new GenderedGridmaker();
			grd.Rows = builder.lookups["islands"];
			grd.Columns = builder.lookups["classLevels"];

			grd.Tag = "TRIN";
			document.Add(grd.Make(builder, document));
			return document;
		}
	}
}

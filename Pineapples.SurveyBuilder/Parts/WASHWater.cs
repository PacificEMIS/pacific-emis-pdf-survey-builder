using iText.Forms.Fields;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Colors;
using iText.Forms;
using iText.Forms.Fields.Properties;

namespace surveybuilder
{
	public class WASHWater
	{
		public WASHWater() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{

			document.Add(builder.Heading_3("Water Source"));

			document.Add(new Paragraph()
				.Add(@"On the following scale, rate the adequacy of your water supply for pupils and staff in relationship "
				+ @"to the standard you would like to be able to provide. Tick the category which describes it best.")
			);

			string[] Columns = { "Excellent", "Very Good", "Satisfactory", "Poor", "Very Bad" };
			int[] values = { 4, 3, 2, 1, 0 };

			var chk = new CheckBoxPickmaker();
			chk.Names = Columns;
			chk.Values = values;
			chk.Types = new CheckBoxType[] {CheckBoxType.CHECK, CheckBoxType.SQUARE
					,CheckBoxType.CIRCLE,CheckBoxType.DIAMOND,CheckBoxType.CROSS };
			chk.Tag = "Wash.Water.Rating";
			chk.Make(builder, document);

			document.Add(builder.Heading_3("Drinking Water"));

			document.Add(new Paragraph()
				.Add(@"What is the primary source of drinking water in the school?.")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Rainwater", "Groundwater", "Desalinated water", "Bottled water" };
			chk.Values = new int[] { 1, 2, 3, 4 };
			chk.Tag = "Wash.Water.Source";
			chk.Make(builder, document);

			document.Add(new Paragraph()
				.Add(@"For drinking water, what type of water treatment is practiced at the school? (select one only)")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Boiling", "Chlorination", "SODIS", "No water treatment" };
			chk.Values = new int[] { 1, 2, 3, 4 };
			chk.Tag = "Wash.Water.Treatment";
			chk.Make(builder, document);			

			return document;
		}
	}
}

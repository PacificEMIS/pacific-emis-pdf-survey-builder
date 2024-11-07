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
	public class Wash
	{
		public Wash() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{

			document.Add(builder.Heading_4("Water Source"));

			string prompt = @"On the following scale, rate the adequacy 
of your water supply for pupils and staff in relationship to the standard you would like to be able to provide. Tick the category which describes it best";
			document.Add(new Paragraph(prompt));
			string[] Columns = { "Excellent", "Very Good", "Satisfactory", "Poor", "Very Bad" };
			int[] values = { 4, 3, 2, 1, 0 };

			var chk = new CheckBoxPickmaker();
			chk.Names = Columns;
			chk.Values = values;
			chk.Types = new CheckBoxType[] {CheckBoxType.CHECK, CheckBoxType.SQUARE
					,CheckBoxType.CIRCLE,CheckBoxType.DIAMOND,CheckBoxType.CROSS };
			chk.Tag = "Wash.Water.Rating";
			chk.Make(builder, document);

			document.Add(builder.Heading_4("Drinking Water"));

			prompt = "What is the primary source of drinking water in the school?";

			document.Add(new Paragraph(prompt));
			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Rainwater", "Groundwater", "Desalinated water", "Bottled water" };
			chk.Values = new int[] { 1, 2, 3, 4 };
			chk.Tag = "Wash.Water.Source";
			chk.Make(builder, document);


			prompt = "For drinking water, what type of water treatment is practiced at the school? (select one only";
			document.Add(new Paragraph(prompt));

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Boiling", "Chlorination", "SODIS", "No water treatment" };
			chk.Values = new int[] { 1, 2, 3, 4 };
			chk.Tag = "Wash.Water.Treatment";
			chk.Make(builder, document);

			// Toilets
			builder.NewPage(document);
			//builder.SetPageHeader("left", "wash: Toilets");

			document.Add(builder.Heading_4("Toilets"));

			prompt = "What type of handwashing station is provided at the school";
			document.Add(new Paragraph(prompt));

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Stand tap", "Tippy Tap", "Piped Water", "Basin With Tap", "None" };
			chk.Values = new int[] { 1, 2, 3, 4, 0 };
			chk.Types = new CheckBoxType[] {CheckBoxType.SQUARE, CheckBoxType.SQUARE
					,CheckBoxType.SQUARE,CheckBoxType.SQUARE,CheckBoxType.CROSS };
			chk.Tag = "Wash.Handwashing.Type";
			chk.Make(builder, document);

			prompt = "Are there both water and soap available at the handwashing facilities";
			document.Add(new Paragraph(prompt));

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Soap and water", "Water only", "Soap Only", "Neither" };
			chk.Values = new int[] { 1, 2, 3, 0 };
			chk.Tag = "Wash.Handwashing.SoapAndWater";
			chk.Make(builder, document);

			return document;
		}
	}
}

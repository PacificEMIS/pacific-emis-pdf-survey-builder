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
	public class WASHHygiene
	{
		public WASHHygiene() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{			
			document.Add(new Paragraph()
				.Add(@"What type of handwashing station is provided at the school.")
			);

			var chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Stand tap", "Tippy Tap", "Piped Water", "Basin With Tap", "None" };
			chk.Values = new int[] { 1, 2, 3, 4, 0 };
			chk.Types = new CheckBoxType[] {CheckBoxType.SQUARE, CheckBoxType.SQUARE
					,CheckBoxType.SQUARE,CheckBoxType.SQUARE,CheckBoxType.CROSS };
			chk.Tag = "Wash.Handwashing.Type";
			chk.Make(builder, document);

			document.Add(new Paragraph()
				.Add(@"Are there both water and soap available at the handwashing facilities.")
			);

			chk = new CheckBoxPickmaker();
			chk.Names = new string[] { "Soap and water", "Water only", "Soap Only", "Neither" };
			chk.Values = new int[] { 1, 2, 3, 0 };
			chk.Tag = "Wash.Handwashing.SoapAndWater";
			chk.Make(builder, document);

			return document;
		}
	}
}

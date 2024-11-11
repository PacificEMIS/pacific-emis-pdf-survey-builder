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
	public class WASHSanitation
	{
		public WASHSanitation() { }

		public Document Build(KEMIS_PRI_Builder builder, Document document)
		{

			// Toilets
			document.Add(builder.Heading_3("Toilets"));

			document.Add(new Paragraph()
				.Add(@"On the following scale, rate the adequacy of your water supply for pupils and staff in relationship "
				+ @"to the standard you would like to be able to provide. Tick the category which describes it best.")
			);


			return document;
		}
	}
}

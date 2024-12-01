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

namespace surveybuilder
{
	public class Dormitories
	{
		// Import common table styles
		PdfTableStylesheet ts = new PdfTableStylesheet();
		public Dormitories()
		{

		}
		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Dormitories");
			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model1_20 = CellStyleFactory.CreateCell(1, 20);
			var model12 = CellStyleFactory.TwoColumn;
			var model21 = CellStyleFactory.TwoRowOneColumn;

			// Classrooms
			document.Add(new Paragraph()
				.Add(@"Record the details of the dormitories at your school (if any).  ")
			);

			var widths = new float[] {  4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
			Table table = new Table(UnitValue.CreatePercentArray(widths))
						.UseAllAvailableWidth();

			table.AddRow(
				ts.TableHeaderStyle(TextCell(model21, ts.TableHeaderStyle("Boarding Facilities"))),
				ts.TableHeaderStyle(TextCell(model1_20, ts.TableHeaderStyle("Dormitory No")))
			);

			// dorm numbers
			for (int i = 1; i <= 10; i++)
			{
				table.AddRow(
					ts.TableHeaderStyle(TextCell(model12, ts.TableHeaderStyle($"{i}")))
				);
			}


			List<PdfButtonFormField> grps = new List<PdfButtonFormField>();
			Dictionary<string, List<PdfButtonFormField>> DormRGrps = new Dictionary<string, List<PdfButtonFormField>>();

			string[] ynrows = new string[] { "Door", "Window", "WChair", "Laundry" };

			foreach (string rowId in ynrows)
			{
				grps = new List<PdfButtonFormField>();
				for (int i = 0; i <= 9; i++)
				{
					grps.Add(new RadioFormFieldBuilder(builder.pdfDoc, $"Room.Dorm.D.{i:00}.{rowId}")
					.CreateRadioGroup()
					);
				}
				DormRGrps.Add(rowId, grps);
			}

			YnRow(table, "Secure Doors (YN)", DormRGrps["Door"]);
			YnRow(table, "Secure Windows (YN)", DormRGrps["Window"]);
			YnRow(table, "Wheelchair Accessible?", DormRGrps["WChair"]);
			NumberRow(table, "Capacity M", "Room.Dorm.D.{0:00}.Capacity.M");
			NumberRow(table, "Capacity F", "Room.Dorm.D.{0:00}.Capacity.F");
			NumberRow(table, "No of Toilets M", "Room.Dorm.D.{0:00}.Toilets.M");
			NumberRow(table, "No of Toilets F", "Room.Dorm.D.{0:00}.Toilets.F");
			NumberRow(table, "No of Showers M", "Room.Dorm.D.{0:00}.Showers.M");
			NumberRow(table, "No of Showers F", "Room.Dorm.D.{0:00}.Showers.F");

			YnRow(table, "Laundry (Y/N)", DormRGrps["Laundry"]);

			document.Add(table);

			return document;
		}

		private Table YnRow(Table table, string rowLabel, List<PdfButtonFormField> grps)
		{
			var model = CellStyleFactory.Default;
			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle(rowLabel))
				);
			for (int i = 0; i <= 9; i++)
			{
				table.AddRow(
					YesCell(model, grps[i]),
					NoCell(model, grps[i])
					);
			}

			return table;
		}

		private Table NumberRow(Table table, string rowLabel, string nameTemplate)
		{
			var model = CellStyleFactory.Default;
			var model12 = CellStyleFactory.TwoColumn;
			table.AddRow(
				TextCell(model, ts.TableRowHeaderStyle(rowLabel))
				);
			for (int i = 0; i <= 9; i++)
			{
				string fieldName = string.Format(nameTemplate, i);
				table.AddRow(
					NumberCell(model12,fieldName)
				);
			}

			return table;
		}
	}
}

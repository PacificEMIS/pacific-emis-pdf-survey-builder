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
using surveybuilder.Utilities;
using Org.BouncyCastle.Tls.Crypto.Impl;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Forms.Form.Renderer;
using System.Web.UI.WebControls;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace surveybuilder
{

	public class ExpectedStaff
	{

		const int MaxTeachers = 80;

		PdfStylesheet ss;
		PdfStyle ThinHeader;
		PdfStyle ThinHeaderLeft;

		PdfStyle ThinData;


		RequiredFields requireds = new RequiredFields("Staff",
			"Repeater data is not complete. Review now?"
		);

		ConditionalFields condOnStaff = new ConditionalFields("OnStaff",
				"On Staff must be entered for all expected staff. Review now?"
		);
		ConditionalFields condTeachers = new ConditionalFields("Teachers",
				"Teacher information is missing some required values. Review now?"
		)
		{
			Filter = "isReadWrite"      // only check familyName fields that are readwrite
		};
		ConditionalFields condDuties = new ConditionalFields("TeacherDuties",
			"Teacher class levels not recorded. Review now?"
		)
		{
			Filter = "isReadWrite"      // only check Duties fields that are readwrite
		};

		public Document Build(PdfBuilder builder, Document document)
		{
			Console.WriteLine("Part: Expected Staff");
			// Import common table styles
			ss = builder.stylesheet;
			PdfTableStylesheet ts = new PdfTableStylesheet(builder.stylesheet);
			ss.Add("colheaderthin", ThinHeader = new PdfStyle(ss["colheader"])
			{
				FontBold = false,
				FontSize = 8,
				Height = 12
			});

			// left text
			ss.Add("tleft", ThinHeaderLeft = new PdfStyle(ss["colheaderthin"])
			{
				TextAlignment = TextAlignment.LEFT
			});

			ss.Add("tdata", ThinData = new PdfStyle(ss["tablebase"])
			{
				TextAlignment = TextAlignment.LEFT,
				FontSize = 10,
				Height = 18

			});

			document.Add(new Paragraph()
				.Add(@"This list shows all the teachers recorded at your school in the last survey you submitted. "
				+ @"As well, it includes all teachers appointed to teach at your school in the current year. "
				+ @"To complete this table, answer Y in the On Staff? column to confirm that the teacher is working at your school. "
				+ @"Answer N otherwise. For teachers who are On Staff, review the remaining fields, and make any corrections required.")
			);

			document.Add(new Paragraph()
				.Add(@"If there are teachers at your school who are not in this list, add their details in the next section – New Staff.")
			);

			// Cell layout/styling models
			var model = CellStyleFactory.Default;
			var model12 = CellStyleFactory.TwoColumn;
			var model13 = CellStyleFactory.ThreeColumn;
			var model14 = CellStyleFactory.FourColumn;
			var model15 = CellStyleFactory.FiveColumn;
			var model21 = CellStyleFactory.TwoRowOneColumn;
			var model31 = CellStyleFactory.ThreeRowOneColumn;

			var colwidths = new float[] { 40, 60, 60, 10, 10, 30, 10, 10 };
			var subcolwidths = new float[] { 40, 20, 20, 20, 5, 20, 20, 20, 20, 20, 20, 10 };
			int dutiesLength = builder.lookups["classLevels"].Count + 2;
			float[] subsubcolwidths = Enumerable.Repeat<float>(10f, dutiesLength).ToArray();

			PdfAction onOnStaffChange = PdfAction.CreateJavaScript("actions.onOnStaffChange(event);");

			const int MaxTeachers = 80;
			for (int i = 0; i < MaxTeachers; i++)
			{
				condOnStaff.Add(ConditionalField.IfAny($"TL.{i:00}.tID", $"TL.{i:00}.OnStaff"));
				// expand this to make any field required on the existence of a teacher
				condTeachers.Add(ConditionalField.IfAny(
					$"TL.{i:00}.FamilyName",
					new string[] { $"TL.{i:00}.FirstName", $"TL.{i:00}.Gender", $"TL.{i:00}.Duties" }
				));
				// if duties T or Mixed, must record the class levels
				condDuties.Add(
					new ConditionalField($"TL.{i:00}.Duties")
					{
						Value = new string[] { "T", "M" },
						Rq = new string[] { "TL.{i:00}.Class.Min", "TL.{i:00}.Class.Max" }
					}
				);

				const int teachersPerPage = 2;
				builder.NewPageIf(document, ((i % teachersPerPage) == teachersPerPage - 1));

				Table table = CellStyleFactory.DefaultTable(colwidths);
				table.AddHeaderRow(ThinHeader,
					TextCell(model, "")
					, TextCell(model, "")
					, TextCell(model, "")
					, TextCell(model12, "Gender")
					, TextCell(model, "")
					, TextCell(model12, "On Staff")
				);
				table.AddHeaderRow(ThinHeader,
					TextCell(model, "Payroll No").Style(ThinHeaderLeft)
					, TextCell(model, "Family Name").Style(ThinHeaderLeft)
					, TextCell(model, "Given Name").Style(ThinHeaderLeft)

					, TextCell(model, "M")
					, TextCell(model, "F")
					, TextCell(model, "DoB")
					, TextCell(model, "Y")
					, TextCell(model, "N")
				);

				var grpOnStaff = new RadioFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.OnStaff")
					.CreateRadioGroup();
				grpOnStaff.SetAlternativeName("Is this teacher working at your school this year?");

				var grpGender = new RadioFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.Gender")
					.CreateRadioGroup();

				var grpFP = new RadioFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.FP")
					.CreateRadioGroup();
				grpFP.SetAlternativeName("Does this teacher work full-time or part-time this year?");

				var grpTAM = new RadioFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.Duties")
					.CreateRadioGroup();
				grpTAM.SetAlternativeName("Teachers duties:\nT = teaching only\nA = administration only\nM = mixed; some teaching and some admin");

				var grpHouse = new RadioFormFieldBuilder(builder.pdfDoc, $"TL.{i:00}.House")
					.CreateRadioGroup();
				grpHouse.SetAlternativeName("Is this teacher provided with a house?");

				table.AddRow(
					InputCell(model, $"TL.{i:00}.PayrollNo", 10),
					InputCell(model, $"TL.{i:00}.FamilyName", 30),
					InputCell(model, $"TL.{i:00}.FirstName", 30),
					SelectCell(model, grpGender, "M"),
					SelectCell(model, grpGender, "F"),
					DateCell(model, $"TL.{i:00}.DoB"),
					YesCell(model, grpOnStaff),
					NoCell(model, grpOnStaff)
				);

				// details in sub table
				Table subtable = CellStyleFactory.DefaultTable(subcolwidths)
					.SetMarginBottom(0);

				// holds the duties array as copied from the Census workbook sheet
				Table subsubTable = CellStyleFactory.DefaultTable(subsubcolwidths)
					.SetMarginBottom(0);

				for (int idx = 0; idx < builder.lookups["classLevels"].Count; idx++)
				{
					subsubTable
							.AddRow(ThinHeader,
								TextCell(model, builder.lookups["classLevels"][idx].N)
							);
				}
				subsubTable.AddRow(ThinHeader,
					TextCell(model, "Admin"),
					TextCell(model, "Other")
					);
				// number cells per class level	
				for (int idx = 0; idx < builder.lookups["classLevels"].Count; idx++)
				{
					subsubTable
						.AddRow(ThinData,
							NumberCell(model, $"TL.{i:00}.Class.A.{idx:00}"));
				}
				subsubTable.AddRow(ThinData,
					NumberCell(model, $"TL.{i:00}.Class.A.A"),
					NumberCell(model, $"TL.{i:00}.Class.A.X")
					);
				//CheckBox cells per class level
				PdfButtonFormField grp;
				for (int idx = 0; idx < dutiesLength; idx++)
				{
					string id;
					if (idx == dutiesLength)
					{
						id = $"TL.{i:00}.Class.X.X";
					}
					if (idx == dutiesLength - 1)
					{
						id = $"TL.{i:00}.Class.X.A";
					}
					else
					{
						id = $"TL.{i:00}.Class.X.{idx:00}";
					}
					grp = new RadioFormFieldBuilder(builder.pdfDoc, id)
							.CreateRadioGroup();
					subsubTable
							.AddRow(ThinData,
						CellMakers.CheckCell(model, grp, "X", CheckBoxType.SQUARE));
					
				}

				// Role and TAM - most important
				subtable.AddRow(ThinHeader,
					TextCell(model21, "Role"),
					TextCell(model13, ""),
					TextCell(model, ""),
					TextCell(model, "T"),
					TextCell(model, "A"),
					TextCell(model, "M"),
					TextCell(model, "F/T"),
					TextCell(model, "P/T"),
					TextCell(model12, "days / week")

					);
				subtable.AddRow(ThinData,

					ComboCell(model13, $"TL.{i:00}.Role", builder.lookups.Opt("filteredRoles")),
					TextCell(model, ""),
					SelectCell(model, grpTAM, "T"),
					SelectCell(model, grpTAM, "A"),
					SelectCell(model, grpTAM, "M"),
					SelectCell(model, grpFP, "FT"),
					SelectCell(model, grpFP, "PT"),
					NumberCell(model12, $"TL.{i:00}.Days")
					);

				// class levels
				subtable.AddRow(ThinHeader,
					TextCell(model31, "Levels Taught"),
					TextCell(model13, "From:"),
					TextCell(model, ""),
					TextCell(model13, "To"),
					TextCell(model14, "")
					);

				subtable.AddRow(ThinData,

					ComboCell(model13, $"TL.{i:00}.Class.Min", builder.lookups.Opt("classLevels")),
					TextCell(model, ""),
					ComboCell(model13, $"TL.{i:00}.Class.Max", builder.lookups.Opt("classLevels")),
					TextCell(model14, "")
					);

				// Activities array
				Cell activities = new Cell(1, subtable.GetNumberOfColumns() - 1);
				subtable.AddRow(ElementCell(activities, subsubTable).Style(ss["abstractcell"]));

				// above Qualifications, put the labels 'Education' 'General'
				subtable.AddRow(ThinHeader,
					TextCell(model21, "Qualification"),
					TextCell(model13, "Education"),
					TextCell(model, ""),
					TextCell(model13, "General"),
					TextCell(model14, "Last Inservice  year/topic")
					);

				subtable.AddRow(ThinData,
					//TextCell(model, "Qualification"),
					ComboCell(model13, $"TL.{i:00}.Qual", builder.lookups.Opt("qualN")),
					TextCell(model, ""),
					ComboCell(model13, $"TL.{i:00}.QualEd", builder.lookups.Opt("qualEd")),
					NumberCell(model, $"TL.{i:00}.InService.Year"),
					TextCell(model, "Topic"),
					InputCell(model13, $"TL.{i:00}.InService.Topic", 20)
					);

				
				//////////// in service
				//////////subtable.AddRow(
				//////////	TextCell(model, "Last Inservice"),
				//////////	NumberCell(model13, $"TL.{i:00}.InService.Year"),
				//////////	TextCell(model, "Topic"),
				//////////	InputCell(model13, $"TL.{i:00}.InService.Topic", 20),
				//////////	TextCell(model14, "")
				//////////);

				// Employment
				subtable.AddRow(ThinHeader,
					TextCell(model21, "Employment"),
					TextCell(model13, "Salary Paid By"),
					TextCell(model, ""),
					TextCell(model13, "Status"),
					TextCell(model14, "Year started teaching")
					);
				subtable.AddRow(ThinData,

					ComboCell(model13, $"TL.{i:00}.PaidBy", builder.lookups.Opt("nationalities")),
					TextCell(model, ""),
					ComboCell(model13, $"TL.{i:00}.EmpStatus", builder.lookups.Opt("nationalities")),
					NumberCell(model12, $"TL.{i:00}.YearStarted"),
					TextCell(model12, "")
				);

				////// Employment
				////subtable.AddRow(
				////	TextCell(model, "FP"),
				////	SelectCell(model, grpFP, "FT"),
				////	SelectCell(model, grpFP, "PT"),
				////	TextCell(model, ""),
				////	TextCell(model, "Days"),
				////	NumberCell(model13, $"TL.{i:00}.Days"),
				////	TextCell(model14, "")
				////);
				// demographic
				// Employment
				subtable.AddRow(ThinHeader,
					TextCell(model21, "Citizenship"),
					TextCell(model13, "Citizenship"),
					TextCell(model, ""),
					TextCell(model13, "Home Island"),
					TextCell(model14, "")
					);
				subtable.AddRow(ThinData,

					ComboCell(model13.SetHeight(16), $"TL.{i:00}.Citizenship", builder.lookups.Opt("nationalities")),
					TextCell(model.SetHeight(16), ""),
					ComboCell(model13.SetHeight(16), $"TL.{i:00}.HomeIsland", builder.lookups.Opt("islands")),
					TextCell(model14.SetHeight(16), "")
					);

				// family status
				subtable.AddRow(ThinHeader,
					TextCell(model21, "Family"),
					TextCell(model13, "Marital Status"),
					TextCell(model, ""),
					TextCell(model13, "Dependant children"),
					TextCell(model14, "House Provided")
					);

				subtable.AddRow(ThinData,

					ComboCell(model13.SetHeight(18), $"TL.{i:00}.MaritalStatus", builder.lookups.Opt("nationalities")),
					TextCell(model.SetHeight(18), ""),
					NumberCell(model13.SetHeight(18), $"TL.{i:00}.Dep"),
					YesCell(model.SetHeight(18), grpHouse),
					NoCell(model.SetHeight(18), grpHouse),
					TextCell(model12.SetHeight(18), "")
				);

				table.AddRow(ElementCell(CellStyleFactory.SixColumn, subtable).Style(ss["abstractcell"]));
				document.Add(table);

				foreach (var w in grpOnStaff.GetWidgets())
				{
					w.SetAdditionalAction(PdfName.U, onOnStaffChange);
				}
				// now we also have to create the hidden field for tID on every teacher instance
				// like the Grid, if we create a form field and don;t place it on the page, it will
				// exists somewhere in the ether not visible
				ExportValue(builder.pdfDoc, $"TL.{i:00}.tID", "");
			}
			condOnStaff.GenerateJavaScript(document.GetPdfDocument());
			condTeachers.GenerateJavaScript(document.GetPdfDocument());
			condDuties.GenerateJavaScript(document.GetPdfDocument());
			return document;
		}
	}
}

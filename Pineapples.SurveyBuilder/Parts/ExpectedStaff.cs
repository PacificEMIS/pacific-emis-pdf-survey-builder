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
using iText.Layout.Borders;

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
			"Repeater data is not complete."
		);

		ConditionalFields condOnStaff = new ConditionalFields("OnStaff",
				"On Staff must be entered for all expected staff."
		)
		{
			Filter = "isExpectedStaff"      // check that OnStaff is entered for all expected staff
		};
		ConditionalFields condExpectedStaff = new ConditionalFields("Expected Staff",
				"Teacher information is missing some required values."
		)
		{
			Filter = "isExpectedStaff"      // when OnStaff = Y, check that all required fields are entered
		};
		ConditionalFields condNewStaff = new ConditionalFields("New Staff",
				"Teacher information is missing some required values."
		)
		{
			Filter = "isNewStaff"      // only check familyName fields that are not null in new staff
		};
		ConditionalFields condActivitiesExpected = new ConditionalFields("Expected Staff Duties",
			"Teacher activities (class levels taught, Admin, or Other) are not recorded."
		)
		{
			Filter = "isExpectedStaff"      // only check Activities fields in expected staff with OnStaff=Y
		};

		ConditionalFields condActivitiesNew = new ConditionalFields("New Staff Duties",
		"Teacher activities (class levels taught, Admin, or Other) are not recorded."
	)
		{
			Filter = "isNewStaff"      // check activities of new staff
		};
		ConditionalFields condFPDays = new ConditionalFields("FullTime part Time",
		"For part-time teachers, enter the equivalent number of full days worked."
	)
		{
			Filter = "isTeacherOnStaff"      // check FP status of all active staff
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

			// shorthand
			LookupList classLevels = builder.lookups["classLevels"];

			var colwidths = new float[] { 40, 60, 60, 10, 10, 30, 10, 10 };
			var subcolwidths = new float[] { 40, 20, 20, 20, 5, 20, 20, 20, 20, 20, 20, 10 };
			int dutiesLength = classLevels.Count + 2;
			float[] subsubcolwidths = Enumerable.Repeat<float>(10f, dutiesLength).ToArray();

			PdfAction onOnStaffChange = PdfAction.CreateJavaScript("actions.onOnStaffChange(event);");

			const int MaxTeachers = 80;
			for (int i = 0; i < MaxTeachers; i++)
			{



				const int teachersPerPage = 3;
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
					.SetMarginBottom(0)
					.SetMarginTop(0);

				//subsubTable
				//	.AddRow(ThinHeader,
				//			TextCell(CellStyleFactory.CreateCell(1, subsubTable.GetNumberOfColumns()),
				//				"Classes Taught/Tasks Performed (select all that apply)")
				//);
				for (int idx = 0; idx < classLevels.Count; idx++)
				{
					subsubTable
							.AddRow(ThinHeader,
								TextCell(model, classLevels[idx].N)
							);
				}
				subsubTable.AddRow(ThinHeader,
					TextCell(model, "Admin"),
					TextCell(model, "Other")
					);
				//CheckBox cells per class level
				PdfButtonFormField grp;
				List<string> activitiesList = new List<string>();


				for (int idx = 0; idx < dutiesLength; idx++)
				{

					string id;
					string tooltip;
					if (idx == dutiesLength - 1)
					{
						id = $"TL.{i:00}.Activity.X";
						tooltip = "Other duties";
					}
					else
					if (idx == dutiesLength - 2)
					{
						id = $"TL.{i:00}.Activity.A";
						tooltip = "Admin duties";
					}
					else
					{
						id = $"TL.{i:00}.Activity.{classLevels[idx].C}";
						tooltip = $"Teaches {classLevels[idx].N}";
					}
					grp = new RadioFormFieldBuilder(builder.pdfDoc, id)
							.CreateRadioGroup();
					grp.SetAlternativeName(tooltip);
					subsubTable
							.AddRow(ThinData,
						CellMakers.CheckCell(model, grp, "X", CheckBoxType.SQUARE));
					// build the array for validation
					activitiesList.Add(id);

				}

				// Role and TAM - most important
				subtable.AddRow(ThinHeader,
					TextCell(model12, "Role"),
					TextCell(CellStyleFactory.CreateCell(1, subtable.GetNumberOfColumns() - 2),
						"Classes Taught/Tasks Performed (select all that apply)")

					);
				subtable.AddRow(ThinData,

					ComboCell(model12, $"TL.{i:00}.Role", builder.lookups.Opt("filteredRoles"))
					//TextCell(model, "")

					);

				// Activities array
				Cell activities = CellStyleFactory.CreateCell(1, subtable.GetNumberOfColumns() - 1);

				subtable.AddRow(ss["unpaddedabstractcell"],
					ElementCell(activities, subsubTable)
					);

				// above Qualifications, put the labels 'Education' 'General'
				subtable.AddRow(ThinHeader,
					TextCell(model21, "Qualification"),
					TextCell(model13, "Education"),
					TextCell(model, ""),
					TextCell(model13, "General"),
					TextCell(model14, "Last Inservice: year & topic")
					);

				subtable.AddRow(ThinData,
					//TextCell(model, "Qualification"),
					ComboCell(model13, $"TL.{i:00}.QualEd", builder.lookups.Opt("qualEd")),
					TextCell(model, ""),
					ComboCell(model13, $"TL.{i:00}.Qual", builder.lookups.Opt("qualN")),
					NumberCell(model, $"TL.{i:00}.InService.Year"),
					//TextCell(model, "Topic"),
					InputCell(model14, $"TL.{i:00}.InService.Topic", 20)
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
					TextCell(model, "F/T"),
					TextCell(model, "P/T"),
					TextCell(model, "days/week"),
					TextCell(model, "")
					);
				subtable.AddRow(ThinData,

					ComboCell(model13, $"TL.{i:00}.PaidBy", builder.lookups.Opt("authorities")),
					TextCell(model, ""),
					ComboCell(model13, $"TL.{i:00}.EmpStatus", builder.lookups.Opt("teacherStatus")),
					// now collected as F P, not FT PT note tchFullPart is nvarchar(1) so this has always neem truncated
					SelectCell(model, grpFP, "F"),
					SelectCell(model, grpFP, "P"),
					NumberCell(model, $"TL.{i:00}.Days",null,1),		// 1 decimal point
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
					TextCell(model, ""),
					TextCell(model13, "Year Started Teaching")
					);
				subtable.AddRow(ThinData,

					ComboCell(model13.SetHeight(16), $"TL.{i:00}.Citizenship", builder.lookups.Opt("nationalities")),
					TextCell(model.SetHeight(16), ""),
					ComboCell(model13.SetHeight(16), $"TL.{i:00}.HomeIsland", builder.lookups.Opt("islands")),
					TextCell(model.SetHeight(16), ""),
					NumberCell(model, $"TL.{i:00}.YearStarted"),
					TextCell(model12.SetHeight(16), "")

					);

				// family status
				subtable.AddRow(ThinHeader,
					TextCell(model21, "Family"),
					TextCell(model13, "Marital Status"),
					TextCell(model, ""),
					TextCell(model13, "Dependant children"),
					TextCell(model12, "House Provided"),
					TextCell(model12, "")
					);

				subtable.AddRow(ThinData,

					ComboCell(model13.SetHeight(18), $"TL.{i:00}.MaritalStatus", builder.lookups.Opt("maritalStatus")),
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

				// validations
				condOnStaff.Add(ConditionalField.IfAny($"TL.{i:00}.tID", $"TL.{i:00}.OnStaff"));
				// expand this to make any field required on the existence of a teacher
				condExpectedStaff.Add(ConditionalField.IfYes(
					$"TL.{i:00}.OnStaff",
					new string[] { $"TL.{i:00}.FirstName", $"TL.{i:00}.Gender", $"TL.{i:00}.DoB",
						$"TL.{i:00}.Role", $"TL.{i:00}.FP" }
				));
				condNewStaff.Add(ConditionalField.IfAny(
				$"TL.{i:00}.FamilyName",
				new string[] { $"TL.{i:00}.FirstName", $"TL.{i:00}.Gender", $"TL.{i:00}.DoB",
						$"TL.{i:00}.Role", $"TL.{i:00}.FP" }

				));
				condActivitiesExpected.Add(ConditionalField.IfYesAlternatives(
					$"TL.{i:00}.OnStaff",
					activitiesList.ToArray()
				));
				condActivitiesNew.Add(ConditionalField.IfAnyAlternatives(
					$"TL.{i:00}.FamilyName",
					activitiesList.ToArray()
				));
				var days = new ConditionalField($"TL.{i:00}.FP")
				{
					Value = new[] { "P" },
				};
				days.AddAll($"TL.{i:00}.Days");
				condFPDays.Add(days);

			}
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), condOnStaff);
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), condExpectedStaff);
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), condActivitiesExpected);
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), condNewStaff);
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), condActivitiesNew);
			ValidationManager.AddConditionalFields(document.GetPdfDocument(), condFPDays);

			return document;
		}
	}
}

console.println("core Loading...");

// Core functionality for Acrobat forms
function OnStartup() {

	console.println("On Startup");
	console.println("Dynamic requireds:");

	if (requiredsTable) {
		// sort the elements of requiredsTable based on sort field
		requiredsTable.sort(function (a, b) {
			return (a.sort > b.sort) ? 1 : ((b.sort > a.sort) ? -1 : 0);
		});	
		for (var i = 0; i < requiredsTable.length; i++) {
			console.println(requiredsTable[i].name );
		}
	}
	// calculate the number of expected staff
	// This is the highest index in TL table for which tID is not null
	var idx = 0;
	do {
		var n = "TL." + padStart(idx.toString(), 2, "0") + ".tID";
		console.println(n);
		var ff = gf(n);
		if (ff == null) {
			console.println(n + " not found");
			break;
		}
		if (!ff.value) {
			console.println(n + " no value");
			break; // no more prepopulated entries
		}
		idx++;
	} while (true);
	numExpectedStaff = idx;
	console.println("Expected staff: " + numExpectedStaff);
	return;

	var sy = gf("Survey.SurveyYear");
	var year = sy.value;
	var title = gthis().info.Title + ' ' + year.toString();

	var school =
		(gf("Survey.SchoolNo").value) ?
			gf("Survey.SchoolNo").value + " " + gf("Survey.SchoolName").value : title;

	// Filter field names that start with "Footer."

	var idx = 2;			// footers start at page 2 since itext numbers from 1
	do {
		var n = "Footer." + padStart(idx.toString(), 2, "0");
		console.println(n);
		var ff = gf(n);
		if (ff == null) {
			console.println(n + " not found");
			break;
		}
		var page = gfpage(ff);
		if (page % 2 == 0) {  // inside acrobat, page num is 0 based. Therefore the page shown as 1 is actually page 1
			if (ff.value != title) {
				ff.value = title;
			}
		}
		else {
			if (ff.value != school) {
				ff.value = school;
			}
		}
		idx++;
	} while (true);

	idx = 0;
	do {
		var n = "TL." + padStart(idx.toString(), 2, "0") + ".tID";
		console.println(n);
		var ff = gf(n);
		if (ff == null) {
			console.println(n + " not found");
			break;
		}
		if (!ff.value) {
			console.println(n + " no value");
			break; // no more prepopulated entries
		}

		var fldOnStaff = gf(n.replace("tID", "OnStaff"));
		actions.applyOnStaff(fldOnStaff);
		idx++;

	} while (true);



	//}
}

//****************** Utilities and wrappers *************************

// return values from alerts
var alertYes = 4;
var alertNo = 3;
var alertCancel = 2;
var alerOK = 1;

function pvalert(msg) {
	return app.alert(msg, 1, 0, "Validating your data");
}

function inform(msg) {
	return app.alert(msg, 3, 0, this.title);
}

function exclaim(msg) {
	return app.alert(msg, 1, 0, this.title);
}

function okCancel(msg) {
	return app.alert(msg, 2, 1, this.title);
}

function yesNo(msg) {
	return app.alert(msg, 2, 2, this.title);
}

function yesNoCancel(msg) {
	return app.alert(msg, 2, 3, this.title);
}

function dlgTitle() {
	return this.title;
}
// helper functions to detect if controls has values
function emptyCombobox(f) {
	if ((f == "") || (f == "00") || (f == "(select from list...)"))
		return true;
	return false;
}

function emptyCheckbox(f) {
	if ((f == "") || (f == "Off"))
		return true;
	return false;
}

var gthis = function () {
	return this;
}.bind(this);

// simple wrapper around getField
function gf(fieldname) {
	return this.getField(fieldname);
}

// simple wrapper around getField.value
function gfv(fieldname) {
	return this.getField(fieldname).value;
}

// extended version deals with dummy values in checkboxes and combos

// extended version deals with dummy values in checkboxes and combos
function gfvx(fieldname) {
	var f = this.getField(fieldname);
	var ret = "";
	if (f == null) {
		console.println(fieldname + "(gfvx)");
		return "";
	}
	switch (f.type) {
		case 'combobox':
			ret = (emptyCombobox(f.valueAsString) ? '' : f.valueAsString);
			break;
		case 'checkbox':
			ret = (emptyCheckbox(f.value) ? '' : f.value);
			break;
		default:
			ret = f.valueAsString;		//7 11 2012
			break;

	}
	return ret;
}



// gets the displayable value for a combobox
function gfvt(fieldname) {
	var f = this.getField(fieldname);
	var ret = "";
	switch (f.type) {
		case 'combobox':
			if (emptyCombobox(f.valueAsString))
				ret = "";
			else {
				ret = f.getItemAt(f.currentValueIndices, false);
			}

			break;
		case 'checkbox':
			ret = (emptyCheckbox(f.value) ? '' : f.value);
			break;
		default:
			ret = f.value;
			break;

	}
	return ret;
}

// get the page number that a field is on
function gfpage(fld) {
	if (typeof fld.page == "number") {
		return fld.page;
	}
	// field may be on multiple pages theoretically
	return fld.page[0];
}

var gthis = function () {
	return this;
}.bind(this);

function padStart(str, targetLength, padString) {
	str = String(str); // Ensure the input is a string
	padString = String(padString || ' '); // Default pad string is a space

	if (str.length >= targetLength) {
		return str; // Return the string unchanged if it's already long enough
	}

	var padding = "";
	while (padding.length < targetLength - str.length) {
		padding += padString;
	}

	return padding.substring(0, targetLength - str.length) + str;
}
// *********************** Wrappers around common items

function surveyYear() {
	var f = gf("Survey.SurveyYear");
	if (f == null)
		return "";
	app.alert(f.value);
	return f.value;
}

function schoolNo() {
	var f = gf("schoolNo");
	if (f == null)
		return "";
	return f.value;
}

function schoolName() {
	var f = gf("schoolName");
	if (f == null)
		return "";
	return f.value;
}

// *********************** Validation and formatting ************************

function validateDateRange(eventvalue, min, max) {

	if ((eventvalue < min) || (eventvalue > max)) {
		app.alert("Date is not in the allowed range " + util.printd("yyyy-mm-dd", min) + " - " + util.printd("yyyy-mm-dd", max));
		return false;
	}

	return true;

}


function dateAfter(eventvalue, min) {
	if (min == null)
		return true;


	if (eventvalue < min) {
		return false;
	}

	return true;
}



function dateFromString(inValue) {

	if (inValue == "")
		return "";

	var eventDate;

	switch (inValue.length) {

		case 10:
			if (inValue.charAt(4) == "-" && inValue.charAt(7) == "-") {
				eventDate = util.scand("yyyy-mm-dd", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			if (inValue.charAt(4) == "/" && inValue.charAt(7) == "/") {
				eventDate = util.scand("yyyy/mm/dd", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			if (inValue.charAt(2) == "-" && inValue.charAt(5) == "-") {
				eventDate = util.scand("dd-mm-yyyy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			if (inValue.charAt(2) == "/" && inValue.charAt(5) == "/") {
				eventDate = util.scand("dd/mm/yyyy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			break;
		case 9:
			if (inValue.charAt(4) == "-" && inValue.charAt(7) == "-") {
				eventDate = util.scand("yyyy-mm-d", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(4) == "/" && inValue.charAt(7) == "/") {
				eventDate = util.scand("yyyy/mm/d", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(4) == "-" && inValue.charAt(6) == "-") {
				eventDate = util.scand("yyyy-m-dd", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(4) == "/" && inValue.charAt(6) == "/") {
				eventDate = util.scand("yyyy/m/dd", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(1) == "/" && inValue.charAt(4) == "/") {
				eventDate = util.scand("d/mm/yyyy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(2) == "/" && inValue.charAt(4) == "/") {
				eventDate = util.scand("dd/m/yyyy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			break;
		case 8:
			if (inValue.charAt(4) == "-" && inValue.charAt(6) == "-") {
				eventDate = util.scand("yyyy-m-d", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			if (inValue.charAt(4) == "/" && inValue.charAt(6) == "/") {
				eventDate = util.scand("yyyy/m/d", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			if (inValue.charAt(1) == "-" && inValue.charAt(3) == "-") {

				eventDate = util.scand("d-m-yyyy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(1) == "/" && inValue.charAt(3) == "/") {

				eventDate = util.scand("d/m/yyyy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(2) == "-" && inValue.charAt(5) == "-") {

				eventDate = util.scand("dd-mm-yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

			if (inValue.charAt(2) == "/" && inValue.charAt(5) == "/") {

				eventDate = util.scand("dd/mm/yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			break;
		case 7:
			if (inValue.charAt(1) == "-" && inValue.charAt(4) == "-") {
				eventDate = util.scand("d-mm-yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);

			}
			if (inValue.charAt(1) == "/" && inValue.charAt(4) == "/") {
				eventDate = util.scand("d/mm/yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);

			}

			if (inValue.charAt(2) == "-" && inValue.charAt(4) == "-") {
				eventDate = util.scand("dd-m-yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);

			}
			if (inValue.charAt(2) == "/" && inValue.charAt(4) == "/") {
				eventDate = util.scand("dd/m/yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);

			}
			break;
		case 6:
			if (inValue.charAt(1) == "/" && inValue.charAt(3) == "/") {
				eventDate = util.scand("d/m/yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}
			if (inValue.charAt(1) == "-" && inValue.charAt(3) == "-") {
				eventDate = util.scand("d-m-yy", inValue);
				if (eventDate != null)
					return util.printd("yyyy-mm-dd", eventDate);
			}

	}

	return null;

}

function formatDate(eventvalue) {
	var dt = dateFromString(eventvalue);
	if (dt == null)
		return "";
	return dt;

}

function validateDateFormat(eventvalue) {
	if (eventvalue == "")
		return true;        // don;t try to validate an empty



	var eventDate = dateFromString(eventvalue);

	if (eventDate == null) {
		//if there is any ambiguity about this date, force re-entry in yyyy-mm-dd format

		app.alert("Enter a valid date in the format yyyy-mm-dd");
		return false;

	}

	// no date should be ealier than 1930, or later than 2040

	if ((util.scand("yyyy-mm-dd", eventDate) < util.scand("yyyy-mm-dd", "1930-01-01")) ||
		(util.scand("yyyy-mm-dd", eventDate) >= util.scand("yyyy-mm-dd", "2040-01-01"))
	) {
		pvalert("Date outside allowed range - please check value and re-enter in the format yyyy-mm-dd");
		return false;
	}

	return true;

}

// validate date anad range
function validateDateAndRange(eventvalue, min, max) {
	if (eventvalue == "")
		return true;        // don;t try to validate an empty


	var eventDate = dateFromString(eventvalue);

	if (eventDate == null) {
		//if there is any ambiguity about this date, force re-entry in yyyy-mm-dd format

		app.alert("Enter a valid date in the format yyyy-mm-dd");
		return false;

	}


	var rc = validateDateRange(util.scand("yyyy-mm-dd", eventDate), min, max);
	return rc;


}


// some regular expression validations
// core routine has no ui
// email
function validateEmail(value) {

	var rx = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
	if (value == "")
		return true;

	return rx.test(value);

}

//-------------- Phone number in canonical format ---------------------------------

function validatePhone(value) {
	// this omits the brackets around area code

	var rx = /^(\+[1-9]\d{0,3}\s[1-9][0-9\s]{1,11}\s?X?\s?\d{1,4})?$/

	//var rx = /^\+\d{1,3}\s?(\(([1-9]\d{0,3})?\))?\s?(\d{1,4}(\s|-)?\d{1,4}|\d{1,6})(\s?X[1-9]\d{0,3})?$/

	//return true;
	if (value == "")
		return true;
	return rx.test(value);

}


try {
	var i = this.numFields;
	var n = gf("Survey.SurveyYear");
}
catch (ex) {
	console.println("Dummy JS call");
}

console.println("Core Loaded");
console.println("core Loading...");

// Core functionality for Acrobat forms
function OnStartup()
{
	console.println("On startup");
	var sy = this.getField("Survey.SurveyYear");
	var year = sy.value;
	var title = this.info.Title + ' ' + year.toString();
	
	console.println(title);
	console.println(year);
	var school = 
	(this.getField("Survey.SchoolNo").value) ?
			this.getField("Survey.SchoolNo").value + " " + this.getField("Survey.SchoolName").value : title;
	
	// Filter field names that start with "Footer."
	for (var i = 0; i < this.numFields; i++) {

		if (this.getNthFieldName(i).startsWith("Footer.")) {
			var n = this.getNthFieldName(i);
			var ff = this.getField(n);

			if (i % 2 == 1) {
				ff.value = title;
			}
			else {
				ff.value = school;

			}

		}
	}
}

//****************** Utilities and wrappers *************************

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

// gets the displayble value for a combobox
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


// *********** end Validation and formatting ***************//



//*************** Email Handling ************************* //
function targetEmail() {
	return "applications@adsafrica.com.au";
}

function trueCopy() {
	return "I certify the attached document is a true copy";
}

// this is a wrapper around mailMsg which doesn;t work in Foxit 5.0 as at 8 10 2011

function doMailMsg(subject, body) {
	var str = "mailto:" + targetEmail() + "?subject=" + subject + "&body=" + body;
	console.println(str);
	app.launchURL(str);
}

// this function produces a test email, and set the flag accordingly
function testEMail() {
	//
	var str;
	str = "The application form will now try to make a test email message. If this succeeeds, and you can send the test message, you can select the Desktop Mail option for delivering emails";
	inform(str);
	doMailMsg("EMail test", "Send this message to confirm that " + this.title + " can send email using your desktop email system.");
	var result = yesNo("Did the message send OK?");
	if (result == 4) {//yes
		// were able to send
		inform("You can complete the application using Desktop email.");
		this.getField("chkEmailType").value = "DESKTOP";
		return true;
	}
	// couldn't send
	// If this document is not reader-extended, we can't send it?

	else {      // dont send by desktop email
		this.getField("chkEmailType").value = "WEB";
		if (isFoxit()) { // foxit can save data
			inform("You cannot submit the application using Desktop email. To send, you will need to save the application data to your disk, then manually attach the file to an email message. Use the Submit Application button on the first page of this Application form to step through this process.");
			return;
		}

		if (RE() == true) {
			inform("You cannot submit the application using Desktop email. To send, you will need to save the application to your disk, then manually attach the file to an email message.");
			return true;
		}
		// test further if not RE
		inform("You cannot submit the application using Desktop email. To send, you will need to save the application to your disk, then manually attach the file to an email message. If you are not able to Save the form with all its contents using your current PDF Reader software, you need to open the application using different reader software.");


		result = okCancel("Try to save the form with contents now? (If you are not able to do this, you will need to use different PDF software)");
		if (result == 1) {
			try {
				app.execMenuItem("SaveAs");
				result = yesNo("Were you able to save the form including the contents of filled-in fields?");
			}
			catch (err) {
				exclaim("The application form could not be saved.");
				result = 0;
			}
			if (result == 4) {
				inform("You can continue to complete the form. When the form is complete, Save it to your hard drive, then email that file to " + targetEmail());
				return true;
			}
			exclaim("You will not be able to complete the form using your current PDF Reader software without a desktop email system. " +
				"Consider using an alternative PDF Reader, such as Foxit Reader from www.foxitsoftware.com. " +
				"Alternatively, you can continue to fill in the form, then, when it is complete and checked, print the filled-in form, and post to the address provided on the AAA website.");
			return false;
		}

		return true;
	}
}

function sendDoc(subject, body) {
	if (isWebEMail() == 1) {


		// foxit doesn;t draw the repsonse box well
		inform(util.printf("Create a new mail message in your internet email to this recipient: %s with subject: '%s'. " +
			"(You may copy the document title from the window displayed after this" +
			" and paste into your email message 'Subject' field.) Attach the document to this email and Send. ", targetEmail(), subject));


		var alertText = "Subject for email (copy and paste this text to your email message 'Subject'):";
		var result = app.response(alertText, "Web Mail", subject);
	}
	else
		//app.mailMsg(true, targetEmail(),"","",subject, body);
		doMailMsg(subject, body);
}

function isWebEMail() {
	if (this.getField("chkEmailType").value == "WEB")
		return 1;
	else
		return 0;
}


function webMailURL() {

	var email = this.getField("sEmail1").value;

	if (email.indexOf("@hotmail.com") > 0)
		return "http://www.hotmail.com";

	if (email.indexOf("@gmail.com") > 0)
		return "http://www.gmail.com";

	if (email.indexOf("@fastmail.fm") > 0)
		return "http://www.fastmail.fm";

	if (email.indexOf("yahoo") > 0)
		return "http://mail.yahoo.com";

	return "";
}
function confirmEMail(msgText) {



	var checkBoxText = "Open " + webMailURL();

	var alertBox = new Object();
	var alertResult;
	alertBox.ocheckBox = {
		cMsg: checkBoxText,
		bInitialValue: false
	}

	if (isWebEMail() == 1 && webMailURL() > "") {

		alertResult = app.alert(msgText, 3, 1, dlgTitle(), this, alertBox.ocheckBox);
		if (alertResult == 1 &&
			alertBox.ocheckBox.bAfterValue == true)
			app.launchURL(webMailURL(), true);
		return alertResult;
	}
	else
		return app.alert(msgText, 3, 1, dlgTitle(), this);
}



function doPreValidate() {
	try {
		return preValidate();
	}
	catch (err) {
		return "OK";
	}

}


function checkApplication() {

	// checkEligibility can be a document specific call out - NOT defined here
	if (doCheckEligibility() == 0)
		return 0;

	// preValidate is document specific - not defined here
	var fname = doPreValidate();
	if (fname == "OK") {
		inform("Your application is ready to submit", 3, 0);
		return;
	}

	if (fname == "") {
		inform("Your application is not yet ready to submit.");
	}
	else {
		console.println(fname);
		var f = this.getField(fname);
		var toolTip = f.userName;
		var msg = "Your application is not ready to submit. Go to the first identified problem now";
		if (toolTip != "")
			msg = msg + " (" + toolTip + ")?";
		else
			msg = msg + "?";
		var resp = okCancel(msg);
		if (resp == 1) { //ok
			var f = this.getField(fname);
			var p = f.page;
			this.pageNum = p[0];
			f.setFocus();
		}
	}
}

//-----------------------------------------------------------------------

//Console.Writeln('Welcome to ' + this.info.Title + ' ' + this.getField("Survey.SurveyYear").toString());
//app.alert('Welcome to ' + this.info.Title + ' ' + this.getField("Survey.SurveyYear").value);
//app.alert('Welcome to ' + this.info.Title);
//app.alert(this.getField("Survey.SurveyYear").value);
try {
	OnStartup();
}
catch (ex) {
	console.println("OnStartup dummy invoke failed");
	//console.println(ex);
};

console.println("Core Loaded");
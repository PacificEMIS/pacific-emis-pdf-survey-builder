/*******************************************************************************
 * Project      : Pineapples.SurveyBuilder
 * File         : Completeness.js
 * Description  : embedded JavaScript for Adobe Acrobat forms - 
 *								completeness testing utility functions
 * Author       : Brian Lewis
 * Created      : 2024-10-01
 * Last Updated : 2025-01-24 by Brian Lewis
 * Version      : 1.0.0
 * 
 * Copyright (c) 2024, 2025.
 * 
 *******************************************************************************/
console.println("Completeness loading...");
/**
 * requiredsTable is an array of 'requiredFields' or 'conditionalFields' objects.
 * requiredFields objects have the following properties:
 * name: the name of the collection
 * tests: the name of a function that returns an array of field names
 * message: message to display to the user if required fields are found',
 * type: 'R' indicates a collection of requiredFields,
 * 
 * a sample:
 * requiredsTable[requiredsTable.length] = {
		name: 'Repeaters',
		tests: repeaters_Conditionals,
		message: 'Repeater data is not complete.',
		type: 'C',
		filter: null
	}
 *
 * conditionalFields objects have the following properties:
 * name: the name of the collection
 * type: 'C' indicates a collection of conditionalFields
 * tests: the name of a function that returns an array of conditionalField objects
 * message: message to display to the user if conditional fields are found
 *	
 * conditionalField objects have the following properties:
 * test: the field to test
 * value: an array of values that trigger the dependency An empty array matches any non-null value 
 * rq: array of field names that are required if test field has one of the defined values 
 * 
 * sample:
 * var repeaters_Conditionals = function() {
    var a = [];
    var j = 0;
    a[j++] = {
        test: "Rep.HasData",
        value: ["Y"],
        rq: ["Rep.T.T.T.T"]
    };
    return a;
	};
 * In a requiredFields entry, the tests function returns an array of arrays of field names
 * These fields are unconditionally required: at least one item in the array must have a value
 * else, the first entry is required
 * var school_Site_Requireds = function() {
    var a = [];
    var j = 0;
    a[j++] = ["Site.Secure"];
    a[j++] = ["Site.Services"];
    a[j++] = ["Site.Size"];
    a[j++] = ["Site.Playground.Size"];
    a[j++] = ["Site.Garden.Size"];
    return a;
};
requiredsTable[requiredsTable.length] = {
		name: 'School Site',
		tests: school_Site_Requireds,
		message: 'School site is missing information.',
		type: 'R',
		filter: null
}
 * The sample code above is generated and written to the javascript tree
 * in ConditionalFields and RequiredFields objects defined ValidationManager.cs
 *
 */
var requiredsTable = [];
var numExpectedStaff = 0;
var numAllStaff = 80;

/**
 * Return if a form field is read/write
 * @param {any} fieldName
 * @returns
 */
var isReadWrite = function (fieldName) {
	return(!gf(fieldName).readonly);
}

var isExpectedStaff = function (fieldName) {
	// extract the number from the field name
	// field name assumed to be a teacher field TL.nn.<name>
	var n = fieldName.split('.');
	return parseInt(n[1]) < numExpectedStaff;
}

var isNewStaff = function (fieldName) {
	// extract the number from the field name
	// field name assumed to be a teacher field TL.nn.<name>
	var n = fieldName.split('.');
	return parseInt(n[1]) >= numExpectedStaff;
}
/**
 * Change the appearance of a field depending on its Required status
 * @param {any} fld the form field to act on
 * @param {any} required boolean
 * @param {any} isAlternative boolean the field is part of a group of alternatives
 */
function setRequired(fld, required, isAlternative) {
	fname = fld.name;
	if (required) {
		fld.required = true;
		switch (fld.type) {
			case "text":
			case "combobox":
				fld.borderWidth = 1;
				fld.strokeColor = isAlternative? color.magenta: color.red;
				break;
			case "checkbox":
				// Modify properties for each widget
				var widgets = fld.getArray();
				for (var j = 0; j < widgets.length; j++) {
					// Get the individual widget
					var widget = widgets[j];
					// save the fill and text properties
					var fillc = widget.fillColor;
					var txtc = widget.textColor;
					bw = widget.borderWidth;

					//widget.strokeColor = color.red;
					widget.fillColor = isAlternative ? color.magenta : color.red;
					widget.textColor = txtc;
					//widget.borderWidth = bw;
				}


		}
		fld.userName = 'requiredUI';
	}
	else {
		//not empty
		fld.required = false;
		if (fld.userName == "requiredUI") {

			switch (fld.type) {
				case "text":
				case "combobox":
					fld.strokeColor = fld.fillColor;
					fld.borderWidth = 0;
					break;
				case "checkbox":
					var widgets = fld.getArray();

					// Modify properties for each widget
					for (var j = 0; j < widgets.length; j++) {
						// Get the individual widget
						var widget = widgets[j];


						//widget.strokeColor = color.gray;   // Green border
						//widget.borderWidth = 1.8;
						widget.fillColor = ["RGB", 0.988, 0.957, 0.898];  // these come from defaultbackcolor()

					}
			}
			fld.userName = "";
		}
	}
}

/**
 * Convenience function to set required value for a list of fields
 * @param {any} rq array of field names
 * @param {any} required boolean value
 */
function setRequiredArray(rq, required) {
	for (j = 0; j < rq.length; j++) {
		setRequired(gf(rq[j]), required, rq.length > 1? true:false);
	}
}
/**
 * find the first required field that is empty
 * if rqArray is supplied, check only those fields
 * @param {any} rqArray fields to check
 * @returns {string} name of the first empty required field
 */
function firstRequired(rqArray) {
	var fieldname;
	var problemname = "";
	var fld;
	var count = 0;
	var firstproblempage = 0;
	var firstproblemtop = 0;

	try {

		var numFields = (rqArray == undefined) ?gthis().numFields : rqArray.length;
		for (var i = 0; i < numFields; i++) {

			fieldname = (rqArray == undefined) ? gthis().getNthFieldName(i) :rqArray[i];

			fld = gf(fieldname);
			switch (fld.type) {
				case "text":
				case "checkbox":
				case "combobox":
					if (fld.required && gfvx(fieldname) == "") {
						count++;
						var pg = gfpage(fld);
						var top = fld.rect[1];

						if (firstproblempage == 0 || firstproblempage > pg
							|| (firstproblempage == pg && firstproblemtop < top)) {
							problemname = fieldname;
							firstproblempage = pg;
							firstproblemtop = top;
						}
					}
					break;
			}
		}
		return problemname;
	}
	catch (ex) {
		console.println("firstRequired: " + ex.message);
		throw ex;
	}

}

///function teacher checks

console.println("Completeness loaded");
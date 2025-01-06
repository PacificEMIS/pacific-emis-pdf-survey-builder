console.println("Completeness loading...");

var requiredsTable = [];

var isReadWrite = function (fieldName) {
	return(!gf(fieldname).readonly);
}


function setRequired(fld, required) {
	fname = fld.name;
	if (required) {
		fld.required = true;
		switch (fld.type) {
			case "text":
				fld.borderWidth = 1;
				fld.strokeColor = color.red;
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
					widget.fillColor = color.red;
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
						widget.fillColor = ["RGB", 0.988, 0.957, 0.898];  // these come from defaaultbackcolor()

					}
			}
			fld.userName = "";
		}
	}
}

///convenince function to set required value
/// rq - array of names
function setRequiredArray(rq, required) {
	for (j = 0; j < rq.length; j++) {
		setRequired(gf(rq[i]));
	}
	
}
/// find the first required field that is empty
// if rqArray is supplied, check only those fields
// returns array
function firstRequired(rqArray) {
	var fieldname;
	var problemname = "";
	var fld;
	var count = 0;
	var firstproblempage = 0;
	var firstproblemtop = 0;

	try {

		var numFields = (rqArray == undefined) ?gthis().numFields : rqArray.length;
		console.println("firstRequired numFields = " + numFields);
		for (var i = 0; i < numFields; i++) {

			fieldname = (rqArray == undefined) ? gthis().getNthFieldName(i) :rqArray[i];

			fld = gf(fieldname);
			switch (fld.type) {
				case "text":
				case "checkbox":
					if (fld.required && gfvx(fieldname) == "") {
						count++;
						var pg = gfpage(fld);
						var top = fld.rect[1];

						console.println("Page " + pg);
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
		console.println("firstRequired: " + problemname);
		return problemname;
	}
	catch (ex) {
		console.println("firstRequired: " + ex.message);
		throw ex;
	}

}

///function teacher checks

console.println("Completeness loaded");
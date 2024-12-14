
console.println("Completeness loading...");

// - conditionals -- fields that be be mandatory depending on the value of another
// members of the conditional object are
// test - the field to check
// value the array of values to which the condition applies - usually Y or N
// rq the array of field names that become required if the condition is met
function conditionals() {
	var a = [];
	var j = 0;

	// Parent Committee
	a[j++] = {
		test: "PC.Exists",
		value: ["Y"],
		rq: [["PC.Members.M", 1], ["PC.Members.F", 1], ["PC.Meet", 1]]
	};

	return a;
}

function mandatoryFields() {
	console.println("MandatoryFields");
	var a = [
		["PC.Exists", 2],
		["Survey.Pupils", 2],
		["Survey.Teachers", 2],

		// grid control
		["Rep.HasData", 2],
		["DIS.HasData", 2],
		["TRIN.HasData", 2]
	];
	console.println(a.length);
	return a;
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
/// find the first required field that is empty
// returns array
function firstRequired() {
	var fieldname;
	var problemname = "";
	var fld;
	var count = 0;
	var firstproblempage = 0;
	var firstproblemtop = 0;

	try {
		var numFields = gthis().numFields;
		console.println("firstRequired numFields = " + numFields);
		for (var i = 0; i < numFields; i++) {
			fieldname = gthis().getNthFieldName(i);

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

console.println("Completeness loaded");
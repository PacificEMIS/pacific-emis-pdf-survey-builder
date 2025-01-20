console.println("Validation Loading...");

/// validations object
var v = {

	//#region Main validation routine

	/**
	 * Main Validation
	 * @param {any} event - event invoking this function - the MouseUp event of the 
	 * Validation button FormField 
	 * @returns sets the Status of the form to Complete or Incomplete
	 */
	doAllValidations: function (event) {
		var result;
		var status = "Complete";

		console.show();
		console.clear();
		// step through the requireds table checking all entries
		var idx = 0;
		for (idx = 0; idx < requiredsTable.length; idx++) {
			var entry = requiredsTable[idx];
			var ca = entry.tests.call(this);
			var m = entry.message;
			event.target.value = "Checking " + entry.name + (".").repeat(idx);
			
			var rtype = entry.type;		// C for conditional, R for required
			switch (rtype) {
				case 'C':
						result = this.doCheckAllConditionals(entry);
					break;
				case 'R':
					result = this.doCheckIncompleteness(entry);
			}
			console.println("conditionals result: " + entry.name + ': ' + result);


			switch (result) {
				case 0: // no errors press ahead
					break;
				case alertNo:
					status = "Incomplete";
					break;
				default:
					this.setStatus(event.target, "Incomplete");
					return;
			}
		}

		this.setStatus(event.target, status);
		switch (status) {
			case "Complete":
				inform("Congratulations! Your survey is now complete!");
				break;
			case "Incomplete":
				break
		}

	},

	setStatus: function (btn, status) {
		switch (status) {
			case "Complete":
				btn.value = "Survey is Complete!"
				btn.fillColor = color.green;
				return;
			case "Incomplete":
				btn.value = "Survey is not Complete! - Click to test again"
				btn.fillColor = color.red;
				return;
		}
	},

	//#endregion


	//------------ Mandatory field testing----------------------------------
	// make a field required if it has no value
	// return number of fields found as now required ie if 0 returned all is OK
	// a: Array of arrays of field names to test 
	// rqArray: required fields are added to rqArray
	mandatoryIfEmpty: function (a, rqArray) {
		var numFound = 0;
		var result = false;
		for (i = 0; i < a.length; i++) {
			try {
				// array if alternative fields - usually only one will be required
				var alternativeArray = (a[i]);
				if (!Array.isArray(alternativeArray)) {
					inform("alternative Not an array - " + alternativeArray);
				}
				var altOK = false;
				for (j = 0; j < alternativeArray.length; j++) {
					fname = alternativeArray[j];
					
					var fld = gf(fname);
					if (fld != null) {
						if (gfvx(fname)) {
							altOK = true;
							break;
						}
					}

				}
				if (altOK == false) {
					setRequiredArray(alternativeArray, true);
					numFound++;
					// add all the alternative fields to the required array
					for (var k = 0; k < alternativeArray.length; k++) {
						rqArray.push(alternativeArray[k]);
					};
				}
				else {
					setRequiredArray(alternativeArray, false);
				}

			}
			catch (ex) {
				console.println("Error in mandatory If Empty" + a[i][0] + " " + ex);
				throw ex;
			}
		}
		return numFound;
	},

	// #region Required Fields
	// returns true if any mandatory fields remain not filled in
	checkIncomplete: function (entry, rqArray) {
		// to do this wrapper doesn;t do anything
		var result = false;

		var toCheck = entry.tests.call(this);
		return this.mandatoryIfEmpty(toCheck, rqArray);
	},
	//	return 0 (false): no issues found
	// if issues found
	//  1 (Yes) - reviewing
	//  2 (No) Don't review continue reporting
	//  3 (Cancel) stop
	doCheckIncompleteness: function (entry) {
		var rqArray = [];
		var result = this.checkIncomplete(entry, rqArray); // true if incomplete, false if no problems
		if (!result) return 0;


		var msg = entry.message + "\nReview now?\n\nYes: Review Now\n\nNo: Continue checking\n\nCancel: Exit check";
		var resp = yesNoCancel(msg);
		
		switch (resp) {
			case alertYes:
				var problemname = firstRequired(rqArray);
				console.println("Problemname: " + problemname);

				if (problemname != "") {
					var f = gf(problemname);
					this.pageNum = gfpage(f);
					f.setFocus();
				}
		}
		return resp;
	},
	//#endregion

	//#region Conditional Field Testing

	/**
	 * Make fields required based on the value in another field
	 * Get user feedback if required fields found empty
	 * @param {any} conditionalDef a conditionalFields item
	 * @returns
	 *  0 (false): no issues found
	 * if issues found
	 *  1 (Yes) - reviewing
	 *  2 (No) Don't review continue reporting
	 *  3 (Cancel) stop
	 */
	doCheckAllConditionals: function (conditionalDef) {
		var rqArray = [];
		result = this.checkAllConditionals(conditionalDef, rqArray);
		console.println(rqArray.join(", "));
		if (result == 0) return 0;

		var msg = conditionalDef.message + "\nReview now?\n\nYes: Review Now\n\nNo: Continue checking\n\nCancel: Exit check";
		var resp = yesNoCancel(msg);
		switch (resp) {
			case alertYes:
				var problemname = firstRequired(rqArray);
				if (problemname != "") {
					var f = gf(problemname);
					this.pageNum = gfpage(f);
					f.setFocus();
				}
		}
		return resp;
	},

	checkAllConditionals: function (conditionalDef, rqArray) {
		var found = 0;

		conditionalsArray = conditionalDef.tests.call(this);

		for (var j = 0; j < conditionalsArray.length; j++) {
	//		console.println("Conditional " + j + " " + conditionalsArray[j].test);
			if (conditionalDef.filter) {
				

				if (conditionalDef.filter.call(this, conditionalsArray[j].test)) {
//					console.println("Filtering passed - " + conditionalsArray[j].test);	
					found += this.checkConditional(conditionalsArray[j], rqArray)
				}
			}
			else {
				found += this.checkConditional(conditionalsArray[j], rqArray);
			}
		}
		return found;
	},

	/**
	 * Check a single conditional field
	 * @param {any} o conditional field definition test, value rq
	 * @param {any} rqArray required fields ar epushed onto this array
	 * @returns
	 */
	checkConditional: function (o, rqArray) {
		var found = 0;
		var match = false;
		var fld = gf(o.test);

		if (fld != null) {
			
			var v = gfvx(o.test);
			// if array of matching values is empty, match any non-null
			if (Array.isArray(o.value) && o.value.length === 0 && typeof v === "string" && v.trim() !== "") {
				match = true;
			}
			else {
				for (var i = 0; i < o.value.length; i++) {
					if (v == o.value[i]) {
						match = true;
						break;
					}
				}
			}
		}
		// if match is true, set required if there is no value
		// if match is false, set required = false
		if (match == true) {
			// pass these fields to the mandatoryIfEmpty function, each in its own array
			
			var found = this.mandatoryIfEmpty(o.rq, rqArray);
		}
		else {
			for (var i = 0; i < o.rq.length; i++) {
					setRequiredArray(o.rq[i], false);
			}
		}
		console.println("Check conditional: " + found);
		return found;
	},
	//#endregion



}

console.println("Validation Loaded");

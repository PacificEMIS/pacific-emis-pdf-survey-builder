console.println("Validation Loading...");

/// validations object
var v = {

	//#region Main validation routine
	// validation routein
	doAllValidations: function (event) {
		var result;
		var status = "Complete";

		// step through the requireds table checking all entries
		var idx = 0;
		for (idx = 0; idx < requiredsTable.length; idx++) {
			var entry = requiredsTable[idx];
			var ca = entry.tests.call(this);
			var m = entry.message;

			console.println(entry.name);
			console.println(ca.length);

			var rtype = entry.type;
			switch (rtype) {
				case 'C':
					console.println("Checking conditionals...");
					result = this.doCheckAllConditionals(entry);
					
					break;
				case 'R':
					console.println("Checking requireds...");
					result = 0;
					result = this.doCheckIncompleteness(entry);
			}
			console.println("conditionals result : " + result);

			
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
	// a: Array of field names to test 
	// rqArray: required fields are added to rqArray
	mandatoryIfEmpty: function (a, rqArray) {
		console.println("Mandatory if empty " + a.length);
		var numFound = 0;
		var result = false;
		for (i = 0; i < a.length; i++) {
			try {
				var fname = (a[i]);
				var fld = gf(fname);		// if the field is not there, it must be ok
				if (fld == null) {
					return false;
				}

				if (gfvx(fname) == "") {
					// empty
					setRequired(fld, true);
					numFound++;
					rqArray[rqArray.length] = fname;
				}
				else {
					setRequired(fld, false);
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

		var resp = yesNoCancel(entry.message);
		switch (resp) {
			case alertYes:
				var problemname = firstRequired(rqArray);
				console.println("Problemname" + problemname);

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

	//------------------- Conditional field testing
	//---- Make fields required based n the value in another field
	//----- return true if any field is made required
	// conditionalDef - a conditional object 
	//	return 0 (false): no issues found
	// if issues found
	//  1 (Yes) - reviewing
	//  2 (No) Don't review continue reporting
	//  3 (Cancel) stop
	doCheckAllConditionals: function (conditionalDef) {
		var rqArray = [];
		console.println(rqArray.join(", "));
		result = this.checkAllConditionals(conditionalDef, rqArray);
		console.println(rqArray.join(", "));
		if (result == 0) return 0;

		var resp = yesNoCancel(conditionalDef.message);
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
		console.println("checkAllConditionals")
		console.println("rqArray: " + rqArray.join(", "));

		conditionalsArray = conditionalDef.tests.call(this);

			
		for (var j = 0; j < conditionalsArray.length; j++) {
			console.println(j + " " + conditionalsArray[j] + conditionalsArray[j].test);
			if (conditionalDef.filter && !(conditionalDef.filter.call(this, gf(conditionalsArray[j].test)))) {
				setRequiredArray(ConditionsArray[j].rq, false);
			}
			else {
				found += this.checkConditional(conditionalsArray[j], rqArray)
			}
		}
		return found;
	},

	checkConditional: function (o, rqArray) {
		var found = 0;
		console.println("conditional - " + o.test);
		//console.println("rqArray: " + rqArray.join(", "));
		var match = false;
		var fld = gf(o.test);

		//console.println(" fld  " + o.test + " Null: " + (fld == null) + " " + fld.readonly + " value: " + fld.value);
		if (fld != null) {
			var v = gfvx(o.test);
			//console.println(" o.value is array " + Array.isArray(o.value));
			//console.println(" o.value.length: " + o.value.length);
			if(Array.isArray(o.value) && o.value.length === 0 && typeof v === "string" && v.trim() !== "") {
				console.println("conditional - matched any");
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
		console.println("Match to " + o.value.join("|") + "=" + match);
		// if match is true, set required if there is no value
		// if match is false, set required = false
		if (match == true)
			found = this.mandatoryIfEmpty(o.rq,rqArray);
		else {
			for (var i = 0; i < o.rq.length; i++) {
				var fld = gf(o.rq[i]);			
				if (fld != null)
					setRequired(fld, false);
			}
		}
		
		console.println(rqArray.join(", "));
		console.println("Check conditional: " + found);
		return found;
	},
	//#endregion



}

console.println("Validation Loaded");

console.println("Validation Loading...");
/// validations object
var v = {

	/// onStaff is On if the corresponding tID is empty, and tFamiy is supplied
	// onStaff is Off if tID is empty, and tFamiy is empty
	// OnStaff is mandatory when tId is supplied (ie prepopulated data)
	checkAllOnStaff: function () {
		i = 0;
		while ((this.onStaff(i) != null)) {
			if (!this.checkOnStaff(i)) {
				return i;
			}
			i++;
		}
		return -1; //true
	},

	// on staff field
	onStaff: function (index) {
		return this.TL(index, "OnStaff");
	},

	// on staff value
	onStaffv: function (index) {
		return (this.onStaff(index) != null ? this.onStaff(index).value : null);
	},

	// returns true if the OnStaff flag is valid, false otherwise
	checkOnStaff: function (index) {
		// if its prepopulated (ie existing teacher list), OnStaff has to have a value
		if (this.tlPrePop(index)) {
			return (!emptyCheckbox(this.onStaffv(index)));
		}
		return true;
	},

	tlPrePop: function (index) {
		return (this.TL(index, "tID").value ? true : false);
	},

	TeacherName: function (index) {
		return this.TL(index, "FirstName").value + " " + this.TL(index, "FamilyName").value
	},

	TL: function (index, item) {
		// Ensure the index is two digits by padding with a leading zero if necessary
		var formattedIndex = padStart(index, 2, "0");
		// Construct the field name
		var fieldName = "TL." + formattedIndex + "." + item;
		console.println(fieldName);
		// Get the field using the constructed name
		return gf(fieldName);
	},

	test: function (index) {
		console.println("testing validation object v")
		console.println(this);
		console.println("On Staff " + index + " " + this.onStaffv(i));
		console.println(this.checkOnStaff(index));

		for (var i = 0; i < 10; i++) {
			console.println(this.tlPrePop(i));
			console.println(this.TL(i, "tID").value);
			console.println(this.TL(i, "FirstName").value + " " + this.TL(i, "FamilyName").value);
		}
		var result = this.checkAllOnStaff();
		console.println(result);
		return result;
	},

//// validation routein
	doAllValidations: function () {
		if (this.doCheckInCompleteness()) return false;
		if (this.doCheckAllConditionals()) return false;
		if (!this.doCheckAllOnStaff()) return false;
		Inform("All data is verified valid");
	},
	/// Check OnStaff fields
	doCheckAllOnStaff: function () {
		var result = this.checkAllOnStaff();
		if (result == -1) return true;
		var teacher = this.TeacherName(result);
		pvalert("On Staff (Y/N) is not set for expected teacher " + teacher);
	},
	// general validation of Required

	// returns true if any mandatory fields remain not filled in
	checkIncomplete: function () {

		var result = false;

		if (this.mandatoryIfEmpty(mandatoryFields())) {
			result = true;
		}
		return result;
	},
	//------------ Mandatory field testing----------------------------------
	// make a field required if it has no value
	// return true if any field in the array is now required
	mandatoryIfEmpty: function (a) {

		var result = false;
		for (i = 0; i < a.length; i++) {
			try {
				var fname = (a[i][0]);
				console.println(a[i][0]);
				var fld = gf(fname);		// if the field is not there, it must be ok
				if (fld == null) {
					return false;
				}

				if (gfvx(fname) == "") {
					// empty
					setRequired(fld, true);
					result = true;
				}
				else {
					setRequired(fld, false);
				}
			}
			catch (ex) {
				console.println(a[i][0]);
				throw ex;
			}
		}
		return result;
	},

	doCheckInCompleteness: function () {
		var result = this.checkIncomplete();
		if (result) {

			var problemname = firstRequired();
			console.println("Problemname" + problemname);

			var msg = "You have not yet filled in all the required information. Go to the first missing item now?";

			var resp = okCancel(msg);
			if (resp == 1) { //ok
				if (problemname != "") {
					var f = gf(problemname);

					this.pageNum = gfpage(f);
					f.setFocus();
				}
				else {
					console.println("problem name not identitified");
				}
			}
		}

		return result;
	},
	//------------------- Conditional field testing
	//---- Make fields required based n the value in another field
	//----- return true if any field is made required

	doCheckAllConditionals: function () {
		result = this.checkAllConditionals(conditionals());
		if (result) {
			
			var msg = "Some fields dependent on other fields need to be filled in. Go to the first missing item now?";

			var resp = okCancel(msg);
			if (resp == 1) { //ok
				var problemname = firstRequired();
				if (problemname != "") {
					var f = gf(problemname);

					this.pageNum = gfpage(f);
					f.setFocus();
				}
				else {
					console.println("problem name not identitified");
				}
			}
		}
		return result;
	},
	checkAllConditionals: function (a) {
		var result = false;

		for (var j = 0; j < a.length; j++) {
			if (this.checkConditional(a[j]))
				result = true;
		}
		return result;
	},
	checkConditional: function (o) {
		console.println("conditional - " + o.test);
		var match = false;
		var fld = gf(o.test);
		if (fld != null) {
			var v = gfvx(o.test);

			for (var i = 0; i < o.value.length; i++) {
				if (v == o.value[i]) {
					match = true;
					break;
				}
			}
		}

		// if match is true, set required if there is no value
		// if match is false, set required = false
		if (match == true)
			return this.mandatoryIfEmpty(o.rq);
		else {
			for (var i = 0; i < o.rq.length; i++) {
				var fld = gf(o.rq[i][0]);			// use the same arrays for all forms and account for little differences
				if (fld != null)
					setRequired(fld, false);
			}
		}
		return false;
	}
}

console.println("Validation Loaded");

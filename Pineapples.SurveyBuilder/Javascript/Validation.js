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
	onStaff: function(index) {
		return this.TL(index, "OnStaff");
	},

	// on staff value
	onStaffv: function (index) {
		return (this.onStaff(index) != null ? this.onStaff(index).value:null);
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
		return (this.TL(index, "tID").value?true:false);
	},

	TeacherName: function (index) {
		return this.TL(index, "FirstName").value + " " + this.TL(index, "FamilyName").value
	},

	TL: function(index, item) {
		// Ensure the index is two digits by padding with a leading zero if necessary
		var formattedIndex = padStart(index,2,"0");
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
	doAllValidations: function () {
		if (!this.doCheckAllOnStaff()) return false;
		Inform("All data is verified valid");
	},
	doCheckAllOnStaff: function() {
		var result = this.checkAllOnStaff();
		if (result == -1) return true;
		var teacher = this.TeacherName(result);
		pvalert("On Staff (Y/N) is not set for expected teacher " + teacher);
	}

}

console.println("Validation Loaded");

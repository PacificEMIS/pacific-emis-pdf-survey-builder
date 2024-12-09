console.println("Validation Loading...");
/// validations object
var v = {

	/// onStaff is On if the corresponding tID is empty, and tFamiy is supplied
	// onStaff is Off if tID is empty, and tFamiy is empty
	// OnStaff is mandatory when tId is supplied (ie prepopulated data)
	checkAllOnStaff: function () {
		i = 0;
		while ((this.OnStaff(i) != null)) { 
			if (!this.checkOnStaff(i)) {
				return i
			}
		}
		return -1; //true
	},

	// on staff field
	onStaff: function(index) {
		return TL(i, "OnStaff");
	},

	// on staff value
	onStaffv: function (index) {
		return (this.onStaff(i) != null ? this.onStaff(i).value:null);
	},

	// returns true if the OnStaff flag is valid, false otherwise
	checkOnStaff: function (i) {
		// if its prepopped, it has to have a value
		if (tlPrePop(i)) {
			return (!emptyCheckbox(this.onStaffv));
		}
		return true;
	},

	tlPrePop: function (i) {
		return (this.TL(i, "tID").value != null);
	},

	TL: function(index, item) {
		// Ensure the index is two digits by padding with a leading zero if necessary
		var formattedIndex = padStart(index,2,"0");
		// Construct the field name
		var fieldName = "TL." + formattedIndex + "." + item;
		app.alert(fieldName);
		// Get the field using the constructed name
		return gf(fieldName);
	},

	test: function (i) {
		return i;
	}

}

console.println("Validation Loaded");

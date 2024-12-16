console.println("Actions Loading...");

var actions = {

	// setting the Onstaff switch for a prepopped teacher
	onOnStaffChange: function (event) {
		// first extract the tID part
		var n = event.target.name;

		var iD = gfv(n.replace("OnStaff", "tID"));
		if (!iD) {
			// that means its not predefined
			return;
		}
		return this.applyOnStaff(event.target);
	},
	applyOnStaff: function (fldonstaff) {
		var n = fldonstaff.name;
		var ffamily = gf(n.replace("OnStaff", "FamilyName"));
		var ffirst = gf(n.replace("OnStaff", "FirstName"));

		var palegreen = ["RGB", 0.88, 1, 0.88];
		var palegrey = ["RGB", 0.9, 0.9, 0.9];
		switch (fldonstaff.value) {
			case "Y":
				ffamily.fillColor = palegreen;
				ffirst.fillColor = palegreen;
				ffamily.fontColor = color.black;
				ffirst.fontColor = color.black;
				break;
			case "N":
				ffamily.fillColor = palegrey;
				ffirst.fillColor = palegrey;
				ffamily.fontColor = color.dkGray;
				ffirst.fontColor = color.dkGray;
				break;
			default:
				ffamily.fillColor = color.white;
				ffirst.fillColor = color.white;
				ffamily.fontColor = color.black;
				ffirst.fontColor = color.black;

		}
		// now control the readonly status of everything in this group
		var tag = fldonstaff.name.replace("OnStaff", "");
		for (var i = 0; i < gthis().numFields; i++) {
			if (gthis().getNthFieldName(i).startsWith(tag)) {
				var nn = gthis().getNthFieldName(i);
				var fieldID = nn.replace(tag, "");
				switch (fieldID) {
					case "OnStaff":
					case "FamilyName":
					case "FirstName":
						break;
					default:
						var teacherfield = gf(nn);
						switch (fldonstaff.value) {
							case "Y":
								teacherfield.readonly = false;
								break;
							default:
								teacherfield.readonly = true;

						}
						switch (teacherfield.type) {
							case "text":
							case "combobox":
								switch (fldonstaff.value) {
									case "Y":
										teacherfield.fillColor = gf("Survey.Comment").fillColor;
										break;
									default:
										teacherfield.fillColor = palegrey;

								}
						}
				}
			}
		}
	}
}



console.println("Actions Loaded...");

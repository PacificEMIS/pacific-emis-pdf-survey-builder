console.println("Pineapples Loading...");

var p = {
	gridTotal: function (tag) {

		var darr = this.getField(tag + '.D').getArray();



		var rtots = {};
		var ctots = {};

		for (var i = 0; i < darr.length; i++) {
			var d = darr[i];
			var v = d.value;
			if (v >= 0) {
				var n = d.name.split('.');
				var r = n[2];
				var c = n[3];
				var g = n[4];
				console.println(r);
				if (rtots[r] == undefined) {
					rtots[r] = { M: 0, F: 0, T: 0 };
				};
				rtots[r][g] += v;
				rtots[r].T += v;

				if (ctots[c] == undefined) {
					ctots[c] = { M: 0, F: 0, T: 0 };
				};
				ctots[c][g] += v;
				ctots[c].T += v;


			};
		};
	},
	gf: function (host, a) {
		return host.getField(a.join("."));
	},
	setTot: function (host, namearr, delta) {
		var t = this.gf(host, namearr);
		if (t) {
			t.value += delta;
			if (t.value == 0) {
				t.value = t.defaultValue;
			}
		}
	},
	// validator function for grids - updates the 5 affected totals
	numchk: function (event, min, max, intReq) {
		if (event.value < min) {
			if (min == 0) {
				app.alert('Value cannot be negative');
			}
			else {
				app.alert('Enter a value >= ' + min);
			}
			event.rc = false;
			return;
		};
		if (event.value > max) {
			app.alert('Value cannot be greater than ' + max);
			event.rc = false;
			return;
		};
		if (intReq && (event.value % 1 != 0)) {
			app.alert('Enter a whole number - no decimal places');
			event.rc = false;
			return;
		};

	},
	grdv: function (event) {
		// first validate that the number is not -ve, or > 1000 or a fraction
		this.numchk(event, 0, 9999, true);
		if (event.rc == false)
			return;

		var delta = event.value - event.target.value;
		var n = event.target.name.split('.');
		var r = n[2];
		var c = n[3];
		var g = n[4];

		var host = event.target.doc;

		this.setTot(host, [n[0], 'T', 'T', n[3], n[4]], delta);
		this.setTot(host, [n[0], 'T', n[2], 'T', n[4]], delta);
		this.setTot(host, [n[0], 'T', 'T', 'T', n[4]], delta);

		if (g == 'M' || g == 'F') {
			this.setTot(host, [n[0], 'T', 'T', n[3], 'T'], delta);
			this.setTot(host, [n[0], 'T', n[2], 'T', 'T'], delta);
			this.setTot(host, [n[0], 'T', 'T', 'T', 'T'], delta);
		};
	},
	/// get the field name of the row header field for the event target
	grdrh: function (name) {
		var n = name.split('.');
		return [n[0], "R", n[2], "V"].join('.');

	},
	grdch: function (name) {
		var n = name.split('.');
		return [n[0], "C", n[3], "V"].join('.');

	},
	// focus event in the grid, change row / col header colors
	grdfo: function (event) {
		var host = event.target.doc;
		var colname = this.grdch(event.target.name);
		var col = host.getField(colname);
		var rowname = this.grdrh(event.target.name);
		var row = host.getField(rowname);

		//console.println('col ' + col.fillColor + ' ' + col.textColor);
		if (col != null) {
			col.userName = col.fillColor;
			//console.println('col.userName ' + col.userName);
			col.fillColor = color.dkGray;
			col.textColor = color.white;
			col.textSize = 8;
		}
		if (row != null) {
			row.userName = row.fillColor;
			//console.println('col.userName ' + col.userName);
			row.fillColor = color.dkGray;
			row.textColor = color.white;

			row.textSize = 8;
		}
	},
	// blur event in the grid, restore original colors
	grdbl: function (event) {

		var host = event.target.doc;
		var colname = this.grdch(event.target.name);
		var col = host.getField(colname);
		var rowname = this.grdrh(event.target.name);
		var row = host.getField(rowname);

		if (col != null) {
			col.fillColor = col.userName.split(',');
			col.textColor = color.black;
			col.textSize = 8;
		}
		if (row != null) {
			row.fillColor = row.userName.split(',');
			row.textColor = color.black;
			row.textSize = 8;
		}
	},


	// validator for on staff ( Y / N) - sets the remaining fields in the row editable or not editable
	tlov: function (event) {
		var present = (event.value == 'Y');
		var n = event.target.name.split('.');
		// name looks like TL.02.DoB ie. TL.rr.<datapt>
		var r = n[1];
		var host = event.target.doc;
		this.gf(host, [n[0], n[1], "Gender"]).readonly = !present;
		this.gf(host, [n[0], n[1], "DoB"]).readonly = !present;
		this.gf(host, [n[0], n[1], "Role"]).readonly = !present;
		this.gf(host, [n[0], n[1], "Duties"]).readonly = !present;
		this.gf(host, [n[0], n[1], "FP"]).readonly = !present;

	},
	// validator for funding

	fv: function (event) {
		var delta = event.value - event.target.value;
		var n = event.target.name.split('.');
		var rx = n[1];

		var host = event.target.doc;
		this.setTot(host, [n[0], n[1], 'Total'], delta);
		delta = (rx == 'Exp' ? -1 : 1) * delta;
		this.setTot(host, [n[0], 'Total'], delta);
	},

	// new date handling
	dateKeystrokeUI: function (event) {
		if (event.willCommit) {
			if (event.value == "") {
				event.rc = true;
				return;
			}
			else {
				// Single regex to handle all valid formats
				//var rx = /^((?:19|20)\d{2})(?:-|\/)(\d{1,2})(?:-|\/)(\d{1,2})$/;
var rx = /^((?:19|20)\d{2})(?:-|\/)(\d{1,2})(?:-|\/)(\d{1,2})|(\d{1,2})(?:-|\/)(\d{1,2})(?:-|\/)((?:19|20)?\d{2})$/;

				console.println(rx);
				console.println(event.value);
				var matches = event.value.match(rx);
				console.println(matches);
				if (matches) {
					
					// Extract year, month, and day using regex groups
					var [_, yyyy, m, d,dd,mm,yy,] = matches;

					if (yyyy) {		// match year first pattern
						// Convert to integers for date comparison
						yyyy = parseInt(yyyy, 10);
						m = parseInt(m, 10); // Month
						d = parseInt(d, 10); // Day
					} else {
						// match d-m-y pattern
						yy = parseInt(yy, 10);
						m = parseInt(mm, 10); // Month
						d = parseInt(dd, 10); // Day
						var currentYear = new Date().getFullYear();
						var currentLastTwoDigits = currentYear % 100;
						yyyy = (yy >= 1900 ? yy : ((yy > currentLastTwoDigits) ? 1900 : 2000) + yy);
					}
					if (m > 12) {			// catch m-d-y where possible
						[d, m] = [m, d];
					}
					// Create Date object (month is 0-based in JavaScript);
					var dd = new Date(yyyy, m - 1, d);
					console.println(dd);
					// Validate the date components
					event.rc = (dd.getFullYear() === yyyy && dd.getMonth() + 1 === m && dd.getDate() === d);
					if (event.rc) {
						var formattedDate = dd.getFullYear() + "-" +
							("0" + (dd.getMonth() + 1)).slice(-2) + "-" +
							("0" + dd.getDate()).slice(-2);

						// Set the value of the current field
						event.value = formattedDate;
						return;
					}
				}
				inform("Enter a date value in the format\nyyyy-mm-dd");
				event.rc = false;
			}
		}
		if (!event.willCommit) {
			console.println("event.change " + event.change)
			var rxc = /^([0-9]|-|\/){0,10}$/;
			event.rc = rxc.test(event.change);
			if (event.rc == false)
				app.beep();
		}
	}

};

console.println("Pineapples loaded");

//app.alert(v);
//app.alert(v.test(1));
//app.alert("2".toString());
//app.alert(padStart);
////app.alert(globalThis);
//app.alert(v.TL(2, "tID"));
//app.alert(v.onStaff(0));
//app.alert(v.onStaffv(1));
//app.alert(v.checkOnStaff(1));
//app.alert(v.checkAllOnStaff());
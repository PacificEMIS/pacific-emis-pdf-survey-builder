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
				rtots[r] = {M: 0, F:0, T: 0};
			};
			rtots[r][g] += v;
			rtots[r].T += v;
		
			if (ctots[c] == undefined) {
				ctots[c] = {M: 0, F:0, T: 0};
			};
			ctots[c][g] += v;
			ctots[c].T += v;
			
			
		};
            };
	},
	gf: function (host, a) {
		return host.getField(a.join("."));
	},
	setTot: function(host, namearr, delta) {
		var t = this.gf(host, namearr);
		if  (t) {
			t.value += delta;
			if (t.value == 0) {
				t.value = t.defaultValue;
			}
		}
	},
// validator function for grids - updates the 5 affected totals
	numchk: function(event, min, max, intReq) {
		if (event.value < min ) {
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
		if (intReq && (event.value %1 != 0)) {
			app.alert('Enter a whole number - no decimal places');
			event.rc = false;
			return;
		};
			
	},
	grdv: function(event) {
		// first validate that the number is not -ve, or > 1000 or a fraction
		this.numchk(event, 0,9999, true);
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
// validator for on staff ( Y / N) - sets the remaining fields in the row editable or not editable
	tlov: function(event) {
		var present = (event.value == 'Y');
		var n = event.target.name.split('.');
		// name looks like TL.02.DoB ie. TL.rr.<datapt>
		var r = n[1];
		var host = event.target.doc;
		this.gf(host,[n[0],n[1],"Gender"]).readonly = !present;
		this.gf(host,[n[0],n[1],"DoB"]).readonly = !present;
		this.gf(host,[n[0],n[1],"Role"]).readonly = !present;
		this.gf(host,[n[0],n[1],"Duties"]).readonly = !present;
		this.gf(host,[n[0],n[1],"FP"]).readonly = !present;
		
	},
// validator for funding
	
	fv: function(event) {
		var delta = event.value - event.target.value;
		var n = event.target.name.split('.');
		var rx = n[1];
		
		var host = event.target.doc;
		this.setTot(host, [n[0], n[1], 'Total'], delta);
		delta = (rx=='Exp'? -1:1) * delta;
		this.setTot(host, [n[0], 'Total'], delta);
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
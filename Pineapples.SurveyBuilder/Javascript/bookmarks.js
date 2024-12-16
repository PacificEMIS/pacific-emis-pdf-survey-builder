console.println("bookmarks Loading...");

// Unfortunately, this code has proved unusable
// due to security constraints - remove from project but keep in repo to preserve

// Function to find a bookmark by name, including nested ones
function findBookmark(bookmark, name) {
	console.println("Entering findBookmark " + bookmark + ' ' + bookmark.name + ' ' + name);

	inform("Entering findBookmark " + bookmark + ' ' + bookmark.name + ' ' + name);
	return bookmark;

	if (!bookmark) {
		console.println("Finding bookmark NULL Bookmark");
		return null;    // shouldnt happen

	}
	console.println("Finding bookmark " + name + ' in ' + bookmark.name);
	inform("Finding bookmark " + name + ' in ' + bookmark.name);
	if (bookmark.name == name) {
		console.println("returned " + bookmark);
		return bookmark;
	}
	if (!bookmark.children) {
		return null;
	}
	//var found;
	//for (var i = 0; i < bookmark.children.length; i++) {
	//  found = this.findBookmark(bookmark.children[i], name);
	//  if (found) {
	//    inform(found + found.name + ' ' + bookmark.name);
	//    return found;
	//  }
	//}
	inform("NULL exit at " + bookmark.name);

	return null;
}

// Function to find a bookmark by its stored index under a specific parent
function findBookmarkByIndex(parentBookmark, index) {
	console.println("Finding bookmark by index" + index + ' in ' + parentBookmark.name);
	if (!parentBookmark.children) {
		return null;
	}
	for (var i = 0; i < parentBookmark.children.length; i++) {
		var bookmark = parentBookmark.children[i];
		var action = bookmark.action;

		// Check if the bookmark's action contains the index
		if (action) {
			var match = action.match(/\/\/Index: (\d{2})/);
			if (match && match[1] === index) {
				return bookmark;
			}
		}
	}
	return null;
}

// Function to add or update bookmarks for existing staff
function addStaffBookmarks() {
	// Find the 'School Staff' parent bookmark
	console.println("addStaffBookmarks");
	var schoolStaffBookmark = findBookmark(gthis().bookmarkRoot, "School Staff Information");
	
	if (!schoolStaffBookmark) {
		console.println("Parent bookmark 'School Staff Information' not found.");
		return;
	}

	// Find the 'Existing Staff' child bookmark under 'School Staff'
	var existingStaffBookmark = findBookmark(schoolStaffBookmark, "Expected Staff List");
	if (!existingStaffBookmark) {
		console.println("Child bookmark 'Expected Staff List' not found.");
		return;
	}
	existngStaffBookmark.name = "Fred Nurk";

	// Loop through all fields in the document
	for (var i = 0; i < gthis().numFields; i++) {
		var fieldName = gthis().getNthFieldName(i);

		// Match field names of the form TL.{i:00}.tID
		var match = fieldName.match(/^TL\.(\d{2})\.tID$/);
		if (match) {
			var index = match[1]; // Extract the index portion

			var tIDField = gf(fieldName);
			if (tIDField && tIDField.value) {
				var familyNameField = gf("TL." + index + ".FamilyName");
				var firstNameField = gf("TL." + index + ".FirstName");
				var onStaffField = gf("TL." + index + ".OnStaff");

				var familyName = familyNameField ? familyNameField.value : "";
				var firstName = firstNameField ? firstNameField.value : "";
				var onStaff = onStaffField ? onStaffField.value : "";

				var bookmarkTitle = familyName + ", " + firstName + " " + onStaff;

				// Find existing bookmark by index
				var bookmark = this.findBookmarkByIndex(existingStaffBookmark, index);

				if (bookmark) {
					// Update the title if the bookmark exists
					bookmark.name = bookmarkTitle;
					console.println("Updated bookmark: " + bookmarkTitle);
				} else {
					// Create a new bookmark and store the index
					var newBookmark = existingStaffBookmark.createChild(bookmarkTitle, "pageNum=0");
					newBookmark.setAction("//Index: " + index);
					console.println("Added new bookmark: " + bookmarkTitle);
				}
			}
		}
	}
}

console.println("bookmarks loaded");

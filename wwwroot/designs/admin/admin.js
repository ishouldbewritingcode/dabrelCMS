window.onload = function () {
	addEvents();
}

function addEvents() {
	document.getElementById('adminsitecontrol').addEventListener('click', function (e) {
		document.getElementById('overlaybox').showModal();
	}, false);
	document.getElementById('adminsitesave').addEventListener('click', function (e) {
		document.getElementById('overlaybox').close();
	}, false);
	document.getElementById('overlayclose').addEventListener('click', function (e) {
		document.getElementById('overlaybox').close();
	}, false);
}
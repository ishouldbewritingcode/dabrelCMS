window.onload = function () {
	addEvents();
}

function addEvents() {
	document.getElementById('adminsitecontrol').addEventListener('click', function () {
		document.getElementById('overlay').style.display = 'flex';
		document.getElementById('overlayadminsite').style.display = 'block';
	}, false);
	document.getElementById('adminsitesave').addEventListener('click', function () {
		document.getElementById('overlay').style.display = 'none';
		document.getElementById('overlayadminsite').style.display = 'none';
	}, false);
	document.getElementById('overlayclose').addEventListener('click', function (e) {
		document.getElementById('overlay').style.display = 'none';
		document.getElementById('overlayadminsite').style.display = 'none';
	}, false);
}
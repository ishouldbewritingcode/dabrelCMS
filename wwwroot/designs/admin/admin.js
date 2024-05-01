
$(document).ready(function () {
	$('#cmsdialogclose').click(function () { document.getElementById('cmsdialog').close(); });
});

function SetupEvents() {
	console.log('setup events');
	$('#content').click(function () { LetsEdit(); });
	$('#ce_pagetitle').click(function () { LetsEdit(); });
}

function LetsEdit() {
	console.log('let edit');
	$('#content').trumbowyg();
	$('.buttonright.hide').show();
}

//document.getElementById('cmsdialogclose').addEventListener('click', function () {
//	document.getElementById('cmsdialog').close();
//});


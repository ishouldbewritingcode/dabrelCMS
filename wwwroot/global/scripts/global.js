var nav = document.getElementById('nav');
var navitems = nav.querySelectorAll('a');
Array.from(navitems).forEach(function (item) {
	item.addEventListener('click', navClicked, false);
});

function navClicked(event) {
	// turn off all paths
	var paths = nav.querySelectorAll('li.path');
	Array.from(paths).forEach(function (p) {
		p.classList.remove('path');
	});
	// add the correct path
	this.parentNode.classList.add('path');
	document.getElementById('nav-toggle').checked = false; //close mobile nav
}

var subs = nav.getElementsByClassName('sub');
Array.from(subs).forEach(function (sub) {
	sub.addEventListener('click', openChildNav, false);
});

function openChildNav(event) {
	event.stopPropagation();
	if (this.classList.contains('open')) {
		this.classList.remove('open');
		this.parentNode.classList.remove('open');
	}
	else {
		this.classList.add('open');
		this.parentNode.classList.add('open');
	}
}
let nav = document.getElementById('nav');
let sb = document.getElementById('searchbox');
let navitems = nav.querySelectorAll('a');
Array.from(navitems).forEach(function (item) {
	item.addEventListener('click', navClicked, false);
});

document.getElementById('lightdark').addEventListener('click', function () {
	let thehtml = document.body.parentElement;
	if (thehtml.classList.contains('dark')) {
		thehtml.classList.remove('dark');
		thehtml.classList.add('light');
	}
	else { 
		thehtml.classList.remove('light');
		thehtml.classList.add('dark');
	}
});

// mobile nav
document.getElementById('navicon').addEventListener('click', function () {
	if (nav.classList.contains('show'))
		nav.classList.remove('show');
	else
		nav.classList.add('show');
});

// mobile search
document.getElementById('searchicon').addEventListener('click', function () {
	if (sb.classList.contains('show'))
		sb.classList.remove('show');
	else
		sb.classList.add('show');
});

function navClicked(event) {
	// turn off all paths / opens
	let paths = nav.querySelectorAll('.path');
	Array.from(paths).forEach(function (p) {
		p.classList.remove('path');
	});
	let opens = nav.querySelectorAll('.open');
	Array.from(opens).forEach(function (p) {
		p.classList.remove('open');
	});
	// add the correct path
	this.parentNode.classList.add('path');
	document.getElementById('nav').classList.remove('show');
}

let subs = nav.getElementsByClassName('sub');
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
		let opens = nav.querySelectorAll('.open');
		Array.from(opens).forEach(function (p) {
			p.classList.remove('open');
		});
		this.classList.add('open');
		this.parentNode.classList.add('open');
	}
}
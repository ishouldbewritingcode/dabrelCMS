let nav = document.getElementById('nav');
let navitems = nav.querySelectorAll('a');
Array.from(navitems).forEach(function (item) {
	item.addEventListener('click', navClicked, false);
});

if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
	// browser in dark mode - add the class unless they've switched
	if (getCookie('lightdark') == 'light') {
		document.body.parentElement.classList.add('light');
	}
	else {
		document.body.parentElement.classList.add('dark');
		$('.lightdark i').removeClass('fa-moon').addClass('fa-sun');
	}
}
document.getElementById('lightdark').addEventListener('click', function () { lightDark(); });

// mobile nav
document.getElementById('navicon').addEventListener('click', function () {
	if (nav.classList.contains('show'))
		nav.classList.remove('show');
	else
		nav.classList.add('show');
});

// mobile search
document.getElementById('searchicon').addEventListener('click', function () {
	if ($('#searchform').hasClass('show'))
		$('#searchform').removeClass('show');
	else
		$('#searchform').addClass('show');
});

let subs = nav.getElementsByClassName('sub');
Array.from(subs).forEach(function (sub) {
	sub.addEventListener('click', openChildNav, false);
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

function lightDark() {
	let thehtml = document.body.parentElement;
	if (thehtml.classList.contains('dark')) {
		thehtml.classList.remove('dark');
		thehtml.classList.add('light');
		$('.lightdark i').removeClass('fa-sun').addClass('fa-moon');
		setCookie('lightdark', 'light', 1)
	}
	else {
		thehtml.classList.remove('light');
		thehtml.classList.add('dark');
		$('.lightdark i').removeClass('fa-moon').addClass('fa-sun');
		setCookie('lightdark', 'dark', 1)
	}
}

// cookie functions
function setCookie(name, value, hours) {
	var expires = "";
	if (hours) {
		var date = new Date();
		date.setTime(date.getTime() + (hours * 60 * 60 * 1000));
		expires = "; expires=" + date.toUTCString();
	}
	document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(name) {
	var nameEQ = name + "=";
	var ca = document.cookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') c = c.substring(1, c.length);
		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
	}
	return '';
}
function eraseCookie(name) {
	document.cookie = name + '=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}
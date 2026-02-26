var icons = [
	"fa-solid fa-screwdriver-wrench",
	"fa-solid fa-hammer",
	"fa-solid fa-wrench",
	"fa-solid fa-laptop-code",
	"fa-solid fa-print",
	"fa-solid fa-screwdriver",
	"fa-solid fa-calculator",
	"fa-solid fa-toolbox",
	"fa-solid fa-computer",
	"fa-solid fa-gear",
	"fa-solid fa-gears",
	"fa-solid fa-ruler",
	"fa-brands fa-raspberry-pi",
	"fa-brands fa-aws",
	"fa-brands fa-microsoft",
];

document.addEventListener('DOMContentLoaded', function () {
	var count = 12;

	// generate 12 distinct sizes between 2rem and 5rem and shuffle
	var sizes = [];
	for (var i = 0; i < count; i++) {
		var s = 2 + (3 * i) / (count - 1); // linear from 2 to 5
		sizes.push(parseFloat(s.toFixed(3)));
	}
	sizes = sizes.sort(function () { return 0.5 - Math.random(); });

	// prepare icon order: use each icon once before reusing
	var pool = icons.slice();
	var iconOrder = [];
	function shuffleArr(a) { for (var i = a.length - 1; i > 0; i--) { var j = Math.floor(Math.random() * (i + 1)); var t = a[i]; a[i] = a[j]; a[j] = t; } }
	shuffleArr(pool);
	while (iconOrder.length < count) {
		if (pool.length === 0) { pool = icons.slice(); shuffleArr(pool); }
		iconOrder.push(pool.pop());
	}

	// collision detection: store placed rects
	var rects = [];
	function intersects(r1, r2) {
		return !(r1.right < r2.left || r1.left > r2.right || r1.bottom < r2.top || r1.top > r2.bottom);
	}

	for (var i = 0; i < count; i++) {
		var iconClass = iconOrder[i];
		var el = document.createElement('i');
		el.className = iconClass + ' bg-icon';
		el.setAttribute('aria-hidden', 'true');
		el.style.fontSize = sizes[i] + 'rem';
		// add subtle random opacity for depth (increased minimum so icons are visible)
		el.style.opacity = (0.32 + Math.random() * 0.48).toFixed(3);
		el.style.visibility = 'hidden';
		document.body.appendChild(el);

		// measure and attempt positions until no overlap or max attempts
		var maxAttempts = 300;
		var placed = false;
		var lastRect = null;
		for (var attempt = 0; attempt < maxAttempts; attempt++) {
			var left = (Math.random() * 90) + 5; // 5%..95%
			var top = (Math.random() * 90) + 5;
			el.style.left = left + '%';
			el.style.top = top + '%';
			// force reflow to get accurate bounds
			var r = el.getBoundingClientRect();
			lastRect = r;
			var ok = true;
			for (var k = 0; k < rects.length; k++) {
				if (intersects(r, rects[k])) { ok = false; break; }
			}
			if (ok) { rects.push(r); el.style.visibility = 'visible'; placed = true; break; }
		}
		if (!placed) { // accept last position if no free spot found
			rects.push(lastRect);
			el.style.visibility = 'visible';
		}
	}
});

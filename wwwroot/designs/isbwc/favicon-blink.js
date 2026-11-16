// Blinking-cursor favicon: swaps the tab icon every 550 ms.
(function () {
  if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;
  var link = document.querySelector('link[rel="icon"]');
  if (!link) return;
  var on = true;
  setInterval(function () {
    on = !on;
    link.href = on ? '/designs/isbwc/images/favicon-on.svg' : '/designs/isbwc/images/favicon-off.svg';
  }, 550);
})();

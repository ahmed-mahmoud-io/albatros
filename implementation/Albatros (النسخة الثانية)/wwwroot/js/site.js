(function () {
  var splash = document.getElementById('splash');
  if (splash) {
    document.body.style.overflow = 'hidden';
    var dismissed = false;
    function dismiss() {
      if (dismissed) return;
      dismissed = true;
      splash.classList.add('hide');
      document.body.style.overflow = '';
      setTimeout(function () { splash.style.display = 'none'; }, 700);
    }
    splash.addEventListener('click', dismiss);
    setTimeout(dismiss, 2600);
  }

  var hamburger = document.getElementById('hamburger');
  var mobileNav = document.getElementById('mobileNav');
  if (hamburger && mobileNav) {
    hamburger.addEventListener('click', function () {
      hamburger.classList.toggle('open');
      mobileNav.classList.toggle('open');
      document.body.style.overflow = mobileNav.classList.contains('open') ? 'hidden' : '';
    });
    mobileNav.querySelectorAll('a').forEach(function (link) {
      link.addEventListener('click', function () {
        hamburger.classList.remove('open');
        mobileNav.classList.remove('open');
        document.body.style.overflow = '';
      });
    });
  }
})();

/* Heart / Favorite toggle */
function toggleFav(btn, e) {
  if (e) e.stopPropagation();
  var pid = btn.getAttribute('data-id');
  btn.classList.toggle('active');
  var isActive = btn.classList.contains('active');
  var svg = btn.querySelector('svg');
  svg.setAttribute('fill', isActive ? '#9C2A2A' : 'none');

  fetch('/Favorites/ToggleAjax', {
    method: 'POST',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    body: 'propertyId=' + pid + '&__RequestVerificationToken=' + encodeURIComponent(document.querySelector('input[name="__RequestVerificationToken"]')?.value || '')
  })
  .then(function (r) { return r.json(); })
  .then(function (d) {
    if (!d.success) {
      btn.classList.toggle('active');
      svg.setAttribute('fill', btn.classList.contains('active') ? '#9C2A2A' : 'none');
      if (d.requireLogin) window.location.href = '/Account/Login';
    }
  })
  .catch(function () {
    btn.classList.toggle('active');
    svg.setAttribute('fill', btn.classList.contains('active') ? '#9C2A2A' : 'none');
  });
}

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
})();

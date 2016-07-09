//smooth scrolling by navigation
$(function() {
  $('a[href*="#"]:not([href="#"])').click(function() {
    if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'') && location.hostname == this.hostname) {
      var target = $(this.hash);
      target = target.length ? target : $('[name=' + this.hash.slice(1) +']');
      if (target.length) {
        $('html, body').animate({
          scrollTop: target.offset().top
        }, 1000);
        return false;
      }
    }
  });
});

//smaller menue when scrolling
$(document).ready(function(){
	$(window).scroll(function() { 
		if($(this).scrollTop() >= 100){
			$('header').addClass('little_menue');
		} else {
			$('header').removeClass('little_menue');
		}
	});
});
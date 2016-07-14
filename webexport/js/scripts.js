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

    var slider_options = {
        orientation: "horizontal",
        range: "min",
        min: 1,
        max: 100,
        value: 50,
        slide: function(event, ui) {
            $("#speed_value").html(ui.value);
        }
    }

    var color1 = $("input[name='player1_color']").val();
    var color2 = $("input[name='player2_color']").val();

    $("#speed_slider").slider(slider_options);

    $("#speed_value").html(slider_options.value);

	$(window).scroll(function() { 
		if($(this).scrollTop() >= 100){
			$('header').addClass('little_menue');
		} else {
			$('header').removeClass('little_menue');
		}
	});

    $("input[name='player_1_name']").on('input', function(){
        var name_player1 = "Player 1";
        if(this.value.trim()){
           name_player1 = this.value;
        } else {
            name_player1 = "Player 1";
        }
        $("#label_col_1").html(name_player1 + ": ");
    });

    $("input[name='player_2_name']").on('input', function(){
        var name_player2 = "Player 2";
        if(this.value.trim()){
           name_player2 = this.value;
        } else {
            name_player2 = "Player 2";
        }
        $("#label_col_2").html(name_player2 + ": ");
    });

    $("input[type='color']").on('input', function(){
        if ($(this).attr('name') == 'player1_color'){
            color1 = $(this).val();
        } else if ($(this).attr('name') == 'player2_color'){
            color2 = $(this).val();
        }
    });
});

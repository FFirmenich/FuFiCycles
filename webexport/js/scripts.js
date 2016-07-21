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
			$mainAssembly.Fusee.FuFiCycles.Web.FuFiCycles.setSpeed(ui.value);
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
		$mainAssembly.Fusee.FuFiCycles.Web.FuFiCycles.setPlayerName(1, name_player1);
	});

	$("input[name='player_2_name']").on('input', function(){
		var name_player2 = "Player 2";
		if(this.value.trim()){
		   name_player2 = this.value;
		} else {
			name_player2 = "Player 2";
		}
		$("#label_col_2").html(name_player2 + ": ");
		$mainAssembly.Fusee.FuFiCycles.Web.FuFiCycles.setPlayerName(2, name_player2);
	});

	$("input[type='color']").on('input', function(rgb){
		if ($(this).attr('name') == 'player1_color'){
			color1 = hexToRgb($(this).val());
			$mainAssembly.Fusee.FuFiCycles.Web.FuFiCycles.setCycleColor(1, color1.r / 255, color1.g / 255, color1.b / 255);
		} else if ($(this).attr('name') == 'player2_color'){
			color2 = hexToRgb($(this).val());
			$mainAssembly.Fusee.FuFiCycles.Web.FuFiCycles.setCycleColor(2, color2.r / 255, color2.g / 255, color2.b / 255);
		}
	});
	//window.onresize();
});
	function hexToRgb(hex) {
		var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
		return result ? {
			r: parseInt(result[1], 16),
			g: parseInt(result[2], 16),
			b: parseInt(result[3], 16)
		} : null;
	}
	function runMain () {
		$mainAssembly = JSIL.GetAssembly("Fusee.FuFiCycles.Web");
		$mainAssembly.Fusee.FuFiCycles.Web.FuFiCycles.setWEB();
		$mainAssembly.Fusee.FuFiCycles.Web.FuFiCycles.Main([]);
	};

	window.onresize = function(event) {
		/*
		var canvas = document.getElementById("canvas");
		canvas.setAttribute('width', $('#game').width());
		canvas.setAttribute('height', $('#game').height());
		*/
	}
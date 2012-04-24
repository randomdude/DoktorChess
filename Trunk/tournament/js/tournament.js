
$(document).ready(function() {
    $("div.roundbox").wrap('<div class="dialog"><div class="bd">' + '<div class="c"><div class="s"></div></div></div></div>');
    $('div.dialog').prepend('<div class="hd">' + '<div class="c"></div></div>').append('<div class="ft">' + '<div class="c"></div></div>');

    $("div.wideroundbox").wrap('<div class="widedialog"><div class="bd">' + '<div class="c"><div class="s"></div></div></div></div>');
    $('div.widedialog').prepend('<div class="hd">' + '<div class="c"></div></div>').append('<div class="ft">' + '<div class="c"></div></div>');

    // Hide children of 'expandable' links, and show them on click to the parent item
    $("a.expandable").parent().children('.expandableChild').hide();
    $("a.expandable").click(function() {
    $(this).parent().children('.expandableChild').toggle("slow");
    });
});
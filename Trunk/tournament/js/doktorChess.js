var xhr;
var gameFinished = false;

function doMove(moveObject) {
    var queryString = "moveHandler.ashx?ourMove=" + JSON.stringify(moveObject);

    markThinking();
    xhr = new XMLHttpRequest();
    xhr.open("GET", queryString, true);
    xhr.onreadystatechange = xhrCallback;
    xhr.send();
};

function xhrCallback() {
    if (xhr.readyState != 4)
        return;
    markNotThinking();

    var parsedJSON = eval("(" + xhr.responseText + ")");

    if (!parsedJSON.isValid) {
        alert('Illegal move');

        // Reload the board table to remove the bad move
        $("#board")[0].innerHTML = parsedJSON.newBoardHTML;
        initDraggables();
        markNotThinking();
        return;
    }

    $("#gameLog")[0].innerHTML += parsedJSON.moveNum + " : " + parsedJSON.whiteMove;

    if (parsedJSON.gameFinished) {
        gameFinished = true;
        $("#gameStatus")[0].innerHTML = "the game is " + parsedJSON.gameResult + "!";
        return;
    }

    $("#gameLog")[0].innerHTML += " " + parsedJSON.blackMove + "\n";
    $("#bestLine")[0].innerHTML = parsedJSON.bestLine;

    // find destination and source squares
    var srcsq = $("[x = " + parsedJSON.movedPieceSrc.x + "][y = " + parsedJSON.movedPieceSrc.y + "]")[0];
    var dstsq = $("[x = " + parsedJSON.movedPieceDst.x + "][y = " + parsedJSON.movedPieceDst.y + "]")[0];
    var capturesq = dstsq;
    if (parsedJSON.hasExtraCaptureSquare) {
        var captureSqPos = parsedJSON.extraCaptureSquarePos;
        capturesq = $("[x = " + captureSqPos.x + "][y = " + captureSqPos.y + "]")[0];
    }

    if (parsedJSON.forceBoardReload) {
        $("#board")[0].innerHTML = parsedJSON.newBoardHTML;
        initDraggables();
    } else {
        // Do a funky 'capture' animation if this is a capture
        animateMove(srcsq, dstsq, capturesq, true);
    }
}

var imgid;
var thedstsq;
function animateMove(srcsq, dstsq, capturesq, slidePiece) {

    var table = document.getElementById("board");

    var srcImageBox = $(srcsq.children[0])[0];
    if (slidePiece == true) {
        alignImageToTD(srcImageBox, srcsq);
    }

    // The capture and dest boxes may be empty. If not, align them.
    var captureImageBox = null;
    if (capturesq != null && $(capturesq.children[0]).length > 0) {
        captureImageBox = $(capturesq.children[0])[0];
        alignImageToTD(captureImageBox, capturesq);
    }
    var dstImageBox = null;
    if ($(dstsq.children[0]).length > 0) {
        dstImageBox = $(dstsq.children[0])[0];
        alignImageToTD(dstImageBox, dstsq);
    }

    var newTop = dstsq.getClientRects()[0].top;
    var newLeft = dstsq.getClientRects()[0].left;

    // Start the 'move' animation of the piece
    if (slidePiece == true) {
        $(srcImageBox).animate({ left: newLeft, top: newTop }, 'slow', function() {
            doCaptureAnimIfNeeded(srcImageBox, dstImageBox, dstsq, captureImageBox, capturesq);
        });
    }
    else {
        doCaptureAnimIfNeeded(srcImageBox, dstImageBox, dstsq, captureImageBox, capturesq);
    }
}

function doCaptureAnimIfNeeded(srcImageBox, dstImageBox, destsq, captureImageBox, capturedsq) {
    // Fade out any captured piece.
    if (captureImageBox != null) {
        $(captureImageBox).animate({ left: '-=50',
            width: '+=100',
            top: '-=50',
            height: '+=100',
            opacity: 'toggle'
        }, 'slow', function() {
            // OK, all FX are over.
            finishMoveAnimation(destsq, capturedsq, srcImageBox);
        });
    } else {
        finishMoveAnimation(destsq, capturedsq, srcImageBox);
    }
}

function finishMoveAnimation(dstsq, capturedsq, srcImageBox) {
    // All move animations are over. Finish off by removing the moving image from its old TD
    // to its new TD.
    for (var i = 0; i < dstsq.children.length; i++)
        dstsq.removeChild(dstsq.children[i]);
    if (capturedsq != null) {
        for (var i = 0; i < capturedsq.children.length; i++)
            capturedsq.removeChild(capturedsq.children[i]);
    }
    dstsq.appendChild(srcImageBox);
}

var TDBeingDragged;
var imageBeingDragged;
function initDraggables() {
    $(".piece").draggable({
        start: function(event, ui) {
            imageBeingDragged = this;
            TDBeingDragged = this.parentNode;
        }
    });

    $(".boardSquare").droppable({
        drop: function(event, ui) {
            var fromx = TDBeingDragged.cellIndex;
            var fromy = TDBeingDragged.parentNode.rowIndex;
            var tox = this.cellIndex;
            var toy = this.parentNode.rowIndex;

            // Align the dragged image nicely
            alignImageToTD(imageBeingDragged, this);

            if (gameFinished) {
                alert("the game has finished");
            }

            // If we have dropped the piece on the same square, don't bother POSTing
            if (fromx == tox && fromy == toy)
                return;

            // Do any capture animation
            animateMove(TDBeingDragged, this, this, false);

            // Tell the backend that we are moving a piece from/to these squares
            var toMove = { "srcSquarePos": { "x": fromx, "y": fromy },
                "dstSquarePos": { "x": tox, "y": toy }
            };

            doMove(toMove);
        }
    });

    // OK, we're ready.
    markReady();
}

function markThinking() {
    $(".progressIndicator")[0].style.visibility = "visible";
}

function markNotThinking() {
    $(".progressIndicator")[0].style.visibility = "hidden";
}

function markBusy() {
    $("#busygif")[0].style.visibility = "visible";
}

function markReady() {
    $("#busygif")[0].style.visibility = "hidden";
    $($("#boardDiv")[0]).removeClass('dimming');
}

function alignImageToTD(image, td) {
    image.style.left = td.getClientRects()[0].left + "px";
    image.style.top = td.getClientRects()[0].top + "px";
}

function onshowbestline() {
    if ($("#showbestline")[0].checked)
        $(".bestLineTextBoxDiv").show("slow");
    else
        $(".bestLineTextBoxDiv").hide("fast");
}

$(document).ready(function() {
    $("div.roundbox").wrap('<div class="dialog"><div class="bd">' + '<div class="c"><div class="s"></div></div></div></div>');
    $('div.dialog').prepend('<div class="hd">' + '<div class="c"></div></div>').append('<div class="ft">' + '<div class="c"></div></div>');
});

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebFrontend._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title></title>
        
        <script type="text/javascript" src="js/json.js"></script>
        <script src="js/jquery.js" type="text/javascript"></script>
        <script src="js/jquery-ui.js" type="text/javascript"></script>

        <link rel="stylesheet" href="style/styles.css" type="text/css" />
        
        <script type="text/javascript">
            var xhr;
            var gameFinished = false;
            
            function doMove(moveObject) {
                var queryString = "moveHandler.ashx?ourMove=" + JSON.stringify(moveObject);

                markBusy();
                xhr = new XMLHttpRequest();
                xhr.open("GET", queryString, true);
                xhr.onreadystatechange = xhrCallback;
                xhr.send();
            };

            function xhrCallback() {
                if (xhr.readyState != 4)
                    return;
            
                var parsedJSON = eval( "(" + xhr.responseText + ")" );

                if (!parsedJSON.isValid) {
                    alert('Illegal move');
                    
                    // Reload the board table to remove the bad move
                    $("#board")[0].innerHTML = parsedJSON.newBoardHTML;
                    initDraggables();
                    return;
                }

                if (parsedJSON.gameFinished) {
                    gameFinished = true;
                    $("#gameStatus")[0].innerHTML = "the game is " + parsedJSON.gameResult + "!";
                    markReady();
                    return;
                }

                // find destination and source squares
                var srcsq = $("[x = " + parsedJSON.movedPieceSrc.x + "][y = " + parsedJSON.movedPieceSrc.y + "]")[0];
                var dstsq = $("[x = " + parsedJSON.movedPieceDst.x + "][y = " + parsedJSON.movedPieceDst.y + "]")[0];
                var capturesq = dstsq;
                if (parsedJSON.hasExtraCaptureSquare) {
                    var captureSqPos = parsedJSON.extraCaptureSquarePos;
                    capturesq = $("[x = " + captureSqPos.x + "][y = " + captureSqPos.y + "]")[0];
                }

                markReady();

                if (parsedJSON.forceBoardReload){
                    $("#board")[0].innerHTML = parsedJSON.newBoardHTML;
                    initDraggables();
                } else {
                    // Do a funky 'capture' animation if this is a capture
                    animateMove(srcsq, dstsq, capturesq, true );
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

            function markBusy() {
                $("#busygif")[0].style.visibility = "block";
//                $($("#boardDiv")[0]).addClass('dimming');
            }

            function markReady() {
                $("#busygif")[0].style.visibility = "hidden";
                $($("#boardDiv")[0]).removeClass('dimming');
            }

            function alignImageToTD(image, td) {
                image.style.left = td.getClientRects()[0].left + "px";
                image.style.top = td.getClientRects()[0].top + "px";
            }
        </script>
        
    </head>

    <body onload=" javascript: initDraggables(); ">
        <div id="ourdiv">
            <div id="boardDiv" class="board dimming"  >
                <asp:Table runat="Server" ID="board" > </asp:Table>
            </div>
            <div id="gameStatus">test</div>
            <img src="images/status_anim.gif" id="busygif" style="visibility: block; text-align:center; display:block; " />
        </div>
    </body>
</html>

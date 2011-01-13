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
            
            function doMove(moveObject) {
                var queryString = "moveHandler.ashx?ourMove=" + JSON.stringify(moveObject);

                markBusy();
                xhr = new XMLHttpRequest();
                xhr.open("GET", queryString, true);
                xhr.onreadystatechange = helloWorldCallback;
                xhr.send();
            };

            function helloWorldCallback() {
                if (xhr.readyState != 4)
                    return;
            
                var table = document.getElementById("board");

                var parsedJSON = eval( "(" + xhr.responseText + ")" );

                if (!parsedJSON.isValid) {
                    alert('Illegal move');
                    // Reload the board table to remove the bad move
                    table.innerHTML = parsedJSON.newBoardHTML;
                    initDraggables();
                    return;
                }

                // find destination and source squares
                var srcsq = $("[x = " + parsedJSON.movedPieceSrc.x + "][y = " + parsedJSON.movedPieceSrc.y + "]")[0];
                var dstsq = $("[x = " + parsedJSON.movedPieceDst.x + "][y = " + parsedJSON.movedPieceDst.y + "]")[0];

                markReady();

                // Do a funky 'capture' animation if this is a capture
                animateMove(srcsq, dstsq,  table, parsedJSON.newBoardHTML);
            }

            var imgid;
            var thedstsq;
            function animateMove(srcsq, dstsq, table, newTableHTML) {

                var srcImageBox = $(srcsq.children[0])[0];
                alignImageToTD(srcImageBox, srcsq);
                
                // The destination box may be empty.
                var dstImageBox = null; 
                if ($(dstsq.children[0]).length > 0) {
                    dstImageBox = $(dstsq.children[0])[0];
                    alignImageToTD(dstImageBox, dstsq);
                }

                var newTop = dstsq.getClientRects()[0].top;
                var newLeft = dstsq.getClientRects()[0].left;

                // Start the 'move' animation of the piece
                $(srcImageBox).animate({ left: newLeft, top: newTop }, 'slow', function() {
                    // Move animation is over. Start to play any 'capture' animation necessary.
                    if (dstImageBox != null) {
                        $(dstImageBox).animate({    left: '-=50',
                                                    width: '+=100',
                                                    top: '-=50',
                                                    height: '+=100',
                                                    opacity: 'toggle'
                                                }, 'slow', function() {
                                                    // OK, all FX are over. Remove the image from the srcSq and assign it to the dst.
                                                    for (var i = 0; i < dstsq.children.length; i++)
                                                        dstsq.removeChild(dstsq.children[i]);
                                                    dstsq.appendChild(srcImageBox);
                        });
                    } else {
                        for (var i = 0; i < dstsq.children.length; i++)
                            dstsq.removeChild(dstsq.children[i]);
                        dstsq.appendChild(srcImageBox);
                    }
                });
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

                        // remove dragged image from the square it's at, and append it to the square it was dropped
                        // on.
                        TDBeingDragged.removeChild(imageBeingDragged);
                        this.appendChild(imageBeingDragged);

                        // Align the dragged image nicely
                        alignImageToTD(imageBeingDragged, this);

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
            <img src="images/status_anim.gif" id="busygif" style="visibility: block; text-align:center; display:block; " />
        </div>
    </body>
</html>

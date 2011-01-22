<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebFrontend._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Doktor Chess</title>
        
        <link rel="stylesheet" href="style/styles.css" type="text/css" />
        <link rel="stylesheet" href="style/roundedbox.css" type="text/css" />

        <script src="js/json.js" type="text/javascript"></script>
        <script src="js/jquery.js" type="text/javascript"></script>
        <script src="js/jquery-ui.js" type="text/javascript"></script>
        <script src="js/doktorChess.js" type="text/javascript" ></script>        
    </head>

    <body onload="javascript: initDraggables();"  >
        <div id="everything">
            <div style="float: left; width: 653px;">
                <div class="roundbox">
                    <div id="boardDiv" class="board dimming" >
                        <asp:Table runat="Server" ID="board" class="boardTable" CellPadding="0" CellSpacing="0" > </asp:Table>
                    </div>
                </div>
            </div>
            
            <div id="statusPanel" class="statusPanel" >
                <div class="statusPanelDiv"> 
                
                    <div class="progressIndicator" style="position: relative; visibility: hidden; width: 100px; float: right; top: 45%" >
                        <img src="images/spinny.gif" alt="please wait.."/>
                        <br />
                        <span>Thinking..</span>
                    </div>

                    <div style="float: left" >
                        <div class="gameLogDiv statusPanelBox">
                            <div class="roundbox" >
                                <span>Game so far:</span>
                                <textarea cols="10" rows="10" id="gameLog" class="statusPanelTextArea"></textarea>
                            </div>
                        </div>
                        
                        <div class="bestLineDiv statusPanelBox">
                            <input type="checkbox" id="showbestline" onclick="javascript: onshowbestline();" >Show computed best line</input>
                            <div class="bestLineTextBoxDiv" style="display: none" >
                                <div class="roundbox" style="statusPanelBox">
                                    <span>Computed best line:</span>
                                    <textarea cols="10" rows="10" id="bestLine" class="statusPanelTextArea" ></textarea>
                                </div>
                            </div>
                        </div>
                        <div id="gameStatus">
                            <span class="gameStatusLabel"></span>
                        </div>
                    </div>
                </div>
            </div>
            
            <div id="statusDiv" style="position: absolute; top: 50%; left: 50%">
                <img src="images/status_anim.gif" id="busygif" style="display: block; z-index: 99" />
            </div>
        </div>
        
    </body>
</html>

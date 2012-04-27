<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="watchGame.aspx.cs" Inherits="tournament.watchGame" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="style/styles.css" type="text/css" />
    <link rel="stylesheet" href="style/roundedbox.css" type="text/css" />

    <script src="js/json.js" type="text/javascript"></script>
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/tournament.js" type="text/javascript" ></script>   
</head>
<body>
    <form id="form1" runat="server">

    <div class="wideroundbox">
        <div class="title">
            <asp:Label ID="lblGameDetail" runat="server">test</asp:Label>
        </div>
    </div>

    <br />
    <br />
    
    <div class="roundbox" >
        <div style="width: 100%" >
            <br />
            <div style="text-align: center">
                <asp:Label ID="lblStatus" runat="server" ></asp:Label><br />
            </div>
            <br />
            <br />
            <div style="width:650px; margin:0 auto;">
                <div style="text-align: center">
                    <asp:Label ID="lblBlack" runat="server"></asp:Label><br />
                </div>
                <br />
                <asp:Label ID="lblTheGame" runat="server"></asp:Label><br />
                <br />
                <div style="text-align: center">                
                    <asp:Label ID="lblWhite" runat="server"></asp:Label><br />
                </div>
            </div>
            <br />
            <i>No AJAX yet I'm afraid - you'll need to refresh periodically to watch the game!</i>            
        </div>
    </div>

    </form>
</body>
</html>

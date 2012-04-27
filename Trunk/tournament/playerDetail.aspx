<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="playerDetail.aspx.cs" Inherits="tournament.playerDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="style/styles.css" type="text/css" />
    <link rel="stylesheet" href="style/roundedbox.css" type="text/css" />
    <link rel="stylesheet" href="style/ui-darkness/jquery-ui-1.8.19.custom.css" type="text/css" />

    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/tournament.js" type="text/javascript" ></script> 
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="wideroundbox">
        <div class="title">
            <asp:Label ID="lblplayerName" runat="server">test</asp:Label>
        </div>
        </div>

        <br />
        <br />
        
        <div class="roundbox">
            <asp:Table ID="gameTable" runat="server" class="normalTable">
                <asp:TableHeaderRow>
                    <asp:TableHeaderCell Width="20%">My colour</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="20%">Opponent</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="20%">Result</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="20%">Move count</asp:TableHeaderCell>
                    <asp:TableHeaderCell Width="20%">Move list</asp:TableHeaderCell>
                </asp:TableHeaderRow>
            </asp:Table>
        </div>            
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="scoreboard.aspx.cs" Inherits="tournament.scoreboard" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="style/styles.css" type="text/css" />
    <link rel="stylesheet" href="style/roundedbox.css" type="text/css" />

    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery-ui.js" type="text/javascript"></script>
    <script src="js/tournament.js" type="text/javascript" ></script>   
</head>
<body>
    <form id="form1" runat="server">

        <div class="wideroundbox" >
            <div class="title">Scoreboard</div>
        </div>

        <asp:Panel ID="results" runat="server">
            <br />
            <br />

            <div class="roundbox">
                <p class="title">Leaderboard</p>
                <asp:Table ID="Leaderboard" runat="server" class="normalTable">
                    <asp:TableRow ID="TableRow1" runat="server"> 
                        <asp:TableHeaderCell ID="TableHeaderCell1" runat="server">Rank</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell4" runat="server">Name</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell8" runat="server">Owner</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell2" runat="server">Score</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell5" runat="server">Wins</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell6" runat="server">Draws</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell7" runat="server">Losses</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell3" runat="server">Error count</asp:TableHeaderCell>
                    </asp:TableRow>
                </asp:Table>
            </div>

            <br />
            
            <div class="roundbox">
                <p class="title">Match fixtures</p>
                <asp:Table ID="matchList" runat="server" class="normalTable">
                    <asp:TableRow ID="TableRow2" runat="server" BorderStyle="Dotted" BorderColor="Black"> 
                        <asp:TableHeaderCell ID="TableHeaderCell9" runat="server">White</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell10" runat="server">White result</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell11" runat="server"></asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell12" runat="server">Black result</asp:TableHeaderCell>
                        <asp:TableHeaderCell ID="TableHeaderCell13" runat="server">Black</asp:TableHeaderCell>
                    </asp:TableRow>
                </asp:Table>
            </div>

            <br />

            <div class="roundbox">
                <p class="title">Result grid</p>
                <asp:Table ID="resultGrid" runat="server" class="gridTable" ></asp:Table>
            </div>
        
            <br />
        
            <div class="roundbox">
                <p class="title">Tournament status</p>
                <asp:Label ID="lblTourneyStatus" runat="server"></asp:Label><br />
                <asp:Label ID="lblCurrentGameStatus" runat="server"></asp:Label><br />
                <asp:Label ID="lblCurrentGameLink" runat="server"></asp:Label><br />
            </div>
        </asp:Panel>
    
        <br />

        <asp:Panel ID="PanelNoTournament" runat="server">
            <div class="roundbox">
                <p class="title">No tournament is in progress</p>
            </div>    
        </asp:Panel>

        <br />

            <div class="roundbox">
                <p class="title">Control panel</p>
                <table style="width: 100%">
                    <tr>
                        <td style="width: 33%"></td>
                        <td style="width: 33%">
                            <div>
                                <table class="normalTable" style="width: 100%">
                                    <tr>
                                    <th colspan="2">Authentication</th>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            <asp:Label ID="lblYouAre" runat="server"></asp:Label>
                                            <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="login.aspx" Text="Login now"></asp:HyperLink>
                                        </td>
                                    </tr>
                                    <asp:Panel ID="authOnly" runat="server">
                                        <tr>
                                            <td style="width: 50%">
                                                <asp:LinkButton ID="cmdLogout" runat="server" Text="Log out" onclick="cmdLogout_Click" />
                                            </td>
                                            <td>
                                                <asp:LinkButton ID="cmdChangePass" runat="server" Text="Change password" onclick="cmdChangePass_Click" />
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                </table>
                                
                                <br />
                                <br />
                                
                                <asp:Panel ID="UploadPanel" runat="server">
                                    <table class="normalTable" style="width: 100%">
                                        <tr>
                                        <th>Upload player</th>
                                        </tr>
                                        <tr>
                                            <td>
                                                AI player path :
                                                <asp:FileUpload ID="FileUpload" runat="server" />
                                                <br />
                                                <asp:Button ID="cmdUploadAI" runat="server" Text="UploadAI" onclick="cmdUploadAI_Click" /><br />
                                            </td>
                                        </tr>
                                    </table>
                                    <br/>
                                    <br/>
                                    <table class="normalTable" style="width: 100%">
                                        <tr>
                                            <th colspan="2">Tournament control</th>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Button ID="cmdRemoveMyPlayers" runat="server" Text="Remove my players" onclick="cmdRemoveMyPlayers_Click" />
                                            </td>
                                            <td>
                                                <asp:Button ID="cmdRestartTournament" runat="server" Text="Restart tournament" onclick="cmdRestartTournament_Click" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </div>
                        </td>
                        <td style="width: 33%"></td>
                    </tr>
                </table>
            </div>    
    </form>
</body>
</html>

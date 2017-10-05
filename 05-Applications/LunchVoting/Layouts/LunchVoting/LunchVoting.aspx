<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LunchVoting.aspx.cs" Inherits="LunchVoting.Layouts.LunchVoting.LunchVoting" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    Lunch Voting (Additional Page Head)
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Lunch Voting
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Lunch Voting
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:Panel runat="server" ID="VotingClosedPanel" Visible="false">
        Lunch Voting is closed. Try again tomorrow.
    </asp:Panel>
    <asp:Panel runat="server" ID="MessagePanel" Visible="false">
        <asp:Literal runat="server" ID="Message" />
        <br />
        <a href="LunchVoting.aspx">Back to voting.</a>
    </asp:Panel>
    <asp:Panel runat="server" ID="InputPanel" Visible="false">
        <table>
            <tr>
                <td>Selection</td>
                <td>
                    <asp:DropDownList runat="server" ID="Selection" /> 
                    [<asp:HyperLink runat="server" ID="AddLocation">Add Location</asp:HyperLink>]
                </td>
            </tr>
            <tr>
                <td>On Behalf of</td>
                <td>
                    <SharePoint:ClientPeoplePicker runat="server" ID="OnBehalfOf" AllowMultipleEntities="false" />
                </td>
            </tr>
        </table>
        <br />
        <asp:Button runat="server" ID="Vote" Text="Vote" />
    </asp:Panel>
</asp:Content>

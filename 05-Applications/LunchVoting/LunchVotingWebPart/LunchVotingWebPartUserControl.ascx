<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LunchVotingWebPartUserControl.ascx.cs" Inherits="LunchVoting.LunchVotingWebPart.LunchVotingWebPartUserControl" %>

<%-- Панель, которая отображается, если ещё нет результатов голосования --%>
<asp:Panel runat="server" ID="NoResultsPanel" Visible="false">
    Nobody has voted yet!
</asp:Panel>

<%-- Будет генерировать разметку для каждого элемента из источника данных --%>
<asp:Repeater runat="server" ID="TopLunchPicks">
    <HeaderTemplate><ul></HeaderTemplate>
    <ItemTemplate>
        <li>
            <b><asp:Literal runat="server" ID="Selection" /></b>
            <ul>
                <li>Total Votes: <asp:Literal runat="server" ID="VoteCount" /></li>
                <li>Proxy Votes: <asp:Literal runat="server" ID="VotesOnBehalf" /></li>
                <asp:Literal runat="server" ID="YourVote" Visible="false" ><li>Your choice!</li></asp:Literal>
                <asp:Literal runat="server" ID="YourVoteByProxy" Visible="false" ><li>Chosen for you</li></asp:Literal>
            </ul>
        </li>
    </ItemTemplate>
    <FooterTemplate></ul></FooterTemplate>
</asp:Repeater>

<div style="text-align: center;">
    <asp:HyperLink runat="server" ID="GoVoteLink">Go Vote!</asp:HyperLink>
</div>
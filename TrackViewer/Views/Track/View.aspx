<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TrackViewer.Domain.Entities.Track>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	View
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%: Model.Name %></h2>
	<span style="float: right"><%: Html.ActionLink("Back to List", "List") %></span>
	<h3><%: Model.TrackDate.ToShortDateString() %>, <span id="distance"><%: (Model.TotalDistance / 1609.344).ToString("N") %></span> mi, <%: Model.TypeOfTravel %></h3>
	<%: Html.HiddenFor(trk => trk.TrimStart) %>
	<%: Html.HiddenFor(trk => trk.TrimEnd) %>

	<%: Html.Partial("Map") %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<link type="text/css" rel="Stylesheet" href="/TrackViewer/Content/jquery.jqplot.min.css" />
    <script src="http://maps.google.com/maps/api/js?libraries=geometry&sensor=false" type="text/javascript"></script>
	<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.1.min.js"></script>
	<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.10/jquery-ui.min.js"></script>
	<script type="text/javascript" src="/TrackViewer/Scripts/jquery.jqplot.min.js"></script>
	<script type="text/javascript" src="/TrackViewer/Scripts/jqplot/jqplot.dateAxisRenderer.min.js"></script>
</asp:Content>

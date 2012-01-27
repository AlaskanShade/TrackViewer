<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TrackViewer.Domain.Entities.Track>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<link type="text/css" rel="Stylesheet" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.10/themes/redmond/jquery-ui.css" />
	<link type="text/css" rel="Stylesheet" href="/TrackViewer/Content/jquery.jqplot.min.css" />
    <script src="http://maps.google.com/maps/api/js?libraries=geometry&sensor=false" type="text/javascript"></script>
	<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.1.min.js"></script>
	<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.10/jquery-ui.min.js"></script>
	<script type="text/javascript" src="/TrackViewer/Scripts/jquery.jqplot.min.js"></script>
	<script type="text/javascript" src="/TrackViewer/Scripts/jqplot/jqplot.dateAxisRenderer.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	
    <h2>Edit</h2>

	<table style="width: 100%; height: 100%">
		<tr>
			<td style="width: 35%; vertical-align: top">
				<% using (Html.BeginForm()) {%>

					<%: Html.ValidationSummary(true) %>
					<%: Html.LabelFor(trk => trk.SourceFile) %>
					<%: Model.SourceFile %>
					<br />
					<%: Html.LabelFor(trk => trk.Name) %>
					<%: Html.EditorFor(trk => trk.Name) %>
					<br />
					<%: Html.LabelFor(trk => trk.TrackDate) %>
					<%: Model.TrackDate.ToShortDateString() %>
					<%: Html.HiddenFor( trk => trk.TrackDate) %>
					<br />
					<%: Html.LabelFor(trk => trk.TotalDistance) %>
					<span id="distance"><%: (Model.TotalDistance / 1609.344).ToString("N") %></span>
					<%: Html.HiddenFor( trk => trk.TotalDistance) %>
					<br />
					<%: Html.LabelFor(trk => trk.TypeOfTravel) %>
					<%: Html.DropDownListFor(trk => trk.TypeOfTravel, (SelectList)ViewData["TravelMethods"], "- Select one -") %>
					<br />
					<%: Html.LabelFor(trk => trk.TrimStart) %>
 					<%: Html.EditorFor(trk => trk.TrimStart) %>
					<%: Html.LabelFor(trk => trk.TrimEnd) %>
 					<%: Html.EditorFor(trk => trk.TrimEnd) %>
 					<%: Html.HiddenFor(trk => trk.TrackID) %>
					<div id="slider"></div>
           
					<input name="save" type="submit" value="Save" />
					<input name="trim" type="submit" value="Save and Trim" />

				<% } %>

				<div>
					<%: Html.ActionLink("Back to List", "List") %>
				</div>
			</td>
			<td>
				<%: Html.Partial("Map") %>
			</td>
		</tr>
	</table>


</asp:Content>


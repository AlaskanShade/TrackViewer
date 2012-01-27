<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<TrackViewer.Domain.Entities.Track>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Tracks
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Tracks</h2>
	<% if (User.IsInRole("Admin")) { %>
	<%: Html.ActionLink("Upload a new file", "Upload") %>
	<%: Html.ActionLink("Update Metadata", "UpdateMetadata") %>
	<% } %>

	<table>
		<thead>
			<tr>
				<th>Date</th>
				<th>Type</th>
				<th>Name</th>
				<th>Distance</th>
				<th>Time</th>
				<th>Points</th>
				<% if (User.IsInRole("Admin")) { %>
				<th></th>
				<% } %>
				<th></th>
				<th></th>
			</tr>
		</thead>
		<% foreach (var track in Model) { %>
			<tr>
				<td><%: track.TrackDate.ToShortDateString() %></td>
				<td><%: track.TypeOfTravel %></td>
				<td><%: track.Name %></td>
				<td><%: (track.TotalDistance / 1609.344).ToString("N") %> miles</td>
				<td><%: track.TotalTime.TotalHours.ToString("N") %> hours</td>
				<td><%: track.GetPoints().Count() %></td>
				<% if (User.IsInRole("Admin")) { %>
				<td><% using (Html.BeginForm("Delete", "Track")) { %>
					<%: Html.Hidden("trackId", track.TrackID)%>
					<button type="submit">Delete</button>
				<% } %></td>
				<% } %>
				<td><%: Html.ActionLink("View", "View", new { id = track.TrackID }) %></td>
				<td><%: Html.ActionLink("Edit", "Edit", new { id = track.TrackID }) %></td>
			</tr>
		<% } %>
	</table>

</asp:Content>

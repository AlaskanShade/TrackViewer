<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<TrackViewer.Domain.Entities.Track>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Tracks
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Tracks</h2>
	<%: Html.ActionLink("Upload a new file", "Upload") %>
	<%: Html.ActionLink("Import All", "ImportAll") %>

	<table>
		<thead>
			<tr>
				<th>Source</th>
				<th>Name</th>
				<th>Points</th>
				<th></th>
				<th></th>
			</tr>
		</thead>
		<% foreach (var track in Model) { %>
			<tr>
				<td><%: track.SourceFile %></td>
				<td><%: track.Name %></td>
				<td><%: track.GetPoints().Count() %></td>
				<td><% using (Html.BeginForm("Delete", "Track")) { %>
					<%: Html.Hidden("trackId", track.TrackID) %>
					<button type="submit">Delete</button>
				<% } %></td>
				<td><%: Html.ActionLink("Edit", "Edit", new { id = track.TrackID }) %></td>
			</tr>
		<% } %>
	</table>

</asp:Content>

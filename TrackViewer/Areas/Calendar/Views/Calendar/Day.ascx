<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TrackViewer.Areas.Calendar.Models.DayData>" %>

<% if (HttpContext.Current.User.Identity.IsAuthenticated) { %>
	<% using (Html.BeginForm("UpdateDay", "Calendar", new { month = Model.Day.Month, day = Model.Day.Day, year = Model.Day.Year })) { %>
		<div class="dayCellDate"><%: Model.Day.Day%></div>
		<input type="hidden" name="month" value='<%: Model.Day.Month %>' />
		<input type="hidden" name="day" value='<%: Model.Day.Day %>' />
		<input type="hidden" name="year" value='<%: Model.Day.Year %>' />
		<% foreach (var line in Model.Lines) { %>
			<div class='line<%: line.Color %>'><%: line.GetDisplay()%></div>
		<% } %>
		<% foreach (var track in Model.Tracks) { %>
			<div><%: Html.ActionLink(track.Name, "Edit", "Track", new { area = "", id = track.TrackID }, null) %></div>
		<% } %>
		<textarea name="data" cols="25" rows="5" style="height: 100%; width: 100%; display: none"><% foreach (var line in Model.Lines)
																							  { %><%: line.ToString()%><% } %></textarea>
		<input type="submit" value="Done" style="display: none" />
	<% } %>
<% } else { %>
	<div class="dayCellDate"><%: Model.Day.Day%></div>
	<% foreach (var line in Model.Lines) { %>
		<div class='line<%: line.Color %>'><%: line.GetDisplay()%></div>
	<% } %>
<% } %>

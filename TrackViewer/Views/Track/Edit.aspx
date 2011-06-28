<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TrackViewer.Domain.Entities.Track>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<link type="text/css" rel="Stylesheet" href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.10/themes/redmond/jquery-ui.css" />
    <script src="http://maps.google.com/maps/api/js?libraries=geometry&sensor=false" type="text/javascript"></script>
	<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.1.min.js"></script>
	<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.10/jquery-ui.min.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	
    <h2>Edit</h2>

	<table style="width: 100%; height: 100%">
		<tr>
			<td style="width: 35%; vertical-align: top">
				<% using (Html.BeginForm()) {%>

					<%: Html.ValidationSummary(true) %>
					<%: Html.LabelFor(trk => trk.SourceFile) %>
					<%: Html.EditorFor(trk => trk.SourceFile) %>
					<br />
					<%: Html.LabelFor(trk => trk.Name) %>
					<%: Html.EditorFor(trk => trk.Name) %>
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
				<div id="map" style="height: 400px; width: 100%">
				</div>
			</td>
		</tr>
	</table>


	<script type="text/javascript">
		$(document).ready(function() { 
			load(); 
		});
		var map, startMark, endMark, path, polyline;
		function load() {
			var SW = new google.maps.LatLng(<%: Model.GetGmapData().South %>, <%: Model.GetGmapData().West %>);
			var NE = new google.maps.LatLng(<%: Model.GetGmapData().North %>, <%: Model.GetGmapData().East %>);
			var Bnd = new google.maps.LatLngBounds(SW, NE);
			var myOptions = { zoom: 8, center: Bnd.getCenter(), panControl: true, zoomControl: true, scaleControl: true, mapTypeId: google.maps.MapTypeId.ROADMAP }; // HYBRID, SATELLITE, TERRAIN
			map = new google.maps.Map(document.getElementById("map"), myOptions);
			map.fitBounds(Bnd);
			startMark = new google.maps.Marker({position: new google.maps.LatLng(<%: Model.GetGmapData().StartLat %>, <%: Model.GetGmapData().StartLon %>), title: "Start", icon: '/TrackViewer/Content/mm_20_green.png', shadow: '/TrackViewer/Content/mm_20_shadow.png'} );
			endMark = new google.maps.Marker({position: new google.maps.LatLng(<%: Model.GetGmapData().EndLat %>, <%: Model.GetGmapData().EndLon %>), title: "Start", icon: '/TrackViewer/Content/mm_20_red.png', shadow: '/TrackViewer/Content/mm_20_shadow.png'} );
			startMark.setMap(map);
			endMark.setMap(map);
			path = google.maps.geometry.encoding.decodePath('<%: Model.GetGmapData().EncodedPolyline %>');
			polyline = new google.maps.Polyline({ map: map, path: path, strokeColor: '#0000FF', strokeOpacity: .5 });

			var start = 0, end = path.length;
			if ($('#TrimStart').val() != '') start = parseInt($('#TrimStart').val());
			if ($('#TrimEnd').val() != '') end = parseInt($('#TrimEnd').val());
			$('#TrimStart').blur(function() { updateTrim(); });
			$('#TrimEnd').blur(function() { updateTrim(); });
			$('#slider').slider({
				range: true,
				min: 0,
				max: path.length,
				values: [start, end],
				slide: function(event, ui) { $('#TrimStart').val(ui.values[0]); $('#TrimEnd').val(ui.values[1]); updateTrim(ui.values[0], ui.values[1]); }
			});
			updateTrim(start, end);
		}
		function updateTrim(start, end)
		{
			if (!start) start = parseInt($('#TrimStart').val());
			if (isNaN(start)) start = 0;
			if (!end) end = parseInt($('#TrimEnd').val());
			if (isNaN(end)) end = path.length;
			var newPath = [];
			for (var i = start; i < end; i++)
            {
            	newPath.push(path[i]);
            }
			polyline.setPath(newPath);
			// move markers
			startMark.setPosition(path[start]);
			endMark.setPosition(path[end - 1]);
		}
	</script>
</asp:Content>


<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TrackViewer.Domain.Entities.Track>" %>

<%--				<div id="tabs">
					<ul>
						<li><a href="#tabs-1">Map</a></li>
						<li><a href="#tabs-2">Speed</a></li>
						<li><a href="#tabs-3">Elevation</a></li>
					</ul>
--%>					<div id="tabs-1">
						<div id="map" style="height: 500px; width: 100%">
						</div>
					</div>
					<div id="tabs-3" style="height: 50%; width: 100%"></div>
					<div id="tabs-2" style="height: 50%; width: 100%"></div>
<%--				</div>
--%>
	<script type="text/javascript">
		var distances;
		$(document).ready(function() { 
			//$('#tabs').tabs();
			load(); 
			$.getJSON('<%: Url.Action("TrackData", new { id = Model.TrackID, type = "Speed" }) %>', null, function(results) { loadChart(2, results); });
			$.getJSON('<%: Url.Action("TrackData", new { id = Model.TrackID, type = "Elevation" }) %>', null, function(results) { loadChart(3, results); });
		});
		function loadChart(tab, results) {
			$('#tabs-' + tab).append('<div id="chart' + tab + '"></div>');
			$.jqplot('chart' + tab, 
				[results.Data], 
				{ title: results.Title, 
					axes: { 
						xaxis: { 
							renderer:$.jqplot.DateAxisRenderer, 
							tickOptions: { formatString: '%H:%M' }, 
							tickInterval: results.Tick, 
							min: results.MinX, 
							max: results.MaxX, 
							pad: 1.05 
						}
					},
					seriesDefaults: { 
						lineWidth: 1.5,
						markerOptions: { size: 5 }
					},
					highlighter: { show: true, sizeAdjust: 2 },
					cursor: { show: false } });
		};
		var map, startMark, endMark, path, polyline, distances;
		function load() {
			distances = <%: Model.GetDistances() %>
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
			$('#distance').html((distances[end - 1] - distances[start]).toFixed(2));
		}
	</script>

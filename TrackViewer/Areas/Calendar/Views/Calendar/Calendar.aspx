<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<TrackViewer.Areas.Calendar.Models.DayCollection>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
        <title>Calendar</title> 
	<script type="text/javascript" src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.6.1.min.js"></script>
		<style>
BODY {
  margin-left: 2%;
  margin-right: 2%;
  background-color: white;
}
.TH {
  	font-family: Arial, Helvetica, sans-serif;
	font-size: 18px;
	font-style: normal;
	font-weight: bold;
	color: #00007C;
	text-align: center;
}
.dayCell {
  width: 14%;
  height: 100px;
  background-color: rgb(240,240,240);
  vertical-align: top;
  text-align: left;
}
.dayCellDate {
  text-align: right;
}
.lineYellow {
  background-color: #FEF2C0;
  font: normal 12px Arial, Helvetica, Tahoma, sans-serif;
  color: #333399;
  height: 16px;
}
.lineBlue {
  background-color: #C1DFFF;  
  font: normal 12px Arial, Helvetica, Tahoma, sans-serif;
  color: #333399;
  height: 16px;
}
.lineRed {
  background-color: #FDCABF;  
  font: normal 12px Arial, Helvetica, Tahoma, sans-serif;
  color: #333399;
  height: 16px;
}
.lineGreen {
  background-color: #CEFBBE;  
  font: normal 12px Arial, Helvetica, Tahoma, sans-serif;
  color: #333399;
  height: 16px;
}
		</style>
</head> 
<body> 
	<asp:HyperLink runat="server" NavigateUrl="~/LogOn.aspx" style="float: right">Login</asp:HyperLink>
	<table border="1" width="100%" style="clear: both"> 
		<thead>
		    <tr> 
				<th width="14%">Sun</th> 
				<th width="14%">Mon</th> 
				<th width="14%">Tue</th> 
				<th width="14%">Wed</th> 
				<th width="14%">Thu</th> 
				<th width="14%">Fri</th> 
				<th width="14%">Sat</th> 
		    </tr> 
		</thead>
		<tbody>
			<% for (int week = 0; week < 5; week++) { %>
			<tr>
				<% for (int weekday = 0; weekday < 7; weekday++) { %>
				<td class="dayCell">
					<%: Html.Partial("Day", Model.GetData(((DateTime)ViewData["date"]).AddDays(7 * week + weekday))) %>
				</td>
				<% } %>
			</tr>
			<% } %>
		</tbody>
	</table> 
	<% if (HttpContext.Current.User.Identity.IsAuthenticated) { %>
	<script type="text/javascript">
		function cancelEdit(cell) {
			$('div:not(.dayCellDate)', cell).css('display', 'block');
			$('textarea', cell).css('display', 'none');
			$('input', cell).css('display', 'none');
			$('.dayCellDate', cell).attr('mode', 'show');
		}
		$(function () {
			$('.dayCellDate').click(function () {
				if ($(this).attr('mode') == 'edit') {
					cancelEdit($(this).parent());
				}
				else {
					$('td:has(.dayCellDate[mode=edit])').each(cancelEdit);
					$(this).nextAll('div').css('display', 'none');
					$(this).nextAll('textarea').css('display', 'block');
					$(this).nextAll('textarea').focus();
					$(this).nextAll('input').css('display', 'block');
					$(this).attr('mode', 'edit');
				}
			});
		});
	</script>
	<% } %>
</body>
</html>

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Upload
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Upload</h2>
	<% using (Html.BeginForm("Upload", "Track", FormMethod.Post, new { enctype = "multipart/form-data" })) { %>
		<div>Upload .gpx file: <input type="file" name="gpx" /></div>
		<input type="submit" value="Upload" />
		<span style="color: Red"><%: Html.ActionLink("Cancel", "List") %></span><br />
		<%: Html.ViewData["message"] %>
	<% } %>
</asp:Content>

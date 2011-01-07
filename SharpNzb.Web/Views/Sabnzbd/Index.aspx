<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SharpNzb.Web.Models.SabnzbdModels>" %>

<%@ Import Namespace="Telerik.Web.Mvc.UI" %>
<%@ Import Namespace="SharpNzb.Core.Helper" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Scripts" runat="server">
<%--    <script type="text/javascript">
        function onRowDataBound(e) {

            e.row.style.boarder = "";

            if (e.dataItem.Size > 0) {
                e.dataItem.Size = "#FFD700";
            }
            else if (e.dataItem.Level == 4) {
                e.row.style.backgroundColor = "#FF7500";
            }
            else if (e.dataItem.Level == 5) {
                e.row.style.backgroundColor = "black";
                e.row.style.color = "red";
            }
            //e.row.style.color = 'blue';
        }
    </script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="QueueMenu" ContentPlaceHolderID="ActionMenu" runat="server">
    <%
        Html.Telerik().Menu().Name("queueMenu").Items(items => items.Add().Text("Purge").Action("Purge", "Sabnzbd")).Render();
    %>
</asp:Content>
<asp:Content ID="QueueContent" ContentPlaceHolderID="MainContent" runat="server">
    <%:Html.Label("Queue") %>
    <%
        var format = new FileSizeFormatHelper(); %>
    <%Html.Telerik().Grid(Model.Queue).Name("queue")
                          .Columns(columns =>
                                       {
                                           columns.Bound(c => c.Name).Title("Name").Width(0);
                                           columns.Bound(c => c.Size);
                                           columns.Bound(c => c.Category);
                                           columns.Bound(c => c.PostProcessing);
                                           columns.Bound(c => c.Priority);
                                       })
            //               .DetailView(detailView => detailView.ClientTemplate(

            //    "<div><#= Logger #></div>" +
            //    "<div><#= ExceptionType #></div>" +
            //    "<div><#= ExceptionMessage #></div>" +
            //    "<div class='stackframe'><#= ExceptionString #></div>"

            //)).DataBinding(data => data.Ajax().Select("_AjaxBinding", "Sabnzbd"))
                          //.Sortable(rows => rows.OrderBy(epSort => epSort.Add(c => c.Name).Descending()).Enabled(true))
                          .Pageable(c => c.PageSize(10).Position(GridPagerPosition.Both).Style(GridPagerStyles.NextPreviousAndNumeric))
                          .Filterable()
                          .ClientEvents(c => c.OnRowDataBound("onRowDataBound"))
                          .Render();
    %>
</asp:Content>

<asp:Content ID="HistoryMenu" ContentPlaceHolderID="ActionMenu2" runat="server">
    <%
        Html.Telerik().Menu().Name("queueMenu").Items(items => items.Add().Text("Purge").Action("Purge", "Sabnzbd")).Render();
    %>
</asp:Content>
<asp:Content ID="HistoryContent" ContentPlaceHolderID="MainContent2" runat="server">
    <%:Html.Label("History") %>
    <%Html.Telerik().Grid(Model.History).Name("history")
                          .Columns(columns =>
                                       {
                                           columns.Bound(c => c.Name).Title("Name").Width(0);
                                           columns.Bound(c => c.DownloadTime);
                                           columns.Bound(c => c.Category);
                                           columns.Bound(c => c.RepairTime);
                                           columns.Bound(c => c.ScriptOutput);
                                       })
            //               .DetailView(detailView => detailView.ClientTemplate(

            //    "<div><#= Logger #></div>" +
            //    "<div><#= ExceptionType #></div>" +
            //    "<div><#= ExceptionMessage #></div>" +
            //    "<div class='stackframe'><#= ExceptionString #></div>"

            //)).DataBinding(data => data.Ajax().Select("_AjaxBinding", "Sabnzbd"))
                          //.Sortable(rows => rows.OrderBy(epSort => epSort.Add(c => c.Name).Descending()).Enabled(true))
                          .Pageable(c => c.PageSize(10).Position(GridPagerPosition.Both).Style(GridPagerStyles.NextPreviousAndNumeric))
                          .Filterable()
                          .ClientEvents(c => c.OnRowDataBound("onRowDataBound"))
                          .Render();
    %>
</asp:Content>
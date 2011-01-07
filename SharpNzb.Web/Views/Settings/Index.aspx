<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<SharpNzb.Web.Models.SettingsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Settings
</asp:Content>
<asp:Content ID="General" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       { %>
    <%: Html.ValidationSummary(true, "Unable to save your settings. Please correct the errors and try again.") %>
    <div>
        <fieldset>
            <legend>General</legend>
            <div class="editor-label">
                
                
            </div>
            <div class="editor-field">
                <%: Html.LabelFor(m => m.TempDir) %>
                <%: Html.TextBoxFor(m => m.TempDir) %>
                <%: Html.ValidationMessageFor(m => m.TempDir) %>

                <br />
                <%: Html.LabelFor(m => m.CompleteDir) %>
                <br />
                <%: Html.TextBoxFor(m => m.CompleteDir) %>
                <%: Html.ValidationMessageFor(m => m.CompleteDir) %>
            </div>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>

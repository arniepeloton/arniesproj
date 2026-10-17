<%@ Page Language="VB" MasterPageFile="~/Views/IT/IT.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SQL
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        SQL</h2>
    <form method="post">
    <textarea name="txtSql" rows="2" cols="20" id="txtSql" style="height: 300px; width: 572px;"><%: ViewData("SQL") %></textarea>
    <br />
    <input type="submit" value="Run" />
    </form>
    <% If CStr2(ViewData("Message")).Length > 0 Then%>
    <div class="divMessage">
        <%: ViewData("Message")%>
    </div>
    <% End If%>
    <% If ViewData("DataSet") IsNot Nothing Then%>
    <div class="divResult">
        <% For Each pDT As DataTable In CType(ViewData("DataSet"), DataSet).Tables%>
        <% Html.RenderPartial("/Views/Shared/ProGrid.ascx", New ProGrid(pDT.DefaultView))%>
        <% Next pDT%>
    </div>
    <% End If%>
</asp:Content>

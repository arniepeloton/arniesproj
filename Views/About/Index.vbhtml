@Code
    ViewData("Title") = "About Us"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code
Last web app update: @ViewData("LastUpdate")<br />
Current Windows user: @System.Security.Principal.WindowsIdentity.GetCurrent.Name<br />
Use minified scripts: @ProWeb.Common.UseMinifiedJavascripts<br />
Server Agent LastUpdated: @ViewData("ServerAgentLastUpdated")<br />
Operating System: @ViewData("OSVersion")<br />
Server: @ViewData("SERVER_SOFTWARE")<br />

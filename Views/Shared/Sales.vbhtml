@Code
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
@Section head
    @RenderSection("head", False)
End Section
@Section Header
End Section
@RenderBody()
@Section OtherPages
    @RenderSection("OtherPages", False)
End Section

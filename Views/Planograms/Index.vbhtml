@Code
    ViewData("Title") = "Planograming"
    Layout = "~/Views/Planograms/MobileLayout.vbhtml"
End Code

<script type="text/javascript">
    require([
              "dojox/mobile/FixedSplitter",
              "dojox/mobile/Button",
              "dojox/mobile/TextBox",
              "dojox/mobile/ToolBarButton"
            ]);
</script>

<div dojoType="dojox.mobile.View" style="width:100%; text-align:center;"> 
        <h1 dojoType="dojox.mobile.Heading">DUC - The Paper Tree</h1>
        <br />
        <br />
        <div dojoType="dojox.mobile.View" style="margin-bottom:40px;">
            <input dojoType="dojox.mobile.TextBox" type="text" name="username" style="width:240px;height:35px;padding:5px;margin-bottom:5px;text-align:center;"/><br />
            <input dojoType="dojox.mobile.TextBox" type="password" name="password" style="width:240px;height:35px;padding:5px;text-align:center;"/>
            <br />
            <br />
            <button dojoType="dojox.mobile.Button">Forgot Password</button>
            <button dojoType="dojox.mobile.Button">Sign In</button>
        </div>
</div>


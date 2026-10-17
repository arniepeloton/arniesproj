<html>
<head>
    <title> 
        @Code
            ViewData("Title") = "MBORT"
            Layout = "~/Views/Main/Layout.vbhtml"
        End Code 
    </title>
   
   <script type="text/javascript" src="../../Scripts/sources/Defaults/MBORT.js"></script>

</head>
 
<body>   
    <h3> Parameters for Back Order Action Module </h3>

    <br /> <br />

    <div id="bd-loading"> </div>

    <div style="font-size:medium">
       <div>Maximum Back Order response time (MBORT) <input type="text" id="txtMBORT"/> Min(s) </div>

       <br />

       <div>
            What do we do when BOA notifications are un-answered for more than MBORT? &nbsp;
            <select id='optMBORT' style="width:300px">
                <option value="cancel"> Cancel Back Orders </option>
                <option value="push"> Push through with system suggestions </option>
            </select>
       </div>
       <div> <button id="btnSave"> Save </button> </div> 
    </div>
</body>
</html>

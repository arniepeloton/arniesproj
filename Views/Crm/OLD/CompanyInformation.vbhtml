<html>
<head>
    <title class="customerTitle">
        @Code
            'ViewData("Title") = "Customer Information"
            Layout = "~/Views/Main/Layout.vbhtml"
        End Code
    </title>
    <style>  
            table td, table th {
                border: 0px none;
                border-bottom: 1px solid;
                vertical-align:middle;
                font-weight:bold;
            }           
            
    </style>
    
    <script type="text/javascript" src="../../Scripts/sources/WhiteLabel/datatables.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/WhiteLabel/wl_Widget.js"></script>
    <script type="text/javascript" src="../../Scripts/sources/CRM/customerInformation.js"></script>
</head>
 
<body>   
    <br /> <br />

    <div style="padding-left:10%; font-size:large">
        <table style="font-size:40px;">
            <tr> 
                <td>
                    <div id="companyLogo"> </div>
                </td>
                <td style="border:0; border-style:none"> </td>
                <td style="border:0; border-style:none">
                    <div id="companyName"> </div>       
                </td>     
             </tr>           
        </table>
        <br /> <br />
    </div>

    <div id="companyInformation" style="padding-left:10%; font-size:small;">        
    </div>

    <hr />

    <div id="purchaseHistory" style="padding-left:5%; font-size:medium; text-decoration:underline;">
        <b> Purchase History (Under Construction)</b>
        <br /> <br /> 
    </div>

    <hr />

    <div class="g4 widgets ui-sortable">
	    <div class="widget number-widget" id="number_widget">
		    <h3 class="handle green"> <b> Notes </b> <a class="collapse" title="collapse widget"></a></h3>
                <div id="notes" style="padding-left:5%; font-size:medium;">        
                </div>
	    </div>
				
	</div>

      
</body>
</html>
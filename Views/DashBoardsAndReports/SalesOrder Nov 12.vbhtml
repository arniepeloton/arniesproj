@Code
    ViewData("Title") = "Order Central"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/SalesOrder.js?crc=')]);
    </script>
    <style>
    table.myTable { 
            border-collapse:collapse; 
            width: 100%;
        }
        .ui-table-reflow.ui-responsive {
            display: table;
        }

     .SearchText {
        margin:0px;
        border: 0px;
        text-shadow: 0px;
        background: #FFF;
        height: 40px;
        border-radius: 0px;
        
    }
    .ui-focus {
     -moz-box-shadow: none !important;
     -webkit-box-shadow: none !important;
     box-shadow: none !important;
    }
    .ui-shadow-inset {
        box-shadow:none !important;
    }
    /* Add alternating row stripes */

    .table-stripe tbody tr:hover {
      background-color: #E6E6E6; /* non-RGBA fallback  */

    }
    /* Add stroke to the header and last item */
    .table-stripe thead th,
    .table-stripe tbody tr:hover {
      background-color: #E6E6E6; /* non-RGBA fallback  */

    }
    .ui-dialog-contain 
    {
        max-width: 700px;
    }
    
    .ui-dialog-background {
        opacity: 0.5;
        display: block !important;
        -webkit-transition: opacity 0.5s ease-in;
    }

    .ui-dialog-background.pop.in {
        opacity: 1;
        -webkit-transition: opacity 0.5s ease-in;
    }

    .ui-dialog {
        min-height: 100% !important;
        background: transparent !important;
    }
    /* Button */
    .HotButtons .ui-btn-inner {
        
        padding-top: 1.4em;
    }
    
    
    .HotButtonsText .ui-btn-text {
        color:White;   
    }
    

    </style>
End Section

<div id="pageMain" data-role="page">
    <div data-role="header" data-position="fixed" style="background: none white; border: none">
        <div class="ui-grid-d" >
            <div class="ui-block-a" style="width: 12%;">
                <div id="btnOrderCentralHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span style="margin-top:5px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 22%;">
                <div id="" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                  Pending <br />Orders
                </div>
            </div>
            <div class="ui-block-c" style="width: 22%;">
                <div id="" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                   Invoiced <br />Orders
                </div>
            </div>
            <div class="ui-block-d" style="width: 22%;">
                <div id="btnAddFilter" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                   Add <br />Filter
                </div>
            </div>
            <div class="ui-block-e" style="width: 22%;">
                <div id="btnRemoveFilter"style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                 Remove <br />Filter
                </div>
            </div>
        </div>
        <div>
            <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
           
                @*Filtered By None > None > None*@Sales Orders by <label id="lblColumnName">Creation Date</label>
           
            </div>
        </div>
   </div>

   <div data-role="content" style="padding: 5px 5px 0px 5px;">
            <div class="ui-grid-a" style="margin:0px; padding: 0px;">
                <div class="ui-block-a" style="width:80%;height: 50px;padding: 0px;margin-bottom:5px;">
                    <form id="formSearch">
                        <input id="txtSearch" type="search" placeholder="Search orders" class="SearchText" style="padding:0px;" autofocus />
            
                    </form>

                </div>
                <div class="ui-block-b" style="width: 20%;padding: 0px;margin-bottom:5px;">
                   <a data-role="button" data-theme="c" id="btnSearch">Go</a>    
                </div>
            </div>
 
  
     
    </div>
  
       <table data-role="table" id="table-custom-2" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
                <thead>
                    <tr style="cursor: pointer">
                        <th></th>
                        <th data-sort="date">Date</th>
                        <th data-sort="string">Order No.</th>
                        <th data-sort="string">Customer</th>
                        <th data-sort="string">Sales Representative</th>
                        <th data-sort="string">Delivery Address</th>
                        <th data-sort="string">FOWST</th>
                        <th data-sort="string">Status</th>
                    </tr>
                                   
                </thead> 
                            
                <tbody id='tblSalesOrderReportBody'>
                 
                </tbody>
                  
            </table>

 
</div>




<div data-role="page" id="pageOrderStatus" >
    <div data-role="header">
        <h1 id="hdrSalesOrder">Sales Order No. <label id="lblOrderNo"></label></h1>
          <a href="#" data-icon="arrow-l" id="btnOrderDetailsBack">Back</a>
    </div>

            <div data-role="navbar" class="MiniNavBar1" style="padding-top:47px">
                <ul>
                    <li><a href="#" data-href="MainNav1a" id="navViewOrders">View Orders</a></li>
                    <li><a href="#" data-href="MainNav1b" id="btnSignature">Signature</a></li>
                    <li><a href="#" data-href="MainNav1c" id="btnInvoice">Invoice</a></li>
                    <li><a href="#" data-href="MainNav1d" id="btnStatusLog">Status</a></li>
                </ul>
            </div>
            <div id="MainNav1a" class="content_div">
        
                     <div data-role="content" >
                           <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%">
                                   Date: 
                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                                    <label id="headerInvoiceDate"></label>
                                </div>
                            </div>
                               <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%">
                                   Sales Representative:
                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                                    <label id="headerSalesRep"></label>

                                </div>
                            </div>
                            <br />
                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%">
                                    Customer: 

                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                                    <label id="headerClientName">[CustomerName]</label>

                                </div>
                            </div>
              
                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%">

                                    Delivery Address: 
                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                                    <label id="headerClientDeliveryAddress">[CustomerAddress]</label> 
                                </div>
                            </div>

                            <div class="ui-grid-a">
                                <div class="ui-block-a" style="width:30%">
                                     Billing Address: 
                                </div>
                                <div class="ui-block-b" style="text-align:right;width:70%">
                  
                                    <label id="headerClientBillingAddress">[BillingAddress]</label>
                                </div>
                            </div>
                                    
                     </div>
            
                 <br />
 
                <table data-role="table" id="tblInvoice" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
                    <thead>
                        <tr>
                            <th>Stock No.</th>
                            <th>Stock Description</th>
                            <th style="text-align: center">Order Qty</th>
                            <th style="text-align: center">Served Qty</th>
                            <th>Unit Measure</th>
                            <th style="text-align: right">Unit Price</th>
                            <th style="text-align: right">Sub Total</th>
                        </tr>                    
                    </thead>                
                    <tbody id='tblInvoiceBody'>
                    </tbody>
                </table>
                <br />
                <div data-role="content">
                    <div style="text-align: right;"><label id="lblSKUCount" ></label></div><br />
                    <div style="text-align: right;"><label id="lblTotal" style="font-size: x-large" ></label></div>

                    <br />
               </div>
               <div class="ui-grid-a" >
                    <div class="ui-block-a">
                        <div id="btnDownloadFile" style="margin:0px; height: 80px;font-weight:normal;" data-role="button" data-corners="false">
                  
                        <img src="../../Content/images/download.png" width="45" height="45" /><br /> Download CSV
                        </div>
                           
                          
                    </div>
                    <div class="ui-block-b">
                        <div id="btnPrint" style="margin:0px; height: 80px;font-weight:normal" data-role="button" data-corners="false">
                     
                        <img src="../../Content/images/print.png" width="45" height="45" /><br /> Print Picking List
                       
                        </div>
                    </div>
                </div>
        </div>
   

         <div id="MainNav1b" class="content_div">

              
                <div data-role="content">
                    Signatory: <label id="lblSignatory"></label>
                    <br /><br />
                        <img id="imgSignature" alt="" />
                        
                    <br />
                
                </div>
            
 
          </div>

           <div id="MainNav1c" class="content_div">
               <div data-role="content">
        
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="width:30%;vertical-align:middle">
                            <label for="selInvoiceType" style="line-height: 3;">Invoice Type: <span style="color: #FF0000;" >*</span></label>
                        </div>
                        <div class="ui-block-b" style="text-align:right;width:70%">
                            <select id="selInvoiceType">
                                <option value="Charge Invoice">Charge Invoice</option>
                                <option value="Add New">Add New...</option>
                            </select>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="width:30%" >
                             <label for="txtInvoiceNo" style="line-height: 3;">Invoice Number: <span style="color: #FF0000;" >*</span></label>
                        </div>
                        <div class="ui-block-b" style="text-align:right;width:70%">
                            <input type="text" id="txtInvoiceNo" maxlength="15"/>
                        </div>
                    </div>
                    <div class="ui-grid-a">
                        <div class="ui-block-a" style="width:30%" >
                             <label for="txtRemarks" style="line-height: 3;">Remarks:</label>
                        </div>
                        <div class="ui-block-b" style="text-align:right;width:70%">
                            <textarea id="txtRemarks" cols="5"  rows="5" maxlength="255"></textarea>
                        </div>
                    </div>
                    <br />
                    <div style="text-align: center">
                        <a id="btnSaveInvoiceDetails" data-role="button" data-theme="e">
                        <h3>
                            Save</h3>
                        </a>
                    </div>
                    <div style="text-align: center;">
                        <a id="btnPrintInvoiceDetails" data-role="button" data-theme="e">
                        <h3>
                           Print Invoice</h3>
                        </a>
     
          </div>

            <div data-role="popup" id="pageInvoiceType" data-theme="b" class="ui-corner-all"> 
                <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
                <center><h3>Type of Invoice</h3></center>
                <div data-role="content">

                    <label for="txtInvoiceType">New Invoice Type:</label>
                    <input type="text" id="txtInvoiceType"/>

                    <label for="txtInvoiceTypeAbbr">New Invoice Type Abbreviation:</label>
                    <input type="text" id="txtInvoiceTypeAbbr"/>
                             
                    <br /> 
                    <button id="btnAddInvoiceType" data-icon="add">Add</button>
                </div>       
            </div>

           </div>
          
	    </div>
         <div id="MainNav1d" class="content_div">
         
                     <div data-role="content">
                      <table data-role="table" id="tblStatus" data-mode="reflow" class="ui-responsive myTable">
                        <thead>
                            <tr style="cursor: pointer">
                                <th data-sort="date">Date</th>
                                <th data-sort="string">Action</th>
                                <th data-sort="string">User</th>
                            </tr>
                                   
                        </thead> 
                            
                        <tbody id='tblStatusTable'>
                 
                        </tbody>
                  
                    </table>
                 <hr />  <br />
                 <div class="ui-grid-a">
                        <div class="ui-block-a" style="width: 33%;padding-left:8px;">
                            No. of Downloads: 
                        </div>
                        <div class="ui-block-b" style="width: 5%;text-align:right">
                  
                            <label id="lblDownloadCount"></label>
                        </div>
                  
                    </div>  <br />
                     <div class="ui-grid-a">
                        <div class="ui-block-a" style="width: 33%;padding-left:8px;">

                            No. of Prints: 
                        </div>
                        <div class="ui-block-b" style="width: 5%;text-align:right">
                            <label id="lblPrintCount"></label> 
                        </div>
                    </div>

                    <br />

                </div>
          
	    </div>

            
           
    </div>
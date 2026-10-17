@Code
    ViewData("Title") = "Materials Requisition"
    Layout = "~/Views/Shared/Layout4.vbhtml"
End Code

@Section head
    <link type="text/css" rel="Stylesheet" href="/Content/flatui-css-custom.css" />
    <script type="text/javascript" src="/Scripts/PrintPDF/require.js"></script>
    <script type="text/javascript" src="/Scripts/sources/BOMProjectExcel/GeneratePDF.js"></script>
    @*<script type="text/javascript" src="/Scripts/PrintPDF/addhtml.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/html2canvas.js"></script>*@
    

    <script>
        $(document).ready(function () {
            $('#divFooterAjax').hide();
        })

        function demoFromHTML() {
//            var doc = new jsPDF();

//            // We'll make our own renderer to skip this editor
//            var specialElementHandlers = {
//                '#fromHTMLtestdiv': function (element, renderer) {
//                    return true;
//                }
//            };

//            // All units are in the set measurement for the document
//            // This can be changed to "pt" (points), "mm" (Default), "cm", "in"
//            doc.fromHTML($('#fromHTMLtestdiv').html(), 15, 15, {
//                'width': 600,
//                'elementHandlers': specialElementHandlers
//            });

//            doc.save('Test.pdf');
//            return;
            var pdf = new jsPDF('p', 'pt', 'letter')

            // source can be HTML-formatted string, or a reference
            // to an actual DOM element from which the text will be scraped.
	, source = $('#fromHTMLtestdiv').html()

            // we support special element handlers. Register them with jQuery-style 
            // ID selector for either ID or node name. ("#iAmID", "div", "span" etc.)
            // There is no support for any other type of selectors 
            // (class, of compound) at this time.
	, specialElementHandlers = {
	    // element with id of "bypass" - jQuery style selector
	    '#fromHTMLtestdiv': function (element, renderer) {
	        // true = "handled elsewhere, bypass text extraction"
	        return true
	    }
	}

            margins = {
                top: 40,
                bottom: 60,
                left: 40,
                width: 600
            };
            // all coords and widths are in jsPDF instance's declared units
            // 'inches' in this case
            pdf.fromHTML(
    	source // HTML string or DOM elem ref.
    	, margins.left // x coord
    	, margins.top // y coord
    	, {
    	    'width': margins.width // max width of content on PDF
    		, 'elementHandlers': specialElementHandlers
    	},
    	function (dispose) {
    	    // dispose: object with X, Y of the last line add to the PDF 
    	    //          this allow the insertion of new lines after html
    	    pdf.save('Test.pdf');
    	},
    	margins
    )
        }
    </script>
End Section

<div id="dvMain" data-role="page">
    <a data-role="button" id="btnPrint">print</a>
    <div id="fromHTMLtestdiv" style="@*overflow:auto; *@padding:50px; background-color:#FFF; color:#000 !important; line-height: 1.3; @*height: 1265px; width:595px;*@">
        <div id="divHeaderDetails">
            <div>
                <div style="float:left; width:350px; text-align:left;">
                    <h2>ABC Company</h2>
                </div>
                <div style="float:right; text-align:right;">
                    <h2>Sales Order</h2>
                </div>
                <div style="clear:both;"></div>
            </div>

            <div>
                <div style="float:left; width:300px; text-align:left;">
                    <span>
                        Mezzanine Floor, The Paper Tree Building Bolton St., Davao City 8000, Philippines
                    </span>
                </div>
                <div style="float:right; text-align:right;">
                    SO Number: <b>CHI-CAS 0063289</b><br />
                    SO Date: <b>Wednesday, Sept 2, 2015</b><br />
                    Delivery Date: <b>Friday, Sept 4, 2015</b><br />
                    <b>Page: 1 of 3</b>
                </div>
                <div style="clear:both;"></div>
            </div>

            <br />

            <div>
                <div style="float:left; width:300px; text-align:left;">
                    Customer: <b>Jarlego and Associates</b>
                    <br />
                    <span>
                        192 Pulang Bato St., Purok San Diego Batangas City, Philippines
                    </span>
                </div>
                <div>
                    <img src="/Content/images/qrcode.jpg" id="imgQrCode" alt="qrcode" style="margin-left:100px;" />
                </div>
                <div style="clear:both;"></div>
            </div>
        </div> @*end divHeaderDetails*@

        <br />

        <div>
            <div style="height:280px; width:32.33%; float:left; padding:5px;">
                <div style="border:1px solid #000; height:100%; padding:5px;">
                    <b>BILLING DETAILS</b>
                    <br /><br />

                    <div style="height:175px;">
                        <b>Delivery Address:</b>
                        <br />
                        <span>Mezzanine Floor, The Paper Tree Building Bolton St., Davao City 8000, Philippines</span>
                    </div>                   
                    
                    <b>Contact No.:</b>
                    <br />
                    <span>+63-82-286-9579</span><br />
                    <span>+63-82-221-1698</span>
                </div>
            </div>
            <div style="height:280px; width:32%; float:left; padding:5px;">
                <div style="border:1px solid #000; height:100%; padding:5px;">
                    <b>DELIVERY DETAILS</b>
                    <br /><br />

                    <div style="height:175px;">
                        <b>Delivery Address:</b>
                        <br />
                        <span>T. Calo Extension Street, Purok 2, Barangay 14, Poblacion Limaha, Butuan City, 8600, Philippines</span>
                    </div>                   
                    
                    <b>Contact No.:</b>
                    <br />
                    <span>+63-917-490-7853</span><br />
                    <span>+63-085-300-3104</span>
                </div>
            </div>
            <div style="height:280px; width:30.33%; float:left; padding:5px;">
                <div style="border:1px solid #000; height:100%; padding:5px;">
                    <b>DELIVERY INSTRUCTIONS</b>
                    <br /><br />

                    <span>
                        The Landing Pilot is the Non-Handling Pilot until the decision altitude call, when the Handling Non-Landing Pilot hands the handling to the Non-Handling Landing Pilot, unless the latter 'calls go around,'
                    </span>
                </div>
            </div>
            <div style="clear:both;"></div>
        </div>
        
        <br />
        <br />

        <table style="width:100%;" cellspacing="0" cellpadding="3">
            <thead>
                <tr style="text-align:left;">
                    <th style="width:160px; border-bottom: 2px solid #000;">Item Code:</th>
                    <th style="width:380px; border-bottom: 2px solid #000;">Item Description:</th>
                    <th style="width:80px; border-bottom: 2px solid #000;">Quantity:</th>
                    <th style="width:80px; border-bottom: 2px solid #000;">UoM:</th>
                    <th style="width:100px; border-bottom: 2px solid #000;">Unit Price:</th>
                    <th style="width:100px; border-bottom: 2px solid #000;">Sub Total:</th>
                </tr>
            </thead>
            <tbody id="tbodyDetails">
                
            </tbody>
        </table>

        <br />
        <br />
    </div>
</div>
$(function () {
    var divPage = $('#divCustomerServiceLevel');
    var divResult = divPage.find('[name=divResult]');
    var divData = divPage.find('[name=divData]');
    var divChart = divPage.find('[name=divChart]');
    var divChart2 = divPage.find('[name=divChart2]');
    var txtInvoiceDateFrom = divPage.find('[name=txtInvoiceDateFrom]')
        .width(100)
        .datepicker({
            changeMonth: true,
            changeYear: true
        });
    var txtInvoiceDateTo = divPage.find('[name=txtInvoiceDateTo]')
        .width(100)
        .datepicker({
            changeMonth: true,
            changeYear: true
        });
    var cbDurationType = divPage.find('[name=cbDurationType]');
    var butDisplay = divPage.find('[name=butDisplay]')
        .click(butDisplay_Click);
    var mValidator;
    var mChart, mChart2; // store in variable coz we have to destroy an existing chart before we draw another to prevent overlap

    (function () {  // Init

        mValidator = new Validator;
        mValidator.SummaryDiv = divResult;

        mValidator.AddItem({
            Label: 'Invoice Date From',
            Control: txtInvoiceDateFrom,
            ErrorControl: null
        }).AddDateValidation()
        .AddRequiredValidation();

        mValidator.AddItem({
            Label: 'Invoice Date To',
            Control: txtInvoiceDateTo,
            ErrorControl: null
        }).AddDateValidation()
        .AddRequiredValidation();

    })(); // END Init

    function butDisplay_Click() {

        if (mValidator.Validate() == false) return;

        divPage.block();
        divResult.hide();
        AjaxSend({
            url: '/Reports/GetCustomerServiceLevel',
            data: {
                DateFrom: txtInvoiceDateFrom.val(),
                DateTo: txtInvoiceDateTo.val(),
                DurationType: cbDurationType.val()
            },
            success: function (vData) {
                divPage.unblock();
                if (HandleSessionExpired(vData) == false) return;
                if (vData.d.Error)
                    divResult.text(vData.d.Error).MakeError();
                else
                    divResult.hide();
                LoadGrid(vData.d.Data);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divPage.unblock();
                divResult.text(XMLHttpRequest.responseText).MakeError();
            }
        });

    }; // END butDisplay_Click

    function LoadGrid(vData) {

        var pMaxY = 0;

        for (var p = 0; p <= vData.Rows.length - 1; p++) {
            vData.Rows[p].DateRange = 'Week #' + vData.Rows[p].WeekNum + ': ' + vData.Rows[p].DateRange;
            if (vData.Rows[p].Total > 0) {
                vData.Rows[p].Percent = parseInt(vData.Rows[p].Perfect / vData.Rows[p].Total * 100);
                if (pMaxY < vData.Rows[p].Total) pMaxY = vData.Rows[p].Total * 1.2;
            } else
                vData.Rows[p].Percent = 0;
        };

        vData.Columns = {
            'DateRange': {
                'Label': 'Invoice date range'
            },
            'Total': {
                'Label': 'Line items',
                'MajorDataType': 'Numeric',
                'Width': 100,
                'Format': '#,##0'
            },
            'Perfect': {
                'Label': 'Pefect line items',
                'MajorDataType': 'Numeric',
                'Width': 100,
                'Format': '#,##0'
            },
            'Percent': {
                'Label': 'Pefect %',
                'MajorDataType': 'Numeric',
                'Width': 100,
                'Format': '#,##0'
            }
        };
        divData.ProWebGrid(vData);
        LoadChart(vData, pMaxY);

    }; // END LoadGrid

    function LoadChart(vData, vMaxY) {

        var pLineTotal = [];
        var pLinePerfect = [];
        var pLinePercent = [];
        var pWeek;

        for (var p = 0; p <= vData.Rows.length - 1; p++) {
            pWeek = vData.Rows[p].DateRange;
            pLineTotal.push([pWeek, vData.Rows[p].Total]);
            pLinePerfect.push([pWeek, vData.Rows[p].Perfect]);
            pLinePercent.push([pWeek, vData.Rows[p].Percent]);
        };
         
        $.jqplot.config.enablePlugins = true; // enable for zoom
        if (mChart) mChart.destroy();
        if (mChart2) mChart2.destroy();

        mChart = $.jqplot('divChart', [pLineTotal, pLinePerfect], {
            title: 'Customer Service Level',
            series: [
                {
                    renderer: $.jqplot.BarRenderer,
                    rendererOptions: {
                        barMargin: 20
                    },
                    label: 'Line items',
                    color: 'green'
                },
                {
                    renderer: $.jqplot.BarRenderer,
                    rendererOptions: {
                        barMargin: 20
                    },
                    label: 'Perfect line items',
                    color: 'red'
                }
            ],
            axes: {
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    label: 'Date',
                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                    tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                    tickOptions: {
                        enableFontSupport: true,
                        angle: -30
                    }
                },
                yaxis: {
                    autoscale: true,
                    min: 0,
                    max: vMaxY,
                    label: 'Number of line items',
                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                    tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                    tickOptions: {
                        enableFontSupport: true,
                        formatString: '%d'
                    }
                }
            },
            legend: {
                show: true
            },
            cursor: {
                zoom: true,
                showTooltip: false
            },
            highlighter: {
                tooltipLocation: 'n',
                showMarker: false,
                tooltipContentEditor: function (str, si, pi, plot) {

                    return HtmlEncode(plot.data[si][pi][0]) + '<br />' + HtmlEncode(plot.series[si].label) + ': ' + HtmlEncode(plot.data[si][pi][1]);

                }
            },
            gridPadding: {left: 120}
        });

        mChart2 = $.jqplot('divChart2', [pLinePercent], {
            title: 'Customer Service Level',
            series: [
                {
                    renderer: $.jqplot.BarRenderer,
                    label: 'Perfect %',
                    color: 'blue'
                }
            ],
            axes: {
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    label: 'Date',
                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                    tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                    tickOptions: {
                        enableFontSupport: true,
                        angle: -30
                    }
                },
                yaxis: {
                    min: 0,
                    max: 100,
                    label: 'Perfect %',
                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                    labelOptions: {
                        angle: 90
                    },
                    tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                    tickOptions: {
                        formatString: '%d'
                    }
                }
            },
            legend: {
                show: true
            },
            cursor: {
                zoom: true,
                showTooltip: false
            },
            highlighter: {
                tooltipLocation: 'n',
                tooltipContentEditor: function (str, si, pi, plot) {

                    return HtmlEncode(plot.data[si][pi][0]) + '<br />' + HtmlEncode(plot.series[si].label) + ': ' + HtmlEncode(plot.data[si][pi][1]);

                }
            },
            gridPadding: { left: 120 }
        });

    }; // END LoadChart
});
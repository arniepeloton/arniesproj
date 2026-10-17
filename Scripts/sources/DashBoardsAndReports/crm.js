require([
    'common',
    ], function () {

        var GroupBy;
        var MonthlyQuota = 0;
        var MonthList = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

        $(function () {
            //GetAllTeams();
            $('#btnTeams').click(function () { 
                $('#panTeams').popup('open');
            });

            $('#liMonth').click(function () { 
                $('#panSalesQuotaRange').popup('close');
                $('#panSalesQuotaMonthPeriod').popup('open');
            });

            $('#liQuarter').click(function () {
                $('#panSalesQuotaRange').popup('close');
                $('#panSalesQuotaQuarterPeriod').popup('open');
            });

            $('#liYearToDate').click(function () {
                $('#panSalesQuotaRange').popup('close');
                $('#lblPeriod').text('[ ' + $('#liYearToDate').text() + ']');
                SetPeriod('YearToDate', 0);
                //                SetSettings();
            });

            $('#btnDoneSalesQuotaMonthPeriod').click(function(){                 
                $('#lblPeriod').text('[ ' + $('#selSalesQuotaMonth :selected').text() + ' ]');                
                $('#panSalesQuotaMonthPeriod').popup('close');
                SetPeriod('Month', 0);
            });

            $('#btnDoneSalesQuotaQuarterPeriod').click(function(){                 
                $('#lblPeriod').text('[ ' + $('#selSalesQuotaQuarter :selected').text() + ' ]');                
                $('#panSalesQuotaQuarterPeriod').popup('close');
                SetPeriod('Quarter', 0);
            });
        });

        window.SetMonth = function(){           

            var pLV = $('#selSalesQuotaMonth');

            pLV.empty();

            pLV.append(new Option('Select Month', '', true));

            for(i=0;i<12;i++){
                pLV.append(new Option(MonthList[i], i, false));
            }
            try{
                pLV.selectmenu('refresh');
            }
            catch(e){
                pLV.selectmenu();
            }
        }

        window.GetAllTeams = function () {
            AjaxSend({
                url: 'crm/GetAllTeams',
                success: function (vData) {
                    DisplayTeams(vData.Results.Rows);
                }
            });
        }

        function DisplayTeams(vRows) {
            var pLV = $("#tblFOWST");

            $('#tblFOWST tbody').empty();
            $('#lstFOWSTFrame li').not(':eq(0)').not(':eq(0)').remove();

            for (var i = 0; i <= vRows.length - 1; i++) {

                var checked = undefined;

                if (vRows[i].ID == sessionStorage.FowstID) {
                    checked = 'checked';
                }

                pItem = $('<tr>')
                    .append($('<td>')
                        .append($('<input type=radio data-theme=e />')
                            .attr('ID', vRows[i].ID)
                            .attr('name', 'Teams')
                            .attr('checked', checked)
                            .attr('data-mini', 'true')
                            .data('data', vRows[i])
                            .click(function () {
                                $('#panFOWST').popup('close');
                                $('#lblFOWST').text('[ ' + $(this).data('data').TeamName + ' ]')
                            })
                        )
                        .append($('<label>')
                            .attr('for', vRows[i].ID)
                            .text(vRows[i].TeamName)
                        )
                    )//td                    

                pLV.append(pItem);

                $('#' + vRows[i].ID).checkboxradio();

            }

            $('#lstFOWSTFrame').listview('refresh');
        }

        window.GetSalesOrderData = function (data,GroupDateBy) {
            GroupBy = GroupDateBy;
            AjaxSend({
                url: 'crm/GetSalesOrderData',
                data: data,
                success: function (vData) {
                    if(vData.Results.Rows.length>0){                        
                        var mGrandTotal = 0;
                        for(i=0;i<=vData.Results.Rows.length-1;i++){
                            mGrandTotal += parseFloat(vData.Results.Rows[i].Total);
                            MonthlyQuota = parseFloat(vData.Results.Rows[i].Tier1Quota);
                        }

                        if(mGrandTotal >0){
                            DisplaySalesOrderData(vData.Results.Rows);
                        }
                        else{
                            MonthlyQuota = 0;
                            HideSalesDistributionTables();
                            $('#IsDataPresent').css('visibility', 'visible')
                            $('#lblMainChartInstruction').css('visibility', 'hidden');
                            $('#tblSalesOrder').css('visibility', 'hidden');
                            $('#tblSalesOrderEmployeeContribution').css('visibility', 'hidden');
                            $('#tblSalesOrderWeek').css('visibility', 'hidden');                            
                        }
                        
                    }
                    else{
                        MonthlyQuota = 0;
                        HideSalesDistributionTables();
                        $('#IsDataPresent').css('visibility', 'visible')
                        $('#lblMainChartInstruction').css('visibility', 'hidden');
                        $('#tblSalesOrder').css('visibility', 'hidden');
                        $('#tblSalesOrderEmployeeContribution').css('visibility', 'hidden');
                        $('#tblSalesOrderWeek').css('visibility', 'hidden');
                    }
                }
            });

            if (GroupBy=='Month'){
                GetEmployeeSalesData(data);
                $('#tblSalesOrderEmployeeContribution').css('visibility','visible');
                $('#tblSalesOrderEmployeeContribution').css('display','block');
                $('#tblSalesOrderWeek').css('visibility','visible');
                $('#tblSalesOrderWeek').css('display','block');
            }
            else{
                $('#tblSalesOrderEmployeeContribution').css('visibility','hidden');
                $('#tblSalesOrderEmployeeContribution').css('display','none');
                $('#tblSalesOrderWeek').css('visibility','hidden');
                $('#tblSalesOrderWeek').css('display','none');
                $('#tblSalesOrderWeek').css('visibility','visible');
                $('#tblSalesOrderWeek').css('display','block');
            }
        }


        function DisplaySalesOrderData(vRows) {
            var pLV = $('#tblSalesOrderDetails')
            var pLV2 = $('#tblSalesOrderWeekDetails')
            var Total = 0;
            var GrandTotal = 0;
            var m_Totals = [];
            var m_Labels = [];
            var m_TotalsCount = 0;

            m_Totals[0] = [];

            pLV.empty();
            pLV2.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                if(vRows[i].Total!='0'){
                    tData = $('<tr>')
                            .append($('<td style="text-align:center">')
                                .text(vRows[i].SalesOrderNo)
                            )
                            .append($('<td style="text-align:center">')
                                .text(vRows[i].Invoicedate)
                            )
                            .append($('<td style="text-align:center">')
                                .text(vRows[i].CompanyName)
                            )
                            .append($('<td style="text-align:center">')
                                .text(AccountingFormat($.format.number(vRows[i].Total)))
                            )
               
                    pLV.append(tData);

                }//if

                GrandTotal+= parseFloat(vRows[i].Total);
                Total += parseFloat(vRows[i].Total);

                if(GroupBy=='Month'){

                    $('#lblWeekOrMonth').text('Week No.');

                    if(i == (vRows.length - 1)){
                        m_Totals[0][m_TotalsCount] = parseFloat(vRows[i].QuotaPerWeek);
                        m_Totals[0][m_TotalsCount+1] = Total;

                        m_Labels[m_TotalsCount] = 'Quota'
                        m_Labels[m_TotalsCount+1] = 'Week ' + vRows[i].Week
                        m_TotalsCount++;

                        tData = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].Week)
                            )
                            .append($('<td style="text-align:right">')
                                .text(AccountingFormat(vRows[i].QuotaPerWeek))
                            )
                            .append($('<td style="text-align:right">')
                                .text(AccountingFormat(Total))
                            )
                            
                    pLV2.append(tData);

                        //visible if naa sa monthly view.
                        $('#lblMainChartInstruction').css('visibility', 'visible');
                        $('#lblMainChartInstruction').css('font-size', 'large');
                        $('#lblMainChartInstruction').text('Sales Orders: Sales Target : ' + (AccountingFormat(GrandTotal/parseFloat(vRows[i].Tier1Quota)*100)) + '%');
                    }
                    else{
                        if(vRows[i].Week!=vRows[i+1].Week){
                            
                            m_Totals[0][m_TotalsCount] = parseFloat(vRows[i].QuotaPerWeek);
                            m_Totals[0][m_TotalsCount+1] = Total;

                            m_Labels[m_TotalsCount] = 'Quota'
                            m_Labels[m_TotalsCount+1] = 'Week ' + vRows[i].Week

                            tData = $('<tr>')
                                    .append($('<td>')
                                        .text(vRows[i].Week)
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(vRows[i].QuotaPerWeek))
                                    )
                                    .append($('<td style="text-align:right">')
                                        .text(AccountingFormat(Total))
                                    )
                            
                            pLV2.append(tData);

                            m_TotalsCount = m_TotalsCount+2;
                            Total=0;
                        }
                    }
                }
                else{
                    $('#lblWeekOrMonth').text('Month');

                    if(i == (vRows.length - 1)){
                        m_Totals[0][m_TotalsCount] = parseFloat(vRows[i].Tier1Quota);
                        m_Totals[0][m_TotalsCount+1] = Total;

                        m_Labels[m_TotalsCount] = 'Quota';
                        m_Labels[m_TotalsCount+1] = MonthList[parseInt(vRows[i].Month)-1];

                        tData = $('<tr>')
                            .append($('<td>')
                                .text(MonthList[parseInt(vRows[i].Month)-1])
                            )
                            .append($('<td style="text-align:right">')
                                .text(AccountingFormat(parseFloat(vRows[i].Tier1Quota)))
                            )
                            .append($('<td style="text-align:right">')
                                .text(AccountingFormat(Total))
                            )
                            
                        pLV2.append(tData);

                        m_TotalsCount++;
                    }
                    else{
                        if(vRows[i].Month!=vRows[i+1].Month){

                            m_Totals[0][m_TotalsCount] = parseFloat(vRows[i].Tier1Quota);
                            m_Totals[0][m_TotalsCount+1] = Total;

                            m_Labels[m_TotalsCount] = 'Quota';
                            m_Labels[m_TotalsCount+1] = MonthList[parseInt(vRows[i].Month)-1];

                            tData = $('<tr>')
                            .append($('<td>')
                                .text(MonthList[parseInt(vRows[i].Month)-1])
                            )
                            .append($('<td style="text-align:right">')
                                .text(AccountingFormat(parseFloat(vRows[i].Tier1Quota)))
                            )
                            .append($('<td style="text-align:right">')
                                .text(AccountingFormat(Total))
                            )
                            
                        pLV2.append(tData);

                            m_TotalsCount = m_TotalsCount+2;
                            Total=0;
                        }
                    }
                }                
            }//for
                      

            DisplaySalesOrderChart(m_Totals, m_Labels);

            try{
                $('#tblSalesOrder').table('refresh');
            }
            catch(e){
                $('#tblSalesOrder').table();
            }

            HideSalesDistributionTables();
        }

        function DisplaySalesOrderChart(m_Totals, m_Labels) {
            var plot2 = $.jqplot('dvMainChart', m_Totals, {
                        seriesColors: [ "#73C774", "#C7754C"],
                        animate: !$.jqplot.use_excanvas,
                        seriesDefaults: {
                            renderer: $.jqplot.BarRenderer,
                            pointLabels: {
                                show: true,
                                //labels: m_Labels
                            },
                            rendererOptions: {
                                // Set varyBarColor to tru to use the custom colors on the bars.
                                varyBarColor: true
                            }
                        },
                        axes: {
                            xaxis: {
                                renderer: $.jqplot.CategoryAxisRenderer,
                                ticks: m_Labels
                            },
                            yaxis: {

                        }
                    },
                    highlighter: { show: false }                    
                });
        }

         function GetEmployeeSalesData(data){
            AjaxSend({
                url: 'crm/GetEmployeeSalesData',
                data: data,
                success: function (vData) {
                    if(vData.Results.Rows.length>0){
                        DisplayEmployeeSalesData(vData.Results.Rows);                        
                    }                 
                }
            });
        }

        function DisplayEmployeeSalesData(vRows) {
            var pLV = $('#tblSalesOrderEmployeeContributionDetails');
           
           var mToTal = 0;
           var mTotalPercent = 0;
           var mQuotaPerMember = parseFloat(parseFloat(MonthlyQuota) / parseInt(vRows.length));

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                tData = $('<tr>')
                        .append($('<td style="text-align:center">')
                            .text(vRows[i].Name)
                        )                        
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat(mQuotaPerMember))
                        )                        
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat(vRows[i].Total))
                        )
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat((parseFloat(vRows[i].Total)/MonthlyQuota)*100) + '%')
                        )
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat(mQuotaPerMember - parseFloat(vRows[i].Total)))
                        )
               
                    pLV.append(tData);

                    mToTal += parseFloat(vRows[i].Total);
                    mTotalPercent += (parseFloat(vRows[i].Total)/MonthlyQuota);
                }

                //total
                tData = $('<tr>')
                        .append($('<td style="text-align:center">')
                            .text('Total')
                        )                        
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat(MonthlyQuota))
                        )                        
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat(mToTal))
                        )
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat(parseFloat(mTotalPercent)*100) + '%')
                        )
                        .append($('<td style="text-align:center">')
                            .text(AccountingFormat(MonthlyQuota - mToTal))
                        )
               
                    pLV.append(tData);
            try{
                $('#tblSalesOrderEmployeeContribution').table('refresh');
            }
            catch(e){
                $('#tblSalesOrderEmployeeContribution').table();
            }
        }

       

        function HideSalesDistributionTables(){
            $('#tblSalesOrder').css('visibility', 'visible');
            
            $('#tblGrossSalesLabels').css('visibility', 'hidden');
            $('#tblGrossSalesLabels').css('display', 'block');

            if(GroupBy!='Month'){
                $('#lblMainChartInstruction').css('visibility', 'hidden');
            }
             
            $('#tblLastInvoiceDetails').css('visibility', 'hidden');
            $('#IsDataPresent').css('visibility', 'hidden')

            $('#dvSalesDistributionTable').css('display', 'none');
                        
            $('#dvSalesOrderTable').css('display', 'block');
        }

    });
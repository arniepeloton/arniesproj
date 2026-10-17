require(['common',
    '/Scripts/sources/ImportExportData/ImportExportData.js?crc=',
    '/Scripts/sources/ImportExportData/ImportExportData_offline.js?crc=',
    'chartjs'
], function () {
    var boardUUID = '';
    var callback; // will be used after it gets respond from the machine
    var frommotorno, tomotorno;
    var totalAmountCashInserted = 0;
    var htmlInventory = '';
    var DBCreationStart = '';

    window.InAppSettings = function (response) {
        $('pre').text(JSON.stringify(response, null, 2))

        if (response.action == 'PEER_LIST') {
            var boards = response.devices;
            $('#selAvailableBoards').empty();

            for (var i in boards) {
                $('#selAvailableBoards').append(new Option((boards[i].modelId == 0 ? 'Main Board - ' : 'Daugther Board - ') + boards[i].uuid, boards[i].uuid));
            }

            if (boards.length > 0) boardUUID = boards[0].uuid;
        } else if (response.action == 'CASH_COLLECT') {
            if (response.status == 'CREDIT') {
                totalAmountCashInserted += CDec2(response.amount);

                if (CDec2($('#CashAcceptAmount').val()) == totalAmountCashInserted) {
                    UsbSerial_Send({
                        "action": "CASH_COLLECT",
                        "uuid": boardUUID,
                        "accept": false
                    })
                }               
            }
        }

        if (typeof callback == 'function') {
            callback();
        }
    }

    function UsbSerial_Send(params, cb) {
        callback = null;
        sessionStorage.IsInAppSettings = true;
        $('pre').text(JSON.stringify(params, null, 2));

        Js.Execute({
            command: 'UsbSerial.Send',
            value: JSON.stringify(params)
        })

        if (typeof cb == 'function') {
            callback = cb;
        }
    }

    function QtyDisplay(action, field) {
        var txtField = $('#' + field);
        var qty = CDec2(txtField.val());

        if (action == '-') {
            if (qty > 0) qty -= 1;
        }

        if (action == '+') {
            if (qty > 0) qty += 1;
        }

        txtField.val(qty || 1);
    }

    function DispenseItem() {
        UsbSerial_Send({
            "action": "DISPENSE_ITEM",
            "uuid": boardUUID,
            "motor": frommotorno,
            "turns": CDec2($('#TurnNo').val())
        }, function () {
            if (frommotorno < tomotorno) {
                frommotorno += 1;
                DispenseItem();
            }
        })
    }

    function GetInventoryPage() {
        if (htmlInventory == '') {
            AjaxSend({
                ExactUrl: '/InventoryCount/BsIndex?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    htmlInventory = vData;
                    $('#inventory').append(vData).children().not('#pageCountByBinAndArea').remove();
                    $('#pageCountByBinAndArea').css('font-size', '1rem');
                    $('#pageCountByBinAndArea nav').remove();
                    $('#pageCountByBinAndArea #btnBinLocBack').parent().remove();
                    $('#pageCountByBinAndArea').show();
                    $('#lblWorkFowstName').val(sessionStorage.FowstID);
                }
            });
        } else {
            GetNamedFowstBin();
        }
    }

    var chartADS = null;
    function DisplayBarChart(data) {
        var dS = [
            {
                data: [], 
                borderColor: 'rgb(24, 135, 80)',
                backgroundColor: 'rgba(24, 135, 80, 1)',
                borderWidth: 2,
                borderRadius: Number.MAX_VALUE,
                borderSkipped: false,
                maxBarThickness: 50, //in px
            }
        ], lbl = [];
        var ads = document.getElementById('chartADS');

        if (chartADS != null) {
            chartADS.destroy();
        }

        for (var i in data.DataSets) {
            lbl.push(data.Label[i]);
            dS[0].data.push(data.DataSets[i]);
        }

        chartADS = new Chart(ads, {
            type: 'bar',
            data: {
                labels: lbl,
                datasets: dS
            },
            options: {
                showTooltips: false,
                animation: {
                    onComplete: function (animation) {
                        const chart = animation.chart;
                        const ctx = chart.ctx;

                        // Set font styles (v3 scale fonts are deeply nested, setting directly is safer)
                        ctx.font = "14px Outfit";
                        ctx.fillStyle = "#ffffff";
                        ctx.textAlign = "center";
                        ctx.textBaseline = "middle";

                        // Iterate through each dataset
                        chart.data.datasets.forEach(function (dataset, i) {
                            const meta = chart.getDatasetMeta(i);

                            meta.data.forEach(function (bar, index) {
                                const dataValue = dataset.data[index];

                                // Avoid rendering labels for empty or null data points
                                if (dataValue === null || dataValue === undefined) return;

                                // For vertical bar charts, the middle is halfway between bar.y and bar.base
                                const middleY = bar.y + (bar.base - bar.y) / 2;

                                // Draw the text exactly in the center of the bar
                                ctx.fillText(dataValue, bar.x, middleY);
                            });
                        });
                    }
                },

                plugins: {
                    tooltip: {
                        enabled: false
                    },
                    legend: {
                        display: false // Wrapped inside 'plugins' for v3+
                    },
                    datalabels: {
                        display: true,
                        anchor: 'end',
                        align: 'end',
                        color: '#000000',
                        font: {
                            size: 14,
                            weight: 'bold'
                        }
                    },
                    onAnimationComplete: function () {
                        this.showTooltip(this.datasets[0].points, true);
                    }
                },
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        //stacked: true,
                        stacked: false,
                        ticks: {
                            beginAtZero: true,
                            maxRotation: 0,
                            minRotation: 0
                        }
                    }],
                    yAxes: [{
                        stacked: false,
                        grace: '20%'
                    }]
                },
            }
        });
    }

    VendingMachine.RefreshSalesDashboard = function () {
        var sales = VendingMachine.GetSalesSummary($('[name="chbSalesPeriod"]:checked').val());
        var listMostSoldItems = $('#listMostSoldItems').empty();
        var tbodySalesByCategory = $('#tbodySalesByCategory').empty();

        $('#lblNetSales').text(AccountingFormat(sales.CurNetSales, 2, '0.00'));
        $('#lblNetSalesPercentage').parent().removeAttr('class').addClass('badge p-2 pr-3 pl-3 rounded-pill ' + (CDec2(sales.CurSalesPercentage) >= 0 ? 'textvm-primary bgvm-secondary' : 'text-danger bg-danger-light'))
        $('#lblNetSalesPercentage').text((CDec2(sales.CurSalesPercentage) >= 0 ? '+' : '-') + AccountingFormat(sales.CurSalesPercentage, 2, '0.00', '', '') + '%');
        $('#lblItemsSold').text(sales.CurSoldItems);
        $('#lblItemsSoldPercentage').parent().removeAttr('class').addClass('badge p-2 pr-3 pl-3 rounded-pill ' + (CDec2(sales.CurSoldItemsPercentage) >= 0 ? 'textvm-primary bgvm-secondary' : 'text-danger bg-danger-light'))
        $('#lblItemsSoldPercentage').text((CDec2(sales.CurSoldItemsPercentage) >= 0 ? '+' : '-') + AccountingFormat(sales.CurSoldItemsPercentage, 2, '0.00', '', '') + '%');
        $('#lblTopItemSoldName').text(sales.TopItemSoldName);
        $('#lblTopItemSoldCount').text(sales.TopItemSoldCount);

        for (var i in sales.MostItemsSold) {
            var maxunit = CDec2(sales.MostItemsSold[0].NoOfSoldItems);
            listMostSoldItems.append($('<li class="list-group-item">')
                .append($('<div class="row align-items-center">')
                    .append($('<div class="col-auto">').append('<div class="bgvm-secondary p-2 textvm-primary rounded-circle text-center font-weight-bold" style="width: 40px;">'+ (CInt2(i) + 1) +'</div>'))
                    .append($('<div class="col-5">')
                        .append('<div class="fs-5 textvm-primary font-weight-bold text-truncate">' + sales.MostItemsSold[i].ProductName +'</div>')
                        .append('<div class="fs-6 text-secondary">' + sales.MostItemsSold[i].ProductCategory +'</div>')
                    )
                    .append($('<div class="col-3">')
                        .append($('<div class="progress" style="height:10px;">')
                            .append('<div class="progress-bar bgvm-primary" role="progressbar" style="width: ' + ((CDec2(sales.MostItemsSold[i].NoOfSoldItems) / maxunit) * 100) + '%"></div>')
                        )
                    )
                    .append($('<div class="col-2 text-center fs-5">').text(sales.MostItemsSold[i].NoOfSoldItems))
                )
            )
        }

        DisplayBarChart(sales.SalesOvertime);

        var totalSalesByCategory = sales.SalesByCategory.reduce((sum, item) => sum + +item.Gross, 0);
        sales.SalesByCategory = sales.SalesByCategory.map(item => {
            const gross = Number(item.Gross);

            // Calculate percentage (avoid division by 0)
            const percentage = totalSalesByCategory > 0 ? (gross / totalSalesByCategory) * 100 : 0;

            return {
                ...item,
                PercentageNumber: percentage, // Raw numeric value (e.g., 30.123)
                PercentageFormatted: `${Math.round(percentage)}%` // Rounded display (e.g., "30%")
            };
        });

        for (var i in sales.SalesByCategory) {
            tbodySalesByCategory.append($('<tr>')
                .append('<td></td>')
                .append('<td>' + sales.SalesByCategory[i].ProductCategory + '</td>')
                .append('<td>' + AccountingFormat(sales.SalesByCategory[i].Gross) + '</td>')
                .append('<td>' + sales.SalesByCategory[i].PercentageFormatted + '</td>')
            )
        }
    }

    $(document).ready(function () {
        UsbSerial_Send({
            action: 'PEER_LIST'
        })

        $('#lblMachineVersion').text('Version: ' + Android.AppVersion());

        var db = PeopleMenu_offline().getPeopleOrganizationList().Rows;

        for (var i in db) {
            if (ReadyGuid(db[i].OrganizationID) == ReadyGuid(sessionStorage.OrgID)) {
                DBCreationStart = ParseDate(db[i].DbInfo.DateStarted)
                $('#lblMachineDatabaseCreation').text('Database Created: ' + $.format.date(DBCreationStart, 'MM-dd-yyyy hh:mm a'));
                $('#lblMachineLastSync').text('Last Sync: ' + $.format.date(ParseDate(db[i].LastSync), 'MM-dd-yyyy hh:mm a'));
                break;
            }
        }

        $('#selAvailableBoards').change(function () {
            boardUUID = $(this).val()
        })

        $('#btnBacktoMainScreen').click(function () {
            $('#page_VMScreen').pageshow();
        })

        $('#btnVMSyncDB').click(function () {
            SyncDataPrompt(DBCreationStart);
        })

        $('[name="chbSalesPeriod"]').change(function () {
            var period = $(this).val();

            if (period == 'Today' || period == 'Yesterday') {
                $('.lblPreviosPeriod').text('day');
                $('#lblSalesOvertime').html('Hourly Revenue <span class="bi-dot"></span> ' + period);
            } else if (period == 'This Week') {
                $('.lblPreviosPeriod').text('week');
                $('#lblSalesOvertime').html('Daily Revenue <span class="bi-dot"></span> This Week');
            } else if (period == 'This Month') {
                $('.lblPreviosPeriod').text('month');
                $('#lblSalesOvertime').html('Weekly Revenue <span class="bi-dot"></span> This Month');
            } else if (period == 'This Year') {
                $('.lblPreviosPeriod').text('year');
                $('#lblSalesOvertime').html('Monthly Revenue <span class="bi-dot"></span> This Year');
            }
            VendingMachine.RefreshSalesDashboard();
        })

        $('.btnSettingMenu').click(function () {
            var dis = $(this);
            VendingMachine.Passcode(CStr2(dis.attr('accessright')), function () {
                $('.btnSettingMenu').removeClass('btnvm-primary');
                dis.addClass('btnvm-primary');
                $('#page_inappsettings').pageshow();
                $('#' + dis.attr('page')).pageshow();

                if (dis.attr('page') == 'inventory') {
                    GetInventoryPage();
                } else if (dis.attr('page') == 'moresettings') {
                    //GetInventoryPage();
                } else if (dis.attr('page') == 'salessummary') {
                    VendingMachine.RefreshSalesDashboard();
                } else {
                    $('#page_vdmsettings').pageshow();
                }
            })            
        })

        $('.btnFromMotorAction').click(function () {
            QtyDisplay($(this).attr('action'), 'FromMotorNo')
        })

        $('.btnToMotorAction').click(function () {
            QtyDisplay($(this).attr('action'), 'ToMotorNo')
        })

        $('.btnTurnAction').click(function () {
            QtyDisplay($(this).attr('action'), 'TurnNo')
        })

        $('[name="cash_acceptor"]').change(function () {
            if ($(this).val() == 'accept') {
                $('#accept_option').show();
                $('#dispense_option').hide();
            } else {
                $('#accept_option').hide();
                $('#dispense_option').show();
            }
        })

        $('.btnCashAcceptor').click(function () {
            QtyDisplay($(this).attr('action'), 'CashAcceptAmount')
        })

        $('.btnQtyCoin').click(function () {
            QtyDisplay($(this).attr('action'), 'QtyCoin')
        })

        $('#btnCoinAcceptorRun').click(function () {
            if ($('[name="cash_acceptor"]:checked').val() == 'accept') {
                totalAmountCashInserted = 0;

                UsbSerial_Send({
                    "action": "CASH_COLLECT",
                    "uuid": boardUUID,
                    "accept": true
                })
            } else {
                var qtycoin = CDec2($('#QtyCoin').val());
                UsbSerial_Send({
                    "action": "CASH_CHANGE",
                    "uuid": boardUUID,
                    "coin1": $('#coin1').is(':checked') ? qtycoin : 0,
                    "coin2": $('#coin2').is(':checked') ? qtycoin : 0,
                    "coin3": $('#coin3').is(':checked') ? qtycoin : 0,
                    "coin4": $('#coin4').is(':checked') ? qtycoin : 0
                })
            }            
        })

        $('#btnDisableCoinAcceptor').click(function () {
            UsbSerial_Send({
                "action": "CASH_COLLECT",
                "uuid": boardUUID,
                "accept": false
            })
        })

        $('#btnOpenVM').click(function () {
            UsbSerial_Send({
                "action": "CONTROL_LOCK",
                "uuid": boardUUID,
                "num": 1
            })
        })

        $('#btnRunControlPower').click(function () {
            UsbSerial_Send({
                "action": "CONTROL_POWER",
                "uuid": boardUUID,
                "power": CDec2($('[name="control_power"]:checked').val()),
                "state": CBool($('[name="control_power_switch"]:checked').val())
            })
        })

        $('.btnDoorState').click(function () {
            UsbSerial_Send({
                "action": "CONTROL_DOOR",
                "uuid": boardUUID,
                "state": CBool($(this).attr('state')),
                "door": CDec2($('[name="control_door"]:checked').val())
            })
        })

        $('#btnDispenseItem').click(function () {
            frommotorno = CDec2($('#FromMotorNo').val());
            tomotorno = CDec2($('#ToMotorNo').val());
            DispenseItem();
        })

        $('#btnDispenseVolume').click(function () {
            UsbSerial_Send({
                "action": "DISPENSE_VOLUME",
                "uuid": boardUUID,
                "motor": CDec2($('[name="dispense_volume_motor"]:checked').val()),
                "volume": CDec2($('[name="dispense_volume"]:checked').val())
            })
        })

        $('[name="display_screen"]').change(function () {
            if ($(this).val() == 'pin') {
                Android.PinApp();
            } else {
                Android.UnpinApp();
            }
        })

        $('[name="control_led"]').change(function () {
            UsbSerial_Send({
                "action": "CONTROL_LED",
                "uuid": boardUUID,
                "color": CDec2($(this).val())
            })
        })

        $('.btnValveStatus').click(function () {
            UsbSerial_Send({
                "action": "CONTROL_VALVE",
                "uuid": boardUUID,
                "motor": CDec2($('[name="valve_motor"]:checked').val()),
                "status": CBool($(this).attr('state'))
            })
        })

        $('.btnSettingMenu:eq(0)').click();
        //const video = document.getElementById('myVideo');

        //// Slow down playback speed (e.g., 0.8x or 0.5x)
        //video.playbackRate = 0.5;

        //// Optional: toggle pitch retention (supported in modern browsers/WebViews)
        //video.preservesPitch = false; // Gives a deeper, stretched audio effect
    })
});
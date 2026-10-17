define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/DataSummary.js?crc='
    ], function () {

    var mLvWifi;
    var mPage = $('#pageItem');
    var mTxtSsid = mPage.find('#txtSsid');
    var mTxtPassword = mPage.find('#txtPassword');

    EnsureFirstPage();
    OnPageShow();

    function OnPageShow() {
        
        if ($.mobile.activePage == null) {
            setTimeout(OnPageShow, 100);
            return;
        }

        function RefreshWifiList() {
        
            var pEnabled;

            if (mLvWifi != undefined)
                mLvWifi.ClearResults();

            pEnabled = CBool(Device.GetWifiEnabled());
            try {
                $('#chkWifiActive').prop('checked', pEnabled).checkboxradio('refresh');
            } catch (e) {
                // just ignore any errors here
            }

            if (mLvWifi == undefined) {
                mLvWifi = new CreateListView();
                mLvWifi.Element.appendTo($('#divWifiList'));
                mLvWifi.OnItem = function (vDataRow, vItem, e) {
                    vItem.append($('<a>')
                        .data('SSID', vDataRow)
                        .attr('href' ,'#')
                        .click(function(e) {
                            e.preventDefault();
                            ShowItem({
                                Ssid: $(this).data('SSID')
                            });
                        })
                        .append($('<h3>')
                            .css('white-space', 'normal')
                            .text(vDataRow)
                        )
                    );
                };
            }
            mLvWifi.SetData(Device.GetActiveWifi());
        }

        $('#butRefreshWifiList').click(RefreshWifiList);

        $('#chkWifiActive').click(function() {
             Device.SetWifiEnabled($(this).prop('checked'));
        });

        RefreshWifiList();

    } // END OnPageShow

    function ShowItem(vData) {

        var pTxt;
        var pJson;

        mTxtSsid.val(vData.Ssid);
        mTxtPassword.val('');

        pTxt = Transfers.ReadFile('Wifi.txt');
        if (CStr2(pTxt).length > 0) {
            try {
                pJson = JSON.parse(pTxt);
                pJson = pJson[mTxtSsid.val()];
                if (pJson != undefined)
                    mTxtPassword.val(pJson.Password);
            } catch (e) {
            }
        }
        $.mobile.changePage(mPage);
        
    }

    mPage.find('#butSave').click(function() {
        var pTxt;
        var pJson;

        pTxt = Transfers.ReadFile('Wifi.txt');
        if (CStr2(pTxt).length > 0) {
            try {
                pJson = JSON.parse(pTxt);
            } catch (e) {
                pJson = {};
            }
        } else
            pJson = {};
        pJson[mTxtSsid.val()] = {
            Ssid: mTxtSsid.val(),
            Password: mTxtPassword.val()
        };
        pTxt = JSON.stringify(pJson);
        Transfers.WriteFile('Wifi.txt', null, pTxt);

        Device.ConnectWifi(mTxtSsid.val(), mTxtPassword.val());
    });

});
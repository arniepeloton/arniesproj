define([
    ], function () {

        var mDeferred = $.Deferred();
        var mHtml;

        //(function staticInit() {

        //    AjaxSend({
        //        ExactUrl: '/Scripts3/view/PriceChange.html',
        //        type: 'GET',
        //        dataType: 'text',
        //        success: function (vData) {
        //            mHtml = vData;
        //            mDeferred.resolve();
        //        }
        //    });

        //})();

        $(function () {
          
            CreateStringObject('App.controller.UpdateServerIpSettings', null, function () {

                var mOut = {};      
                var mElement;
                var mProducts;  

                mElement = $('#pageUpdateServerIpSettings');
                mOut.element = mElement;

                

                var mTxtUpdateServerIp = mElement.find('[name=txtUpdateServerIp]');
                var mMobizDTRPort = mElement.find('[name=txtMobizDTRPortNo]');
                function Save() {
                    var pData = {
                        UpdateServerIp: mTxtUpdateServerIp.val(),
                        MobizDTRPortNo: mMobizDTRPort.val()
                    };
                    AjaxSend({
                        data: pData,
                        url: '/UpdateServer/SaveUpdateServerIp',
                        success: function(vData) {
                            if (vData.Result == 'ok') 
                                NewMessageBox('Update server IP was successfully updated.');
                            else if (vData.Error)
                                NewMessageBox(vData.Error);
                        }
                    });
                }

                function GetData() {
                    AjaxSend({
                        url: '/UpdateServer/GetUpdateServerIp',
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else
                                DisplayData(vData);
                        }
                    });

                    AjaxSend({
                        url: '/UpdateServer/GetMobizDTRPortNo',
                        success: function (vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else
                                mMobizDTRPort.val(vData.MobizDTRPort);
                        }
                    });
                }

                function DisplayData(vData) {
                    mTxtUpdateServerIp.val(vData.UpdateServerIp);
                }

                mOut.Init = function() {
                    mElement.find('[name=butSave]').click(function() {
                        NewMessageBox('Are you sure you want to save the update server IP?', 'Update IP?', [
                            {
                                Text: 'Yes', Click: function () {
                                    Save();
                                }
                            }, { Text: 'No' }
                        ]);
                    });
                    GetData();
                };
                
                //mOut.Init();

                return mOut;

            }); // END CreateStringObject('App.controller.PriceChange', null, function() {

        }); // END $(function () {

        return mDeferred.promise();

    });

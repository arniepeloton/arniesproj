define(['common'
    ], function() {

    $(function () {
     
        window.Test = {
            AppendPage: function (vUrl) {

                AjaxSend({
                    ExactUrl: '/Test/' + vUrl + '?BodyOnly=1',
                    dataType: 'text',
                    success: function (vData) {
                        var p = $(vData);
                        $.mobile.activePage.children('[data-role=content]').append(p);
                        p.filter('[data-role=collapsible]').collapsible({
                            collapsed: false,
                            theme: 'b',
                            contentTheme: 'd'
                        });
                        p.trigger('create');
                        var pDummy;
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                }); // AjaxSend

            }, // function AppendPage

            CssSuccessResult: {
                'background-color': '#00AA00',
                'color': 'white',
                'padding': '5px',
                'text-shadow': 'none'
            },

            CssFailedResult: {
                'background-color': '#FF0000',
                'color': 'white',
                'padding': '5px',
                'text-shadow': 'none'
            },

            AppendResult: function(vElement, vData) {

                var pElement = $(vElement);
                var pItem;

                for (var pIndex in vData.Result) {
                    pItem = vData.Result[pIndex];
                    if (pItem.Title != undefined) {
                        pElement.append(
                            $('<h4>').text(pItem.Title)
                        );
                    };    

                    if (pItem.Description != undefined) {
                        pElement.append(
                            $('<div>').text(pItem.Description)
                        );
                    };

                    if (pItem.SuccessResult != undefined) {
                        pElement.append(
                            $('<div>')
                                .css(this.CssSuccessResult)
                                .text(pItem.SuccessResult)
                        );
                    };

                    if (pItem.ErrorResult != undefined) {
                        pElement.append(
                            $('<div>')
                                .css(this.CssFailedResult)
                                .text(pItem.ErrorResult)
                        );
                    };

                    if (pItem.ListResult != undefined) {
                        pElement.append(
                            GetListResult(pItem.ListResult)
                        );
                    };

                }; // for (var pIndex in vData.Result) {

            } // AppendResult
        }; // window.Test

        function GetListResult(vData) {

            var pOut = $('<div>');
            var pStr;

            if (!$.isArray(vData)) {
                pOut.css(Test.CssFailedResult);
                pStr = 'Parameter passed to GetListResult is not an array';
                pOut.text(pStr);
                console.log(pStr);
                return pOut;
            };

            pStr = '<ol>';
            for (var p = 0; p <= vData.length - 1; p++) {
                if (pStr != '') pStr += '</br>';
                pStr += '<li>' + HtmlEncode(vData[p]) + '</li>';
            };
            pStr += '</ol>';
            pOut.html(pStr);
            pOut.css(Test.CssSuccessResult);

            return pOut;

        }; // DecodeListResult

    }); // $(function () {
}); 
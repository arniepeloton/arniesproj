(function ($) {

    var mDataKey = 'Poper';

    $.fn.Poper = function (vOptions) {

        var pOptions = $.extend($.Poper.Defaults, vOptions);

        $(this).each(function (vIndex, vItem) {

            var $this = $(vItem);
            var pData;

            pData = $this.data(mDataKey);
            if (!pData) {
                pData = {};
                pData.Label = pOptions.Label;
                pData.HeaderText = pOptions.HeaderText;
                pData.Button = pOptions.Button;
                if (!pData.Button) {
                    pData.Button = $('<button>')
                        .text(pOptions.ButtonText)
                        .appendTo($this);
                };
                pData.Button.click(function () {
                    pData.Button.hide();
                    pData.Dialog.dialog('open');
                });
                pData.PlaceHolder = pOptions.PlaceHolder;
                if (!pData.PlaceHolder) pData.PlaceHolder = $('<span>');

                pData.Dialog = $('<div>');
                pData.Dialog.dialog({
                        autoOpen: pOptions.AutoPop,
                        minWidth: pOptions.MinWidth, 
                        minHeight: pOptions.MinHeight,
                        open: function () {
                            if (pData.Label) pData.Label.hide();
                            $this.after(pData.PlaceHolder);
                            pData.Dialog.append($this);

                        },
                        beforeClose: function () {
                            pData.Button.show();
                            if (pData.Label) pData.Label.show();
                            pData.PlaceHolder.after($this);
                            pData.PlaceHolder.remove();
                        }
                    });
                $this.data(mDataKey, pData);
            };
        });

        return $(this);

    }; // END $.fn.Poper

    $.Poper = {
        Defaults: {
            Label: null, // $(div) optional, hide this in popped mode. Show this in inline mode
            HeaderText: null, // string optional, title of diablog in popped mode
            MinWidth: 500,
            MinHeight: 330,
            Button: null,
            ButtonText: 'Popup this portion',
            PlaceHolder: null,
            AutoPop: false  // if true, popup by default
        }
    };

})(jQuery);
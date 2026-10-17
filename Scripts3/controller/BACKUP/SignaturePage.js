define(['common'], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var isEmpty;


    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/SignaturePage.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.SignaturePage', null, function () {

            var mOut = {};
            var mElement;
            var mCanvas;
            var mContext;
            var mStarted;

            mElement = $(mHtml);
            mOut.element = mElement;
            $('body').append(mElement);

            mCanvas = mElement.find('canvas')[0];
            mContext = mCanvas.getContext('2d');
            clearCanvas();
            initialize();

            ///KITKAT UPDATE
            //NEW CODE BASED ON http://www.codeproject.com/Articles/355230/HTML-Canvas-A-Simple-Paint-Program-Touch-and-Mou
            //////////////
            // works out the X, Y position of the click inside the canvas from the X, Y position on the page
            function getPosition(mouseEvent, mCanvas) {
                var x, y;
                //                if (mouseEvent.pageX != undefined && mouseEvent.pageY != undefined) {
                //                    x = mouseEvent.pageX;
                //                    y = mouseEvent.pageY;
                //                } else {
                x = mouseEvent.clientX + document.body.scrollLeft + document.documentElement.scrollLeft;
                y = mouseEvent.clientY + document.body.scrollTop + document.documentElement.scrollTop;
                //                }

                return { X: x - mCanvas.offsetLeft, Y: y - mCanvas.offsetTop };
            }

            function initialize() {
                // get references to the canvas element as well as the 2D drawing mContext

                mContext.strokeStyle = 'Black';

                // This will be defined on a TOUCH device such as iPad or Android, etc.
                var is_touch_device = 'ontouchstart' in document.documentElement;

                if (is_touch_device) {
                    // create a drawer which tracks touch movements
                    var drawer = {
                        isDrawing: false,
                        touchstart: function (coors) {
                            mContext.beginPath();
                            mContext.moveTo(coors.x, coors.y);
                            this.isDrawing = true;
                            isEmpty = false;
                        },
                        touchmove: function (coors) {
                            if (this.isDrawing) {
                                mContext.lineTo(coors.x, coors.y);
                                mContext.stroke();
                            }
                        },
                        touchend: function (coors) {
                            if (this.isDrawing) {
                                this.touchmove(coors);
                                this.isDrawing = false;
                            }
                        }
                    };

                    // create a function to pass touch events and coordinates to drawer
                    function draw(event) {

                        // get the touch coordinates.  Using the first touch in case of multi-touch
                        var coors = {
                            x: event.targetTouches[0].pageX,
                            y: event.targetTouches[0].pageY
                        };

                        // Now we need to get the offset of the canvas location
                        var obj = mCanvas;

                        if (obj.offsetParent) {
                            // Every time we find a new object, we add its offsetLeft and offsetTop to curleft and curtop.
                            do {
                                coors.x -= obj.offsetLeft;
                                coors.y -= obj.offsetTop;
                            }
                            // The while loop can be "while (obj = obj.offsetParent)" only, which does return null
                            // when null is passed back, but that creates a warning in some editors (i.e. VS2010).
                            while ((obj = obj.offsetParent) != null);
                        }

                        // pass the coordinates to the appropriate handler
                        drawer[event.type](coors);
                    }


                    // attach the touchstart, touchmove, touchend event listeners.
                    mCanvas.addEventListener('touchstart', draw, false);
                    mCanvas.addEventListener('touchmove', draw, false);
                    //                    mCanvas.addEventListener('touchend', draw, false);

                    // prevent elastic scrolling
                    mCanvas.addEventListener('touchmove', function (event) {
                        event.preventDefault();
                    }, false);
                }
                else {

                    // start drawing when the mousedown event fires, and attach handlers to
                    // draw a line to wherever the mouse moves to
                    mCanvas.mousedown(function (mouseEvent) {
                        var position = getPosition(mouseEvent, mCanvas);

                        mContext.moveTo(position.X, position.Y);
                        mContext.beginPath();

                        // attach event handlers
                        $(this).mousemove(function (mouseEvent) {
                            drawLine(mouseEvent, mCanvas, mContext);
                        }).mouseup(function (mouseEvent) {
                            finishDrawing(mouseEvent, mCanvas, mContext);
                        }).mouseout(function (mouseEvent) {
                            finishDrawing(mouseEvent, mCanvas, mContext);
                        });
                    });

                }
            }

            // draws a line to the x and y coordinates of the mouse event inside
            // the specified element using the specified mContext
            function drawLine(mouseEvent, mCanvas, mContext) {

                var position = getPosition(mouseEvent, mCanvas);

                mContext.lineTo(position.X, position.Y);
                mContext.stroke();
            }

            // draws a line from the last coordiantes in the path to the finishing
            // coordinates and unbind any event handlers which need to be preceded
            // by the mouse down event
            function finishDrawing(mouseEvent, mCanvas, mContext) {
                // draw the line to the finishing coordinates
                drawLine(mouseEvent, mCanvas, mContext);

                mContext.closePath();

                // unbind any events which could draw
                $(mCanvas).unbind("mousemove")
                     .unbind("mouseup")
                     .unbind("mouseout");
            }


            mElement.find('button[name=butSave]').click(function () {
                console.log('butSave.click jsJJ4TMU6Z62!');

                if (isEmpty) {

                    Android.Toast('Please provide a signature.');
                    return;
                }
                if ($.trim(mOut.txtSalesOrderSignatory.val()) == '') {


                    Android.Toast('Please enter the name of the signatory.');
                    return;

                }

                var image = mOut.toDataURL("image/png");
                document.getElementById('getimage').src = image;


                //                var imageDataUrl = mOut.toDataURL('image/png').substring(22);
                //                var byteArray = Base64Binary.decode(imageDataUrl);
                //                var blob = new Blob([byteArray], { type: 'image/png' });
                //                var url = window.URL.createObjectURL(blob);

                //                alert('ok');
                //                mElement.find('button[name=getimage]').setAttribute('href', url);
                //                mElement.find('button[name=getimage]').setAttribute('download', sessionStorage.CartOrderID + '.png');

                //                mElement.find('button[name=getimage]').click();

                return mOut.onSubmit({ Signatory: $.trim(mOut.txtSalesOrderSignatory.val()), ImageURL: image });
            });


            mOut.show = function () {
                $.mobile.changePage(mElement);
            };
            ////////////////////////
            mOut.toDataURL = function () {

                return mCanvas.toDataURL.apply(mCanvas, arguments);
            };

            mElement.find('button[name=getimage]').click(function () {
                var imageDataUrl = mOut.toDataURL('image/png').substring(22);
                var byteArray = Base64Binary.decode(imageDataUrl);
                var blob = new Blob([byteArray], { type: 'image/png' });
                var url = window.URL.createObjectURL(blob);


                mElement.find('button[name=getimage]').setAttribute('href', url);
                mElement.find('button[name=getimage]').setAttribute('download', sessionStorage.CartOrderID + '.png');

                //http://stackoverflow.com/questions/20444499/missing-extension-when-saving-image-from-canvas
            });
            mElement.find('button[name=butClearCanvas]').click(function () {
                console.log('butClearCanvas.click jsJJ4TMU6Z64!');
                clearCanvas();

            });

            function clearCanvas() {

                mContext.clearRect(0, 0, mCanvas.width, mCanvas.height);

                // Restore the transform
                mContext.restore();

                isEmpty = true;

            }

            mOut.txtSalesOrderSignatory = mElement.find('[name=txtSalesOrderSignatory]');



            return mOut;

        }); // END CreateStringObject('App.controller.SignaturePage', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});
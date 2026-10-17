/***
for usage, see D:\Mobiz\WebErp\siesta\tests\basics\basics.js
***/
define(['common'],
    function() {

    CreateStringObject('App.Deferred', null, 
        (function() {

            var pDepth = 0;
            var pDeferred;
            var pOut = {};
            var mID = $.format.date(new Date, 'y_MM_dd__HH:mm:ss:SSS'); // for debugging only. make sure we are debugging the same instance
            var mMaxDepth = 0;
            var mTotalIncrement = 0;
            var pExpecteds = [];
            var pAdvancedExpecteds = [];

            pOut.increment = function(vDepth) {
                if (vDepth == undefined)
                    vDepth = 1;

                if (pDeferred == undefined) {
                    pDeferred = $.Deferred();
                    pDepth = vDepth;
                } else
                    pDepth += vDepth    ;
                mMaxDepth = Math.max(pDepth, mMaxDepth);
                mTotalIncrement += vDepth;
            };

            pOut.expect = function(vKey) {
                
                if (pDeferred == undefined) 
                    pDeferred = $.Deferred();

                if ($.isArray(vKey))
                    for (var pIndex =0; pIndex <= vKey.length - 1; pIndex++) 
                        expect(vKey[pIndex]);
                else
                    expect(vKey);


            }; // END pOut.expect = function(vKey) {

            function expect(vKey) {
                
                if (pExpecteds.indexOf(vKey) == -1) {
                    pExpecteds.push(vKey);
                    resolveAdvanceExpecteds();
                }

            }

            pOut.decrement = function() {
                if (pDeferred == undefined)
                    throw 'Can\'t call decrement. deferred is null';
                pDepth--;
                tryResolve();

            };

            pOut.fulfill = function(vKey) {
                
                var pIndex;

                pIndex = pExpecteds.indexOf(vKey);
                if (pIndex != -1) {
                    pExpecteds.splice(pIndex, 1);
                    resolveAdvanceExpecteds();
                } else {
                    if (pDeferred == undefined) 
                        pDeferred = $.Deferred();
                    pAdvancedExpecteds.push(vKey);
                }

            };

            function tryResolve() {

                if ((pDepth == 0) 
                    && (pExpecteds.length == 0)
                    && (pAdvancedExpecteds.length == 0)) {

                    pDeferred.resolve();
                    pDeferred = null;
                }

            }

            function resolveAdvanceExpecteds() {

                var pAdvancedKey;
                var pExpectedIndex;

                for(var pAdvancedIndex = 0; pAdvancedIndex <= pAdvancedExpecteds.length - 1; null) {
                    pAdvancedKey = pAdvancedExpecteds[pAdvancedIndex];
                    pExpectedIndex = pExpecteds.indexOf(pAdvancedKey);
                    if (pExpectedIndex != -1) {
                        pExpecteds.splice(pExpectedIndex, 1);
                        pAdvancedExpecteds.splice(pAdvancedIndex, 1);
                    } else 
                        pAdvancedIndex++;
                }
                tryResolve();

            }

            pOut.done = function(vFunction) {
                pOut.promise().done(function() {
                    vFunction.call(pOut);
                });
            };

            pOut.promise = function() {
                if (pDeferred == undefined) {
                    pDeferred = $.Deferred();
                    pDepth = 0;
                }
                return pDeferred.promise();
            };

            pOut.reset = function() {
                pDepth = 0;
                pDeferred = null;
                mTotalIncrement = 0;
                pExpecteds = [];
                pAdvancedExpecteds = [];
            };

            pOut.getID = function() {

                return mID;

            };

            pOut.getMaxDepth = function() {

                return mMaxDepth;

            };

            pOut.getTotalIncrement = function() {

                return mTotalIncrement;

            };

            return pOut;

        })()
    );
});
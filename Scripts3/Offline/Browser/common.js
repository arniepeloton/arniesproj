define([
    'promise!business',
    (OfflineMode ? '/Scripts3/Offline/CsvReader.js?crc=' : ''),
    (OfflineMode ? '/Scripts3/Offline/CsvWriter.js?crc=' :  ''),
    '/Scripts3/Offline/Finder.js?crc='
], function() {

    if (window.OfflineBrowser == undefined) {
        window.OfflineBrowser = {};
        window.OfflineBrowser.Base = function(vOptions) {
            var mMyClass = this;

            this.SearchString = '';

            this.CreateFinder = function(vData) {

                var pFinder = new Finder();

                pFinder.SearchString = vData.keywords;
                return pFinder;

            }; // CreateFinder

            this.SubmitFinder = function(vFinder) {

                
            }; // SubmitFinder

            return this;

        }; // window.OfflineBrowser.Base

    }; // if (window.OfflineBrowser == undefined) {
    
}); //  define
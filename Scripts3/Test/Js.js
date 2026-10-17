require(['/Scripts3/Test/common.js?crc='
    ], function() {
     
    // $.bind('pageshow', function() {
    $(function() {

        Test.AppendResult($('#divTestJs'), Exec());

        function Exec() {

            var pOut = {};

            pOut.Result = [
                ExecIsInteger()
            ];

            return pOut;

        }; // Test

        function ExecIsInteger() {

            var pOut = {
                Title: 'IsInteger',
                Description: 'Determines if an object is a integer. Results in true or false'
            };

            pOut.TableResult = {
                Columns: [
                    'Input'
                ],
                Rows: []
            };

            return pOut;

        };

    }); // $(function () {

}); // require(['common'
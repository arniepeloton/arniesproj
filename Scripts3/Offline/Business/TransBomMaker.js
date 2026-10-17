define([], function() {
    window.Business.TransBomMaker = new function(vParent) {
        this.VerifyCounter = function () {
            // requires sessionStorage.FowstID

            var pSql;
            var pCounter;

            if (Web.HandleOrganization() == false) return;

            pSql = 'SELECT MAX(BomMakerLastNum)' +
                ' FROM LibraryDeviceFowst' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID());
            pCounter = CDec2(Sql.ExecuteScalar(pSql));

            return pCounter;

        }; // END VerifyCounter

        this.UpdateCounter = function (vValue) {
            // PARAMS:
            //  vValue (optional). if null, updates value based on max invoice number or POSLastORNo, whichever is higher

            var pIU;
            var pSql;
            var pCounter;
            var pRecords;
            var pInvoiceNumber;

            if ((vValue != undefined)
                && (!IsNumeric(vValue))) {

                throw 'UpdateCounter parameter must be a valid number. You passed ' + vValue + '. jsSLNEL4XYH5';
            }

            pCounter = CDec2(this.VerifyCounter());
            pInvoiceNumber = CDec2(this.LastNumber());
            vValue = Math.max(CDec2(vValue), pCounter, pInvoiceNumber);
            if (vValue <= pCounter)
                return false;

            pIU = new InsertUpdate('LibraryDeviceFowst');
            pIU.Add('BomMakerLastNum', CDec2(vValue));
            pSql = pIU.GetUpdate('FowstID = ' + ReadyString(sessionStorage.FowstID) +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()));
            pRecords = Sql.ExecuteSql(pSql);
            if (pRecords == 0) {
                pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                pSql = pIU.GetInsert();
                pRecords = Sql.ExecuteSql(pSql);
            }

            return true;

        }; // END UpdateCounter

        this.LastNumber = function () {

            // requires sessionStorage.FowstID

            var pSql;
            var pCounter;

            if (Web.HandleOrganization() == false) return;

            pSql = 'SELECT MAX(Num)' +
                ' FROM TransBomMaker' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID());
            pCounter = CDec2(Sql.ExecuteScalar(pSql));

            return pCounter;

        }; // END LastInvoiceNumber

        return this;
    };
});
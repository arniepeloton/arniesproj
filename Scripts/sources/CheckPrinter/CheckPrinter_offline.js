define([
    'common',
    '/Scripts3/Offline/Browser/common.js?crc=',
    '/Scripts3/Offline/finder.js?crc='
    ], function () {

        window.CheckPrinter_offline = function () {



            this.getPrinterFowstIP = function (orgID) {

                try {
                    //open private db
                    Sql.AttachDatabase(orgID);

                }
                catch (e) {

                    return false;
                }

                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.Distinct = true;


                pField = pFields.Add('PrinterName');
                pField.Expression = 'devprntrfowst.PrinterName';

                pField = pFields.Add('PrinterIPAddress');
                pField.Expression = 'devprntrfowst.PrinterIPAddress';

                pField = pFields.Add('PrinterType');
                pField.Expression = 'devprntr.PrinterType';

                pField = pFields.Add('PrinterRole');
                pField.Expression = 'devprntr.PrinterRole';

                pField = pFields.Add('FowstName');
                pField.Expression = 'fowst.Name';

                pField = pFields.Add('CompanyName');
                pField.Expression = 'org.CompanyName';


                pFinder.SqlFrom = 'FROM private.LibraryOrganizationEmployee orgemp' +
                ' INNER JOIN private.LibraryOrganizationEmployeeFOWST empfowst ON (orgemp.PeopleID = empfowst.PeopleID)' +
                ' INNER JOIN LibraryOrganizationFOWST fowst ON (empfowst.FowstID = fowst.ID)' +
                ' INNER JOIN LibraryOrganization org ON (fowst.OrganizationID = org.ID)' +
                ' INNER JOIN LibraryCities cty ON (fowst.FowstCityID = cty.ID)' +
                ' INNER JOIN LibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)' +
                ' INNER JOIN private.LibraryDeviceFowst devfowst ON (fowst.ID = devfowst.FowstID)' +
                ' INNER JOIN private.LibraryDeviceFowstPrinter devfowstprntr ON (devfowst.DeviceID = devfowstprntr.DeviceID)' +
                ' INNER JOIN private.LibraryDevicePrinterFowst devprntrfowst ON (devfowstprntr.PrinterFowstID = devprntrfowst.ID)' +
                ' INNER JOIN private.LibraryDevicePrinter devprntr ON (devprntrfowst.PrinterID = devprntr.ID)' +
                ' INNER JOIN private.LibraryDevice dev ON (dev.ID = devfowst.DeviceID)';

                pFinder.SqlOrder = ' WHERE orgemp.PeopleID = ' + ReadyString(Android.GetSessionData().PeopleUserID) +
                ' AND orgemp.Deactivated = 0 AND dev.UUID = ' + ReadyString(Web.DeviceSerial());


                return pFinder.Requery();

            }; // this.CheckUser = function() {


            return this;

        }; // window.PeopleMenu_offline = function() {

    });       // define


function Validator () {
    // this is a class

    var mItems = []; // type = ValidatorItem
    var MyClass = this;

    this.SummaryDiv = null;
    this.Validate = function () {

        var pInvalid = false;
        var pList; // UL

        if (MyClass.SummaryDiv) {
            this.SummaryDiv.text('');
            pList = $('<ul>').addClass('validation-summary-errors');
        };
        $.each(mItems, function (vIndex, vItem) {

            var pResult;

            pResult = vItem.Validate();
            if (!pResult.Valid) {
                pInvalid = true;
                if (pList)
                    pList.append(pResult.ErrorListItem);
            };
        });

        if (pList && pList.children().length > 0) {
            MyClass.SummaryDiv.append(pList)
                .MakeError()
                .ScrollTo();
        };

        if (this.SummaryDiv) this.SummaryDiv.hide();
        return !pInvalid;

    };   // Validate

    // remove "invalid" UI's
    this.Clear = function () {
        
        $.each(mItems, function (vIndex, vItem) {
            vItem.Clear();
        });

    };

    this.AddItem = function (vParams) {

        var pItem = new ValidatorItem();

        pItem.Parent = MyClass;
        pItem.Label = vParams.Label;
        pItem.Control = vParams.Control;
        pItem.ErrorControl = vParams.ErrorControl;
        mItems[mItems.length] = pItem;

        return pItem;

    };   // AddItem
};

// static methods
Validator.ValidateDate = function (vOptions) {
    // PARAMS:

    // Value1 (string) (required) : the string to validate

    // Value2 (string) (optional) : the string to validate.
    //      if set, vValue2 must be greater than or equal to vValue1 (depends on vMinDaysDiff)

    // Field1 (string) (optional) : friendly label for Value1
    //      if undefined, this will be replaced by 'This'

    // Field2 (string) (optional) : friendly label for Value2
    //      if undefined, this will be replaced by 'This'

    // Min (date) (optional) : minimum date value 

    // Max (date) (optional) : maximum date value

    // IncludeTime (boolean) (optional) : if true, include time during verification.
    //      by default, only date (excluding time) is including in the verification

    // MinDaysDiff (integer) (optional) : if set, vValue2 must be at least vMinDaysDiff days higher than vValue1

    // RETURNS a friendly validation message if invalid.
    //  if valid, returns null

    var pMessage;

    if ((vOptions.Min != undefined) && (IsDate(vOptions.Min) == false)) 
        throw 'vOptions.Min must be a valid date';
    if ((vOptions.Max != undefined) && (IsDate(vOptions.Max) == false)) 
        throw 'vOptions.Max must be a valid date';
    if (vOptions.MinDaysDiff == undefined)
        vOptions.MinDaysDiff = 0;
    if (IsNumeric(vOptions.MinDaysDiff) == false)
        throw 'vOptions.MinDaysDiff must be a valid number';
    if (!vOptions.IncludeTime) {
        if (vOptions.Min != undefined) 
            vOptions.Min = new Date(vOptions.Min.getFullYear(), vOptions.Min.getMonth(), vOptions.Min.getDate());
        if (vOptions.Max != undefined) 
            vOptions.Max = new Date(vOptions.Max.getFullYear(), vOptions.Max.getMonth(), vOptions.Max.getDate());
    };
        
    vOptions.Field1 = vOptions.Field1 || 'This';

    pMessage = Validate(vOptions.Value1)
    if (pMessage != null) return vOptions.Field1 + pMessage;
    if (!vOptions.IncludeTime) 
        vOptions.Value1 = new Date(vOptions.Value1).RemoveTime();

    if (vOptions.Value2 != undefined) {
        vOptions.Field2 = vOptions.Field2 || 'This';
        pMessage = Validate(vOptions.Value2)
        if (pMessage != null) return vOptions.Field2 + pMessage;
        if (!vOptions.IncludeTime) 
            vOptions.Value2 = new Date(vOptions.Value2).RemoveTime();

        if (DateDiff(DateDiff.Day, vOptions.Value1, vOptions.Value2) < vOptions.MinDaysDiff) 
            return vOptions.Field2 + ' must be later than ' + vOptions.Field1;
    };

    function Validate(vValue) {
        // RETURNS a friendly validation message if invalid.
        //  if valid, returns null

        var pMessage;

        // compose the message
        pMessage = ' must be a valid date';
        if ((vOptions.Min != undefined) && (vOptions.Max != undefined)) 
            pMessage += ' from ' + vOptions.Min + ' to ' + vOptions.Max;
        else if ((vOptions.Min == undefined) && (vOptions.Max != undefined))
            pMessage += ' not later than ' + vOptions.Max;
        else if ((vOptions.Min != undefined) && (vOptions.Max == undefined))
            pMessage += ' not earlier than ' + vOptions.Min; 

        if (IsDate(vValue)) {
            vValue = new Date(vValue);
            if (!vOptions.IncludeTime) 
                vValue = vValue.RemoveTime();
            if ((vOptions.Min != undefined) && (vValue < vOptions.Min)) return pMessage;
            if ((vOptions.Max != undefined) && (vOptions.Max < vValue)) return pMessage;
        } else {
            return pMessage;
        };

    }; // END Validate
}; // END Validator.ValidateDate

// END static methods

function ValidatorItem () {
    // this is a class

    var MyClass = this;
    var mValidation = [];

    this.Label = undefined;
    this.Control = undefined;
    this.ErrorControl = undefined; // the DIV that will contain the Control's error (if any)
    this.Parent = undefined; // typeof Validator
                             // only AddItem should set this

    // returns:
    // .ErrorListItem // typeof <li>
    this.Validate = function () {

        var pOut = new Object();
        var pInvalid;
        var pText;
        var pIndex;

        for (pIndex in mValidation) {
            if (MyClass.Control.is(':visible')) {
                pText = mValidation[pIndex]();
                if (pText) break;
            };
        };

        // actual validation here
        pInvalid = MyClass.Control.val().length == 0;

        if (pText) {
            if (MyClass.Parent.SummaryDiv) {
                pOut.ErrorListItem = $('<li>');
                pOut.ErrorListItem.append($('<a>')
                    .attr('href', '#')
                    .text(pText)
                    .addClass('error-validation-error')
                    .attr('title', 'Click to scroll to the offending field')
                    .click(function (e) {
                        e.preventDefault();
                        MyClass.Control.ScrollTo();
                    })
                );
            };
            MyClass.Control.addClass('input-validation-error');
            if (MyClass.ErrorControl) {
                MyClass.ErrorControl.text(pText)
                    .addClass('error-validation-error')
                    .show();
            };
        } else {
            MyClass.Clear();
        };

        pOut.Valid = !pText;
        return pOut;

    };  // Validate

    this.Clear = function () {
        
        MyClass.Control.removeClass('input-validation-error');
        if (MyClass.ErrorControl)
            MyClass.ErrorControl.hide();

    }; // Clear

    this.AddRequiredValidation = function () {

        mValidation[mValidation.length] = function () {
            if (MyClass.Control.val().trim().length == 0) 
                return (MyClass.Label || '') + ' may not be blank';
        };
        return MyClass;
    };

    this.AddEmailValidation = function () {

        mValidation[mValidation.length] = function () {
            if (MyClass.Control.val().trim().length == 0) return null;
            if (IsEmail(MyClass.Control.val()) == false) 
                return (MyClass.Label || '') + ' must be a valid email address';
        };
        return MyClass;
    };

    this.AddNumericValidation = function (vMin, vMax) {
        
        mValidation[mValidation.length] = function () {
            var pMessage;
            var pValue; // float

            if (MyClass.Control.val().trim().length == 0) return null;
            pMessage = (MyClass.Label || 'This') + ' must be a valid number';
            if ((vMin != undefined) && (vMax != undefined)) 
                pMessage += ' from ' + vMin + ' to ' + vMax;
            else if ((vMin == undefined) && (vMax != undefined))
                pMessage += ' not greater than ' + vMax;
            else if ((vMin != undefined) && (vMax == undefined))
                pMessage += ' not lesser than ' + vMin; 

            if (IsNumeric(MyClass.Control.val())) {
                pValue = parseFloat(MyClass.Control.val());
                if ((vMin != undefined) && (pValue < vMin)) return pMessage;
                if ((vMax != undefined) && (vMax < pValue)) return pMessage;
            } else
                return pMessage;

        };
        return MyClass;

    }; // this.AddNumericValidation

    this.AddLengthValidation = function (vMin, vMax) {
        // vMin: integer (optional. If undefined, vMax is required)
        // vMax: integer (optional. If undefined, vMin is required)

        mValidation[mValidation.length] = function () {
            var pMessage;
            var pLength;

            pLength = MyClass.Control.val().trim().length;
            pMessage = (MyClass.Label || 'This') + ' must be ';
            if ((vMin != undefined) && (vMax != undefined)) 
                pMessage += vMin + ' to ' + vMax;
            else if ((vMin == undefined) && (vMax != undefined))
                pMessage += ' not greater than ' + vMax;
            else if ((vMin != undefined) && (vMax == undefined))
                pMessage += ' not lesser than ' + vMin; 
            pMessage += ' character[s] in length';

            if ((vMin != undefined) && (pLength < vMin)) return pMessage;
            if ((vMax != undefined) && (vMax < pLength)) return pMessage;
        };
        return MyClass;

    }; // this.AddLengthValidation

    this.AddDateValidation = function () {

        mValidation[mValidation.length] = function () {
            var pValue;

            pValue = MyClass.Control.val();
            if (pValue.trim().length == 0) return;
            if (!IsDate(pValue)) 
                return (MyClass.Label || 'This') + ' must be a valid date';

        }; // END mValidation[mValidation.length] = function ()
        return MyClass;

    };       // END this.AddDateValidation

    this.AddCustomValidation = function (vCallback) {
        
        mValidation[mValidation.length] = vCallback;
        return MyClass;

    };
};
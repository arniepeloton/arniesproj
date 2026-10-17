$(function() {
    
    new Vendor();

});

// class
function Vendor() {

    var mConfirmDialog;
    var mValidator;

    // controls
    var mForm = $('form#formVendor');
    var txtName = mForm.find('[name=txtName]');
    var txtAddress = mForm.find('[name=txtAddress]');
    var txtEmail =  mForm.find('[name=txtEmail]');
    var divResult = $('#divResult');
    var txtTermDays = $('[name=txtTermDays]');

    // constructor
    $('#butSave').click(butSave_Click);

    InitValidation();

    function InitValidation () {

        mValidator = new Validator;
        mValidator.SummaryDiv = divResult;

        mValidator.AddItem({
            Label: 'Vendor Name',
            Control: txtName,
            ErrorControl: null
        }).AddRequiredValidation();

        mValidator.AddItem({
            Label: 'Email',
            Control: txtEmail,
            ErrorControl: null
        }).AddEmailValidation();

        mValidator.AddItem({
            Label: 'Terms in days',
            Control: txtTermDays,
            ErrorControl: null
        }).AddNumericValidation(0);

        mValidator.AddItem({
            Label: 'Address',
            Control: txtAddress,
            ErrorControl: null
        }).AddLengthValidation(01);

    };

    function butSave_Click(e) {

        e.preventDefault();
        if (mValidator.Validate() == false) return;

        // create html for dialog
        $('<div>')
            .append($('<span>')
                .addClass('ui-icon ui-icon-disk')
                .css('float', 'left')
                .css('margin-right', '7px'))
            .append($('<span>').text('Are you sure you want to save this vendor information sheet?'))
            .dialog({
                title: 'Save?',
                modal: true,
                buttons: {
                    'Yes': function() {
                        $(this).dialog('close');
                    },
                    'No': function() {
                        $(this).dialog('close');
                    }
                }
            });
    }; // butSave_Click
}; // Vender


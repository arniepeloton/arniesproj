$(function () {
    
    DbConnect($('#divDbConnect'));

}); // document load

// class
function DbConnect (vJElement) {

    var mParent = vJElement;
    var mValidator;

    // controls
    var txtServer = mParent.find('[name=txtServer]')
        .attr('maxlength', '100')
        .width('300px');
    var errorServer = mParent.find('[name=errorServer]');
    var txtDatabase = mParent.find('[name=txtDatabase]')
        .attr('maxlength', '100')
        .width('300px');
    var errorDatabase = mParent.find('[name=errorDatabase]');
    var txtUsername = mParent.find('[name=txtUsername]')
        .attr('maxlength', '100')
        .width('300px');
    var errorUsername = mParent.find('[name=errorUsername]');
    var txtPassword = mParent.find('[name=txtPassword]')
        .attr('maxlength', '100')
        .width('300px');
    var errorPassword = mParent.find('[name=errorPassword]');
    var chkIntegratedSecurity = mParent.find('[name=chkIntegratedSecurity]')
        .change(chkIntegratedSecurity_Check);
    var butSave = mParent.find('[name=butSave]')
        .click(butSave_Click);
    var divResult = mParent.find('[name=divResult]');
    var mForm = mParent.find('form');

    InitValidation();

    function InitValidation () {

        mValidator = new Validator;
        mValidator.SummaryDiv = divResult;

        mValidator.AddItem({
            Label: 'Server',
            Control: txtServer,
            ErrorControl: errorServer
        }).AddRequiredValidation();

        mValidator.AddItem({
            Label: 'Database',
            Control: txtDatabase,
            ErrorControl: errorDatabase
        }).AddRequiredValidation();

        mValidator.AddItem({
            Label: 'Username',
            Control: txtUsername,
            ErrorControl: errorUsername
        }).AddRequiredValidation();

        mValidator.AddItem({
            Label: 'Password',
            Control: txtPassword,
            ErrorControl: errorPassword
        }).AddRequiredValidation();

    };

    function chkIntegratedSecurity_Check () {
        
        if (chkIntegratedSecurity.attr('checked'))
            mParent.find('[name=trSql]').hide();
        else  
            mParent.find('[name=trSql]').show();

    };

    function butSave_Click (e) {
        
        e.preventDefault();
        if (mValidator.Validate() == false) return;
        mParent.block();

        AjaxSend({
            url: '/DbConnect/Save',
            data: mForm.serializeObject(),
            success: function (vData) {
                var pMessage;

                divResult.ScrollTo();

                if (vData.d.Text == 'OK') 
                    pMessage = 'Database connection was successfully saved';
                else
                    pMessage = vData.d.Error;
                mParent.unblock();
                divResult.show().ScrollTo().text(pMessage);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                mParent.unblock();
                divResult.hide()
                    .fadeIn(1000)
                    .ScrollTo()
                    .text('An error occured while saving data. Data was not saved. Error: ' + XMLHttpRequest.responseText);
            }
        }); // AjaxSend

    };
};

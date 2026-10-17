require(['common'], function () {
    $(document).ready(function () {
        if (CStr2(sessionStorage.sonum) == '') {
            location.href = '/Ecommerce?OrgID=' + sessionStorage.OrgID;
        }

        $('#lblSONum').text(sessionStorage.sonum);

        $('body')
        .on('click', '#btnOSHomepage', function () {
            location.href = '/Ecommerce?OrgID=' + sessionStorage.OrgID;
        })
        .on('click', '#btnOSOrderHistory', function () {
            location.href = '/Ecommerce/MyAccount?OrgID=' + sessionStorage.OrgID + '#orderhistory';
        })
    })
})
require(['common'], function () {
    $(document).ready(function () {
        $('.emailaddress').text(sessionStorage.EmailAddress);
    })
})
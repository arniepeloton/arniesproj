@Code
    ViewData("Title") = "Registration Success"

    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code

@Section head

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/Franchisee/Success.js?v=' + assetversion)]);
            })
        })
    </script>

    <style type="text/css">
        .ui-page-active {
            background-color: #f8f9fa !important;
        }
    </style>
End Section

<div class="container">
    <div class="row pt-3 pb-3">
        <div class="offset-md-3 col-md-6 bg-white rounded p-3 border text-center text-success">
            <span class="bi-check-circle-fill" style="font-size:100px;"></span>
            <div class="mt-3" style="font-size:20px;">
                Your registration has been successfully submitted to @ViewData("FranchisorName"). <br /><br />
                We will notify you thru your e-mail (<i class="emailaddress"></i>) once your registration is approved. <br /><br />
                Thank you!
            </div>
        </div>
    </div>
</div>
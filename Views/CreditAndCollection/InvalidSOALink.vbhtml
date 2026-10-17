@Code
    ViewData("Title") = "Invalid SOA Link"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
End Section

<div class="text-center">
    <div class="text-primary rounded p-3">
        <div class="bi-exclamation-circle-fill text-primary mb-4" style="font-size:100px;"></div>
        <h1>Link is broken or expired.</h1>
        <br />
        <h5 class="text-secondary">
            Please get in touch with @ViewData("Name") to get a fresh link of your billing statement. 
            <br />
            Thank you and keep safe.
        </h5>
    </div>    
</div>
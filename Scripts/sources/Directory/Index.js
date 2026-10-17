require(['common'], function () {
    $(document).ready(function () {
        var filter = 'all';

        window.onpopstate = function (event) {
            if (event.state) {
                CheckLink();
            } else {
                console.log('onload')
            }
        }

        CheckLink();

        $('#formSearchSupplier').submit(function (e) {
            e.preventDefault();
            e.stopPropagation();

            var divSupplierResults = $('#divSupplierResults');
            var accountype, companydetails, companyemail, companymobile, companytelephone;

            divSupplierResults.empty();

            AjaxSend({
                url: 'Directory/SearchSupplier',
                data: { keywords: $.trim($('#txtSearchSupplier').val()), filter: filter },
                success: function (e) {
                    var vRows = e.data;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            divSupplierResults.append($('<div class="suppliermaindiv">')
                                    .data('data', vRows[i])
                                    .append($('<div class="suppliername">')
                                        .text(vRows[i].SupplierName)
                                    )
                                    .append($('<div class="suppliercatbrand">')
                                        .append($('<i>').text('Products or Services Offer: '))
                                        .append(CStr2(vRows[i].Categories[0].Names || 'Nothing'))
                                    )
                                    .append($('<div class="suppliercatbrand">')
                                        .append($('<i>').text('Brands Offer: '))
                                        .append(CStr2(vRows[i].Brands[0].Names || 'Nothing'))
                                    )
                                    .click(function (e) {
                                        var data = $(this).data('data');

                                        history.pushState({ SupplierID: data.OrganizationID }, data.SupplierName, '/Directory?vid=' + data.OrganizationID);
                                        GetSupplierInfo(data);
                                    })
                                )
                        }
                    } else {
                        divSupplierResults.append($('<h3>').text('No Results Found'))
                    }
                }
            })
        })

        $('.btnFilterSearch').click(function () {
            filter = $(this).attr('value');

            if (filter == 'all') {
                $('#lblFilterText').text('Search By All Suppliers');
            } else if (filter == 'categories') {
                $('#lblFilterText').text('Search By Products Or Services');
            } else {
                $('#lblFilterText').text('Search By Brands');
            }

            if ($.trim($('#txtSearchSupplier').val()) != '') {
                $('#formSearchSupplier').submit();
            }
        })

        $('.btnSearchAgain').click(function () {
            history.pushState({}, 'Mobiz Online Directory', '/Directory');
            $('#divSearchPage').pageshow();
        })

        function CheckLink() {
            var param = GetUrlParameters();

            if (Object.keys(param).length > 0) {
                $('#divSupplierInfo').pageshow();
                GetSupplierInfo({ OrganizationID: param.vid });
            } else {
                $('#divSearchPage').pageshow();
            }
        }

        function GetSupplierInfo(data) {
            AjaxSend({
                url: 'Directory/GetSupplierInfo',
                data: { ID : data.OrganizationID, FileType: 'image' },
                success: function (data) {
                    $('#divSupplierInfo').pageshow();
                    var lstProductsOrServicesOffer = $('#lstProductsOrServicesOffer');
                    var lstBrandsOffer = $('#lstBrandsOffer');
                    var divCompanyPicturesList = $('#divCompanyPicturesList');

                    lstProductsOrServicesOffer.empty();
                    lstBrandsOffer.empty();
                    divCompanyPicturesList.empty();

                    companydetails = data.CompanyDetails[0];
                    companyemail = data.CompanyEmail;
                    companymobile = data.CompanyMobile;
                    companytelephone = data.CompanyTelephone;
                    busindid = CStr2(companydetails.IndustryClassID);                    

                    $('#txtMACompanyName').text(companydetails.CompanyName);
                    $('#txtMABusInd').text('Business Industry: ' + CStr2(companydetails.IndustryName));
                    $('#txtMACompanyAddress').text('Address: ' + $.trim(companydetails.Address));
                    $('#txtMACompanyBackground').text(companydetails.Background || 'No Company Background');

                    if (data.Categories.length > 0){
                        for (var i = 0; i < data.Categories.length; i++) {
                            lstProductsOrServicesOffer.append($('<li>').text(data.Categories[i].Name + ' (' + data.Categories[i].BusinessActivities + ')'));
                        }
                    } else {
                        lstProductsOrServicesOffer.append('Nothing to offer');
                    }

                    if (data.Brands.length > 0) {
                        for (var i = 0; i < data.Brands.length; i++) {
                            lstBrandsOffer.append($('<li>').text(data.Brands[i].Name + ' (' + data.Brands[i].BusinessActivities + ')'));
                        }
                    } else {
                        lstBrandsOffer.append('Nothing to offer');
                    }

                    DisplayEmailList(companyemail);
                    DisplayMobileList(companymobile);
                    DisplayTelephoneList(companytelephone);

                    if (data.CompanyPictures.length > 0) {
                        for (var i = 0; i < data.CompanyPictures.length; i++) {
                            var src = '/FileTemplates/' + data.CompanyPictures[i].FileLocation.replace(new RegExp(/\\/g), '/').split('/FileTemplates')[1];

                            divCompanyPicturesList.append($('<div class="col-sm-4" id="divimg-' + data.CompanyPictures[i].ID + '">')
                                    .append($('<div>')
                                        .append($('<a data-role="none" data-lightbox="companypictures">')
                                            .attr('href', src)
                                            .append($('<img class="img-thumbnail" style="width:100%; height:200px; object-fit:cover;">')
                                                .attr('src', src)
                                            )
                                        )
                                    )
                                )
                        }
                    } else {
                        divCompanyPicturesList.append($('<div style="color:#cacaca; text-align:center; width:100%;">')
                                .append($('<span class="glyphicon glyphicon-picture" style="font-size: 20vh;">'))
                                .append('<br>')
                                .append($('<span style="font-size:25px;">').text('No Company Pictures Found'))
                            );
                    }
                }
            })
        }

        function DisplayTelephoneList(data) {
            var isEmpty = true;
            var lstCompanyTelephone = $('#lstCompanyTelephone');
            lstCompanyTelephone.empty();

            for (var i = 0; i < data.length; i++) {
                data[i].Index = i;

                if (data[i].State == 'remove') {
                    continue;
                }
                isEmpty = false;
                lstCompanyTelephone.append($('<li>')
                        .text('+63 (' + $.trim(data[i].AreaCode.split('-')[1]) + ') ' + data[i].TelephoneNo + (CStr2(data[i].ExtensionNo) != '' ? '.' + data[i].ExtensionNo : ''))
                    )
            }

            if (isEmpty) {
                lstCompanyTelephone.append('No telephone number found')
            }
        }

        function DisplayMobileList(data) {
            var isEmpty = true;
            var lstCompanyMobile = $('#lstCompanyMobile');
            lstCompanyMobile.empty();

            for (var i = 0; i < data.length; i++) {
                data[i].Index = i;

                if (data[i].State == 'remove') {
                    continue;
                }
                isEmpty = false;
                lstCompanyMobile.append($('<li>')
                        .text('+63' + data[i].MobileNo)
                    )
            }

            if (isEmpty) {
                lstCompanyMobile.append('No mobile number found')
            }
        }

        function DisplayEmailList(data) {
            var isEmpty = true;
            var lstCompanyEmail = $('#lstCompanyEmail');
            lstCompanyEmail.empty();

            for (var i = 0; i < data.length; i++) {
                data[i].Index = i;

                if (data[i].State == 'remove') {
                    continue;
                }
                isEmpty = false;
                lstCompanyEmail.append($('<li>')
                        .append($('<a href="mailto:' + data[i].EmailAdd + '">').text(data[i].EmailAdd))
                    )
            }

            if (isEmpty) {
                lstCompanyEmail.append('No email address found')
            }
        }
    })
})
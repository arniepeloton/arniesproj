$(document).ready(function () {
    
    $('#readmore').click(function () {
        if (this.text == "Read More") {
            $('#showlessmore').show();
            $('#readmore').text('Read Less');
        } else if (this.text == "Read Less") {
            $('#showlessmore').hide();
            $('#readmore').text('Read More');
        }
    })

    $('.dropdown-toggle').dropdown();
    $('.bxslider').bxSlider({
        auto: true,
        pager: true,
        touchEnabled: true,
        responsive: true
    });
    $('.bxslider2').bxSlider({
        pager: true,
        responsive: true
    });

    
    $(window).scroll(function () {
        if ($(this).scrollTop()) {
            $('#toTop').fadeIn();
        } else {
            $('#toTop').fadeOut();
        }
    });

    $("#toTop").click(function () {
        $("html, body").animate({ scrollTop: 0 }, 700);
    });

    function toggleChevron(e) {
        $(e.target)
            .prev('.panel-heading')
            .find("i.indicator")
            .toggleClass('glyphicon-chevron-down glyphicon-chevron-up');
    }

    function toggleChevron2(e2) {
        $(e2.target)
            .prev('.panel-heading')
            .find("i.indicator2")
            .toggleClass('glyphicon-chevron-down glyphicon-chevron-up');
    }

    // ////////////////////////////////////////////////////////////////////////

    // --->> START HOME PAGE ACCORDION
    $('#easyaccordion').on('hidden.bs.collapse', toggleChevron);
    $('#easyaccordion').on('shown.bs.collapse', toggleChevron);

    $('#completeaccordion').on('hidden.bs.collapse', toggleChevron);
    $('#completeaccordion').on('shown.bs.collapse', toggleChevron);

    $('#affordableaccordion').on('hidden.bs.collapse', toggleChevron);
    $('#affordableaccordion').on('shown.bs.collapse', toggleChevron);

    $('#reliableaccordion').on('hidden.bs.collapse', toggleChevron);
    $('#reliableaccordion').on('shown.bs.collapse', toggleChevron);

    $('#secureaccordion').on('hidden.bs.collapse', toggleChevron);
    $('#secureaccordion').on('shown.bs.collapse', toggleChevron);

    $('#crbenefitaccordion').on('hidden.bs.collapse', toggleChevron);
    $('#crbenefitaccordion').on('shown.bs.collapse', toggleChevron);
    // --->> END HOME PAGE ACCORDION

    // /////////////////////////////////////////////////////////////////////

    // --->> START Complete Retail FAQ and Table of Contents ACCORDION
    $('#q1panel').on('hidden.bs.collapse', toggleChevron);
    $('#q1panel').on('shown.bs.collapse', toggleChevron);

    $('#q2panel').on('hidden.bs.collapse', toggleChevron);
    $('#q2panel').on('shown.bs.collapse', toggleChevron);

    $('#q3panel').on('hidden.bs.collapse', toggleChevron);
    $('#q3panel').on('shown.bs.collapse', toggleChevron);

    $('#q4panel').on('hidden.bs.collapse', toggleChevron);
    $('#q4panel').on('shown.bs.collapse', toggleChevron);

    $('#q5panel').on('hidden.bs.collapse', toggleChevron);
    $('#q5panel').on('shown.bs.collapse', toggleChevron);

    $('#q6panel').on('hidden.bs.collapse', toggleChevron);
    $('#q6panel').on('shown.bs.collapse', toggleChevron);

    $('#q7panel').on('hidden.bs.collapse', toggleChevron);
    $('#q7panel').on('shown.bs.collapse', toggleChevron);

    $('#q8panel').on('hidden.bs.collapse', toggleChevron);
    $('#q8panel').on('shown.bs.collapse', toggleChevron);

    $('#q9panel').on('hidden.bs.collapse', toggleChevron);
    $('#q9panel').on('shown.bs.collapse', toggleChevron);

    $('#q10panel').on('hidden.bs.collapse', toggleChevron);
    $('#q10panel').on('shown.bs.collapse', toggleChevron);

    $('#q11panel').on('hidden.bs.collapse', toggleChevron);
    $('#q11panel').on('shown.bs.collapse', toggleChevron);

    $('#q12panel').on('hidden.bs.collapse', toggleChevron);
    $('#q12panel').on('shown.bs.collapse', toggleChevron);

    $('#q13panel').on('hidden.bs.collapse', toggleChevron);
    $('#q13panel').on('shown.bs.collapse', toggleChevron);

    $('#q14panel').on('hidden.bs.collapse', toggleChevron);
    $('#q14panel').on('shown.bs.collapse', toggleChevron);

    $('#su1panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su1panel').on('shown.bs.collapse', toggleChevron2);

    $('#su2panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su2panel').on('shown.bs.collapse', toggleChevron2);

    $('#su3panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su3panel').on('shown.bs.collapse', toggleChevron2);

    $('#su4panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su4panel').on('shown.bs.collapse', toggleChevron2);

    $('#su5panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su5panel').on('shown.bs.collapse', toggleChevron2);

    $('#su6panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su6panel').on('shown.bs.collapse', toggleChevron2);

    $('#su7panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su7panel').on('shown.bs.collapse', toggleChevron2);

    $('#su8panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su8panel').on('shown.bs.collapse', toggleChevron2);

    $('#su9panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su9panel').on('shown.bs.collapse', toggleChevron2);

    $('#su11panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su11panel').on('shown.bs.collapse', toggleChevron2);

    $('#su12panel').on('hidden.bs.collapse', toggleChevron2);
    $('#su12panel').on('shown.bs.collapse', toggleChevron2);
    // --->> END Complete Retail FAQ and Table of Contents ACCORDION

    // ////////////////////////////////////////////////////////////////////////

    // --->> START ALL DROPDOWN TOGGLE
    $(".dropdown-menu li a").click(function () {
        var selText = $(this).text();
        $(this).parents('.btn-group').find('.dropdown-toggle').html(selText + ' <span class="caret"></span>');
    });
    // --->> END ALL DROPDOWN TOGGLE

    // ///////////////////////////////////////////////////////////////////////

    // --->> START USERSMANUAL AND FAQ TOPIC
    $("#opencompleteretail").click(function () {
        $("#completeretaildiv").show();
        $("#directorydiv").hide();
        $("#crmdiv").hide();
        $("#salesforcediv").hide();
        $("#collectiondiv").hide();
        $("#warehousediv").hide();
    })
    $("#opendirectory").click(function () {
        $("#completeretaildiv").hide();
        $("#directorydiv").show();
        $("#crmdiv").hide();
        $("#salesforcediv").hide();
        $("#collectiondiv").hide();
        $("#warehousediv").hide();
    })
    $("#opencrm").click(function () {
        $("#completeretaildiv").hide();
        $("#directorydiv").hide();
        $("#crmdiv").show();
        $("#salesforcediv").hide();
        $("#collectiondiv").hide();
        $("#warehousediv").hide();
    })
    $("#opensalesforce").click(function () {
        $("#completeretaildiv").hide();
        $("#directorydiv").hide();
        $("#crmdiv").hide();
        $("#salesforcediv").show();
        $("#collectiondiv").hide();
        $("#warehousediv").hide();
    })
    $("#opencollection").click(function () {
        $("#completeretaildiv").hide();
        $("#directorydiv").hide();
        $("#crmdiv").hide();
        $("#salesforcediv").hide();
        $("#collectiondiv").show();
        $("#warehousediv").hide();
    })
    $("#openwarehouse").click(function () {
        $("#completeretaildiv").hide();
        $("#directorydiv").hide();
        $("#crmdiv").hide();
        $("#salesforcediv").hide();
        $("#collectiondiv").hide();
        $("#warehousediv").show();
    })
    // --->> END USERSMANUAL AND FAQ TOPIC
});
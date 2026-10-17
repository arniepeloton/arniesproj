var hasLink = new Array();
var links = new Array();

$(function () {

    $("#level0").click(function () {
        getClick(0);
    });

    $("#level1").click(function () {
        getClick(1);
    });

    $("#level2").click(function () {
        getClick(2);
    });

    $("#level3").click(function () {
        getClick(3);
    });
})

function getClick(linkNum) {
    clearLinks();
    if (hasLink[linkNum] == 1) {
            window.open(links[linkNum], '_self', false);
        }
        else {
            links[linkNum].click();
        }    
}

function clearLinks(linkNum) {
    switch (linkNum) {
        case 0: break;
        case 1: break;
        case 2: break;
        case 3: break;        
    }
}

function setLevel0(value, link, isLink){
    if (value != "") {
        $("#level0").html(value);
        $("#level0").css({ 'visibility': 'visible' });        
        //hasLink[0] = "1";
        //links[0]=link;        
    }
}

function setLevel1(value, link) {
    if (value != "") {
        $("#level1").html(value);
        $("#level1").css({ 'visibility': 'visible' });
    }
}

function setLevel2(value, link) {
    if (value != "") {
        $("#level2").html(value);
        $("#level2").css({ 'visibility': 'visible' });
    }
}

function setLevel3(value, link) {
    if (value != "") {
        $("#level3").html(value);
        $("#level3").css({ 'visibility': 'visible' });
    }
}
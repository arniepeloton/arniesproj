function setskuID(itemID) {
    $("#skuID").attr("name", $("#ID").attr("ID"));
    alert($("#skuID").attr("name"));
}

function getskuID() {
    return $("#skuID").attr("name");
}
$(function() {

    new pageDepartments();

});

function pageDepartments() {

    // controls
    var mParent = $('#divParent');
    var formDepartmentSearch = $('#formDepartmentSearch')
        .submit(formDepartmentSearch_Submit);
    var txtSearch = formDepartmentSearch.find('[name=txtSearch]');
    var divResult = $('#divResult');
    var mDataGrid;
    var butCheckAll = $('#butCheckAll')
        .click(butCheckAll_Click);
    var butCheckNone = $('#butCheckNone')
        .click(butCheckNone_Click);

    // constructor
    LoadData(mData);

    function formDepartmentSearch_Submit(e) {

        e.preventDefault();
        mParent.block();
        AjaxSend({
            url: '/Departments/Search',
            data: formDepartmentSearch.serializeObject(),
            success: function (vData) {
                LoadData(vData);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                mParent.unblock();
                divResult.hide()
                    .fadeIn(1000)
                    .ScrollTo()
                    .text('An error occured while retrieving data. ' + errorThrown + ' Error: ' + XMLHttpRequest.responseText);
            }
        }); // AjaxSend

    }; // formDepartmentSearch_Submit

    function LoadData(vData) {

        var pColumns;

        pColumns = {
            'Include': {
                'Control': 'Checkbox',
                'MajorDataType': 'Numeric'
            },
            'Select': {
                'Control': 'Button',
                'Events': {
                    'click': function() {
                        $('<div>')
                            .text('Department name: ' + mDataGrid.DataProps(this).DataRow.DepartmentName)
                            .dialog({
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
                    } // END click
                } // END Events
            }, // END Select
            'DepartmentName': {
                'Label': 'Department Name'
            },
            'Description': null,
            'IsActive': {
                'Label': 'Active',
                'MajorDataType': 'Numeric'
            },
            'LastUpdated': {
                'Label': 'Last Updated',
                'MajorDataType': 'Date',
                'Format': 'MMMM d, yyyy h:mm:ss a'
            }
        };
        vData.d.Columns = pColumns;
        $('#divContainer').html(''); // remove its children and text
        mDataGrid = $('#divContainer').ProWebGrid(vData.d);
        mParent.unblock();

    }; // LoadData

    function butCheckAll_Click(e) {
        
        mDataGrid.CheckAll('Include');

    }; // END butCheckAll_Click

    function butCheckNone_Click(e) {

        mDataGrid.CheckAll('Include', 0);

    }; // END butCheckNone_Click

}; // pageDepartments
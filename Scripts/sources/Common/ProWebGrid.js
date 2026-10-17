(function ($) {

    $.fn.ProWebGrid = function (vOptions) {

        var $this = $(this);
        var pGrid;
        var pDataRows;

        pGrid = $this.data('ProWebGrid');
        if (pGrid) {
            if (vOptions.Data == undefined)
                pDataRows = vOptions.Rows;
            else
                pDataRows = vOptions.Data.Rows;
            pGrid.ReplaceDataRows(pDataRows);
        } else {
            pGrid = new ProWebGrid($this, vOptions);
            $this.data('ProWebGrid', pGrid);
        };

        function ProWebGrid(vDiv, vOptions) {

            var mDT;
            ProWebGrid.prototype.DT = function () { return mDT };
            var mOriginalDiv = vDiv;
            var mParentDiv; // parent of the head and body tables
            var mHeadTable;
            var mHeadDiv;
            var mFootDiv;
            var mBodyTable; // table
            ProWebGrid.prototype.BodyTable = function () { return mBodyTable };
            var mBodyTableBody; // tbody of mBodyTable
            var mBodyDiv;   // container of mBodyTable;
            var mResizing = false;  // true when user is resizing a column
            var mDivResizersParent; // the div that contains the resizer div's
            var mColumnKey = 'ProWebGridColumnKey';     // key to mDT.Columns[x] referencing the corresponding TD
            var mRowKey = 'ProWebGridRowKey';           // key to mDT.Rows[x] containing the corresponding TR
            var mDataRowKey = 'ProWebGridDataRowKey';   // data to table rows referencing its corresponding mDT.Rows
            // attached to: TR.data()
            // value: element of mDT.Rows
            var mDataColumnKey = 'ProWebGridDataColumnKey'; // attached to: TD.data()
            // value: element of mDT.Columns
            var mDataRowIndexKey = 'ProWebGridDataRowIndexKey'; // data to tables rows referencing its corresponding mDT.Rows index
            // attached to: TR.data()
            // value: index to an element of mDT.Rows
            var mSelectedTr;
            var mHiddenRowCount = 1;   // times 2, count for top, and count for bottom
            var mVirtualRowHeight = 32;
            var mMinColWidth = 10;
            var mVirtualBufferHeight = 300; // pixels; data rows will be created to fill the viewport (mBodyDiv) height and this many pixels
            var mRowIndex; // 0-based index to mDT.Rows of the first actual row
            var mVirtualRowTop, mVirtualRowBottom;
            var mInitRowsCalled = false; // boolean
            var mScrollPos = {
                Left: 0,
                Top: 0
            };  // scrollLeft and scrollTop of mBodyDiv
            var mSortKey;
            var mSortAscending = false;

            Init();

            function Init() {

                mOriginalDiv.unbind(); // remove all events to prevent double binding
                mOriginalDiv.empty();
                if (mOriginalDiv.height() == 0) mOriginalDiv.height(400);
                HandleOptions();
                function HandleOptions() {
                    var pOptions;

                    pOptions = {};
                    if (vOptions.Data == undefined)
                        pOptions.Data = vOptions;
                    else
                        pOptions.Data = vOptions.Data;

                    // option defaults
                    pOptions.HoverEffect = vOptions.HoverEffect == undefined ? true : vOptions.HoverEffect;
                    pOptions.Sortable = vOptions.Sortable == undefined ? true : vOptions.Sortable;
                    pOptions.FixedHeaders = vOptions.FixedHeaders == undefined ? true : vOptions.FixedHeaders;

                    vOptions = pOptions;
                }; // END HandleOptions

                mDT = vOptions.Data;
                mParentDiv = $('<div>');
                mParentDiv.css('position', 'absolute');

                mOriginalDiv.append(mParentDiv);
                CreateTable();
                ConvertTable();
                StyleTable();
                ResizableTable();
                MakeSortable();
                //HandleFoot();

                mOriginalDiv.resize(mOriginalDiv_Resize);
                mParentDiv.resize(HeadDiv_Resize);
                mOriginalDiv_Resize();
                HeadDiv_Resize();
                InitRows();
                AdjustHeader();

                function mOriginalDiv_Resize() {
                    mParentDiv.width(mOriginalDiv.width() - 0);
                    mParentDiv.height(mOriginalDiv.height() - 0);
                }; // END mOriginalDiv_Resize

            }; // END Init

            function CreateTable() {
                // create HTML table from JSON
                // the created table is just a simple table, no style, no behaviour

                var pTHead;
                var pTr;
                var pTd;
                var pDivCell;
                var pColumn;    // key in mDT.Columns
                var pText;      // columns header text
                var pIndex;

                mBodyTable = $('<table>');
                mParentDiv.append(mBodyTable);
                pTHead = $('<thead>');
                mBodyTable.append(pTHead);
                mBodyTableBody = $('<tbody>');
                mBodyTable.append(mBodyTableBody);

                // create the head
                pTr = $('<tr>');
                pIndex = 0;
                for (pColumn in mDT.Columns) {
                    if (mDT.Columns[pColumn] == null)
                        mDT.Columns[pColumn] = {};
                    if (mDT.Columns[pColumn].Label != undefined)
                        pText = mDT.Columns[pColumn].Label;
                    else
                        pText = pColumn;
                    mDT.Columns[pColumn].Name = pColumn;
                    pDivCell = $('<div>');
                    pDivCell.addClass('DataCellDiv');
                    pDivCell.text(pText);
                    pDivCell.css('overflow', 'hidden');
                    pTd = $('<td>');
                    pTd.addClass('DataCell');
                    pTd.append(pDivCell);
                    mDT.Columns[pColumn][mColumnKey] = pTd;

                    pTr.append(pTd);
                    pIndex++;
                };
                pTHead.append(pTr);
            }; // CreateTable

            this.InitRows = InitRows;
            function InitRows() {
                /// <param name="vStartIndex">0-based index to mDT.Rows</param>
                /// <param name="vEndIndex">0-based index to mDT.Rows of the last visible row</param>

                var pRowIndexEnd;
                var pTrs; // jQuery TRs that were created

                pTrs = CreateRows();
                SetCellsWidth(mInitRowsCalled, pTrs);
                mInitRowsCalled = true;
                StyleTable(pTrs);

                function CreateRows() {
                    /// <summary>
                    ///     This will add and remove the neccessary rows
                    /// </summary>

                    var pIndex;
                    var pTrs;
                    var pTr;
                    var pVisibleCell;
                    var pFrom, pTo;
                    var pOut; // jQuery TRs. this is the output
                    var pScrollTop;
                    var pHeight;
                    var pFirstTop;  // position().top of the 1st row (including virtual rows and data rows) in the table
                    // well use this as offset to other rows
                    var pFirstDataTop;  // position().top of the 1st data row
                    var pFirstViewTop;  // the top (relative to the viewport) of the first data row
                    var pVirtualTopHeight;
                    var pAddedHeight;   // height of added TR's.
                    // useful only when user scrolled up

                    // find the first visible row
                    function GetFirstVisibleRow() {
                        // RETURNS
                        //  FirstIndex: (integer) 0-based index to mDT.Rows
                        //  LastIndex: (integer) 0-based index to mDT.Rows of the last visible TR
                        //  EdgeIndex: (integer) 0-based index to mDT.Rows of the last visible TR that is on the edge of the viewport.
                        //  ScrollTop: (integer) (optional) mBodyDiv.scrollTop()
                        //  HiddenTops: ($TRs)
                        //  VisibleTop: (integer) top of the first visible TR relative to the top of the first row
                        //  Visibles: ($TRs) visible rows
                        //  HiddenBottoms: ($TRs)

                        var pTrs;
                        var pScrollTop;
                        var pTr;
                        var pOut;
                        var pHeight;
                        var pIndex;
                        var pTrTop;
                        var pFirstTop;  // position().top of the 1st row (including virtual rows and data rows) in the table
                        // we'll use this as offset to other rows
                        pOut = {};
                        pScrollTop = mBodyDiv.scrollTop();
                        pHeight = mBodyDiv.height();
                        pTrs = mBodyTableBody.children('tr'); // include the virtual row and data rows
                        if (pTrs.length > 0)
                            pFirstTop = parseInt($(pTrs[0]).position().top);
                        else
                            pFirstTop = 0;
                        pTrs = pTrs.filter('tr.DataRow');

                        for (var p = 0; p <= pTrs.length - 1; p++) {
                            pTr = $(pTrs[p]);
                            pIndex = pTr.data(mDataRowIndexKey);
                            pTrTop = parseInt(pTr.position().top) - pFirstTop;

                            if (pScrollTop + pHeight + mVirtualBufferHeight < pTrTop) {
                                // hidden bottoms
                                if (pOut.HiddenBottoms == undefined)
                                    pOut.HiddenBottoms = pTr;
                                else
                                    pOut.HiddenBottoms = pOut.HiddenBottoms.add(pTr);
                            } else if (pScrollTop - 2 < pTrTop + pTr.height() + mVirtualBufferHeight) {
                                // visible
                                if (pOut.Visibles == undefined) {
                                    pOut.Visibles = pTr;
                                    pOut.FirstIndex = pIndex;
                                    pOut.ScrollTop = pScrollTop;
                                    pOut.VisibleTop = pTrTop;
                                } else {
                                    pOut.Visibles = pOut.Visibles.add(pTr);
                                };
                                pOut.LastIndex = pIndex;
                                if (pScrollTop + pHeight - 2 <= pTrTop + pTr.height())
                                    pOut.EdgeIndex = pIndex;
                            } else {
                                // hidden tops
                                if (pOut.HiddenTops == undefined) {
                                    pOut.HiddenTops = pTr;
                                } else {
                                    pOut.HiddenTops = pOut.HiddenTops.add(pTr);
                                };
                            };
                        };

                        if (pOut.FirstIndex == undefined)
                            pOut.FirstIndex = parseInt(pScrollTop / mVirtualRowHeight) - 1;
                        return pOut;

                    }; // END GetFirstVisibleRow

                    pVisibleCell = GetFirstVisibleRow();
                    pHeight = mBodyDiv.height();
                    pScrollTop = mBodyDiv.scrollTop();  // save the scroll position before removing and adding rows

                    // determine the height of the virtual top row 
                    if (pVisibleCell.VisibleTop != undefined
                        && pVisibleCell.FirstIndex > 0) {
                        // there is at least 1 visible data row and it is not the first in the set

                        pVirtualTopHeight = pVisibleCell.VisibleTop;
                    } else if (pScrollTop > 0) {
                        pVirtualTopHeight = parseInt(Math.max(pScrollTop - mVirtualBufferHeight, 0) / mVirtualRowHeight) * mVirtualRowHeight;
                        // special situation: when user scrolled to the bottom (and the bottom virtual row was removed)
                        // and then scrolled up.
                        // Create the bottom virtual row first before adding, removing rows or adjusting virtual row.
                        if (mVirtualRowBottom == undefined) {
                            mVirtualRowBottom = $('<tr>').addClass('VirtualRow');
                            mVirtualRowBottom.append($('<td>')
                                .addClass('VirtualCell')
                                .addClass('ui-widget-content')
                                .attr('colspan', PropertyCount(mDT.Columns))
                            );
                            mBodyTableBody.append(mVirtualRowBottom);
                            mVirtualRowBottom.height(mBodyDiv[0].scrollHeight - pVirtualTopHeight);
                        };
                    };

                    // remove the top hidden rows
                    if (pVisibleCell.HiddenTops != undefined)
                        pVisibleCell.HiddenTops.remove();
                    if (pVisibleCell.HiddenBottoms != undefined)
                        pVisibleCell.HiddenBottoms.remove();

                    // create the top virtual row
                    if (CDec2(pVirtualTopHeight) != 0) {
                        if (mVirtualRowTop == undefined) {
                            mVirtualRowTop = $('<tr>').addClass('VirtualRow');
                            mVirtualRowTop.append($('<td>')
                                .addClass('VirtualCell')
                                .addClass('ui-widget-content')
                                .attr('colspan', PropertyCount(mDT.Columns))
                            );
                            mBodyTableBody.prepend(mVirtualRowTop);
                        };
                        mVirtualRowTop.height(pVirtualTopHeight);
                        pTr = mVirtualRowTop;
                    } else {
                        if (mVirtualRowTop != undefined) {
                            mVirtualRowTop.remove();
                            mVirtualRowTop = undefined;
                        };
                    };

                    // determine the top offset
                    pTrs = mBodyTableBody.children('tr'); // include the virtual row and data rows
                    if (pTrs.length > 0)
                        pFirstTop = parseInt($(pTrs[0]).position().top);
                    else
                        pFirstTop = 0;

                    // determine the top (relative to the viewport) of the first data row
                    pTrs = pTrs.filter('tr.DataRow');
                    if (pTrs.length > 0) {
                        pFirstDataTop = $(pTrs[0]).position().top;
                        pFirstViewTop = pFirstDataTop - pFirstTop - pScrollTop;
                    } else
                        pFirstViewTop = 0;

                    // create the rows
                    pAddedHeight = 0;
                    if (pVisibleCell.Visibles == undefined
                        || mScrollPos.Top <= pScrollTop) {

                        // user scrolled down
                        if (pVisibleCell.EdgeIndex == undefined)
                            if (pVisibleCell.LastIndex == undefined)
                                pFrom = parseInt(Math.max(pScrollTop - mVirtualBufferHeight, 0) / mVirtualRowHeight);
                            else
                                pFrom = pVisibleCell.LastIndex + 1;
                        else
                            pFrom = pVisibleCell.EdgeIndex + 1; // this prevents executing the loop

                        if (pVisibleCell.Visibles != undefined)
                            pTr = $(pVisibleCell.Visibles[pVisibleCell.Visibles.length - 1]);
                        pTo = mDT.Rows.length - 1; // just loop until the newly created TR's bottom is below the viewport
                        for (pIndex = pFrom; pIndex <= pTo; pIndex++) {
                            pTr = CreateRow(pIndex, pTr, true);
                            if (pOut)
                                pOut = pOut.add(pTr);
                            else
                                pOut = pTr;

                            if (pScrollTop + pHeight + mVirtualBufferHeight < pTr.position().top - pFirstTop + pTr.height()) break;
                        };
                    } else {
                        // user scrolled up
                        pFrom = pVisibleCell.FirstIndex - 1;
                        pTr = $(pTrs[0]);
                        for (pIndex = pFrom; 0 <= pIndex; pIndex--) {
                            pTr = CreateRow(pIndex, pTr, false);
                            pAddedHeight += pTr.height();
                            if (pOut)
                                pOut = pOut.add(pTr);
                            else
                                pOut = pTr;

                            if (pAddedHeight - pFirstViewTop >= mVirtualBufferHeight) break; // change zero to "hidden height buffer"
                        };

                        // create the top virtual row
                        if (pIndex > 0) {
                            // there is at least 1 visible data row and it is not the first in the set
                            pVirtualTopHeight = pIndex * mVirtualRowHeight;
                        } else
                            pVirtualTopHeight = undefined;
                        if (pVirtualTopHeight != undefined) {
                            if (mVirtualRowTop == undefined) {
                                mVirtualRowTop = $('<tr>').addClass('VirtualRow');
                                mVirtualRowTop.append($('<td>')
                                    .addClass('VirtualCell')
                                    .addClass('ui-widget-content')
                                    .attr('colspan', PropertyCount(mDT.Columns))
                                );
                                mBodyTableBody.prepend(mVirtualRowTop);
                            };
                            mVirtualRowTop.height(pVirtualTopHeight);
                        } else {
                            if (mVirtualRowTop != undefined) {
                                mVirtualRowTop.remove();
                                mVirtualRowTop = undefined;
                            };
                        };

                    }; // END if (pFirstViewTop <= 0) {

                    // create the bottom virtual row
                    if (mBodyTableBody.children('tr.DataRow:last').data(mDataRowIndexKey) < mDT.Rows.length - 1) {
                        if (mVirtualRowBottom == undefined) {
                            mVirtualRowBottom = $('<tr>').addClass('VirtualRow');
                            mVirtualRowBottom.append($('<td>')
                                .addClass('VirtualCell')
                                .addClass('ui-widget-content')
                                .attr('colspan', PropertyCount(mDT.Columns))
                            );
                            mBodyTableBody.append(mVirtualRowBottom);
                        };
                        mVirtualRowBottom.height((pTo - pIndex) * mVirtualRowHeight);
                    } else {
                        if (mVirtualRowBottom != undefined) {
                            mVirtualRowBottom.remove();
                            mVirtualRowBottom = undefined;
                        };
                    };

                    // re-scroll mBodyDiv
                    if (pScrollTop != 500 && pFirstDataTop != undefined
                        && pVisibleCell.Visibles != undefined) {

                        var pTemp = pScrollTop - pFirstDataTop + $(pVisibleCell.Visibles[0]).position().top;
                        mBodyDiv.scrollTop(pTemp);
                    };

                    // debug
                    //Log('Total data rows: ' + mBodyTableBody.children('tr.DataRow').length);

                    function CreateRow(vIndex, vTrSibling, vAddToLast) {
                        /// <summary>
                        ///     Returns the created jQuery TR
                        /// </summary>
                        /// <param name="vIndex" type="integer">0-based index to mDT.Rows</param>
                        /// <param name="vTrSibling" type="jQuery">Append/Prepend (depending on vAddToLast) the new row to this row</param>

                        var pTr;
                        var pTd;
                        var pDivCell;
                        var pText;
                        var pColumn;
                        var pCustomCallbackOutput;

                        pTr = $('<tr>');
                        pTr.addClass('DataRow');
                        pTr.data(mDataRowIndexKey, vIndex);
                        pTr.data(mDataRowKey, mDT.Rows[vIndex]);

                        mDT.Rows[vIndex][mRowKey] = pTr;
                        if (vTrSibling == undefined
                            || vTrSibling.length == 0)

                            if (mVirtualRowTop)
                                mVirtualRowTop.after(pTr);
                            else if (mVirtualRowBottom)
                                mVirtualRowBottom.before(pTr);
                            else
                                mBodyTableBody.append(pTr);
                        else if (vAddToLast)
                            vTrSibling.after(pTr);
                        else
                            vTrSibling.before(pTr);
                        for (pColumn in mDT.Columns) {
                            switch (mDT.Columns[pColumn].ContentType) {
                                case Enum.ProWebGrid.ContentType.RowNumber:
                                    pText = vIndex + 1;
                                    break;

                                default:
                                    pText = mDT.Rows[vIndex][pColumn];
                                    break;
                            };
                            if (mDT.Columns[pColumn].Format != undefined
                                && mDT.Columns[pColumn].MajorDataType != undefined) {

                                switch (mDT.Columns[pColumn].MajorDataType) {
                                    case 'Numeric':
                                        if (IsNumeric(pText))
                                            pText = $.format.number(parseFloat(pText), mDT.Columns[pColumn].Format);
                                        break;

                                    case 'Date':
                                        if (IsDate(pText))
                                            pText = $.format.date(new Date(pText), mDT.Columns[pColumn].Format);
                                        break;
                                };
                            };
                            if (pText == undefined) pText = '';

                            pDivCell = $('<div>');
                            pDivCell.addClass('DataCellDiv');
                            pDivCell.css('height', '100%');
                            if (mDT.Columns[pColumn] != null) {
                                switch (CStr2(mDT.Columns[pColumn].Alignment).toLowerCase()) {
                                    case 'right':
                                        pDivCell.css('text-align', 'right');
                                        break;

                                    case 'center':
                                        pDivCell.css('text-align', 'center');
                                        break;

                                    default: // blank = Left

                                };
                            };

                            if (mDT.Columns[pColumn] != null
                                && mDT.Columns[pColumn].Control != null) {

                                switch (mDT.Columns[pColumn].Control) {
                                    case 'Checkbox':
                                        var pCheckbox;

                                        pCheckbox = $('<input type="checkbox" />')
                                        pCheckbox.prop('checked', CDec2(pText));
                                        pCheckbox.click(function (e) {
                                            mDT.Rows[vIndex][$(this).closest('.DataCell').data(mDataColumnKey).Name] = $(this).prop('checked') ? 1 : 0;
                                            e.stopPropagation();
                                        });
                                        pDivCell.click(function (e) {
                                            var pCheckbox;

                                            pCheckbox = $(this).find(':checkbox');
                                            pCheckbox.prop('checked', !pCheckbox.prop('checked'));
                                            mDT.Rows[vIndex][$(this).closest('.DataCell').data(mDataColumnKey).Name] = pCheckbox.prop('checked') ? 1 : 0;
                                        });
                                        pDivCell.append(pCheckbox);
                                        break;

                                    case 'Button':
                                        var pButton;

                                        pButton = $('<button>')
                                            .addClass('button')
                                            .text('Select')
                                            .button();
                                        pButton.bind(mDT.Columns[pColumn]['Events']);
                                        pDivCell.append(pButton);
                                        break;

                                    case 'Custom':
                                        if (mDT.Columns[pColumn].CustomCallback) {
                                            pCustomCallbackOutput = mDT.Columns[pColumn].CustomCallback(mDT.Rows[vIndex]);
                                            if (pCustomCallbackOutput)
                                                pDivCell.append(pCustomCallbackOutput);
                                        };
                                        break;

                                    default:
                                        pDivCell.text(pText);
                                        break;
                                };
                            } else {
                                pDivCell.text(pText);
                            };

                            pTd = $('<td>');
                            pTd.data(mDataColumnKey, mDT.Columns[pColumn]);
                            pTd.addClass('DataCell');
                            pTd.append(pDivCell);
                            pTr.append(pTd);
                        }; // END for (pColumn in mDT.Columns) {

                        return pTr;

                    }; // END function CreateRow() {

                    return pOut; ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                }; // END function CreateRows() {
            }; // END InitRows

            function ConvertTable() {

                var pTHead;

                var mReinitRows = $.debounce(250, function () {
                    // debounce or throttle
                    InitRows();
                    mScrollPos.Left = mBodyDiv.scrollLeft();
                    mScrollPos.Top = mBodyDiv.scrollTop();
                });

                mParentDiv.css('overflow', 'hidden');
                mBodyDiv = $('<div>');
                mBodyDiv.css('overflow', 'auto');
                mBodyDiv.width('100%');
                if (vOptions.FixedHeaders)
                    mBodyDiv.scroll(mBodyDiv_Scroll);

                if (vOptions.FixedHeaders) {
                    pTHead = mBodyTable.find('thead');
                    mHeadTable = $('<table>');
                    mHeadTable.css('margin-bottom', 0);
                    mHeadTable.append(pTHead);
                    mHeadDiv = $('<div>'); // parent of the head table
                    mHeadDiv.css('overflow', 'hidden');
                    mHeadDiv.append(mHeadTable);
                    mParentDiv.append(mHeadDiv);
                } else {
                    mHeadDiv = mBodyDiv;
                };

                mBodyDiv.append(mBodyTable);
                mParentDiv.append(mBodyDiv);

                function mBodyDiv_Scroll(e) {

                    var p;

                    p = '-' + mBodyDiv.scrollLeft() + 'px';
                    mHeadTable.css('margin-left', p);
                    mDivResizersParent.css('margin-left', p);
                    mReinitRows();

                }; // mBodyDiv_Scroll

                return mParentDiv;
            }; // ConvertTable

            function SetCellsWidth(vFromHeader, vElements) {
                /// <param name="vElements">jQuery TRs to be affected. Optional. If ommitted, mBodyTableBody.children('tr.DataRow')</param>
                // set explicit div width to each cell base on headers width
                // called only once by ConvertTable

                var pTds;
                var pWidths = []; // array of integer
                var p;
                var pDC; // data column

                if (mHeadTable == undefined) return;
                if (vElements == undefined)
                    vElements = mBodyTableBody.children('tr.DataRow');
                if (vElements.length == 0) return

                if (CDec2(vFromHeader) == 0) {
                    pTds = $(vElements[0]).children('td');
                    for (p = 0; p <= pTds.length; p++) {
                        pDC = $(pTds[p]).data(mDataColumnKey);
                        if (pDC != undefined
                            && pDC.Width != undefined)

                            pWidths[p] = pDC.Width;
                        else
                            pWidths[p] = $(pTds[p]).children('div').width();
                    };
                } else {
                    pTds = mHeadTable.children('thead').children('tr:first').children('td');
                    for (p = 0; p <= pTds.length; p++) {
                        pWidths[p] = $(pTds[p]).children('div').width();
                    };
                };

                vElements.each(function (vIndex, vItem) {
                    pTds = $(vItem).children('td');
                    for (p = 0; p <= pTds.length; p++) {
                        $($(vItem).children('td')[p]).children('div').width(pWidths[p]);
                    };
                });

            }; // SetCellsWidth

            function StyleTable(vElements) {
                /// <param name="vElements">jQuery TRs to be affected. Optional. If ommitted, mBodyTableBody.children('tr.DataRow')</param>

                var p;

                if (vElements)
                    p = vElements;
                else if (mHeadTable)
                    p = mHeadTable;
                else
                    p = mBodyTable.find('thead');

                if (!vElements) {
                    p.find('td').addClass('ui-state-default');
                    p = mBodyTableBody.children('tr.DataRow');
                };
                if (vOptions.HoverEffect) {
                    p.unbind().bind({
                        mouseover: MouseOver,
                        mouseout: MouseOut,
                        click: Click,
                        dblclick: function () {
                            RaiseRowDoubleClick({
                                SelectedRow: $(this)
                            });
                        }
                    });
                };
                p.children('td').addClass('ui-widget-content'); // the blue border lines of the cells

                function MouseOver(e) {
                    if (mResizing) return;
                    $(this).children('td').addClass('ui-state-hover')
                        .css('font-weight', 'normal');
                }; // END MouseOver

                function MouseOut(e) {
                    if (mResizing) return;
                    $(this).children('td').removeClass('ui-state-hover');
                }; // END MouseOut

                function Click(e) {
                    if (mSelectedTr)
                        mSelectedTr.children('td').removeClass('ui-state-highlight');
                    if (mSelectedTr == undefined || mSelectedTr.index() != $(this).index()) {
                        mSelectedTr = $(this);
                        mSelectedTr.children('td').toggleClass('ui-state-highlight');
                        RaiseAfterRowActivate({
                            SelectedRow: mSelectedTr
                        });
                    };
                    return false;
                }; // END Click
            }; // StyleTable

            function ResizableTable() {
                // make this table's columns user-resizable.
                // "this" should be the parent DIV of the table (head and body)

                var pTHead, pTBody;
                var pDiv;
                var pHeight;

                mDivResizersParent = $('<div>');
                mDivResizersParent.css('position', 'absolute');
                mDivResizersParent.css('top', '0px');
                mParentDiv.append(mDivResizersParent);

                pTHead = mParentDiv.find('thead').children('tr').children('td');
                pTBody = mParentDiv.find('tbody').children('tr').children('td');
                pTHead.each(function (vIndex, vItem) {
                    pDiv = $('<div>');
                    pDiv.width(5);
                    pDiv.css('position', 'absolute');
                    pDiv.css('cursor', 'col-resize');
                    pDiv.css('background-color', '#A6C9E2');
                    pDiv.css('opacity', 0);
                    mDivResizersParent.append(pDiv);

                    pDiv.draggable({
                        axis: "x",
                        start: function (event, ui) {

                            mResizing = true;
                            ui.helper.css('opacity', .5)
                                .height(mBodyDiv[0].clientHeight + mHeadDiv.height());

                        }, // start
                        drag: function (event, ui) {

                        }, // drag
                        stop: function (event, ui) {
                            var pDiv;
                            var pTdIndex;
                            var pTrs;
                            var pWidth;

                            mResizing = false;
                            ui.helper.css('opacity', 0);
                            pDiv = $(mBodyTableBody.find('tr.DataRow:first').children('td')[ui.helper.index()]).children('div:first');
                            pTdIndex = ui.helper.index();
                            pWidth = pDiv.width() + ui.position.left - ui.originalPosition.left;
                            if (pWidth < mMinColWidth) pWidth = mMinColWidth;
                            pTrs = mBodyTableBody.children('tr');
                            pTrs.each(function (vIndex, vItem) {
                                $(vItem.children[pTdIndex]).children('div').width(pWidth);
                            }); // pTrs.each

                            // dont call RepositionResizers directly.
                            // Otherwise, resizers will be incorrectly repositioned when user reduced a column width, causing the grid to re-scroll
                            setTimeout(AdjustHeader, 100); // duration of 1 is not enough
                        } // stop
                    }); // draggable

                }); // pTHead.each

                RepositionResizers(); // set initial postion of resizers

                return mParentDiv;


            }; // ResizableTable

            function RepositionResizers() {

                var pTr;

                if (mDivResizersParent == undefined) return;
                pTr = mBodyTableBody.children('tr.DataRow:first');
                mDivResizersParent.children().each(function (vIndex, vItem) {
                    var pTd;

                    pTd = $(pTr.children('td').get(vIndex));
                    if (pTd == null) return;
                    if (pTd.position() == null) return;
                    $(vItem).css('left', -parseInt(mDivResizersParent.css('margin-left')) + pTd.position().left + pTd.width() - 1 + 'px');

                }); // mDivResizersParent.children().each

                // call this in a timer
                // or else, the header wont be sized correctly when user resize the column too narrowly
                //                setTimeout(AdjustHeader, 1);
            }; // RepositionResizers

            function AdjustHeader() {

                var pTdsHead;
                var pTdsBody;
                var pWidth;

                if (!vOptions.FixedHeaders) return;
                pTdsHead = mHeadTable.children('thead').children('tr:first').children('td');
                pTdsBody = mBodyTableBody.children('tr.DataRow:first').children('td');

                pTdsBody.each(function (vIndex, vItem) {
                    pWidth = $(vItem).children('div').width();
                    if (pWidth < mMinColWidth) {
                        pWidth = mMinColWidth;
                        $(vItem).children('div').width(pWidth);
                    };
                    $(pTdsHead[vIndex]).children('div').width(pWidth);
                });
                HeadDiv_Resize();
                RepositionResizers();

            }; // AdjustHeader

            function MakeSortable() {
                // the user can sort the table rows by clicking on the header

                InitStyleAndBehaviour();

                function InitStyleAndBehaviour() {

                    var pColumn;
                    var pText;
                    var pTd;
                    var pDiv;
                    var pLink;

                    if (!vOptions.Sortable) return;
                    for (pColumn in mDT.Columns) {
                        pTd = mDT.Columns[pColumn][mColumnKey];
                        pDiv = pTd.children('div');
                        pText = pDiv.text();
                        pDiv.html(''); // remove children
                        pLink = $('<span>')
                            .data(mColumnKey, pColumn)
                            .text(pText)
                            .css('float', 'left')
                            .css('cursor', 'pointer')
                            .css('margin-bottom', '2px') // adding the sort image span will make the header 2px taller so add it now to prevent making it taller later
                        //.addClass('spanSorter') // no effect, just make it easier to css-select later
                            .mouseover(function () {
                                $(this).css('text-decoration', 'underline');
                            })
                            .mouseout(function () {
                                $(this).css('text-decoration', '');
                            })
                            .click(function () {

                                var pKey;

                                pKey = $(this).data(mColumnKey);
                                if (mSortKey == pKey)
                                    mSortAscending = !mSortAscending;
                                else
                                    mSortAscending = true;
                                mSortKey = pKey;
                                mHeadTable.find('.ui-icon').remove();
                                $(this).parent().append($('<span>')
                                    .addClass('ui-icon')
                                    .addClass(mSortAscending ? 'ui-icon-triangle-1-n' : 'ui-icon-triangle-1-s')
                                    .css('float', 'left')
                                    );
                                Sort();
                                HeadDiv_Resize();
                            });
                        pDiv.append(pLink);
                    };
                    HeadDiv_Resize();

                }; // InitStyleAndBehaviour

            }; // MakeSortable

            function Sort() {
                // sort the rows using mSortKey

                if (mSortKey != null) {
                    mDT.Rows.sort(function (v1, v2) {
                        var p1, p2;
                        var pMdt;

                        if (mDT.Columns[mSortKey] != null
                            && mDT.Columns[mSortKey].Control != null) {

                            switch (mDT.Columns[mSortKey].Control) {
                                case 'Checkbox':
                                    p1 = v1[mSortKey];
                                    p2 = v2[mSortKey];

                                    break;

                                default:
                                    p1 = v1[mSortKey];
                                    p2 = v2[mSortKey];
                            };
                        } else {
                            p1 = v1[mSortKey];
                            p2 = v2[mSortKey];
                        };

                        if (mDT.Columns[mSortKey] != null
                            && mDT.Columns[mSortKey].MajorDataType != null)
                            pMdt = mDT.Columns[mSortKey].MajorDataType;
                        else
                            pMdt = 'String';
                        switch (pMdt) {
                            case 'Numeric':
                                p1 = CDec2(p1);
                                p2 = CDec2(p2);
                                break;

                            case 'Date':
                                p1 = CDec2(Date.parse(p1));
                                p2 = CDec2(Date.parse(p2));
                                break;

                            default:
                                p1 = p1.toUpperCase();
                                p2 = p2.toUpperCase();

                        }; // END switch (mDT.Columns[mSortKey].MajorDataType)

                        if (p1 > p2)
                            return mSortAscending ? 1 : -1;
                        else if (p1 < p2)
                            return mSortAscending ? -1 : 1;
                        else
                            return 0;
                    });
                }; // END if (mSortKey != null) {

                mBodyTableBody.empty();
                mVirtualRowTop = null;
                mVirtualRowBottom = null;
                InitRows();

            }; // Sort

            function HeadDiv_Resize() {
                // not really an event

                if (vOptions.FixedHeaders) {
                    mBodyDiv.height(mParentDiv.height() - mHeadDiv.height() - FootHeight());
                    if (mDivResizersParent)
                        mDivResizersParent.children().height(mHeadDiv.height());
                } else {
                    mBodyDiv.height(mParentDiv.height());
                    if (mDivResizersParent)
                        mDivResizersParent.children().height(mBodyTable.find('thead').height());
                };

            }; // END HeadDiv_Resize

            function HandleFoot() {

                var pDiv; // flash placeholder

                //return;
                mFootDiv = $('<div>');
                mFootDiv.height(30);
                mParentDiv.append(mFootDiv);

                pDiv = $('<div>');
                mFootDiv.append(pDiv);

                pDiv.downloadify({
                    filename: 'Export.csv',
                    data: function () {
                        return JsonToCsv(mDT.Rows);
                    },
                    onComplete: function () {
                        ShowMessage('Your File Has Been Saved!', "Done");
                    },
                    onCancel: function () {
                        alert('You have cancelled the saving of this file.');
                    },
                    onError: function () {
                        alert('You must put something in the File Contents or there will be nothing to save!');
                    }
                });

            }; // END HandleFoot

            function FootHeight() {

                if (mFootDiv == undefined)
                    return 0;
                else
                    return mFootDiv.height();

            }; // END FootHeight

            ProWebGrid.prototype.GetColumnIndex = function (vColumnName) {

                return mDT.Columns[vColumnName][mColumnKey].index()

            };

            ProWebGrid.prototype.DataProps = function (vElement) {

                var pOut;
                var pTd, pTr;

                if (vElement instanceof $ == false)
                    vElement = $(vElement);
                pOut = {};

                pTd = vElement.closest('td.DataCell');
                pTr = pTd.closest('tr.DataRow');

                pOut.DataColumn = pTd.data(mDataColumnKey);
                pOut.DataRow = pTr.data(mDataRowKey);
                pOut.Data = pOut.DataRow[pOut.DataColumn.Name];

                return pOut;
            };

            ProWebGrid.prototype.RowData = function (vTR) {

                if (vTR instanceof $ == false)
                    vTR = $(vTR);
                return vTR.data(mDataRowKey);

            }; // this.RowData

            ProWebGrid.prototype.SelectedRow = function () {
                // returns the selected/highlighted $tr
                return mSelectedTr;

            }; // END this.SelectedRow

            ProWebGrid.prototype.AfterRowActivate = function (vFunction) {
                mOriginalDiv.bind('AfterRowActivate', vFunction);
                return this;
            };

            function RaiseAfterRowActivate(vParams) {

                var pE = $.Event('AfterRowActivate');
                pE.SelectedRow = vParams.SelectedRow;
                mOriginalDiv.trigger(pE);

            }; // END RaiseAfterRowActivate

            ProWebGrid.prototype.RowDoubleClick = function (vFunction) {
                mOriginalDiv.bind('RowDoubleClick', vFunction);
                return this;
            };

            function RaiseRowDoubleClick(vParams) {

                var pE = $.Event('RowDoubleClick');
                pE.SelectedRow = vParams.SelectedRow;
                mOriginalDiv.trigger(pE);

            }; // END RaiseRowDoubleClick

            this.ClearRows = ClearRows;
            function ClearRows() {

                mBodyTableBody.empty();
                mVirtualRowTop = null;
                mVirtualRowBottom = null;

            }; // END this.ClearRows

            this.ReplaceDataRows = function (vDataRows) {

                mDT.Rows = vDataRows;
                Sort();

            }

            // before removing div from the html, call this
            ProWebGrid.prototype.StoreScroll = function () {
                mScrollPos.Left = mBodyDiv.scrollLeft();
                mScrollPos.Top = mBodyDiv.scrollTop();
            };

            // after re-adding div to the html, call this
            ProWebGrid.prototype.RestoreScroll = function () {
                mBodyDiv.scrollLeft(mScrollPos.Left);
                mBodyDiv.scrollTop(mScrollPos.Top);
            };

        }; // END ProWebGrid

        return pGrid;

    }; // $.fn.ProWebGrid

})(jQuery);

Enum.ProWebGrid = {
    ContentType: {
        RowNumber: 1
    }
};

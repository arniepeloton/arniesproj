require(['common'], function () {
    $(document).ready(function () {
        //GetSales();


      

        ////Yesterday AC
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'Yesterday AC',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'Actual AC',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '110',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'LY Same Date AC'
        //                },
        //                {
        //                    Label: '106',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Growth',
        //                },
        //                {
        //                    Label: '3.9%',
        //                    ClassName: 'text-right'
        //                }
        //            ]
        //        },
        //        {
        //            Title: 'Yesterday Labor',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'SPMH',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '1,571',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'Total Man Hours'
        //                },
        //                {
        //                    Label: '7,000',
        //                    ClassName: 'text-right'
        //                }
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        

        ////Yesterday TC Mix
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'Yesterday Top 5 Products',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 4,
        //            Rows: [
        //                {
        //                    Label: 'No.',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Description',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Sales',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Qty Sold',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: '1.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '100,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '500',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '2.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '50,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '400',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '3.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '20,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '200',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '4.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '10,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '100',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '5.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '5,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '50',
        //                    ClassName: 'text-center'
        //                },
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////MTD Sales
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'MTD Sales',
        //            ClassName: 'bg-warning',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'Actual Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11, 000,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'LY Same Date Sales'
        //                },
        //                {
        //                    Label: '85,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Growth',
        //                },
        //                {
        //                    Label: '22.2%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Average Daily Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'No. of Stores',
        //                },
        //                {
        //                    Label: '1,000',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Same Store Sales',
        //                },
        //                {
        //                    Label: '9,500,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'SSSG',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11.8%',
        //                    ClassName: 'font-weight-bold text-right'
        //                }
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////MTD TC
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'MTD TC',
        //            ClassName: 'bg-warning',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'Actual TC',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '100,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'LY Same Date TC'
        //                },
        //                {
        //                    Label: '85,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Growth',
        //                },
        //                {
        //                    Label: '17.6%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Average TC',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '100',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'No. of Stores',
        //                },
        //                {
        //                    Label: '1,000',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Same Store TC',
        //                },
        //                {
        //                    Label: '75,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Same Store TC Growth',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '36.4%',
        //                    ClassName: 'font-weight-bold text-right'
        //                }
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////MTD AC
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'MTD AC',
        //            ClassName: 'bg-warning',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'Actual AC',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '110',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'LY Same Date AC'
        //                },
        //                {
        //                    Label: '106',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Growth',
        //                },
        //                {
        //                    Label: '3.9%',
        //                    ClassName: 'text-right'
        //                }
        //            ]
        //        },
        //        {
        //            Title: 'MTD Labor',
        //            ClassName: 'bg-warning',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'SPMH',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '1,571',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'Total Man Hours'
        //                },
        //                {
        //                    Label: '7,000',
        //                    ClassName: 'text-right'
        //                }
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////MTD Sales Mix
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'MTD Sales Mix',
        //            ClassName: 'bg-warning',
        //            NoOfColPerRow: 3,
        //            Rows: [
        //                {
        //                    Label: 'Take-out Sales',
        //                },
        //                {
        //                    Label: '7,000.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '63.6%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Delivery Sales',
        //                },
        //                {
        //                    Label: '3,000.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '27.3%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Dine-in Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Drive Thru Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Total Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11,000.000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: '100%',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////MTD TC Mix
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'MTD TC Mix',
        //            ClassName: 'bg-warning',
        //            NoOfColPerRow: 3,
        //            Rows: [
        //                {
        //                    Label: 'Take-out Sales',
        //                },
        //                {
        //                    Label: '7,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '63.6%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Delivery Sales',
        //                },
        //                {
        //                    Label: '3,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '27.3%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Dine-in Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Drive Thru Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Total Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: '100%',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////MTD TC Mix
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'MTD Top 5 Products',
        //            ClassName: 'bg-warning',
        //            NoOfColPerRow: 4,
        //            Rows: [
        //                {
        //                    Label: 'No.',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Description',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Sales',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Qty Sold',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: '1.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '100,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '500',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '2.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '50,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '400',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '3.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '20,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '200',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '4.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '10,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '100',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '5.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '5,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '50',
        //                    ClassName: 'text-center'
        //                },
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////YTD Sales
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'YTD Sales',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'Actual Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11, 000,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'LY Same Date Sales'
        //                },
        //                {
        //                    Label: '85,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Growth',
        //                },
        //                {
        //                    Label: '22.2%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Average Daily Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'No. of Stores',
        //                },
        //                {
        //                    Label: '1,000',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Same Store Sales',
        //                },
        //                {
        //                    Label: '9,500,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'SSSG',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11.8%',
        //                    ClassName: 'font-weight-bold text-right'
        //                }
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////YTD TC
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'YTD TC',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'Actual TC',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '100,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'LY Same Date TC'
        //                },
        //                {
        //                    Label: '85,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Growth',
        //                },
        //                {
        //                    Label: '17.6%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Average TC',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '100',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'No. of Stores',
        //                },
        //                {
        //                    Label: '1,000',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Same Store TC',
        //                },
        //                {
        //                    Label: '75,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Same Store TC Growth',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '36.4%',
        //                    ClassName: 'font-weight-bold text-right'
        //                }
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////YTD AC
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'YTD AC',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'Actual AC',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '110',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'LY Same Date AC'
        //                },
        //                {
        //                    Label: '106',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Growth',
        //                },
        //                {
        //                    Label: '3.9%',
        //                    ClassName: 'text-right'
        //                }
        //            ]
        //        },
        //        {
        //            Title: 'YTD Labor',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 2,
        //            Rows: [
        //                {
        //                    Label: 'SPMH',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '1,571',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: 'Total Man Hours'
        //                },
        //                {
        //                    Label: '7,000',
        //                    ClassName: 'text-right'
        //                }
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////YTD Sales Mix
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'YTD Sales Mix',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 3,
        //            Rows: [
        //                {
        //                    Label: 'Take-out Sales',
        //                },
        //                {
        //                    Label: '7,000.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '63.6%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Delivery Sales',
        //                },
        //                {
        //                    Label: '3,000.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '27.3%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Dine-in Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Drive Thru Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Total Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11,000.000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: '100%',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////YTD TC Mix
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'YTD TC Mix',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 3,
        //            Rows: [
        //                {
        //                    Label: 'Take-out Sales',
        //                },
        //                {
        //                    Label: '7,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '63.6%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Delivery Sales',
        //                },
        //                {
        //                    Label: '3,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '27.3%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Dine-in Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Drive Thru Sales',
        //                },
        //                {
        //                    Label: '500.000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '4.5%',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: 'Total Sales',
        //                    ClassName: 'font-weight-bold'
        //                },
        //                {
        //                    Label: '11,000.00',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //                {
        //                    Label: '100%',
        //                    ClassName: 'font-weight-bold text-right'
        //                },
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})

        ////YTD TC Mix
        //GridReport({
        //    Report: [
        //        {
        //            Title: 'YTD Top 5 Products',
        //            ClassName: 'bg-success',
        //            NoOfColPerRow: 4,
        //            Rows: [
        //                {
        //                    Label: 'No.',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Description',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Sales',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: 'Qty Sold',
        //                    ClassName: 'text-center font-weight-bold'
        //                },
        //                {
        //                    Label: '1.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '100,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '500',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '2.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '50,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '400',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '3.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '20,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '200',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '4.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '10,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '100',
        //                    ClassName: 'text-center'
        //                },
        //                {
        //                    Label: '5.',
        //                },
        //                {
        //                    Label: '',
        //                },
        //                {
        //                    Label: '5,000.00',
        //                    ClassName: 'text-right'
        //                },
        //                {
        //                    Label: '50',
        //                    ClassName: 'text-center'
        //                },
        //            ]
        //        }
        //    ],
        //    Click: function (e) {

        //    }
        //})
    })

    function GridReport(vData) {
        var grid = $('<div class="col mb-3">');
        var container = $('<div class="border rounded h-100">');     

        for (var i in vData.Report) {
            var data = vData.Report[i];            
            var cols = $('<div class="row gx-0 row-cols-' + (data.NoOfColPerRow || 2) + '">');

            container.append($('<div class="text-center text-white p-2 ' + (data.ClassName || '') + '">').text(data.Title));

            for (var m in data.Rows) {
                var rows = data.Rows[m];
                cols.append($('<div class="col p-2 border-bottom ' + (rows.ClassName || '') + '">').html(rows.Label));
            }            

            container.append(cols);            
        }

        grid.append(container);

        $('#divwowreport').append(grid.click(function (e) {
            if (typeof vData.Click == 'function') {
                vData.Click();
            }
        }));
    }

    function GetSales() {
        AjaxSend({
            url: 'CincoCorporation/GetSalesByDate',
            success: function (vData) {                
                AjaxSend({
                    url: 'CincoCorporation/GetSalesMixByDate',
                    success: function (vRowsSalesMix) {
                        $('#divwowreport').empty();
                        DisplaySales(vData, vRowsSalesMix);
                    }
                });
            }
        });
    }

    function DisplaySales(vRows, vRowsSalesMix) {

        //Yesterday Sales
        GridReport({
            Report: [
                {
                    Title: 'Yesterday Sales',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 2,
                    Rows: [
                        {
                            Label: 'Actual Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(vRows.YesterdaySales.Rows[0].GrossSales),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'LY Same Date Sales'
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearYesterdaySales.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Growth',
                        },
                        {
                            Label: AccountingFormat((CDec2(vRows.YesterdaySales.Rows[0].GrossSales) - CDec2(vRows.LastYearYesterdaySales.Rows[0].GrossSales)) / CDec2(vRows.LastYearYesterdaySales.Rows[0].GrossSales) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Average Daily Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRows.YesterdaySales.Rows[0].GrossSales) / CDec2(vRows.LastYearYesterdaySales.Rows[0].StoreCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'No. of Stores',
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearYesterdaySales.Rows[0].StoreCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'SSSG',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: '-',
                            ClassName: 'font-weight-bold text-right'
                        }
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //Yesterday TC
        GridReport({
            Report: [
                {
                    Title: 'Yesterday TC',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 2,
                    Rows: [
                        {
                            Label: 'Actual TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(vRows.YesterdaySales.Rows[0].TransactionCount),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'LY Same Date TC'
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearYesterdaySales.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Growth',
                        },
                        {
                            Label: AccountingFormat((CDec2(vRows.YesterdaySales.Rows[0].TransactionCount) - CDec2(vRows.LastYearYesterdaySales.Rows[0].TransactionCount)) / CDec2(vRows.LastYearYesterdaySales.Rows[0].TransactionCount) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Average Daily TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRows.YesterdaySales.Rows[0].TransactionCount) / CDec2(vRows.LastYearYesterdaySales.Rows[0].StoreCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'No. of Stores',
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearYesterdaySales.Rows[0].StoreCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store TC Growth',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: '-',
                            ClassName: 'font-weight-bold text-right'
                        }
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //Yesterday Sales Mix
        GridReport({
            Report: [
                {
                    Title: 'Yesterday Sales Mix',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 3,
                    Rows: [
                        {
                            Label: 'Take-out Sales',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YesterdayTakeOut.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].GrossSales) / (CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].GrossSales)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Delivery Sales',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YesterdayDelivery.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].GrossSales) / (CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].GrossSales)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Dine-in Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Drive Thru Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Total Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].GrossSales)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: '100%',
                            ClassName: 'font-weight-bold text-right'
                        },
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //Yesterday TC Mix
        GridReport({
            Report: [
                {
                    Title: 'Yesterday TC Mix',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 3,
                    Rows: [
                        {
                            Label: 'Take-out TC',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YesterdayTakeOut.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].TransactionCount) / (CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].TransactionCount)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Delivery TC',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YesterdayDelivery.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].TransactionCount) / (CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].TransactionCount)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Dine-in TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Drive Thru TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Total TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YesterdayTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.YesterdayDelivery.Rows[0].TransactionCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: '100%',
                            ClassName: 'font-weight-bold text-right'
                        },
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //MTD Sales
        GridReport({
            Report: [
                {
                    Title: 'MTD Sales',
                    ClassName: 'bg-warning',
                    NoOfColPerRow: 2,
                    Rows: [
                        {
                            Label: 'Actual Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(vRows.MTD.Rows[0].GrossSales),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'LY Same Date Sales'
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearMTD.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Growth',
                        },
                        {
                            Label: AccountingFormat((CDec2(vRows.MTD.Rows[0].GrossSales) - CDec2(vRows.LastYearMTD.Rows[0].GrossSales)) / CDec2(vRows.MTD.Rows[0].GrossSales) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Average Daily Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRows.MTD.Rows[0].GrossSales) / CDec2(vRows.MTD.Rows[0].StoreCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'No. of Stores',
                        },
                        {
                            Label: AccountingFormat(vRows.MTD.Rows[0].StoreCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'SSSG',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: '-',
                            ClassName: 'font-weight-bold text-right'
                        }
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //MTD TC
        GridReport({
            Report: [
                {
                    Title: 'MTD TC',
                    ClassName: 'bg-warning',
                    NoOfColPerRow: 2,
                    Rows: [
                        {
                            Label: 'Actual TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(vRows.MTD.Rows[0].TransactionCount),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'LY Same Date TC'
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearMTD.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Growth',
                        },
                        {
                            Label: AccountingFormat((CDec2(vRows.MTD.Rows[0].TransactionCount) - CDec2(vRows.LastYearMTD.Rows[0].TransactionCount)) / CDec2(vRows.MTD.Rows[0].TransactionCount) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Average Daily TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRows.MTD.Rows[0].TransactionCount) / CDec2(vRows.MTD.Rows[0].StoreCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'No. of Stores',
                        },
                        {
                            Label: AccountingFormat(vRows.MTD.Rows[0].StoreCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store TC Growth',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: '-',
                            ClassName: 'font-weight-bold text-right'
                        }
                    ]
                }
            ],
            Click: function (e) {

            }
        })
               
        //MTD Sales Mix
        GridReport({
            Report: [
                {
                    Title: 'MTD Sales Mix',
                    ClassName: 'bg-warning',
                    NoOfColPerRow: 3,
                    Rows: [
                        {
                            Label: 'Take-out Sales',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.MTDTakeOut.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].GrossSales) / (CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.MTDDelivery.Rows[0].GrossSales)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Delivery Sales',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.MTDDelivery.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.MTDDelivery.Rows[0].GrossSales) / (CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.MTDDelivery.Rows[0].GrossSales)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Dine-in Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Drive Thru Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Total Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.MTDDelivery.Rows[0].GrossSales)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: '100%',
                            ClassName: 'font-weight-bold text-right'
                        },
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //MTD TC Mix
        GridReport({
            Report: [
                {
                    Title: 'MTD TC Mix',
                    ClassName: 'bg-warning',
                    NoOfColPerRow: 3,
                    Rows: [
                        {
                            Label: 'Take-out TC',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.MTDTakeOut.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].TransactionCount) / (CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.MTDDelivery.Rows[0].TransactionCount)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Delivery TC',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.MTDDelivery.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.MTDDelivery.Rows[0].TransactionCount) / (CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.MTDDelivery.Rows[0].TransactionCount)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Dine-in TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Drive Thru TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Total TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.MTDTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.MTDDelivery.Rows[0].TransactionCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: '100%',
                            ClassName: 'font-weight-bold text-right'
                        },
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //YTD Sales
        GridReport({
            Report: [
                {
                    Title: 'YTD Sales',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 2,
                    Rows: [
                        {
                            Label: 'Actual Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(vRows.YTD.Rows[0].GrossSales),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'LY Same Date Sales'
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearMTD.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Growth',
                        },
                        {
                            Label: AccountingFormat((CDec2(vRows.YTD.Rows[0].GrossSales) - CDec2(vRows.LastYearYTD.Rows[0].GrossSales)) / CDec2(vRows.YTD.Rows[0].GrossSales) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Average Daily Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRows.YTD.Rows[0].GrossSales) / CDec2(vRows.YTD.Rows[0].StoreCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'No. of Stores',
                        },
                        {
                            Label: AccountingFormat(vRows.YTD.Rows[0].StoreCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'SSSG',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: '-',
                            ClassName: 'font-weight-bold text-right'
                        }
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //YTD TC
        GridReport({
            Report: [
                {
                    Title: 'YTD TC',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 2,
                    Rows: [
                        {
                            Label: 'Actual TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(vRows.YTD.Rows[0].TransactionCount),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'LY Same Date TC'
                        },
                        {
                            Label: AccountingFormat(vRows.LastYearYTD.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Growth',
                        },
                        {
                            Label: AccountingFormat((CDec2(vRows.YTD.Rows[0].TransactionCount) - CDec2(vRows.LastYearYTD.Rows[0].TransactionCount)) / CDec2(vRows.YTD.Rows[0].TransactionCount) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Average Daily TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRows.YTD.Rows[0].TransactionCount) / CDec2(vRows.YTD.Rows[0].StoreCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: 'No. of Stores',
                        },
                        {
                            Label: AccountingFormat(vRows.YTD.Rows[0].StoreCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Same Store TC Growth',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: '-',
                            ClassName: 'font-weight-bold text-right'
                        }
                    ]
                }
            ],
            Click: function (e) {

            }
        })


        //YTD Sales Mix
        GridReport({
            Report: [
                {
                    Title: 'YTD Sales Mix',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 3,
                    Rows: [
                        {
                            Label: 'Take-out Sales',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YTDTakeOut.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].GrossSales) / (CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.YTDDelivery.Rows[0].GrossSales)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Delivery Sales',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YTDDelivery.Rows[0].GrossSales),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YTDDelivery.Rows[0].GrossSales) / (CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.YTDDelivery.Rows[0].GrossSales)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Dine-in Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Drive Thru Sales',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Total Sales',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].GrossSales) + CDec2(vRowsSalesMix.YTDDelivery.Rows[0].GrossSales)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: '100%',
                            ClassName: 'font-weight-bold text-right'
                        },
                    ]
                }
            ],
            Click: function (e) {

            }
        })

        //YTD Sales Mix
        GridReport({
            Report: [
                {
                    Title: 'YTD TC Mix',
                    ClassName: 'bg-success',
                    NoOfColPerRow: 3,
                    Rows: [
                        {
                            Label: 'Take-out TC',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YTDTakeOut.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].TransactionCount) / (CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.YTDDelivery.Rows[0].TransactionCount)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Delivery TC',
                        },
                        {
                            Label: AccountingFormat(vRowsSalesMix.YTDDelivery.Rows[0].TransactionCount),
                            ClassName: 'text-right'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YTDDelivery.Rows[0].TransactionCount) / (CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.YTDDelivery.Rows[0].TransactionCount)) * 100) + '%',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Dine-in TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Drive Thru TC',
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: '-',
                            ClassName: 'text-right'
                        },
                        {
                            Label: 'Total TC',
                            ClassName: 'font-weight-bold'
                        },
                        {
                            Label: AccountingFormat(CDec2(vRowsSalesMix.YTDTakeOut.Rows[0].TransactionCount) + CDec2(vRowsSalesMix.YTDDelivery.Rows[0].TransactionCount)),
                            ClassName: 'font-weight-bold text-right'
                        },
                        {
                            Label: '100%',
                            ClassName: 'font-weight-bold text-right'
                        },
                    ]
                }
            ],
            Click: function (e) {

            }
        })

    }
})
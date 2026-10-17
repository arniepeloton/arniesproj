require([], function () {

    var pData = {
        "Header": {
            "SalesOrderNo": "2",
            "SalesOrderDate": "2016-04-25 13:09:18.477",
            "DeliveryDate": "Apr 25, 2016",
            "Remarks": "This is the remarks of this invoice. ",
            "BillingAddress": "Debar Printing Press (at the back of Teccaro School), DELIVER TO DEBAR PRTG. PRESS, Davao City, 8000, Philippines",
            "DeliveryAddress": "Immaculate Printing Press, near Davao Tag Enterprises, Davao City, 8000, Philippines",
            "OrgName": "Davao Unified Corporation",
            "OrgAddress": "Sta. Ana Ave., Davao City, 8000, Philippines",
            "CustomerOrgName": "4BC INNOVATIVE IDEAS",
            "CustomerOrgAddress": "No. 14, Macademia, Calamba, 4027, Cocos (Keeling) Islands",
            "TotalAmount": "927,631.00"
        },
        "Products": [
            {
                "ID": "180f7095-3ac7-4d95-ba28-425dd4f5fc49",
                "ProductNo": "2017063",
                "ProductName": ", BLADE, Spare (Cutter Refill), Small 804B",
                "OrderedQuantity": "54",
                "Quantity": "0",
                "UnitMeasure": "Case",
                "UnitPrice": "6,450.00",
                "SubTotal": "348,300.00",
                "ActualQuantity": "600",
                "ProductID": "180f7095-3ac7-4d95-ba28-425dd4f5fc49",
                "InvoiceID": "25f1e106-b336-4411-a4ed-102891b30c77",
                "LineNumber": "5",
                "PriceLevelID": "01000012-0005-0000-0000-000000000000",
                "ItemID": "f374ced3-433e-4aa4-8173-e1b4338ea519"
            },
            {
                "ID": "180f7095-3ac7-4d95-ba28-425dd4f5fc49",
                "ProductNo": "2017063",
                "ProductName": ", BLADE, Spare (Cutter Refill), Small 804B",
                "OrderedQuantity": "32",
                "Quantity": "0",
                "UnitMeasure": "Pack",
                "UnitPrice": "53.75",
                "SubTotal": "1,720.00",
                "ActualQuantity": "5",
                "ProductID": "180f7095-3ac7-4d95-ba28-425dd4f5fc49",
                "InvoiceID": "25f1e106-b336-4411-a4ed-102891b30c77",
                "LineNumber": "4",
                "PriceLevelID": "01000012-0005-0000-0000-000000000000",
                "ItemID": "00b673d1-9c64-4e31-a259-debaf2e57154"
            },
            {
                "ID": "180f7095-3ac7-4d95-ba28-425dd4f5fc49",
                "ProductNo": "2017063",
                "ProductName": ", BLADE, Spare (Cutter Refill), Small 804B",
                "OrderedQuantity": "13",
                "Quantity": "0",
                "UnitMeasure": "Tube",
                "UnitPrice": "10.75",
                "SubTotal": "139.75",
                "ActualQuantity": "1",
                "ProductID": "180f7095-3ac7-4d95-ba28-425dd4f5fc49",
                "InvoiceID": "25f1e106-b336-4411-a4ed-102891b30c77",
                "LineNumber": "3",
                "PriceLevelID": "01000012-0005-0000-0000-000000000000",
                "ItemID": "9694adbf-ed90-4238-999f-d372c1dd19f1"
            },
            {
                "ID": "218d4f8b-3f94-45bb-bba6-a367d883f838",
                "ProductNo": "4229001",
                "ProductName": ", CASE, Pencil Metal Assorted Princess #B-5003",
                "OrderedQuantity": "213",
                "Quantity": "0",
                "UnitMeasure": "PC",
                "UnitPrice": "25.50",
                "SubTotal": "5,431.50",
                "ActualQuantity": "1",
                "ProductID": "218d4f8b-3f94-45bb-bba6-a367d883f838",
                "InvoiceID": "25f1e106-b336-4411-a4ed-102891b30c77",
                "LineNumber": "2",
                "PriceLevelID": "01000012-0005-0000-0000-000000000000",
                "ItemID": "caa298fe-a60f-467c-aa9a-29fb301e7dbe"
            },
            {
                "ID": "ed91c213-e7f4-471c-a80a-177c486d68a2",
                "ProductNo": "4033141",
                "ProductName": ", ART PAPER, Single Color (repack), 8.5x11 20's",
                "OrderedQuantity": "53,213",
                "Quantity": "0",
                "UnitMeasure": "Pack",
                "UnitPrice": "10.75",
                "SubTotal": "572,040.00",
                "ActualQuantity": "1",
                "ProductID": "ed91c213-e7f4-471c-a80a-177c486d68a2",
                "InvoiceID": "25f1e106-b336-4411-a4ed-102891b30c77",
                "LineNumber": "1",
                "PriceLevelID": "01000012-0005-0000-0000-000000000000",
                "ItemID": "b7937128-8e95-4e9a-8c33-00fbfc888b60"
            }
        ]
    };
    Android.CreatePdf('SalesOrder', pData);

}); // require(['common'
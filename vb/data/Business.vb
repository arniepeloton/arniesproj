Public Class Business

    Public WebRequest As WebRequest

    ReadOnly mProData As ProData
    ReadOnly mTransInvoice As TransInvoice
    ReadOnly mTransInvoiceLegacy As TransInvoiceLegacy
    ReadOnly mLibraryStock As LibraryStock
    ReadOnly mLibraryStockLegacy As LibraryStockLegacy
    ReadOnly mSystemDefaults As SystemDefaults
    ReadOnly mLibraryBranches As LibraryBranches
    ReadOnly mLibraryCustomers As LibraryCustomers
    ReadOnly mEmployeeProfiles As EmployeeProfiles
    ReadOnly mTransDrp As TransDrp
    ReadOnly mTransStockPurchase As TransStockPurchase
    ReadOnly mLibraryShippers As LibraryShippers
    ReadOnly mLibrarySuppliers As LibrarySuppliers
    ReadOnly mTransSalesOrder As TransSalesOrder
    ReadOnly mTransDeliveryLogger As TransDeliveryLogger
    ReadOnly mLibraryStockMeasurements As LibraryStockMeasurements
    ReadOnly mTransQr As TransQr
    ReadOnly mTransPlanogram As TransPlanogram
    ReadOnly mTransStockReturnItems As TransStockReturnItems
    ReadOnly mTransSalesReturnLedger As TransSalesReturnLedger
    ReadOnly mTransMemoSalesReturn As TransMemoSalesReturn
    ReadOnly mTransInventoryLedger As TransInventoryLedger
    ReadOnly mLibraryOrganization As LibraryOrganization
    ReadOnly mLibraryPeople As LibraryPeople
    ReadOnly mLibraryDevice As LibraryDevice
    ReadOnly mTransReceiving As TransReceiving
    ReadOnly mTransBomMaker As TransBomMaker
    ReadOnly mTransDevAlerts As TransDevAlerts
    ReadOnly mLibraryItemModifiers As LibraryItemModifiers
    ReadOnly mUtilities As Utilities
    ReadOnly mMainSecurityLegacy As MainSecurityLegacy
    ReadOnly mTransSalesOrderLegacy As TransSalesOrderLegacy
    Dim mLibraryOrganizationFOWST As LibraryOrganizationFOWST
    ReadOnly mTransBackOrders As TransBackOrders
    ReadOnly mLibraryPrinters As LibraryPrinters
    ReadOnly mTransCustomerPayment As TransCustomerPayment
    ReadOnly mDataCleaner As DataCleaner
    ReadOnly mLibraryDocumentControl As LibraryDocumentControl
    ReadOnly mLibraryAccountAdjustments As LibraryAccountAdjustments
    ReadOnly mLibraryDeviceFowst As LibraryDeviceFowst
    ReadOnly mLibraryPriceCanvass As LibraryPriceCanvass
    ReadOnly mTransReceivingPayment As TransReceivingPayment
    ReadOnly mLibraryRepeatingTransactions As LibraryRepeatingTransactions
    ReadOnly mLibraryTelcoPrefixes As LibraryTelcoPrefixes
    ReadOnly mLibraryPIMFeature As LibraryPIMFeature
    ReadOnly mTransFlushingReport As TransFlushingReport
    ReadOnly mCacheInventoryMovement As CacheInventoryMovement
    ReadOnly mLibraryProductMap As LibraryProductMap
    ReadOnly mTransUnflushedCounts As TransUnflushedCounts
    ReadOnly mCacheSalesTables As CacheSalesTables
    ReadOnly mTransESales As TransESales
    ReadOnly mCacheSalesTablesStarter As CacheSalesTablesStarter
    ReadOnly mCacheSalesTablesSender As CacheSalesTablesSender
    'ReadOnly mFranchiseeDataComparer As FranchiseeDataComparer
    Public EnableExtraLogs As Boolean
    Dim mOrgID As String

    Sub New(ByRef rProData As ProData)

        mProData = rProData
        mTransInvoice = New TransInvoice(mProData, Me)
        mLibraryStock = New LibraryStock(mProData, Me)
        mSystemDefaults = New SystemDefaults(mProData, Me)
        mLibraryBranches = New LibraryBranches(mProData, Me)
        mLibraryCustomers = New LibraryCustomers(mProData, Me)
        mEmployeeProfiles = New EmployeeProfiles(mProData, Me)
        mTransDrp = New TransDrp(mProData, Me)
        mTransStockPurchase = New TransStockPurchase(mProData, Me)
        mLibraryShippers = New LibraryShippers(mProData, Me)
        mLibrarySuppliers = New LibrarySuppliers(mProData, Me)
        mTransSalesOrder = New TransSalesOrder(mProData, Me)
        mTransDeliveryLogger = New TransDeliveryLogger(mProData, Me)
        mTransQr = New TransQr(mProData, Me)
        mLibraryStockMeasurements = New LibraryStockMeasurements(mProData, Me)
        mTransPlanogram = New TransPlanogram(mProData, Me)
        mTransStockReturnItems = New TransStockReturnItems(mProData, Me)
        mTransSalesReturnLedger = New TransSalesReturnLedger(mProData, Me)
        mTransMemoSalesReturn = New TransMemoSalesReturn(mProData, Me)
        mTransInventoryLedger = New TransInventoryLedger(mProData, Me)
        mLibraryOrganization = New LibraryOrganization(mProData, Me)
        mLibraryPeople = New LibraryPeople(mProData, Me)
        mLibraryDevice = New LibraryDevice(mProData, Me)
        mLibraryStockLegacy = New LibraryStockLegacy(mProData, Me)
        mTransInvoiceLegacy = New TransInvoiceLegacy(mProData, Me)
        mTransReceiving = New TransReceiving(mProData, Me)
        mTransBomMaker = New TransBomMaker(mProData, Me)
        mTransDevAlerts = New TransDevAlerts(mProData, Me)
        mLibraryItemModifiers = New LibraryItemModifiers(mProData, Me)
        mUtilities = New Utilities(mProData, Me)
        mMainSecurityLegacy = New MainSecurityLegacy(mProData, Me)
        mTransSalesOrderLegacy = New TransSalesOrderLegacy(mProData, Me)
        mLibraryOrganizationFOWST = New LibraryOrganizationFOWST(mProData, Me)
        mTransBackOrders = New TransBackOrders(mProData, Me)
        mLibraryPrinters = New LibraryPrinters(mProData, Me)
        mTransCustomerPayment = New TransCustomerPayment(mProData, Me)
        mDataCleaner = New DataCleaner(mProData, Me)
        mLibraryDocumentControl = New LibraryDocumentControl(mProData, Me)
        mLibraryAccountAdjustments = New LibraryAccountAdjustments(mProData, Me)
        mLibraryDeviceFowst = New LibraryDeviceFowst(mProData, Me)
        mLibraryPriceCanvass = New LibraryPriceCanvass(mProData, Me)
        mTransReceivingPayment = New TransReceivingPayment(mProData, Me)
        mLibraryRepeatingTransactions = New LibraryRepeatingTransactions(mProData, Me)
        mLibraryTelcoPrefixes = New LibraryTelcoPrefixes(mProData, Me)
        mLibraryPIMFeature = New LibraryPIMFeature(mProData, Me)
        mTransFlushingReport = New TransFlushingReport(mProData, Me)
        mCacheInventoryMovement = New CacheInventoryMovement(mProData, Me)
        mLibraryProductMap = New LibraryProductMap(mProData, Me)
        mTransUnflushedCounts = New TransUnflushedCounts(mProData, Me)
        mCacheSalesTables = New CacheSalesTables(mProData, Me)
        mTransESales = New TransESales(mProData, Me)
        mCacheSalesTablesStarter = New CacheSalesTablesStarter(mProData, Me)
        mCacheSalesTablesSender = New CacheSalesTablesSender(mProData, Me)
        'mFranchiseeDataComparer = New FranchiseeDataComparer(mProData, Me)

    End Sub

    Public ReadOnly Property ProData() As ProData
        Get

            Return mProData

        End Get
    End Property

    Public Function InterpretDependecies(ByVal vTableName As String, ByVal vID As String) As String

        Dim pDR As DataRow

        For Each pDR In mProData.GetDependencies(vTableName, vID).Rows
            Select Case pDR("TableName")
                Case Else
                    Return "At least 1 record in " & pDR("TableName") & " depends on this record"

            End Select
        Next pDR

    End Function

#Region " Business classes "
    Public ReadOnly Property TransInvoice() As TransInvoice
        Get
            Return mTransInvoice
        End Get
    End Property

    Public ReadOnly Property LibraryStock() As LibraryStock
        Get
            Return mLibraryStock
        End Get
    End Property

    Public ReadOnly Property SystemDefaults() As SystemDefaults
        Get
            Return mSystemDefaults
        End Get

    End Property

    Public ReadOnly Property LibraryBranches() As LibraryBranches
        Get
            Return mLibraryBranches
        End Get
    End Property

    Public ReadOnly Property LibraryCustomers() As LibraryCustomers
        Get

            Return mLibraryCustomers

        End Get
    End Property

    Public Function EmployeeProfiles() As EmployeeProfiles

        Return mEmployeeProfiles

    End Function

    Public Function TransDrp() As TransDrp

        Return mTransDrp

    End Function

    Public Function TransStockPurchase() As TransStockPurchase

        Return mTransStockPurchase

    End Function

    Public Function LibraryShippers() As LibraryShippers

        Return mLibraryShippers

    End Function

    Public Function LibrarySuppliers() As LibrarySuppliers

        Return mLibrarySuppliers

    End Function

    Public Function TransSalesOrder() As TransSalesOrder

        Return mTransSalesOrder

    End Function

    Public Function TransDeliveryLogger() As TransDeliveryLogger

        Return mTransDeliveryLogger

    End Function

    Public Function TransQr() As TransQr

        Return mTransQr

    End Function

    Public Function LibraryStockMeasurements() As LibraryStockMeasurements

        Return mLibraryStockMeasurements

    End Function

    Public Function TransPlanogram() As TransPlanogram
        Return mTransPlanogram
    End Function

    Public ReadOnly Property TransStockReturnItems As TransStockReturnItems
        Get

            Return mTransStockReturnItems

        End Get
    End Property

    Public ReadOnly Property TransSalesReturnLedger As TransSalesReturnLedger
        Get

            Return mTransSalesReturnLedger

        End Get
    End Property

    Public ReadOnly Property TransMemoSalesReturn As TransMemoSalesReturn
        Get

            Return mTransMemoSalesReturn

        End Get
    End Property

    Public ReadOnly Property TransInventoryLedger As TransInventoryLedger
        Get

            Return mTransInventoryLedger

        End Get
    End Property

    Public ReadOnly Property LibraryOrganization As LibraryOrganization
        Get

            Return mLibraryOrganization

        End Get
    End Property

    Public ReadOnly Property LibraryPeople As LibraryPeople
        Get

            Return mLibraryPeople

        End Get
    End Property

    Public ReadOnly Property LibraryDevice As LibraryDevice
        Get

            Return mLibraryDevice

        End Get
    End Property

    Public ReadOnly Property LibraryStockLegacy As LibraryStockLegacy
        Get

            Return mLibraryStockLegacy

        End Get
    End Property

    Public ReadOnly Property TransInvoiceLegacy As TransInvoiceLegacy
        Get

            Return mTransInvoiceLegacy

        End Get
    End Property

    Public ReadOnly Property TransReceiving As TransReceiving
        Get

            Return mTransReceiving

        End Get
    End Property

    Public ReadOnly Property TransBomMaker As TransBomMaker
        Get

            Return mTransBomMaker

        End Get
    End Property

    Public ReadOnly Property TransDevAlerts As TransDevAlerts
        Get

            Return mTransDevAlerts

        End Get
    End Property

    Public ReadOnly Property LibraryItemModifiers As LibraryItemModifiers
        Get

            Return mLibraryItemModifiers

        End Get
    End Property

    Public ReadOnly Property Utilities As Utilities
        Get

            Return mUtilities

        End Get
    End Property

    Public ReadOnly Property MainSecurityLegacy As MainSecurityLegacy
        Get

            Return mMainSecurityLegacy

        End Get
    End Property

    Public Property LibraryOrganizationFOWST As LibraryOrganizationFOWST
        Get

            Return mLibraryOrganizationFOWST

        End Get
        Set(value As LibraryOrganizationFOWST)

            mLibraryOrganizationFOWST = value

        End Set
    End Property

    Public Function TransSalesOrderLegacy() As TransSalesOrderLegacy

        Return mTransSalesOrderLegacy

    End Function

    Public ReadOnly Property TransBackOrders As TransBackOrders
        Get

            Return mTransBackOrders

        End Get
    End Property

    Public ReadOnly Property LibraryPrinters As LibraryPrinters
        Get

            Return mLibraryPrinters

        End Get
    End Property

    Public ReadOnly Property TransCustomerPayment As TransCustomerPayment
        Get

            Return mTransCustomerPayment

        End Get
    End Property

    Public ReadOnly Property DataCleaner As DataCleaner
        Get

            Return mDataCleaner

        End Get
    End Property

    Public ReadOnly Property LibraryDocumentControl As LibraryDocumentControl
        Get

            Return mLibraryDocumentControl

        End Get
    End Property

    Public ReadOnly Property LibraryAccountAdjustments As LibraryAccountAdjustments
        Get

            Return mLibraryAccountAdjustments

        End Get
    End Property

    Public ReadOnly Property LibraryDeviceFowst As LibraryDeviceFowst
        Get

            Return mLibraryDeviceFowst

        End Get
    End Property

    Public ReadOnly Property LibraryPriceCanvass As LibraryPriceCanvass
        Get

            Return mLibraryPriceCanvass

        End Get
    End Property

    Public ReadOnly Property TransReceivingPayment As TransReceivingPayment
        Get

            Return mTransReceivingPayment

        End Get
    End Property

    Public ReadOnly Property LibraryRepeatingTransactions As LibraryRepeatingTransactions
        Get

            Return mLibraryRepeatingTransactions

        End Get
    End Property

    Public ReadOnly Property LibraryTelcoPrefixes As LibraryTelcoPrefixes
        Get

            Return mLibraryTelcoPrefixes

        End Get
    End Property

    Public ReadOnly Property LibraryPIMFeature As LibraryPIMFeature
        Get

            Return mLibraryPIMFeature

        End Get
    End Property

    Public ReadOnly Property TransFlushingReport As TransFlushingReport
        Get

            Return mTransFlushingReport

        End Get
    End Property

    Public ReadOnly Property CacheInventoryMovement As CacheInventoryMovement
        Get

            Return mCacheInventoryMovement

        End Get
    End Property

    Public ReadOnly Property LibraryProductMap As LibraryProductMap
        Get

            Return mLibraryProductMap

        End Get
    End Property

    Public ReadOnly Property TransUnflushedCounts As TransUnflushedCounts
        Get

            Return mTransUnflushedCounts

        End Get
    End Property

    Public ReadOnly Property CacheSalesTables As CacheSalesTables
        Get

            Return mCacheSalesTables

        End Get
    End Property

    Public ReadOnly Property TransESales As TransESales
        Get

            Return mTransESales

        End Get
    End Property

    Public ReadOnly Property CacheSalesTablesStarter As CacheSalesTablesStarter
        Get

            Return mCacheSalesTablesStarter

        End Get
    End Property


    Public ReadOnly Property CacheSalesTablesSender As CacheSalesTablesSender
        Get

            Return mCacheSalesTablesSender

        End Get
    End Property

    'Public ReadOnly Property FranchiseeDataComparer As FranchiseeDataComparer
    '    Get

    '       Return mFranchiseeDataComparer

    '  End Get
    'End Property
#End Region

    Public Sub ExtraLog(vText As String)

        If EnableExtraLogs Then
            AppendToLog(vText, , "ExtraLog vbFHG86F6E3L!")
        End If

    End Sub

    ' this function should be in ProPack.InsertUpdate
    Public Shared Function DataRowToInsertUpdate(vDR As DataRow) As InsertUpdate

        Dim pIU As New InsertUpdate()

        For Each pDC As DataColumn In vDR.Table.Columns
            If pDC.ColumnName.StartsWith("Computed") Then
                Continue For
            End If
            If IsDBNull(vDR(pDC.ColumnName)) Then
                pIU(EscapeSqlName(pDC.ColumnName)) = "NULL"
            Else
                Select Case MajorDataType(pDC.DataType)
                    Case eMajorDataType.Numeric
                        pIU(EscapeSqlName(pDC.ColumnName)) = CDec2(vDR(pDC.ColumnName))

                    Case Else
                        pIU(EscapeSqlName(pDC.ColumnName)) = ReadyString(vDR(pDC.ColumnName))

                End Select
            End If
        Next pDC

        Return pIU

    End Function

    Public Property OrgID As String
        Get

            If String.IsNullOrWhiteSpace(mOrgID) Then
                Return WebRequest.Instance.OrgID
            Else
                Return mOrgID
            End If

        End Get
        Set(value As String)

            mOrgID = value

        End Set
    End Property

    Public Sub StopWatch(vText As String)

        If WebRequest IsNot Nothing Then
            WebRequest.StopWatch(vText)
        Else
            ' just do nothing here
        End If

    End Sub
End Class

Public Class BusinessData
    Public Sub New(ByVal vProData As ProData)

    End Sub
End Class
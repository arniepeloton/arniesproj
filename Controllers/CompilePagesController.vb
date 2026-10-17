Public Class CompilePagesController
    Inherits System.Web.Mvc.Controller

    '"D:\PhoneGapMobiz\workspace\Mobiz\assets\www\"
    'Private Const mOutputFolder As String = "C:\dev\compiled pages\www\" ' must end in backslash
    Private Const mBaseOutputFolder As String = "dev\debug\Mobiz\AndroidStudioJava\mobiz\src\main\assets\www\" ' must end in backslash. must not start in backslash
    Private Shared mOutputFolder As String ' must end in backslash
    Private Shared mMessages As Dictionary(Of UInteger, String)
    Private Shared mUrlPrefix As String
    Private Shared mDone As Boolean
    Private Shared mExcludeControllers As List(Of String)
    Private Shared mExlcudeControllerViews As List(Of String)
    Private Shared mSessionKey As String

    Function Index() As ActionResult

        If Request.IsLocal Then
            Return View()
        Else
            Return Content("This page can only be viewed locally.")
        End If

    End Function
    
    <HttpPost()>
    Public Function GetStatus() As String

        Dim pOut As JsonDictionary
        Dim pLastTick As UInteger
        Dim pMessages As List(Of String)
        Dim pSessionKey As String
        Dim pRan As Boolean

        If mUrlPrefix Is Nothing Then
            mUrlPrefix = Request.Url.Scheme & Uri.SchemeDelimiter & Request.Url.Authority
            mUrlPrefix = AppendIfNeeded(mUrlPrefix, "/")
        End If

        pOut = New JsonDictionary
        pSessionKey = Common.WebRequest.Data("SessionKey")
        If (mSessionKey IsNot Nothing) _
            AndAlso CStr2(pSessionKey) <> CStr2(mSessionKey) Then

            Return JsonDictionary.Serialize("Error", "Invalid session key. Current session key is " & mSessionKey)
        End If

        If mOutputFolder Is Nothing Then
            mOutputFolder = IO.Path.GetPathRoot(Common.ServerPath)
            mOutputFolder = IO.Path.Combine(mOutputFolder, mBaseOutputFolder)
            mOutputFolder = AppendBackslash(mOutputFolder)
        End If

        pOut.Add("OutputFolder", mOutputFolder)
        If CDec2(Common.WebRequest.Data("Compile")) <> 0 Then
            pRan = RunThread()
            If pRan Then
                mSessionKey = pSessionKey
            End If
            pOut.Add("Triggered", If(pRan, 1, 0))
        End If

        If CStr2(Common.WebRequest.Data("LastTick")) <> "" Then
            If UInteger.TryParse(Common.WebRequest.Data("LastTick"), pLastTick) = False Then
                Return JsonDictionary.Serialize("Error", "LastTick must be a valid integer")
            End If
        End If
        pMessages = GetMessage(pLastTick)
        If pMessages IsNot Nothing Then
            pOut.Add("Messages", pMessages)
        End If
        pOut.Add("LastTick", pLastTick)
        If mDone Then
            pOut.Add("Done", 1)
        End If
        If mSessionKey IsNot Nothing Then
            pOut.Add("SessionKey", mSessionKey)
        End If
        If CBool2(Common.WebRequest.Data("UseToString")) Then
            Return pOut.ToString()
        Else
            Return pOut.Serialize
        End If

    End Function

    Private Shared Function GetMessage(ByRef rLastKey As UInteger) As List(Of String)

        Dim pOut As List(Of String)
        Dim pLastKey As UInteger = rLastKey ''ByRef' parameter 'rLastKey' cannot be used in a query expression.

        If mMessages Is Nothing _
            OrElse mMessages.Count = 0 Then

            Return Nothing
        End If

        Dim p2 =
            From pItem As KeyValuePair(Of UInteger, String) In mMessages
            Select pItem
            Where pItem.Key > pLastKey
            Order By pItem.Key

        Dim p3 As IEnumerable(Of KeyValuePair(Of UInteger, String)) = CType(p2, IEnumerable(Of KeyValuePair(Of UInteger, String)))
        pOut = New List(Of String)
        For Each pItem As KeyValuePair(Of UInteger, String) In p2
            pOut.Add(pItem.Value)
            rLastKey = pItem.Key
        Next pItem

        Return pOut

    End Function

    Private Shared Function RunThread() As Boolean

        Static sSL As New Threading.SpinLock

        If sSL.IsHeld Then Return False ' app is already running

        TimedThread( _
            Sub()
                Try
                    If sSL.IsHeld Then Exit Sub
                    sSL.Enter(False)

                    mMessages = New Dictionary(Of UInteger, String)
                    CompilePages()
                Catch ex As Exception
                    AddLog("Error: " & ex.Message & vbCrLf & GetStackTrace(, ex))

                Finally
                    If sSL.IsHeldByCurrentThread Then
                        Try
                            'If mBusiness IsNot Nothing Then
                            '    mBusiness.ProData.Disconnect()
                            'End If
                        Catch ex As Exception
                            Stop2() ' just ignore this
                        End Try
                        sSL.Exit()
                    End If
                End Try
            End Sub)

        Return True

    End Function

    Private Shared Sub AddLog(vMessage As String)

        Dim pKey As UInteger

        pKey = TickCount()
        Do
            If mMessages.ContainsKey(pKey) Then
                pKey += 1
            Else
                Exit Do
            End If
        Loop

        mMessages.Add(pKey, "[" & Format(Now, "M/d/yyyy h:mm:ss tt") & "] " & vMessage)

    End Sub
    
    Private Shared Function CompilePages() As String

        Dim pFolders As List(Of String)
        Dim pFiles As List(Of String)
        Dim pPages As List(Of String)

        mDone = False
        AddLog("Start")
        '// list your folders here. entire folders (and subfolders) will be copied to output folder
        '   file path format
        pFolders = New List(Of String)
        With pFolders
            .Add("Content")
            .Add("Scripts3")
            .Add("Scripts")
        End With

        '   file path format
        pFiles = New List(Of String)
        With pFiles
            '.Add("Scripts\jQuery.js")
        End With

        mExcludeControllers = New List(Of String)
        mExcludeControllers.Add("triggers")
        mExcludeControllers.Add("AndroidJsTest")
        mExcludeControllers.Add("HackTest")
        mExcludeControllers.Add("CompilePages")
        mExcludeControllers.Add("FixNetDue")
        mExcludeControllers.Add("test")
        mExcludeControllers.Add("DbConnect")
        mExcludeControllers.Add("DemandData")
        mExcludeControllers.Add("Planogram")
        mExcludeControllers.Add("SqliteAndroid")
        'mExcludeControllers.Add("")

        mExlcudeControllerViews = New List(Of String)
        mExlcudeControllerViews.Add("CRMSetup/checkIfCustomerExist")
        mExlcudeControllerViews.Add("Subscription/CheckForSubscriber")
        'mExlcudeControllerViews.Add("")

        '// add your controllers and actions here
        '   url format
        pPages = GetControllersAndViews()

        'AddUnique(pPages, "SalesMenu")
        'AddUnique(pPages, "SalesMenu/HotButtons")
        'AddUnique(pPages, "Crm")
        'AddUnique(pPages, "CustomerSearch")
        'AddUnique(pPages, "DeliveryInfo")
        'AddUnique(pPages, "DeliveryInfo/EmailPriceQuote")
        'AddUnique(pPages, "device")
        'AddUnique(pPages, "device/DeviceInfo")
        'AddUnique(pPages, "device/DeviceInfoSummary")
        'AddUnique(pPages, "device/DeviceFowst")
        'AddUnique(pPages, "Login3")
        'AddUnique(pPages, "MallLessorAyala")
        'AddUnique(pPages, "Organization")
        'AddUnique(pPages, "Organization/OrganizationAddress")
        'AddUnique(pPages, "Organization/OrganizationAffiliation")
        'AddUnique(pPages, "Organization/OrganizationBrands")
        'AddUnique(pPages, "Organization/OrganizationContact")
        'AddUnique(pPages, "Organization/OrganizationCustomer")
        'AddUnique(pPages, "Organization/OrganizationCustomerFOWST")
        'AddUnique(pPages, "Organization/OrganizationCustomerFOWSTProductList")
        'AddUnique(pPages, "Organization/OrganizationCustomerProduct")
        'AddUnique(pPages, "Organization/OrganizationEmployee")
        'AddUnique(pPages, "Organization/OrganizationEmployeeAssignWorkSchedule")
        'AddUnique(pPages, "Organization/OrganizationEmployeeWorkingHours")
        'AddUnique(pPages, "Organization/OrganizationFinancialReport")
        'AddUnique(pPages, "Organization/OrganizationFOWSTProduct")
        'AddUnique(pPages, "Organization/OrganizationFOWSTProductList")
        'AddUnique(pPages, "Organization/OrganizationInfo")
        'AddUnique(pPages, "Organization/OrganizationProduct")
        'AddUnique(pPages, "Organization/OrganizationOwner")
        'AddUnique(pPages, "Organization/OrganizationRepresentative")
        'AddUnique(pPages, "Organization/OrganizationSummary")
        'AddUnique(pPages, "Organization/OrganizationSupplier")
        'AddUnique(pPages, "OrganizationShort")
        'AddUnique(pPages, "OrganizationShort/OrganizationAddress")
        'AddUnique(pPages, "OrganizationShort/OrganizationInfo")
        'AddUnique(pPages, "OrganizationShort/OrganizationRepresentative")
        'AddUnique(pPages, "OrganizationShort/OrganizationSummary")
        'AddUnique(pPages, "People")
        'AddUnique(pPages, "People/PeopleContactNo")
        'AddUnique(pPages, "People/PeopleEducation")
        'AddUnique(pPages, "People/PeopleEmpHistory")
        'AddUnique(pPages, "People/PeopleInfoAddress")
        'AddUnique(pPages, "People/PeopleInfoAlias")
        'AddUnique(pPages, "People/PeopleInfoName")
        'AddUnique(pPages, "People/PeopleInfoSummary")
        'AddUnique(pPages, "People/PeopleMenu")
        'AddUnique(pPages, "People/PeoplePersonalInfo")
        'AddUnique(pPages, "People/PeoplePrimeIDReference")
        'AddUnique(pPages, "People/PeopleRelatedPerson")
        'AddUnique(pPages, "People/PeopleUserAccount")
        'AddUnique(pPages, "SalesPayments")
        'AddUnique(pPages, "SalesPayments/DuplicateReceipt")
        'AddUnique(pPages, "SalesPayments/EmailSalesReceipt")
        'AddUnique(pPages, "Stock")
        'AddUnique(pPages, "SalesReturns")
        'AddUnique(pPages, "SalesReturns/Stock")
        'AddUnique(pPages, "SalesReturns/Settings")
        'AddUnique(pPages, "SecurityRole")
        'AddUnique(pPages, "SecurityRole/SecurityRoleInfo")
        'AddUnique(pPages, "SecurityRole/SecurityRoleSummary")
        'AddUnique(pPages, "SecurityRole/SecurityRoleUsers")
        'AddUnique(pPages, "ReceiptSettings")
        'AddUnique(pPages, "ReceiptSettings/ReceiptSettingsInfo")
        'AddUnique(pPages, "ReceiptSettings/ReceiptSettingsMessage")
        'AddUnique(pPages, "ReceiptSettings/ReceiptSettingsSummary")
        'AddUnique(pPages, "ReportsPOS/SalesReport")
        'AddUnique(pPages, "ReportsPOS/SalesAdjustmentReport")
        'AddUnique(pPages, "ReportsPOS/ZReport")
        'AddUnique(pPages, "Welcome3")
        'AddUnique(pPages, "SignIn")
        'AddUnique(pPages, "DashBoardsAndReports")
        'AddUnique(pPages, "DashBoardsAndReports/POSTransactionDetail")
        'AddUnique(pPages, "DashBoardsAndReports/Settings")
        'AddUnique(pPages, "InventoryLedger")
        'AddUnique(pPages, "InventoryCount")
        'AddUnique(pPages, "Receiving")
        'AddUnique(pPages, "Browser")
        'AddUnique(pPages, "DeviceMDMConfig")
        'AddUnique(pPages, "DeviceDashboard")
        'AddUnique(pPages, "DevicePolicyAppoint")
        'AddUnique(pPages, "DeviceAppWhiteList")
        'AddUnique(pPages, "DeviceApproval")
        'AddUnique(pPages, "AccountManagement")
        'AddUnique(pPages, "FileNotFound")
        'AddUnique(pPages, "Signin/DownloadDatabase")
        'AddUnique(pPages, "AccountRecovery")
        'AddUnique(pPages, "AccountManagement")
        'AddUnique(pPages, "ImportantReminders")
        'AddUnique(pPages, "SignUp")
        'AddUnique(pPages, "Success")
        'AddUnique(pPages, "DiscountSettings")
        'AddUnique(pPages, "ProductGroup")
        'AddUnique(pPages, "ProductGroup/AssignFOWST")
        'AddUnique(pPages, "ProductGroup/MenuTemplate")
#If DEBUG Then
        AddUnique(pPages, "SqliteAndroid")
#End If

        Dim pWC As Net.WebClient
        Dim pSourceUrl As String
        Dim pDestinationFilename As String
        Dim pStr As String
        Dim pDT As DataTable

        'pOutputFolder = "D:\Mobiz\assets\www\"
        'pOutputFolder = "D:\PhoneGapMobiz\workspace\Mobiz\assets\www\"
        'pOutputFolder = "C:\PhoneGap\www\"
        '"C:\Users\arvin\workspace2\Mobiz\assets\www\" ' Common.ServerPath & "temp\html\"

        '// copy folders
        pWC = New Net.WebClient

        AddLog("Copy folders")
        For Each pStr In pFolders
            pDT = DirectoryToTable(Common.ServerPath & pStr, , {"*.scc", "PrintApplet.zip"}, , , , , True)
            For Each pDR As DataRow In pDT.Rows
                pDestinationFilename = mOutputFolder & GetRelativePath(Common.ServerPath, pDR("FullName"))
                pDestinationFilename = pDestinationFilename.ToLower
                CreateFolder(pDestinationFilename)

                'If pDestinationFilename.Contains(".js") Then
                '    pStr = pWC.DownloadString(pDR("FullName"))

                '    pStr = AssignLinks(pStr, pDestinationFilename)
                '    FileText(pDestinationFilename) = pStr
                'Else
                '    CopyFile(pDR("FullName"), pDestinationFilename)
                'End If

                CopyFile(pDR("FullName"), pDestinationFilename)
            Next pDR
        Next pStr

        '// copy files
        AddLog("Copy files")
        For Each pStr In pFiles
            pDestinationFilename = mOutputFolder & GetRelativePath(Common.ServerPath, pStr.ToLower)
            pDestinationFilename = pDestinationFilename.ToLower
            CreateFolder(pDestinationFilename)
            CopyFile(Common.ServerPath & pStr, pDestinationFilename)
        Next pStr

        '// copy rendered controllers and actions
        AddLog("Copy prerendered controllers and actions")
        pWC = New Net.WebClient
        For Each pStr In pPages
            AddLog("Caching " & pStr)
            pSourceUrl = mUrlPrefix & pStr
            pDestinationFilename = mOutputFolder & pStr.Replace("/", "\").ToLower & ".html"
            pDestinationFilename = pDestinationFilename.ToLower
            CreateFolder(pDestinationFilename)

            'pStr = pWC.DownloadString(pSourceUrl & "?IsMobile=1")
            'pStr = AssignLinks(pStr, pDestinationFilename)
            'FileText(pDestinationFilename) = pStr

            Try
                pWC.DownloadFile(pSourceUrl & "?IsMobile=1", pDestinationFilename)
            Catch ex As Exception
                'Stop2()
            End Try
        Next pStr

        ''// custom
        'pStr = "Scripts3/common/init.js"
        'pDestinationFilename = mOutputFolder & pStr.Replace("/", "\").ToLower
        'pSourceUrl = pPrefix & pStr
        'pStr = pWC.DownloadString(pSourceUrl)
        ''pStr = pStr.Replace("server: ''", "server: 'http://philhr.com:8080'") ''http://192.168.2.101 http://philhr.com http://localhost:3002 http://192.168.1.80 
        ''pStr = pStr.Replace("server: ''", "server: " & ReadyString(Request.Url.Scheme & Request.Url.SchemeDelimiter & Request.Url.Authority))
        'FileText(pDestinationFilename) = pStr
        AddLog("Done")
        AddLog("Compiled to " & mOutputFolder)
        mDone = True

    End Function

    Private Shared Function AddUnique(ByRef rList As List(Of String), vValue As String) As Boolean

        Dim pLinq = From pItem As String In rList
            Select pItem
            Where CStr2(pItem).ToLower = CStr2(vValue).ToLower

        If DirectCast(pLinq, IEnumerable(Of String)).Count = 0 Then
            rList.Add(vValue)
            Return True
        End If

    End Function

    Private Shared Function GetControllersAndViews() As List(Of String)

        Dim pControllers As List(Of String)
        Dim pOut As List(Of String)
        Dim pItem As String
        Dim p As IEnumerable(Of String) ' linq

        pOut = New List(Of String)
        pControllers = GetControllers()
        For Each pController As String In pControllers
            For Each pView As String In GetViews(pController)
                pItem = pController
                If pView.Trim.ToLower <> "index" Then
                    pItem &= "/" & pView
                End If

                ' ReSharper disable AccessToModifiedClosure
                p = From pExcluded As String In mExlcudeControllerViews
                    Select pExcluded
                    Where pExcluded.Trim.ToLower = pItem.Trim.ToLower
                ' ReSharper restore AccessToModifiedClosure
                If p.Count = 0 Then
                    AddUnique(pOut, pItem)
                End If
            Next pView
        Next pController

        Return pOut

    End Function

    Public Shared Function GetControllers() As List(Of String)

        Dim pPath As String
        Dim pOut As List(Of String)
        Dim pControllerName As String
        Dim p As IEnumerable(Of String) ' linq

        pOut = New List(Of String)
        pPath = Common.ServerPath & "Controllers"
        For Each pFileInfo As IO.FileInfo In New IO.DirectoryInfo(pPath).GetFiles("*Controller.vb")
            pControllerName = pFileInfo.Name.Substring(0, pFileInfo.Name.Length - "Controller.vb".Length)
            If mExcludeControllers Is Nothing Then
                pOut.Add(pControllerName)
            Else
                ' ReSharper disable AccessToModifiedClosure
                p = From pExcluded As String In mExcludeControllers
                    Select pExcluded
                    Where pExcluded.Trim.ToLower = pControllerName.Trim.ToLower
                ' ReSharper restore AccessToModifiedClosure
                If p.Count = 0 Then
                    pOut.Add(pControllerName)
                End If
            End If
        Next pFileInfo

        Return pOut

    End Function

    Private Shared Function GetViews(vControllerName As String) As List(Of String)

        Dim pOut As List(Of String)
        Dim pDI As IO.DirectoryInfo
        Dim pPath As String

        pOut = New List(Of String)
        pDI = New IO.DirectoryInfo(Common.ServerPath & "Views\" & vControllerName)
        If pDI.Exists Then
            For Each pFileInfo As IO.FileInfo In pDI.GetFiles("*.vbhtml")
                pOut.Add(IO.Path.GetFileNameWithoutExtension(pFileInfo.Name))
            Next pFileInfo
        End If

        Return pOut

    End Function
End Class

Imports System.IO
Imports System.Runtime.Remoting.Metadata.W3cXsd2001
Imports System.Xml
Imports PdfSharp.Pdf.Content.Objects

Namespace WebProj
    Public Class VendingMachineController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function Draft() As ActionResult
            Return View()
        End Function

        Function PhysicalButtonSettings() As ActionResult
            Return View()
        End Function

        Function Guides() As ActionResult
            Return View()
        End Function

        Function ProductSelectionGuides() As ActionResult
            Return View()
        End Function

        Function GeneralSettings() As ActionResult
            Return View()
        End Function

        Function InAppSettings() As ActionResult
            Return View()
        End Function

        Function Ads() As ActionResult
            Return View()
        End Function

        Function SaveProductSelectionGuides()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim rowid As String = Request.Params.Get("ID").ToString()
                Dim IsMedia As Boolean = CBool2(Request.Params.Get("IsMedia"))
                Dim IsNew As Boolean = CBool2(Request.Params.Get("IsNew"))
                Dim IsRemove As Boolean = CBool2(Request.Params.Get("IsRemove"))
                Dim Media As String = Request.Params.Get("Media")
                Dim Page As String = Request.Params.Get("Page")
                Dim MediaRefID As String = Request.Params.Get("MediaRefID")
                Dim filename As String = "selectionguide_" & Page & "_" & rowid
                Dim p As New InsertUpdate("LibraryVDMProductSelectionGuides")

                p("Page") = ReadyString(Page)
                p("Media") = ReadyString(Media)
                'p("OrderNo") = ReadyString(Description)

                If IsRemove Then
                    Common.WebRequest.ProData.Execute("DELETE FROM LibraryVDMProductSelectionGuides WHERE ID = " & ReadyString(rowid))
                Else
                    Dim folderName As String = Common.ServerPath & "Content\VendingMachine\" & WebRequest.Instance.OrgID.ToLower()

                    If Not System.IO.Directory.Exists(folderName) Then
                        System.IO.Directory.CreateDirectory(folderName)
                    End If

                    If Request.Files.Count > 0 And IsNew Then
                        Dim f As HttpPostedFileBase = Request.Files(0)
                        p("FileName") = ReadyString(f.FileName)
                    End If

                    If Not String.IsNullOrEmpty(MediaRefID) Then
                        p("FileName") = ReadyString(Common.WebRequest.ProData.ExecuteScalar("SELECT FileName FROM LibraryVDMMedia WHERE ID = " & ReadyString(MediaRefID)))
                        p("MediaID") = ReadyString(MediaRefID)
                    End If

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(rowid))) = 0 Then
                        p("ID") = ReadyString(rowid)
                        p("UploadedOn") = "getdate()"
                        p("UploadedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                        If String.IsNullOrEmpty(MediaRefID) Then
                            p("MediaID") = ReadyString(SaveToVDMMediaFolder())
                        End If

                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If
                End If

                Return JsonDictionary.Serialize("Result", "success")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Result", ex.Message)
            End Try
        End Function

        Function SaveToVDMMediaFolder()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim CustomName As String = Request.Params.Get("CustomName")
            Dim Description As String = Request.Params.Get("Description")
            Dim Media As String = Request.Params.Get("Media")
            Dim IsNew As Boolean = CBool2(Request.Params.Get("IsNew"))
            Dim p As New InsertUpdate("LibraryVDMMedia")
            Dim mediaid As String = Guid.NewGuid.ToString()

            Dim folderName As String = Common.ServerPath & "Content\VendingMachine\" & WebRequest.Instance.OrgID.ToLower()

            If Not System.IO.Directory.Exists(folderName) Then
                System.IO.Directory.CreateDirectory(folderName)
            End If

            If Request.Files.Count > 0 And IsNew Then
                Dim f As HttpPostedFileBase = Request.Files(0)
                Dim pSourceURL As String = folderName & "\" & f.FileName
                f.SaveAs(pSourceURL)

                p("FileName") = ReadyString(f.FileName)
            End If

            p("Name") = ReadyString(CustomName)
            p("Description") = ReadyString(Description)
            p("Media") = ReadyString(Media)
            p("UploadedOn") = "getdate()"
            p("UploadedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("ID") = ReadyString(mediaid)

            Common.WebRequest.ProData.Execute(p.GetInsert())

            Return mediaid
        End Function

        Function GetGeneralSettings()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary
            Dim pSql As String

            pSql = " SELECT ads.*, media.Name as CustomName, media.Description FROM LibraryVDMProductSelectionGuides as ads " & vbCrLf &
                    " LEFT JOIN LibraryVDMMedia as media ON media.ID = ads.MediaID"

            pOut.Add("Guides", Common.WebRequest.ProData.Execute(pSql))

            pSql = " SELECT SystemDefaults.*, MobizLibraryPIMProduct.ProductNo, MobizLibraryPIMProduct.ProductName FROM SystemDefaults INNER JOIN MobizLibraryPIMProduct ON MobizLibraryPIMProduct.ID = SystemDefaults.DefaultValue WHERE Description LIKE '%VDM _%' "

            pOut.Add("Denom", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function

        Function SaveVDM()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryVendingMachineBoards") 'aka List of VMC Boards - changed as of Oct 27, 2025, giretain lang ang table name niya
                Dim boardid As String = ""

                'p("Type") = ReadyString(data("Type"))
                p("Name") = ReadyString(data("Name"))
                p("Description") = ReadyString(data("Description"))
                p("VCMModelID") = ReadyString(data("VCMModelID"))
                p("NoOfMotors") = IIf(CInt2(data("NoOfSpringCoilMotors")) = 0, "NULL", ReadyString(data("NoOfSpringCoilMotors")))
                p("NoOfDirectConnMotors") = IIf(CInt2(data("NoOfDirectConnMotors")) = 0, "NULL", ReadyString(data("NoOfDirectConnMotors")))
                p("UUID") = IIf(CStr2(data("UUID")) = "0", "NULL", ReadyString(data("UUID")))
                p("NoOfPhysicalSelectionButtons") = IIf(CInt2(data("NoOfPhysicalSelectionButtons")) = 0, "NULL", ReadyString(data("NoOfPhysicalSelectionButtons")))
                p("NoOfPhysicalNavigationButtons") = IIf(CInt2(data("NoOfPhysicalNavigationButtons")) = 0, "NULL", ReadyString(data("NoOfPhysicalNavigationButtons")))
                p("NoOfMotorActuators") = IIf(CInt2(data("NoOfMotorActuators")) = 0, "NULL", ReadyString(data("NoOfMotorActuators")))
                p("IsActive") = CInt2(data("IsActive"))

                If String.IsNullOrWhiteSpace(data("ID")) Then
                    boardid = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(boardid)
                    p("DateCreated") = "getdate()"
                    p("CreatedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    boardid = data("ID")
                    p("DateModified") = "getdate()"
                    p("ModifiedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(boardid)))
                End If

                Common.WebRequest.ProData.Execute("DELETE FROM LibraryVendingMachineBoardSensorAndDevice WHERE BoardID = " & ReadyString(boardid))

                Dim arrSensors As Object = data("Sensors")
                Dim arrDevices As Object = data("Devices")

                For Each sensor As Object In arrSensors
                    p = New InsertUpdate("LibraryVendingMachineBoardSensorAndDevice")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("BoardID") = ReadyString(boardid)
                    p("Type") = ReadyString(sensor("Type"))
                    p("Description") = ReadyString(sensor("Description"))
                    p("Qty") = ReadyString(sensor("Qty"))

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Next

                For Each devices As Object In arrDevices
                    p = New InsertUpdate("LibraryVendingMachineBoardSensorAndDevice")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("BoardID") = ReadyString(boardid)
                    p("Type") = ReadyString(devices("Type"))
                    p("Description") = ReadyString(devices("Description"))
                    p("Qty") = ReadyString(devices("Qty"))

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Next

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveVMWithListing()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryVendingMachines")
                Dim MachineID As String = CStr2(data("ID"))

                'p("Type") = ReadyString(data("Type"))
                p("Name") = ReadyString(data("Name"))
                p("NoOfBoards") = ReadyString(data("NoOfBoards"))
                p("BoardID") = ReadyString(data("MainBoardID"))
                p("LastUpdated") = "getdate()"
                p("ModifiedBy") = ReadyString(WebSession.Instance.PeopleUserID)

                Common.WebRequest.ProData.BeginTransaction()

                If String.IsNullOrWhiteSpace(MachineID) Then
                    MachineID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(MachineID)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(MachineID)))
                End If

                Common.WebRequest.ProData.Execute("DELETE FROM LibraryVendingMachineSubBoard WHERE MachineID = " & ReadyString(MachineID))
                Common.WebRequest.ProData.Execute("DELETE FROM LibraryVendingMachineBoardSensorAndDeviceLabelling WHERE MachineID = " & ReadyString(MachineID))

                For Each label As Object In data("MainBoardLabelling")
                    label("MachineID") = MachineID
                    SaveMachineSensorsAndDeviceLabelling(label)
                Next

                Dim ctrBoard As Integer = 2
                For Each board As Object In data("DaughterBoards")
                    p = New InsertUpdate("LibraryVendingMachineSubBoard")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("MachineID") = ReadyString(MachineID)
                    p("BoardID") = ReadyString(board("BoardID"))
                    p("BoardOrder") = ReadyString(ctrBoard)

                    ctrBoard += 1

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    For Each label As Object In board("Label")
                        label("MachineID") = MachineID
                        SaveMachineSensorsAndDeviceLabelling(label)
                    Next
                Next

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveMachineSensorsAndDeviceLabelling(ByVal label As Object)
            Dim p = New InsertUpdate("LibraryVendingMachineBoardSensorAndDeviceLabelling")

            p("ID") = ReadyString(Guid.NewGuid.ToString())
            p("MachineID") = ReadyString(label("MachineID"))
            p("BoardID") = ReadyString(label("BoardID"))
            p("SensorDeviceID") = ReadyString(label("SensorDeviceID"))
            p("SensorDeviceName") = ReadyString(label("SensorDeviceName"))
            p("Type") = ReadyString(label("Type"))
            p("Label") = ReadyString(label("Label"))
            p("SKUID") = IIf(String.IsNullOrEmpty(label("SKUID")), "NULL", ReadyString(label("SKUID")))
            p("SKUName") = ReadyString(label("SKUName"))
            p("Criticality") = ReadyString(label("Criticality"))
            Dim pSql As String = p.GetInsert()
            Common.WebRequest.ProData.Execute(pSql)
        End Function

        Function SaveVDMAuthorizer()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryVendingMachineAuthorizers")
                Dim hashPin As String = data("Pin") 'ProPack.Security.Hash.ConstantSaltHashedString(data("Pin"))
                Dim id As String = data("ID")
                'Dim isTugma As Boolean = False

                'If ProPack.Security.Hash.Verify(data("Pin"), hashPin) Then
                '    isTugma = True
                'End If

                Common.WebRequest.ProData.BeginTransaction()

                If Not String.IsNullOrWhiteSpace(data("Pin")) Then
                    If Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryVendingMachineAuthorizers WHERE Pin = " & ReadyString(hashPin) & " AND EmployeeID != " & ReadyString(data("EmployeeID"))).Rows.Count > 0 Then
                        Return JsonDictionary.Serialize("Validation", "The PIN is already taken.")
                    End If

                    p("Pin") = ReadyString(hashPin)
                End If

                p("EmployeeID") = ReadyString(data("EmployeeID"))
                p("UpdatedByID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("UpdatedOn") = "getdate()"

                If String.IsNullOrWhiteSpace(id) Then
                    id = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(id)

                    If Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryVendingMachineAuthorizers WHERE EmployeeID = " & ReadyString(data("EmployeeID"))).Rows.Count > 0 Then
                        Return JsonDictionary.Serialize("Validation", "The selected employee has already been added. Please select another employee.")
                    End If

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(id)))
                End If

                Common.WebRequest.ProData.Execute("DELETE FROM LibraryVendingMachineAuthorizersAccessRights WHERE VDMAuthorizerID = " & ReadyString(id))

                For Each ac As Object In data("AccessRights")
                    p = New InsertUpdate("LibraryVendingMachineAuthorizersAccessRights")

                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("VDMAuthorizerID") = ReadyString(id)
                    p("Description") = ReadyString(ac)

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Next ac

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryVendingMachineAuthorizers")

                Common.WebRequest.ProData.CommitTransaction()
                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetDaughterBoards()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String

            pSql = "SELECT MachineID, BoardID as ID, board.Name " & vbCrLf &
                    " FROM LibraryVendingMachineSubBoard as sub " & vbCrLf &
                    " INNER JOIN LibraryVendingMachineBoards as board ON board.ID = sub.BoardID " & vbCrLf &
                    " WHERE MachineID = " & ReadyString(Common.WebRequest.Data("MainBoardID")) & vbCrLf &
                    " ORDER BY BoardOrder "

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetVMMachines()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            Dim pSql As String = "SELECT machine.ID, machine.Name as MachineName, board.Name as MainBoardName FROM LibraryVendingMachines as machine " & vbCrLf &
                " INNER JOIN LibraryVendingMachineBoards as board ON board.ID = machine.BoardID " & vbCrLf

            pOut.Add("MainBoard", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function

        Function GetVMBoardInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            Dim pSql As String = "SELECT board.ID as BoardID, UUID as BoardUUID, board.Name as BoardName, ISNULL(NoOfMotors, 0) as NoOfSpringCoilMotors, ISNULL(NoOfDirectConnMotors, 0) as NoOfDirectConnMotors, 1 as BoardOrder FROM LibraryVendingMachines as machine " & vbCrLf &
                " INNER JOIN LibraryVendingMachineBoards as board ON machine.BoardID = board.ID " & vbCrLf &
                " WHERE machine.ID = " & ReadyString(Common.WebRequest.Data("ID"))

            pOut.Add("MainBoard", Common.WebRequest.ProData.ExecuteRow(pSql))

            pSql = "SELECT board.ID as BoardID, UUID as BoardUUID, board.Name as BoardName, ISNULL(NoOfMotors, 0) as NoOfSpringCoilMotors, ISNULL(NoOfDirectConnMotors, 0) as NoOfDirectConnMotors, BoardOrder FROM LibraryVendingMachineSubBoard as subboard " & vbCrLf &
                " INNER JOIN LibraryVendingMachines as machine ON subboard.MachineID = machine.ID " & vbCrLf &
                " INNER JOIN LibraryVendingMachineBoards as board ON board.ID = subboard.BoardID " & vbCrLf &
                " WHERE subboard.MachineID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf &
                " ORDER BY BoardOrder "

            pOut.Add("SubBoard", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function

        Function GetBoardSensorAndDevice()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT * FROM LibraryVendingMachineBoardSensorAndDevice WHERE BoardID = " & ReadyString(Common.WebRequest.Data("BoardID")) & vbCrLf &
                " ORDER BY Type, Description "

            Return JsonDictionary.Serialize("Result", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetBoardSensorAndDeviceLabelling()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT * FROM LibraryVendingMachineBoardSensorAndDeviceLabelling WHERE BoardID = " & ReadyString(Common.WebRequest.Data("BoardID")) & " AND MachineID = " & ReadyString(Common.WebRequest.Data("MachineID")) & vbCrLf &
                " ORDER BY Type, SensorDeviceName "

            Return JsonDictionary.Serialize("Result", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetAccessRights()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pOut As New JsonDictionary

            Dim pSql As String = "SELECT Description FROM LibraryVendingMachineAuthorizersAccessRights WHERE VDMAuthorizerID = " & ReadyString(Common.WebRequest.Data("ID"))

            pOut.Add("AccessRights", Common.WebRequest.ProData.Execute(pSql))

            pSql = " SELECT auth.ID, auth.EmployeeID, auth.UpdatedOn " & vbCrLf &
                    ", ISNULL(NULLIF(LTRIM(RTRIM(emp.Surname)) + ', ', ' '), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Firstname)) + ' ', ''), '') + ISNULL(NULLIF(LTRIM(RTRIM(emp.Middlename)) + '', ''), '')  AS [EmployeeName]" & vbCrLf &
                    " FROM LibraryVendingMachineAuthorizers auth " & vbCrLf &
                    " LEFT JOIN MobizLibraryPeople emp ON auth.EmployeeID = emp.ID " & vbCrLf &
                    " WHERE auth.ID = " & ReadyString(Common.WebRequest.Data("ID")) & vbCrLf

            Dim info As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)

            pOut.Add("Info", info)

            Return pOut.Serialize()
        End Function

        Function GetModelB()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT * FROM LibraryVendingMachineBoards WHERE Type = 'Model B'"

            Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function SavePhysicalButtons()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim TemplateID As String = Common.WebRequest.Data("TemplateID")
                Dim Name As String = Common.WebRequest.Data("Name")
                Dim Description As String = Common.WebRequest.Data("Description")
                Dim ModelID As String = Common.WebRequest.Data("ModelID")
                Dim Pages As Object = Common.WebRequest.AllData("Pages")
                Dim AssignedFowst As Object = Common.WebRequest.AllData("AssignedFowst")
                Dim pSql As String
                Dim p As New InsertUpdate("LibraryVDMPhysicalButtonTemplate")

                p("Name") = ReadyString(Name)
                p("Description") = ReadyString(Description)
                p("ModelID") = ReadyString(ModelID)
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                If String.IsNullOrWhiteSpace(TemplateID) Then
                    TemplateID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(TemplateID)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(TemplateID))
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                Dim pageorder As Integer = 1
                For Each page As Object In Pages
                    Dim pageid As String = Guid.NewGuid.ToString()
                    Dim buttonno As Integer = 1
                    If page.ContainsKey("PageID") Then
                        pageid = page("PageID")
                    End If

                    p = New InsertUpdate("LibraryVDMPhysicalButtonTemplatePages")

                    p("TemplateID") = ReadyString(TemplateID)
                    p("PageOrder") = pageorder
                    p("PageName") = ReadyString(page("PageName"))
                    p("PagePurpose") = ReadyString(page("PagePurpose"))
                    p("PageContent") = ReadyString(page("PageContent"))

                    If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(pageid))) = 0 Then
                        p("ID") = ReadyString(pageid)
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    End If

                    If page("PagePurpose") = "Product Selection" Then
                        For Each button As Object In page("SKUButtons")
                            Dim buttonid As String = Guid.NewGuid.ToString()

                            If page.ContainsKey("ButtonID") Then
                                buttonid = page("ButtonID")
                            End If

                            p = New InsertUpdate("LibraryVDMPhysicalButtonTemplatePageButtons")

                            p("PageID") = ReadyString(pageid)
                            p("ButtonNo") = buttonno
                            p("ButtonLabel") = ReadyString(button("ButtonLabel"))
                            p("ProductID") = ReadyString(button("ID"))

                            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(buttonid))) = 0 Then
                                p("ID") = ReadyString(buttonid)
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            End If

                            buttonno += 1
                        Next
                    Else
                        For Each button As Object In page("NavButtons")
                            Dim buttonid As String = Guid.NewGuid.ToString()

                            If page.ContainsKey("ButtonID") Then
                                buttonid = page("ButtonID")
                            End If

                            p = New InsertUpdate("LibraryVDMPhysicalButtonTemplatePageButtons")

                            p("PageID") = ReadyString(pageid)
                            p("ButtonNo") = buttonno
                            p("ButtonLabel") = ReadyString(button("ButtonLabel"))
                            p("Action") = ReadyString(button("Action"))
                            p("RedirectToPage") = ReadyString(button("RedirectToPage"))

                            If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(buttonid))) = 0 Then
                                p("ID") = ReadyString(buttonid)
                                Common.WebRequest.ProData.Execute(p.GetInsert())
                            End If

                            buttonno += 1
                        Next
                    End If

                    pageorder += 1
                Next

                For Each fowst As Object In AssignedFowst
                    p = New InsertUpdate("LibraryVDMPhysicalButtonTemplateAssignedFowst")

                    If fowst("State") <> "delete" Then
                        p("TemplateID") = ReadyString(TemplateID)
                        p("FowstID") = ReadyString(fowst("ID").ToString())

                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" TemplateID = " & ReadyString(TemplateID) & " AND FowstID = " & ReadyString(fowst("ID").ToString()))) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("LastUpdated") = "getdate()"

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    Else
                        pSql = "DELETE FROM LibraryVDMPhysicalButtonTemplateAssignedFowst WHERE TemplateID = " & ReadyString(TemplateID) & " AND FowstID = " & ReadyString(fowst("ID").ToString())
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetPhysicalButtonsTemplateInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim tempID As String = Common.WebRequest.Data("ID")
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim pDT As Object

                pSql = " SELECT * FROM LibraryVDMPhysicalButtonTemplate WHERE ID = " & ReadyString(tempID)
                pOut.Add("Info", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                pSql = " SELECT * FROM LibraryVDMPhysicalButtonTemplatePages WHERE TemplateID = " & ReadyString(tempID) & " ORDER BY PageOrder ASC "
                pDT = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                For Each bt As Object In pDT
                    If bt("PagePurpose") = "Product Selection" Then
                        pSql = " SELECT btn.ID, ButtonNo, ButtonLabel, ProductID, MobizLibraryPimProduct.ProductName " & vbCrLf &
                            " FROM LibraryVDMPhysicalButtonTemplatePageButtons as btn " & vbCrLf &
                            " INNER JOIN MobizLibraryPimProduct ON MobizLibraryPimProduct.ID = btn.ProductID " & vbCrLf &
                            " WHERE PageID = " & ReadyString(bt("ID").ToString()) &
                            " ORDER BY ButtonNo ASC "

                        bt.Add("SKUButtons", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                    Else
                        pSql = " SELECT btn.ID, ButtonNo, ButtonLabel, Action, RedirectToPage " & vbCrLf &
                            " FROM LibraryVDMPhysicalButtonTemplatePageButtons as btn " & vbCrLf &
                            " WHERE PageID = " & ReadyString(bt("ID").ToString()) &
                            " ORDER BY ButtonNo ASC "

                        bt.Add("NavButtons", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
                    End If
                Next

                pOut.Add("Pages", pDT)

                pSql = " SELECT MobizLibraryOrganizationFOWST.ID, Name " & vbCrLf &
                        " FROM LibraryVDMPhysicalButtonTemplateAssignedFowst " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryVDMPhysicalButtonTemplateAssignedFowst.FowstID " & vbCrLf &
                        " WHERE TemplateID = " & ReadyString(tempID) &
                        " ORDER BY Name ASC "
                pOut.Add("AssignedMachine", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetGuidesInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim tempID As String = Common.WebRequest.Data("ID")
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim pDT As Object

                pSql = " SELECT * FROM LibraryVDMGuidesTemplate WHERE ID = " & ReadyString(tempID)
                pOut.Add("Info", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                pSql = " SELECT * FROM LibraryVDMGuides WHERE GuideID = " & ReadyString(tempID) & " ORDER BY OrderNo ASC "
                pDT = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                pOut.Add("Steps", pDT)

                pSql = " SELECT MobizLibraryOrganizationFOWST.ID, Name, 'edit' as State " & vbCrLf &
                        " FROM LibraryVDMGuidesAssignedFowst " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryVDMGuidesAssignedFowst.FowstID " & vbCrLf &
                        " WHERE TemplateID = " & ReadyString(tempID) &
                        " ORDER BY Name ASC "
                pOut.Add("AssignedMachine", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetAdsInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim tempID As String = Common.WebRequest.Data("ID")
                Dim pSql As String
                Dim pOut As New JsonDictionary
                Dim pDT As Object

                pSql = " SELECT * FROM LibraryVDMAdsTemplate WHERE ID = " & ReadyString(tempID)
                pOut.Add("Info", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                pSql = " SELECT * FROM LibraryVDMAds WHERE TemplateID = " & ReadyString(tempID) & " ORDER BY OrderNo ASC "
                pDT = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

                pOut.Add("Ads", pDT)

                pSql = " SELECT MobizLibraryOrganizationFOWST.ID, Name, 'edit' as State " & vbCrLf &
                        " FROM LibraryVDMAdsAssignedFowst " & vbCrLf &
                        " INNER JOIN MobizLibraryOrganizationFOWST ON MobizLibraryOrganizationFOWST.ID = LibraryVDMAdsAssignedFowst.FowstID " & vbCrLf &
                        " WHERE TemplateID = " & ReadyString(tempID) &
                        " ORDER BY Name ASC "
                pOut.Add("AssignedMachine", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveGuides()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim TemplateID As String = Common.WebRequest.Data("TemplateID")
                Dim Name As String = Common.WebRequest.Data("Name")
                Dim Description As String = Common.WebRequest.Data("Description")
                Dim DispensingType As String = Common.WebRequest.Data("DispensingType")
                Dim Steps As Object = Common.WebRequest.AllData("Steps")
                Dim AssignedFowst As Object = Common.WebRequest.AllData("AssignedFowst")
                Dim pSql As String
                Dim p As New InsertUpdate("LibraryVDMGuidesTemplate")

                p("Name") = ReadyString(Name)
                p("Description") = ReadyString(Description)
                p("DispensingType") = ReadyString(DispensingType)
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                If String.IsNullOrWhiteSpace(TemplateID) Then
                    TemplateID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(TemplateID)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(TemplateID))
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                Dim orderno As Integer = 1
                For Each stp As Object In Steps
                    Dim f As HttpPostedFileBase = TryCast(stp("Img"), HttpPostedFileBase)
                    Dim stepid As String = Guid.NewGuid.ToString()
                    Dim buttonno As Integer = 1
                    If stp.ContainsKey("ID") Then
                        stepid = stp("ID")
                    End If

                    p = New InsertUpdate("LibraryVDMGuides")

                    p("GuideID") = ReadyString(TemplateID)
                    p("OrderNo") = orderno
                    p("Description") = ReadyString(stp("Step"))
                    p("OnScreenTime") = ReadyString(stp("OnScreenTime"))
                    p("IsDisplayInfo") = ReadyString(CBool2(stp("IsDisplayInfo")))

                    If Not CBool2(stp("HasAnImage")) Then
                        p("FileName") = "NULL"
                    End If

                    If CBool2(stp("IsRemove")) Then
                        Common.WebRequest.ProData.Execute("DELETE FROM LibraryVDMGuides WHERE ID = " & ReadyString(stepid))
                    Else
                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(stepid))) = 0 Then
                            p("ID") = ReadyString(stepid)
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    End If

                    orderno += 1
                Next

                For Each fowst As Object In AssignedFowst
                    p = New InsertUpdate("LibraryVDMGuidesAssignedFowst")

                    If fowst("State") <> "delete" Then
                        p("TemplateID") = ReadyString(TemplateID)
                        p("FowstID") = ReadyString(fowst("ID").ToString())

                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" TemplateID = " & ReadyString(TemplateID) & " AND FowstID = " & ReadyString(fowst("ID").ToString()))) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("LastUpdated") = "getdate()"

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    Else
                        pSql = "DELETE FROM LibraryVDMGuidesAssignedFowst WHERE TemplateID = " & ReadyString(TemplateID) & " AND FowstID = " & ReadyString(fowst("ID").ToString())
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryVDMGuides")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveImage() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("LibraryVDMGuides")
                Dim stepid As String = Request.Params.Get("StepID")
                Dim media As String = Request.Params.Get("Media")
                Dim files As HttpFileCollectionBase = Request.Files
                Dim folderName As String = Common.ServerPath & "Content\VendingMachine\" & WebRequest.Instance.OrgID.ToLower()

                If Not System.IO.Directory.Exists(folderName) Then
                    System.IO.Directory.CreateDirectory(folderName)
                End If

                If files.Count > 0 Then
                    For i As Integer = 0 To files.Count - 1
                        Dim f As HttpPostedFileBase = files(i)
                        Dim filename As String = Format(Now(), "MMddyyyy_hhmmsstt") & "_" & stepid & Path.GetExtension(f.FileName)
                        Dim pSourceURL As String = folderName & "\" & filename

                        'If System.IO.File.Exists(pSourceURL) Then
                        '    filename = Format(Now(), "MMddyyyy_hhmmsstt") & "_" & stepid
                        '    pSourceURL = folderName & "\" & filename
                        'End If

                        p("FileName") = ReadyString(filename)
                        p("Media") = ReadyString(media)

                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(stepid)))

                        f.SaveAs(pSourceURL)
                    Next
                End If

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveAds()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Try
                Dim TemplateID As String = Common.WebRequest.Data("TemplateID")
                Dim Name As String = Common.WebRequest.Data("Name")
                Dim Description As String = Common.WebRequest.Data("Description")
                Dim Ads As Object = Common.WebRequest.AllData("Ads")
                Dim AssignedFowst As Object = Common.WebRequest.AllData("AssignedFowst")
                Dim pSql As String
                Dim p As New InsertUpdate("LibraryVDMAdsTemplate")

                p("Name") = ReadyString(Name)
                p("Description") = ReadyString(Description)
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                If String.IsNullOrWhiteSpace(TemplateID) Then
                    TemplateID = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(TemplateID)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(TemplateID))
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                Dim orderno As Integer = 1
                For Each ad As Object In Ads
                    Dim stepid As String = Guid.NewGuid.ToString()
                    Dim buttonno As Integer = 1
                    If ad.ContainsKey("ID") Then
                        stepid = ad("ID")
                    End If

                    p = New InsertUpdate("LibraryVDMAds")

                    p("TemplateID") = ReadyString(TemplateID)
                    p("OrderNo") = orderno
                    p("Orientation") = ReadyString(ad("Orientation"))

                    If Not CBool2(ad("IsMedia")) Then
                        p("FileName") = "NULL"
                    End If

                    If CBool2(ad("IsRemove")) Then
                        Common.WebRequest.ProData.Execute("DELETE FROM LibraryVDMAds WHERE ID = " & ReadyString(stepid))
                    Else
                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(stepid))) = 0 Then
                            p("ID") = ReadyString(stepid)
                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    End If

                    orderno += 1
                Next

                For Each fowst As Object In AssignedFowst
                    p = New InsertUpdate("LibraryVDMAdsAssignedFowst")

                    If fowst("State") <> "delete" Then
                        p("TemplateID") = ReadyString(TemplateID)
                        p("FowstID") = ReadyString(fowst("ID").ToString())

                        If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" TemplateID = " & ReadyString(TemplateID) & " AND FowstID = " & ReadyString(fowst("ID").ToString()))) = 0 Then
                            p("ID") = ReadyString(Guid.NewGuid.ToString())
                            p("LastUpdated") = "getdate()"

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    Else
                        pSql = "DELETE FROM LibraryVDMAdsAssignedFowst WHERE TemplateID = " & ReadyString(TemplateID) & " AND FowstID = " & ReadyString(fowst("ID").ToString())
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryVDMAds")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveImageAds() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("LibraryVDMAds")
                Dim adid As String = Request.Params.Get("AdID")
                Dim media As String = Request.Params.Get("Media")
                Dim files As HttpFileCollectionBase = Request.Files
                Dim folderName As String = Common.ServerPath & "Content\VendingMachine\" & WebRequest.Instance.OrgID.ToLower()

                If Not System.IO.Directory.Exists(folderName) Then
                    System.IO.Directory.CreateDirectory(folderName)
                End If

                If files.Count > 0 Then
                    For i As Integer = 0 To files.Count - 1
                        Dim f As HttpPostedFileBase = files(i)
                        Dim filename As String = "ads_" & Format(Now(), "MMddyyyy_hhmmsstt") & "_" & adid & Path.GetExtension(f.FileName)
                        Dim pSourceURL As String = folderName & "\" & filename

                        'If System.IO.File.Exists(pSourceURL) Then
                        '    filename = Format(Now(), "MMddyyyy_hhmmsstt") & "_" & stepid
                        '    pSourceURL = folderName & "\" & filename
                        'End If

                        p("FileName") = ReadyString(filename)
                        p("Media") = ReadyString(media)

                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(adid)))

                        f.SaveAs(pSourceURL)
                    Next
                End If

                Return JsonDictionary.Serialize("Results", "saved")
            Catch ex As Exception
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SaveDefaultCoins()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim denom() As String = {"25_cent", "1_peso", "5_peso", "10_peso", "20_peso", "50_peso", "100_peso", "200_peso", "500_peso", "1000_peso"}
                Dim fowst As DataTable = Common.WebRequest.ProData.Execute("SELECT ID, Name from MobizLibraryOrganizationFOWST WHERE OrganizationID = " & ReadyString(WebRequest.Instance.OrgID) & " AND ISNULL(IsActive, 1) = 1 AND (FOWSTTypeID <> 'dff0f087-4c2b-4b67-aa1d-502f02773e2b' OR FOWSTTypeID IS NULL) ORDER BY Name")
                Dim pSql As String

                Common.WebRequest.ProData.BeginTransaction()

                For Each num As String In denom
                    WebRequest.Instance.Business.SystemDefaults.SetSystemDefault("VDM " & num, data(num))

                    For Each store As Object In fowst.Rows
                        Dim binid As String = CStr2(Common.WebRequest.ProData.ExecuteScalar(" SELECT ID FROM LibraryFowstBin WHERE FowstID = " & ReadyString(store("ID").ToString()) & " AND Name = " & ReadyString(num)))
                        Dim p

                        If String.IsNullOrEmpty(binid) Then
                            binid = Guid.NewGuid.ToString()
                            p = New InsertUpdate("LibraryFowstBin")

                            p("FowstID") = ReadyString(store("ID").ToString())
                            p("Name") = ReadyString(num)
                            p("BinNo") = "NULL"
                            p("IsActive") = 1
                            p("LastUpdated") = "getdate()"
                            p("DateImported") = "getdate()"
                            p("ID") = ReadyString(binid)

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If

                        Dim binproductid As String = CStr2(Common.WebRequest.ProData.ExecuteScalar(" SELECT ID FROM LibraryFowstBinProduct WHERE FowstID = " & ReadyString(store("ID").ToString()) & " AND BinID = " & ReadyString(binid) & " AND ProductID = " & ReadyString(data(num))))

                        If String.IsNullOrEmpty(binproductid) Then
                            pSql = "SELECT ID, ProductNo, ProductName, SRP " & vbCrLf &
                                    " FROM MobizLibraryPIMProduct as pim " & vbCrLf &
                                    " WHERE pim.ID = " & ReadyString(data(num)) & vbCrLf
                            Dim productrow As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)

                            p = New InsertUpdate("LibraryFowstBinProduct")

                            p("FowstID") = ReadyString(store("ID").ToString())
                            p("BinID") = ReadyString(binid)
                            p("ProductID") = ReadyString(data(num).ToString())
                            p("IsDefault") = 1
                            p("LastUpdated") = "getdate()"
                            p("DateImported") = "getdate()"
                            p("IsActive") = 1
                            p("Balance") = 0
                            p("ProductNo") = ReadyString(productrow("ProductNo"))
                            p("ProductName") = ReadyString(productrow("ProductName"))
                            p("SRP") = CDec2(productrow("SRP"))

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    Next
                Next

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryFowstBinProduct")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                AppendToDebug(ex.Message)
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function SavePTMotors()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim ptid As String = Common.WebRequest.Data("PTID")
                Dim machineid As String = Common.WebRequest.Data("MachineID")
                Dim templatename As String = Common.WebRequest.Data("TemplateName")
                Dim updatedby As String = Common.WebRequest.Data("UpdatedBy")
                Dim motors As Object = Common.WebRequest.AllData("Motors")
                Dim pSql As String
                Dim p As New InsertUpdate("LibraryVendingMachinePlanogramTemplate")

                p("MachineModelID") = ReadyString(machineid)
                p("Name") = ReadyString(templatename)
                p("LastUpdated") = "getdate()"
                p("ModifiedBy") = ReadyString(updatedby)

                If String.IsNullOrWhiteSpace(ptid) Then
                    ptid = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(ptid)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(ptid))
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                For Each motor As Object In motors
                    p = New InsertUpdate("LibraryVendingMachinePlanogramTemplateList")

                    p("PlanogramTemplateID") = ReadyString(ptid)
                    p("MotorNo") = ReadyString(motor("MotorNo"))
                    p("BoardNo") = ReadyString(motor("BoardNo"))
                    p("BoardID") = ReadyString(motor("BoardID"))
                    p("BoardUUID") = ReadyString(motor("BoardUUID"))
                    p("DispenserMotorNo") = ReadyString(motor("DispenserMotorNo"))
                    p("DispenserType") = ReadyString(motor("DispenserType"))
                    p("Capacity") = ReadyString(motor("Capacity"))
                    p("ProductID") = IIf(String.IsNullOrWhiteSpace(motor("ProductID")), "NULL", ReadyString(motor("ProductID")))
                    p("ProductName") = ReadyString(motor("ProductName"))
                    p("Category") = ReadyString(motor("Category"))

                    If String.IsNullOrWhiteSpace(motor("ID")) Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        pSql = p.GetInsert()
                    Else
                        pSql = p.GetUpdate(" ID = " & ReadyString(motor("ID")))
                    End If

                    Common.WebRequest.ProData.Execute(pSql)
                Next

                Dim hbtempid As String = Common.WebRequest.ProData.ExecuteScalar("SELECT HotButtonTemplateID FROM LibraryVendingMachinePlanogramTemplate WHERE ID = " & ReadyString(ptid)).ToString()

                p = New InsertUpdate("LibraryPimMenu")
                p("Name") = ReadyString(templatename)
                p("IsActive") = 1
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(updatedby)
                p("VDMModelID") = ReadyString(Common.WebRequest.ProData.ExecuteScalar("SELECT BoardID FROM LibraryVendingMachines WHERE ID = " & ReadyString(machineid)))
                p("IsDisplayAllProducts") = 1

                If String.IsNullOrWhiteSpace(hbtempid) Then
                    hbtempid = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(hbtempid)

                    Common.WebRequest.ProData.Execute(p.GetInsert())

                    p = New InsertUpdate("LibraryVendingMachinePlanogramTemplate")

                    p("HotButtonTemplateID") = ReadyString(hbtempid)

                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(ptid)))
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(hbtempid)))
                End If

                'reset MotorNo of all SKUs under HB Template
                p = New InsertUpdate("LibraryPimGroupPims")
                p("VDMMotorNo") = "NULL"
                pSql = p.GetUpdate(" MenuID = " & ReadyString(hbtempid))

                For Each motor As Object In motors
                    Dim hbgroupid As String = ""

                    If Not String.IsNullOrWhiteSpace(motor("Category")) Then
                        hbgroupid = CStr2(Common.WebRequest.ProData.ExecuteScalar("SELECT ID FROM LibraryPimGroups WHERE MenuID = " & ReadyString(hbtempid) & " AND Name = " & ReadyString(motor("Category"))))

                        If String.IsNullOrWhiteSpace(hbgroupid) Then
                            p = New InsertUpdate("LibraryPimGroups")

                            hbgroupid = Guid.NewGuid.ToString()

                            p("ID") = ReadyString(hbgroupid)
                            p("Name") = ReadyString(motor("Category"))
                            p("IsActive") = 1
                            p("LastUpdated") = "getdate()"
                            p("MenuID") = ReadyString(hbtempid)
                            p("UserID") = ReadyString(updatedby)
                            p("OrderNo") = ReadyString(Common.WebRequest.ProData.ExecuteScalar("SELECT TOP 1 OrderNo + 1 as [OrderNo] FROM LibraryPimGroups WHERE MenuID = " & ReadyString(hbtempid) & " ORDER BY OrderNo DESC "))

                            Common.WebRequest.ProData.Execute(p.GetInsert())
                        End If
                    End If

                    Common.WebRequest.ProData.Execute(pSql)

                    If Not String.IsNullOrWhiteSpace(motor("ProductID")) Then
                        p = New InsertUpdate("LibraryPimGroupPims")

                        p("GroupID") = IIf(String.IsNullOrEmpty(hbgroupid), "NULL", ReadyString(hbgroupid))
                        p("ProductID") = IIf(String.IsNullOrWhiteSpace(motor("ProductID")), "NULL", ReadyString(motor("ProductID")))
                        p("ShortName") = ReadyString(motor("ProductName"))
                        p("MenuID") = ReadyString(hbtempid)
                        p("IsActive") = 1
                        p("OrderNo") = ReadyString(Common.WebRequest.ProData.ExecuteScalar("SELECT TOP 1 OrderNo + 1 as [OrderNo] FROM LibraryPimGroupPims WHERE ProductID = " & ReadyString(motor("ProductID")) & " AND GroupID = " & ReadyString(hbgroupid) & " AND MenuID = " & ReadyString(hbtempid) & " ORDER BY OrderNo DESC "))

                        Dim btninfo As DataRow = Common.WebRequest.ProData.ExecuteRow("SELECT ID, VDMMotorNo FROM LibraryPimGroupPims WHERE ProductID = " & ReadyString(motor("ProductID")) & IIf(String.IsNullOrEmpty(hbgroupid), "", " AND GroupID = " & ReadyString(hbgroupid)) & " AND MenuID = " & ReadyString(hbtempid))
                        Dim btnid As String
                        Dim skuMotorNo As String = ""

                        If btninfo Is Nothing Then
                            btnid = Guid.NewGuid.ToString()
                            p("ID") = ReadyString(btnid)
                            pSql = p.GetInsert()
                        Else
                            btnid = btninfo("ID").ToString()
                            skuMotorNo = CStr2(btninfo("VDMMotorNo"))
                            pSql = p.GetUpdate(" ID = " & ReadyString(btnid))
                        End If

                        Common.WebRequest.ProData.Execute(pSql)

                        Dim numbers As List(Of Integer) = skuMotorNo.Split({","c}, StringSplitOptions.RemoveEmptyEntries).Select(Function(s) Convert.ToInt32(s.Trim())).ToList()

                        If Not numbers.Contains(motor("MotorNo")) Then
                            numbers.Add(motor("MotorNo"))
                            skuMotorNo = String.Join(", ", numbers)
                        End If

                        p = New InsertUpdate("LibraryPimGroupPims")
                        p("VDMMotorNo") = ReadyString(skuMotorNo)
                        p("VDMachineID") = ReadyString(machineid)
                        p("PlanogramTempID") = ReadyString(ptid)
                        pSql = p.GetUpdate(" ID = " & ReadyString(btnid))
                        Common.WebRequest.ProData.Execute(pSql)
                    End If
                Next

                WebRequest.Instance.Business.SystemDefaults.UpdateTableLastUpdated("LibraryVendingMachinePlanogramTemplate")

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Result", "saved")
            Catch ex As Exception
                Common.WebRequest.ProData.RollbackTransaction()
                Return JsonDictionary.Serialize("Error", ex.Message)
            End Try
        End Function

        Function GetPTInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim ptid As String = Common.WebRequest.Data("PTID")
            Dim pOut As New JsonDictionary

            Dim pSql As String = "SELECT * FROM LibraryVendingMachinePlanogramTemplate WHERE ID = " & ReadyString(ptid)

            pOut.Add("Template", Common.WebRequest.ProData.ExecuteRow(pSql))

            pSql = "SELECT list.*, board.Name as BoardName FROM LibraryVendingMachinePlanogramTemplateList as list " & vbCrLf &
                " INNER JOIN LibraryVendingMachineBoards as board ON board.ID = list.BoardID " & vbCrLf &
                " WHERE PlanogramTemplateID = " & ReadyString(ptid) & " ORDER BY MotorNo "

            pOut.Add("Motors", Common.WebRequest.ProData.Execute(pSql))

            Return pOut.Serialize()
        End Function
    End Class
End Namespace
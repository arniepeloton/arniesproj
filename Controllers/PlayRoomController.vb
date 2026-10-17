Namespace WebProj
    Public Class PlayRoomController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult
            Return View()
        End Function

        Function GetCustomerInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim PeopleID As String = Common.WebRequest.Data("PeopleID")
                Dim pSql As String = "SELECT rname.FullName as RepresentativeName, rname.ProfilePicture " & vbCrLf &
                                    " , cno.ID as MobileID, '0' + telco.Prefix + '' + cno.ContactNo as RepresentiveContactNo " & vbCrLf &
                                    " FROM MobizLibraryPeople as rname " & vbCrLf &
                                    " OUTER APPLY(SELECT TOP 1 * FROM MobizLibraryPeopleContactMobile WHERE PeopleID = rname.ID ORDER BY LastUpdated DESC) as cno " & vbCrLf &
                                    " LEFT JOIN MobizLibraryTelcoPrefixes as telco ON telco.ID = cno.TelCoPrefixID " & vbCrLf &
                                    " WHERE rname.ID = " & ReadyString(PeopleID)
                Dim pTrans As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)

                pSql = " SELECT child.PeopleID, cname.FullName, cname.Age, cname.Gender, cname.ProfilePicture " & vbCrLf &
                        " FROM TransPlayRoomChildren as child " & vbCrLf &
                        " INNER JOIN TransPlayRoom as trans ON trans.ID = child.PlayRoomID " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as cname ON cname.ID = child.PeopleID " & vbCrLf &
                        " WHERE trans.PeopleID = " & ReadyString(PeopleID) & vbCrLf
                'child.TotalDuration IS NULL = walay labot ang humanag dula
                Dim pChild As DataTable = Common.WebRequest.ProData.Execute(pSql)

                Dim pOut As New JsonDictionary

                pOut.Add("Representative", pTrans)
                pOut.Add("Children", pChild)

                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetReport()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim searchkey As String = Common.WebRequest.Data("searchkey")
                Dim reportDateFrom As String = Common.WebRequest.Data("reportDateFrom")
                Dim reportDateTo As String = Common.WebRequest.Data("reportDateTo")
                Dim pSql As String = " SELECT child.*, cname.FullName, cname.Age, cname.Gender, cname.ProfilePicture, rname.FullName as RepresentativeName " & vbCrLf &
                                    " FROM TransPlayRoomChildren as child " & vbCrLf &
                                    " INNER JOIN TransPlayRoom as trans ON trans.ID = child.PlayRoomID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as cname ON cname.ID = child.PeopleID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as rname ON rname.ID = trans.PeopleID " & vbCrLf &
                                    " WHERE trans.PlayDate BETWEEN " & ReadyString(reportDateFrom) & " AND " & ReadyString(reportDateTo) & vbCrLf

                If Not String.IsNullOrEmpty(searchkey) Then
                    pSql &= " AND cname.FullName + ' ' + rname.FullName LIKE " & ReadyStringWild(searchkey) & vbCrLf
                End If

                pSql &= " ORDER BY cname.FullName ASC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function EndPlay()
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("TransPlayRoomChildren")
                Dim transID As String = Common.WebRequest.Data("PlayRoomID")

                p("ActualExitTime") = "getdate()"
                p("TotalDuration") = ReadyString(Common.WebRequest.Data("TotalDuration"))
                p("ActualTotalDuration") = ReadyString(Common.WebRequest.Data("ActualTotalDuration"))
                p("LastUpdated") = "getdate()"

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ChildTransID"))))

                Dim arrChild As DataTable = Common.WebRequest.ProData.Execute("SELECT * FROM TransPlayRoomChildren WHERE TotalDuration IS NOT NULL AND PlayRoomID = " & ReadyString(transID))
                Dim pRow As DataRow = Common.WebRequest.ProData.ExecuteRow("SELECT *, (SELECT COUNT(ID) FROM TransPlayRoomChildren WHERE PlayRoomID = TransPlayRoom.ID) as CountChild FROM TransPlayRoom WHERE ID = " & ReadyString(transID))
                Dim accumulatedTotalDuration = TimeSpan.Parse("00:00")
                Dim accumulatedActualTotalDuration = TimeSpan.Parse("00:00")

                If CDec2(arrChild.Rows.Count) = CDec2(pRow("CountChild")) Then
                    For Each child As Object In arrChild.Rows
                        accumulatedTotalDuration += TimeSpan.Parse(child("TotalDuration"))
                        accumulatedActualTotalDuration += TimeSpan.Parse(child("ActualTotalDuration"))
                    Next

                    p = New InsertUpdate("TransPlayRoom")

                    p("TotalDuration") = ReadyString(CStr2(accumulatedTotalDuration))
                    p("ActualTotalDuration") = ReadyString(CStr2(accumulatedActualTotalDuration))
                    p("LastUpdated") = "getdate()"
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(transID)))
                End If

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "ended")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function ExtendTime()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim p As New InsertUpdate("TransPlayRoomChildren")

                p("ExitTime") = ReadyString(Common.WebRequest.Data("ExtendedTime"))

                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(Common.WebRequest.Data("ChildTransID"))))

                Return JsonDictionary.Serialize("Results", "extended")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetPlayersInfo()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim transID As String = Common.WebRequest.Data("TransID")
                Dim pSql As String = "SELECT trans.*, rname.FullName as RepresentativeName, rname.ProfilePicture, recname.FirstName + ' ' + recname.Surname as ReceptionistName " & vbCrLf &
                                    " , cno.ID as MobileID, '0' + telco.Prefix + '' + cno.ContactNo as RepresentiveContactNo " & vbCrLf &
                                    " FROM TransPlayRoom as trans " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as rname ON rname.ID = trans.PeopleID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as recname ON recname.ID = trans.Receptionist " & vbCrLf &
                                    " LEFT JOIN MobizLibraryPeopleContactMobile as cno ON cno.PeopleID = trans.PeopleID " & vbCrLf &
                                    " LEFT JOIN MobizLibraryTelcoPrefixes as telco ON telco.ID = cno.TelCoPrefixID " & vbCrLf &
                                    " WHERE trans.ID = " & ReadyString(transID)
                Dim pTrans As DataRow = Common.WebRequest.ProData.ExecuteRow(pSql)

                pSql = " SELECT child.*, cname.FullName, cname.Age, cname.Gender, cname.ProfilePicture " & vbCrLf &
                        " FROM TransPlayRoomChildren as child " & vbCrLf &
                        " INNER JOIN TransPlayRoom as trans ON trans.ID = child.PlayRoomID " & vbCrLf &
                        " INNER JOIN MobizLibraryPeople as cname ON cname.ID = child.PeopleID " & vbCrLf &
                        " WHERE child.TotalDuration IS NULL AND child.PlayRoomID = " & ReadyString(transID) & vbCrLf
                'child.TotalDuration IS NULL = walay labot ang humanag dula
                Dim pChild As DataTable = Common.WebRequest.ProData.Execute(pSql)

                Dim pOut As New JsonDictionary

                pOut.Add("Trans", pTrans)
                pOut.Add("Children", pChild)

                Return pOut.Serialize()
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function GetPlayers()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim searchkey As String = Common.WebRequest.Data("searchkey")
                Dim pSql As String = " SELECT child.*, cname.FullName, cname.Age, cname.Gender, cname.ProfilePicture, rname.FullName as RepresentativeName, rname.ProfilePicture as RepresentativePicture " & vbCrLf &
                                    " FROM TransPlayRoomChildren as child " & vbCrLf &
                                    " INNER JOIN TransPlayRoom as trans ON trans.ID = child.PlayRoomID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as cname ON cname.ID = child.PeopleID " & vbCrLf &
                                    " INNER JOIN MobizLibraryPeople as rname ON rname.ID = trans.PeopleID " & vbCrLf &
                                    " WHERE child.TotalDuration IS NULL " & vbCrLf

                If Not String.IsNullOrEmpty(searchkey) Then
                    pSql &= " AND cname.FullName + ' ' + rname.FullName LIKE " & ReadyStringWild(searchkey) & vbCrLf
                End If

                pSql &= " ORDER BY child.ExitTime ASC "

                Return JsonDictionary.Serialize("Results", Common.WebRequest.ProData.Execute(pSql))
            Catch ex As Exception
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function Save() As String
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            Try
                Dim data As Object = Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("MobizLibraryOrganization")
                Dim pSql, transid As String
                Dim personid As String = data("PersonID").ToString()
                Dim arrPersonID As New List(Of String)

                'p("ID") = ReadyString(strPersonID)
                p("PersonFullname") = ReadyString(data("RepresentiveName"))
                p("IsOrganization") = "0"
                p("IsActive") = "1"
                p("CreatedByOrgID") = ReadyString(Common.WebRequest.Data("OrgID"))
                'p("LastUpdated") = ReadyString(CDate(DateTime))

                If String.IsNullOrEmpty(personid) Then
                    personid = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(personid)
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(personid))
                End If

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(pSql)

                p = New InsertUpdate("LibraryOrganizationCustomer")

                p("OrganizationID") = ReadyString(personid)

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" OrganizationID = " & ReadyString(personid))) = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    p("LastUpdated") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryPeople")

                p("FullName") = ReadyString(data("RepresentiveName"))

                If Common.WebRequest.ProData.ExecuteSQL(p.GetUpdate(" ID = " & ReadyString(personid))) = 0 Then
                    p("ID") = ReadyString(personid)
                    p("LastUpdated") = "getdate()"
                    p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If

                p = New InsertUpdate("MobizLibraryPeopleContactMobile")

                p("PeopleID") = ReadyString(personid)
                p("CountryID") = ReadyString("e783ed13-aa98-4cc1-918e-d49b05c0167e")
                p("TelCoPrefixID") = ReadyString(WebRequest.Instance.Business.LibraryTelcoPrefixes.GetOrCreateByMobileNumber(data("RepresentiveContactNo"))("ID"))
                p("ContactNo") = ReadyString(LibraryTelcoPrefixes.ParseNumber(data("RepresentiveContactNo")))
                p("ContactType") = ReadyString("Personal Mobile")
                p("LastUpdated") = "getdate()"

                If String.IsNullOrEmpty(data("MobileID").ToString()) Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(data("MobileID").ToString())))
                End If

                p = New InsertUpdate("TransPlayRoom")

                p("FowstID") = ReadyString(Common.WebRequest.Data("FowstID"))
                p("PeopleID") = ReadyString(personid)
                p("TotalNoOfChildren") = ReadyString(data("TotalNoOfChildren"))
                p("TotalNoOfAdult") = ReadyString(data("TotalNoOfAdult"))
                p("EnterTime") = ReadyString(data("EnterTime"))
                p("ExitTime") = ReadyString(data("ExitTime"))
                p("TotalDuration") = IIf(String.IsNullOrEmpty(data("TotalDuration")), "NULL", ReadyString(data("TotalDuration")))
                p("BagLocker") = ReadyString(data("BagLocker"))
                p("ShoeLocker") = ReadyString(data("ShoeLocker"))
                p("Receptionist") = ReadyString(data("Receptionist"))
                'p("TotalAmount") = IIf(String.IsNullOrEmpty(data("TotalAmount")), "NULL", ReadyString(data("TotalAmount")))
                'p("ORNo") = IIf(String.IsNullOrEmpty(data("ORNo")), "NULL", ReadyString(data("ORNo")))
                p("LastUpdated") = "getdate()"
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)

                If String.IsNullOrEmpty(data("TransID").ToString()) Then
                    transid = Guid.NewGuid.ToString()
                    p("ID") = ReadyString(transid)
                    p("PlayDate") = "getdate()"
                    Common.WebRequest.ProData.Execute(p.GetInsert())
                Else
                    transid = data("TransID").ToString()
                    Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(transid)))
                End If

                arrPersonID.Add(data("PicCode") & "|" & personid)

                For Each child As Object In data("Children")
                    p = New InsertUpdate("MobizLibraryPeople")
                    Dim childid As String = child("PeopleID").ToString()
                    p("FullName") = ReadyString(child("FullName"))
                    p("Age") = ReadyString(child("Age"))
                    p("Gender") = ReadyString(child("Gender"))

                    If String.IsNullOrEmpty(childid) Then
                        childid = Guid.NewGuid.ToString()
                        p("ID") = ReadyString(childid)
                        p("LastUpdated") = "getdate()"
                        p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    Else
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(childid)))
                    End If

                    p = New InsertUpdate("TransPlayRoomChildren")

                    p("PlayRoomID") = ReadyString(transid)
                    p("PeopleID") = ReadyString(childid)
                    p("EnterTime") = ReadyString(data("EnterTime"))
                    p("ExitTime") = ReadyString(data("ExitTime"))
                    p("LastUpdated") = "getdate()"

                    If child("Action").ToString() = "new" Then
                        p("ID") = ReadyString(Guid.NewGuid.ToString())
                        p("PlayDate") = "getdate()"
                        Common.WebRequest.ProData.Execute(p.GetInsert())
                    Else
                        If child("Action").ToString() = "remove" Then
                            'tag as done
                            Dim totalduration As String = CStr2(TimeSpan.Parse(Format(Date.Now, "HH:mm")) - TimeSpan.Parse(data("EnterTime")))
                            p("TotalDuration") = ReadyString(totalduration)
                        End If
                        Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(child("ID").ToString())))
                    End If

                    arrPersonID.Add(child("PicCode") & "|" & childid)
                Next child

                Common.WebRequest.ProData.CommitTransaction()

                Dim pOut As New JsonDictionary

                pOut.Add("Results", "saved")
                pOut.Add("PeopleID", arrPersonID) ' use when saving pics

                Return pOut.Serialize()
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function

        Function SavePic() As String
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Try
                Dim peopleid As String = Common.WebRequest.Data("PeopleID")
                Dim imgData As String = Common.WebRequest.Data("imgData")
                Dim file As String = Common.ServerPath & "Content\MobizPeople\" & peopleid & ".jpg"

                Dim p As New InsertUpdate("MobizLibraryPeople")

                p("ProfilePicture") = ReadyString(peopleid & ".jpg")

                Common.WebRequest.ProData.BeginTransaction()
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(peopleid)))

                If System.IO.File.Exists(file) Then
                    System.IO.File.Delete(file)
                End If

                Using imagePath As IO.FileStream = New IO.FileStream(file, IO.FileMode.Create)
                    Using binaryWriting As IO.BinaryWriter = New IO.BinaryWriter(imagePath)
                        binaryWriting.Write(Convert.FromBase64String(imgData))
                        binaryWriting.Close()
                    End Using
                End Using

                Common.WebRequest.ProData.CommitTransaction()

                Return JsonDictionary.Serialize("Results", "success")
            Catch ex As Exception
                AppendToLog(ex.Message)
                Return JsonDictionary.Serialize("Results", ex.Message)
            End Try
        End Function
    End Class
End Namespace
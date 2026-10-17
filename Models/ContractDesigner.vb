Public Class ContractDesigner

    Function RemoveToModule() As String
        Try
            Dim pSql As String

            pSql = " DELETE FROM LibraryModuleContract " & vbCrLf &
                    " WHERE ContractDesignerTemplateID = " & ReadyString(Common.WebRequest.Data("ID"))
            Common.WebRequest.ProData.Execute(pSql)

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function GetByModule() As Object
        Dim pSql As String

        pSql = "SELECT temp.*, ppol.FirstName + ' ' + ppol.Surname as AddedBy " & vbCrLf &
                " FROM LibraryModuleContract as mod " & vbCrLf &
                " INNER JOIN LibraryContractDesignerTemplate as temp ON mod.ContractDesignerTemplateID = temp.ID " & vbCrLf &
                " LEFT JOIN MobizLibraryPeople as ppol ON ppol.ID = mod.UserID " & vbCrLf &
                " WHERE mod.Module = " & ReadyString(Common.WebRequest.Data("Module")) & vbCrLf &
                " ORDER BY temp.TemplateName ASC "

        Dim arrContractDesigner As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        GeTemplateFreeType(arrContractDesigner)

        Return arrContractDesigner
    End Function

    Function AddToModule() As String
        Try
            Dim pSql As String

            Common.WebRequest.ProData.BeginTransaction()

            For Each con As Object In Common.WebRequest.AllData("data")
                Dim p As New InsertUpdate("LibraryModuleContract")

                p("Module") = ReadyString(Common.WebRequest.Data("Module"))
                p("ContractDesignerTemplateID") = ReadyString(con("ID"))
                p("UserID") = ReadyString(WebSession.Instance.PeopleUserID)
                p("LastUpdated") = "getdate()"

                pSql = "SELECT ID FROM LibraryModuleContract WHERE ContractDesignerTemplateID = " & ReadyString(con("ID"))

                If Common.WebRequest.ProData.Execute(pSql).Rows.Count = 0 Then
                    p("ID") = ReadyString(Guid.NewGuid.ToString())

                    Common.WebRequest.ProData.Execute(p.GetInsert())
                End If
            Next con

            Common.WebRequest.ProData.CommitTransaction()

            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function GetContract(ByVal data As Object) As DataTable
        Dim pSql As String

        pSql = "SELECT * FROM LibraryContractDesignerTemplate " & vbCrLf &
                " WHERE Module = " & ReadyString(data("Module"))

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetContractDesignerTemplate() As Object
        Dim pSql As String

        pSql = " SELECT temp.*, " & vbCrLf &
                " ppol.FirstName + ' ' + ppol.Surname as ModifiedByName" & vbCrLf &
                " FROM LibraryContractDesignerTemplate as temp " & vbCrLf &
                " INNER JOIN MobizLibraryPeople as ppol ON temp.ModifiedBy = ppol.ID " & vbCrLf &
                " ORDER BY temp.TemplateName ASC"

        Dim arrContractDesigner As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        GeTemplateFreeType(arrContractDesigner)

        Return arrContractDesigner
    End Function

    Function GeTemplateFreeType(ByRef arrContractDesigner As Object) As Object
        For Each cd As Object In arrContractDesigner
            Dim pSql = "SELECT * FROM LibraryContractDesignerFreeTypeField WHERE ContractDesignerTemplateID = " & ReadyString(cd("ID").ToString())
            cd.Add("FreeTypeFields", jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql))))
        Next cd
    End Function

    Function SaveTemplate(ByVal data As Object) As String
        Try
            Dim p As New InsertUpdate("LibraryContractDesignerTemplate")
            Dim pSql As String
            Dim TemplateID As String

            Common.WebRequest.ProData.BeginTransaction()

            p("TemplateName") = ReadyString(data("TemplateName"))
            p("Module") = ReadyString(data("Module"))
            p("PaperSize") = ReadyString(data("PaperSize"))
            p("Contents") = ReadyString(data("Content"))
            p("ModifiedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "getdate()"

            If CBool2(data("IsNew")) Then
                TemplateID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(TemplateID)
                pSql = p.GetInsert()
            Else
                TemplateID = data("ID")
                pSql = p.GetUpdate(" ID = " & ReadyString(TemplateID))
            End If

            Common.WebRequest.ProData.Execute(pSql)

            Dim freetypeids As New ArrayList()

            For Each ft As Object In data("FreeType")
                freetypeids.Add(ft("FreeTypeID"))
                p = New InsertUpdate("LibraryContractDesignerFreeTypeField")

                p("ContractDesignerTemplateID") = ReadyString(TemplateID)
                p("Label") = ReadyString(ft("Label"))

                If Common.WebRequest.ProData.Execute("SELECT ID FROM LibraryContractDesignerFreeTypeField WHERE ID = " & ReadyString(ft("FreeTypeID"))).Rows.Count = 0 Then
                    p("ID") = ReadyString(ft("FreeTypeID"))
                    pSql = p.GetInsert()
                Else
                    pSql = p.GetUpdate(" ID = " & ReadyString(ft("FreeTypeID")))
                End If

                Common.WebRequest.ProData.Execute(pSql)
            Next ft

            If freetypeids.Count > 0 Then
                pSql = "DELETE FROM LibraryContractDesignerFreeTypeField " & vbCrLf &
                    " WHERE ID NOT IN ( '" & String.Join("', '", freetypeids.ToArray()) & "') "
                Common.WebRequest.ProData.Execute(pSql)
            Else
                pSql = "DELETE FROM LibraryContractDesignerFreeTypeField " & vbCrLf &
                        " WHERE ContractDesignerTemplateID = " & ReadyString(TemplateID)
                Common.WebRequest.ProData.Execute(pSql)
            End If

            Common.WebRequest.ProData.CommitTransaction()
            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Function SaveFreeType(ByVal data As Object) As String
        Try
            Dim pSql As String

            Common.WebRequest.ProData.BeginTransaction()

            For Each ft As Object In data
                Dim p As New InsertUpdate("LibraryContractDesignerFreeTypeField")

                p("Value") = ReadyString(ft("Value"))

                pSql = p.GetUpdate(" ID = " & ReadyString(ft("ID")))

                Common.WebRequest.ProData.Execute(pSql)
            Next ft

            Common.WebRequest.ProData.CommitTransaction()
            Return "saved"
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function



    Function GetContractByModule() As Object
        Dim pSql As String

        pSql = "SELECT temp.* " & vbCrLf &
                " FROM LibraryContractDesignerTemplate as temp " & vbCrLf &
                " WHERE temp.Module = " & ReadyString(Common.WebRequest.Data("Module")) & vbCrLf &
                " ORDER BY temp.TemplateName ASC "

        Dim arrContractDesigner As Object = jss.Deserialize(Of Object)(DataTableToJSON.GetJson(Common.WebRequest.ProData.Execute(pSql)))

        Return arrContractDesigner
    End Function

    Function SaveLeaseContractData()
        Try
            Dim data As Object = Common.WebRequest.AllData("data")
            Dim dbID As String = Common.WebRequest.Data("ID")
            Dim owner As String = Common.WebRequest.Data("Owner")
            Dim templateid As String = Common.WebRequest.Data("TemplateID")
            Dim generatedid As String = Guid.NewGuid.ToString()
            Dim pSql As String
            Dim p As New InsertUpdate("LibraryLeaseGeneratedContract")

            p("TemplateID") = ReadyString(templateid)
            p("ContractName") = ReadyString(owner)
            p("UpdatedBy") = ReadyString(WebSession.Instance.PeopleUserID)
            p("LastUpdated") = "getdate()"

            If String.IsNullOrWhiteSpace(dbID) Then
                dbID = Guid.NewGuid.ToString()
                p("ID") = ReadyString(dbID)
                Common.WebRequest.ProData.Execute(p.GetInsert())
            Else
                Common.WebRequest.ProData.Execute(p.GetUpdate(" ID = " & ReadyString(dbID)))
            End If

            pSql = "DELETE FROM LibraryLeaseGeneratedContractValues WHERE GeneratedContractID = " & ReadyString(dbID)
            Common.WebRequest.ProData.Execute(pSql)

            For Each field As Object In data
                p = New InsertUpdate("LibraryLeaseGeneratedContractValues")

                p("ID") = ReadyString(Guid.NewGuid.ToString())
                p("GeneratedContractID") = ReadyString(dbID)
                p("Field") = ReadyString(field("Field"))
                p("Value") = ReadyString(field("Value"))

                Common.WebRequest.ProData.Execute(p.GetInsert())
            Next

            Return "saved"
        Catch ex As Exception
            AppendToDebug(ex.Message)
            Return "error"
        End Try

    End Function

    Function GetGeneratedContract()
        Dim pSql As String

        pSql = "SELECT * FROM LibraryLeaseGeneratedContract WHERE TemplateID = " & ReadyString(Common.WebRequest.Data("TemplateID"))

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function

    Function GetGeneratedContractValues()
        Dim pSql As String

        pSql = "SELECT Field, Value FROM LibraryLeaseGeneratedContractValues WHERE GeneratedContractID = " & ReadyString(Common.WebRequest.Data("ID"))

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function
End Class

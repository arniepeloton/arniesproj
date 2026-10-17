Namespace WebProj
    Public Class ContractDesignerController
        Inherits System.Web.Mvc.Controller

        Dim contractdesigner As New ContractDesigner

        Function Index()
            Return View()
        End Function

        Function GetContractsCount()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim pSql As String = "SELECT Module, COUNT(ID) as Count FROM LibraryContractDesignerTemplate GROUP BY Module ORDER BY Module"

            Return JsonDictionary.Serialize("Result", Common.WebRequest.ProData.Execute(pSql))
        End Function

        Function GetContractByModule()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return Json(New With {Key .Results = contractdesigner.GetContractByModule()})
        End Function

        Function SaveLeaseContractData()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization
            If WebSession.Instance.PeopleUserID Is Nothing Then Return JsonDictionary.Serialize("Error", "Access denied")

            Return Json(New With {Key .Result = contractdesigner.SaveLeaseContractData()})
        End Function

        Function GetGeneratedContract()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", contractdesigner.GetGeneratedContract())
        End Function

        Function GetGeneratedContractValues()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", contractdesigner.GetGeneratedContractValues())
        End Function
        Function ExportFile()
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Dim newpDT As New DataTable
            Dim newRow As DataRow
            Dim pSql As String
            Dim pReport As ProReport
            Dim pFilename As String

            newRow = newpDT.NewRow
            newpDT.Columns.Add("Content")

            pSql = "SELECT TemplateName, Contents FROM LibraryContractDesignerTemplate WHERE ID = " & ReadyString(Request.Params.Get("ID"))
            Dim res As DataTable = Common.WebRequest.ProData.Execute(pSql)

            If res.Rows.Count > 0 Then
                newRow(0) = res.Rows(0)("Contents")
                newpDT.Rows.Add(newRow)

                pReport = New ProReport()
                pReport.AutoFillHeader = False
                pReport.SourceFilename = "ContractTemplate_A4.mrt"
                pFilename = Common.ServerPath & "ContractTemplate\" & res.Rows(0)("TemplateName") & ".pdf"
                pReport.AddTable("Info", newpDT)

                ProReport.DebugXmlFilename = ProWeb.Common.ServerPath + "Reports\ContractDesigner.xml"

                pReport.ExportXmlFile()
                pReport.ExportFile(ProReport.eExportFormat.Pdf, pFilename)

                Dim file As System.IO.FileInfo = New System.IO.FileInfo(Server.MapPath("~/" & "/ContractTemplate/" & res.Rows(0)("TemplateName") & ".pdf"))

                Response.Clear()
                Response.AddHeader("Content-Disposition", "attachment; filename=" & res.Rows(0)("TemplateName") & ".pdf")
                Response.AddHeader("Content-Length", file.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(file.FullName)
                Response.End()
            End If

        End Function

        <HttpPost>
        Function SaveFreeType() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", contractdesigner.SaveFreeType(Common.WebRequest.AllData("data")))
        End Function

        Function RemoveToModule()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", contractdesigner.RemoveToModule())
        End Function

        Function AddToModule()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", contractdesigner.AddToModule())
        End Function

        Function GetByModule()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return Json(New With {Key .Results = contractdesigner.GetByModule()})
        End Function

        Function GetContractDesignerTemplate()
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return Json(New With {Key .Results = contractdesigner.GetContractDesignerTemplate()})
        End Function

        <HttpPost>
        Function SaveTemplate() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", contractdesigner.SaveTemplate(Common.WebRequest.AllData("data")))
        End Function

        <HttpPost>
        Function GetContract() As String
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            Return JsonDictionary.Serialize("Results", contractdesigner.GetContract(Common.WebRequest.AllData("data")))
        End Function
    End Class
End Namespace
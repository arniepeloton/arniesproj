Imports Newtonsoft.Json

Public Class InvoicePickedItemsController
    Inherits System.Web.Mvc.Controller

    <HttpGet>
    Public Function Index() As ActionResult

        If CDec2(Common.WebRequest.QueryString("IsMobile")) = 0 Then
            If WebRequest.Instance.HandleAccess = False Then Exit Function
        End If

        Return View()
    End Function

    <HttpPost>
    Public Function CreateInvoice() As String

        Dim pCreateInvoiceJson As CreateInvoiceJson
        Dim pError As String
        Dim pOut As JsonDictionary
        Dim pInt As Integer
        Dim pStr As String

        Try
            If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
            If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

            pOut = New JsonDictionary()
            pCreateInvoiceJson = JsonConvert.DeserializeObject(Of CreateInvoiceJson)(Common.WebRequest.InputStreamText)
            If pCreateInvoiceJson.Verify(pError) = False Then
                Return JsonDictionary.Serialize("Error", pError)
            End If

            pInt = WebRequest.Instance.Business.TransInvoice.TagReadyForInvoice(pCreateInvoiceJson.InvoiceID, pCreateInvoiceJson.Sections.Select( _
                Function(vSection As CreateInvoiceJson.SectionClass) As String
                    Return vSection.SectionID
                End Function
            ).ToList())
            pOut.Add("SectionCount", pInt)

            Dim pSortingListCreator As SortingListCreator
            pSortingListCreator = New SortingListCreator()
            pStr = pSortingListCreator.CreateSortingLists()
            pOut.Add("CreatedSortingList", pStr)
            Return pOut.Serialize()
        Catch ex As Exception
            Return JsonDictionary.Serialize("Error", ex.Message)
        End Try

    End Function

    <HttpPost>
    Public Function InvoiceAndSections() As String

        Dim pInvoiceID As String
        Dim pDrInvoice As DataRow
        Dim pDtSections As DataTable
        Dim pOut As JsonDictionary

        pInvoiceID = Common.WebRequest.Data("InvoiceID")
        If IsGuid(pInvoiceID) = False Then
            Return JsonDictionary.Serialize("Error", "InvoiceID must be a valid GUID")
        End If
        pInvoiceID = ReadyGuid(pInvoiceID)

        If WebRequest.Instance.HandleAjax = False Then Return JsonDictionary.Serialize("Error", "Access denied")
        If WebRequest.Instance.HandleOrganization(True) = False Then Return WebRequest.InvalidOrganization

        pOut = New JsonDictionary

        pDrInvoice = WebRequest.Instance.Business.TransInvoice.ById2(pInvoiceID)
        pOut.Add("Invoice", pDrInvoice)

        pDtSections = WebRequest.Instance.Business.TransInvoice.ReadyToInvoiceSections(pInvoiceID)
        pOut.Add("Sections", pDtSections)

        Return pOut.Serialize()

    End Function

    Private Class CreateInvoiceJson

        Public InvoiceID As String
        Public Sections As IList(Of SectionClass)

        Public Function Verify(ByRef oError As String) As Boolean

            oError = Nothing

            If IsGuid(InvoiceID) = False Then
                oError = "Invalid InvoiceID"
                Return False
            End If
            InvoiceID = ReadyGuid(InvoiceID)

            If Sections Is Nothing Then
                oError = "Please check at least 1 section."
                Return False
            Else
                For Each pSection As SectionClass In Sections
                    If pSection.Verify(oError) = False Then
                        Return False
                    End If
                Next pSection
            End If

            Return True

        End Function

        Public Class SectionClass
            Public FowstID As String
            Public SectionID As String

            Public Function Verify(ByRef oError As String) As Boolean

                oError = Nothing

                If IsGuid(FowstID) = False Then
                    oError = "Invoice FowstID"
                    Return False
                End If
                FowstID = ReadyGuid(FowstID)

                If IsGuid(SectionID) = False Then
                    oError = "Invalid SectionID"
                    Return False
                End If
                SectionID = ReadyGuid(SectionID)

                Return True

            End Function
        End Class
    End Class
End Class

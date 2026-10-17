Imports System.Net
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Public Class PimImporter
    Dim mOrgID As String
    Public Business As Business

    Public Function GetFromNetSuite() As PimFromNetSuite

        Dim pWeb As WebClient
        Dim pDownloadResult As String
        Dim pPimFromNetSuite As PimFromNetSuite

        Dim pHeader As String
        Const pAccount As String = "3772489"
        Const pEmail As String = "customercare@mobiz-apps.com"
        Const pSignature As String = "Netsuite123"

        pHeader = "NLAuth nlauth_account=" + pAccount + ", nlauth_email=" + pEmail + ", nlauth_signature= " + pSignature
        pWeb = New WebClient()
        pWeb.Headers.Add("Authorization", pHeader)
        pWeb.Headers.Add("Content-Type", "application/json")
        pDownloadResult = pWeb.DownloadString("https://rest.netsuite.com/app/site/hosting/restlet.nl?script=199&deploy=1")
        pPimFromNetSuite = JsonConvert.DeserializeObject(Of PimFromNetSuite)(pDownloadResult)

        Return pPimFromNetSuite

    End Function

    Public Class PimFromNetSuite
        Inherits List(Of Item)

        Public Overloads Function Add(vInternalID As String, vDescription As String) As Item

            Dim pItem As Item

            pItem = New Item
            pItem.internalid = vInternalID
            pItem.description = vDescription
            MyBase.Add(pItem)

            Return pItem

        End Function

        Public Class Item
            Public internalid As String
            Public description As String
        End Class
    End Class

    Public Function ImportFromNetSuite() As Integer

        Dim pPimFromNetSuite As PimFromNetSuite

        pPimFromNetSuite = GetFromNetSuite()
        Return ImportFromObject(pPimFromNetSuite)

    End Function

    Public Sub SetOrgID(vOrgID As String)

        Dim pDR As DataRow

        mOrgID = vOrgID
        pDR = Business.LibraryOrganization.ByID(mOrgID)
        If pDR Is Nothing Then
            Throw New Exception("Org, " & mOrgID & ", does not exist.")
        End If
        If pDR("Database") Is DBNull.Value Then
            Throw New Exception("Org, " & mOrgID & ", is not a subscriber.")
        End If
        Business.ProData.ExecuteSQL("USE " & EscapeSqlName(pDR("Database")))

    End Sub

    Public Function ImportFromObject(vObj As PimFromNetSuite) As Integer

        For Each pPimItem As PimFromNetSuite.Item In vObj
            Business.LibraryStock.AddPim(mOrgID, pPimItem.internalid, pPimItem.description)
        Next pPimItem

        Return vObj.Count

    End Function

    Public Function ImportFromExcel(vFilename As String) As Integer

        Dim pDT As DataTable
        Dim pError As String
        Dim pPimFromNetSuite As PimFromNetSuite
        Dim pCount As Integer

        pDT = ExcelToTable(vFilename, {"Internal ID", "Name"}, pError)
        If String.IsNullOrWhiteSpace(pError) = False Then
            Throw New ApplicationException(pError)
        End If
        pPimFromNetSuite = New PimFromNetSuite()
        For Each pDR As DataRow In pDT.Rows
            pPimFromNetSuite.Add(CStr2(pDR("Internal ID")), CStr2(pDR("Name")))
        Next pDR
        pCount = ImportFromObject(pPimFromNetSuite)

        Return pCount

    End Function
End Class

Namespace WebProj
    Public Class CountryController
        Inherits System.Web.Mvc.Controller

        Function Index() As ActionResult

            Return View()

        End Function

        Function CountryInfo() As ActionResult
            Return View()
        End Function

        Function CountryLanguage() As ActionResult
            Return View()
        End Function

        Function CountrySummary() As ActionResult
            Return View()
        End Function

        Function SearchCountry() As ActionResult
            Return View()
        End Function

        <HttpPost()>
        Function setCreateNew() As String
            Dim blnIsNew As Boolean
            Dim strReturn As String

            blnIsNew = Common.WebRequest.Data("IsNew")
            WebSession.Instance.IsNew = Convert.ToInt16(blnIsNew)
            WebSession.Instance.CountryID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("IsNew", IIf(blnIsNew, "TRUE", "FALSE"))

            Return strReturn

        End Function

        <HttpPost()>
        Function IsNewEntry() As String
            Dim pOut As New JsonDictionary

            pOut.Add("IsNew", IIf(Convert.ToBoolean(WebSession.Instance.IsNew), "TRUE", "FALSE"))
            pOut.Add("CountryID", WebSession.Instance.CountryID)

            Return pOut.Serialize

        End Function

        <HttpPost()>
        Function CheckCountryUniqueName() As String
            Dim clsCountry As New Country
            Dim strReturn As String

            clsCountry.ID = Common.WebRequest.Data("CountryID")
            clsCountry.CountryName = Common.WebRequest.Data("CountryName")

            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.CheckCountryUniqueName())

            Return strReturn

        End Function

        <HttpPost()>
        Function getFullCountryList() As String
            Dim strKeyWord As String
            Dim clsCountry As New Country
            Dim strReturn As String

            strKeyWord = Common.WebRequest.Data("SearchString")
            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.getCountryList(strKeyWord))

            Return strReturn

        End Function

        <HttpPost()>
        Function getCountryListAll() As String
            Dim strKeyWord As String
            Dim clsCountry As New Country
            Dim strReturn As String

            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.getCountryListAll())

            Return strReturn

        End Function

        <HttpPost()>
        Function getCountryInfo() As String
            Dim strID As String
            Dim clsCountry As New Country
            Dim strReturn As String

            strID = Common.WebRequest.Data("CountryID")
            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.getCountryInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveCountryInfo() As String
            Dim clsCountry As New Country
            Dim strReturn As String

            If (Common.WebRequest.Data("NumericCode").Trim().Length > 0) Then
                strReturn = ""
                If (Not IsNumeric(Common.WebRequest.Data("NumericCode"))) Then
                    strReturn = JsonDictionary.Serialize("CountryData", "NumericCodeInvalid")
                    Return strReturn
                End If
            End If

            clsCountry.ID = Common.WebRequest.Data("CountryID")
            clsCountry.Code = Common.WebRequest.Data("Code")
            clsCountry.CountryName = Common.WebRequest.Data("CountryName")
            clsCountry.Alpha2Code = Common.WebRequest.Data("Alpha2Code")
            clsCountry.NumericCode = Common.WebRequest.Data("NumericCode")
            clsCountry.ISOCode = Common.WebRequest.Data("ISOCode")
            clsCountry.CallingCode = Common.WebRequest.Data("CallingCode")

            WebSession.Instance.CountryID = clsCountry.SaveCountryInfo()
            strReturn = JsonDictionary.Serialize("CountryData", WebSession.Instance.CountryID)

            Return strReturn

        End Function

        Function getCountryList(ByVal Keywords As String) As String

            Dim clsCountry As New Country

            Return DataTableToJSON.GetJson(clsCountry.getCountryList(Keywords))

        End Function

        <HttpPost()>
        Function CheckCountryLanguage() As String
            Dim clsCountry As New Country
            Dim strReturn As String

            clsCountry.ID = Common.WebRequest.Data("ID")
            clsCountry.CountryID = WebSession.Instance.CountryID
            clsCountry.CountryLanguage = Common.WebRequest.Data("CountryLanguage")

            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.CheckCountryLanguage())

            Return strReturn

        End Function

        <HttpPost()>
        Function getCountryLanguageInfo() As String
            Dim strID As String
            Dim clsCountry As New Country
            Dim strReturn As String

            strID = Common.WebRequest.Data("ID")
            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.getCountryLanguageInfo(strID))

            Return strReturn

        End Function

        <HttpPost()>
        Function getCountryLanguageList() As String
            Dim strID As String
            Dim clsCountry As New Country
            Dim strReturn As String

            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.getCountryLanguageList(WebSession.Instance.CountryID))

            Return strReturn

        End Function

        <HttpPost()>
        Function SaveCountryLanguage() As String
            Dim clsCountry As New Country
            Dim strReturn As String

            clsCountry.ID = Common.WebRequest.Data("ID")
            clsCountry.CountryID = WebSession.Instance.CountryID
            clsCountry.CountryLanguage = Common.WebRequest.Data("CountryLanguage")

            strReturn = JsonDictionary.Serialize("CountryData", clsCountry.SaveCountryLanguageName())

            Return strReturn

        End Function

        <HttpPost()>
        Function RemoveCountryLanguage() As String
            Dim clsCountry As New Country
            Dim strReturn As String

            clsCountry.ID = Common.WebRequest.Data("ID")

            strReturn = JsonDictionary.Serialize("CountryData", IIf(clsCountry.RemoveCountryLanguage(), "TRUE", "FALSE"))

            Return strReturn

        End Function

    End Class
End Namespace
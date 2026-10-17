Public Class ConfirmEmployment
    Public Property OrgID As String
    Public Property PeopleID As String


    Function CheckIfAlreadyConfirm() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder

        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolorg.ID"
                End With

                With .Add("IsConfirmed", SqlDbType.UniqueIdentifier)
                    .Expression = "ppolorg.IsConfirmed"
                End With
            End With

            .SqlFrom = " FROM dbo.MobizLibraryPeopleOrganization ppolorg "

            .SqlWhere = "WHERE ppolorg.PeopleID = " & ReadyString(PeopleID) &
                        " AND ppolorg.OrganizationID = " & ReadyString(OrgID)

            pDT = .Requery

        End With

        Return pDT
    End Function
End Class

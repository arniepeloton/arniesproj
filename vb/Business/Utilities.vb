Public Class Utilities

    ReadOnly mProData As ProData
    ReadOnly mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function HasWholesaleDatabase() As Boolean

        Dim pList As List(Of String)

        pList = mProData.GetDatabases(True)
        Dim p = From pStr As String In pList
            Where pStr = "wholesale"

        Return CType(p, IEnumerable(Of String)).Count > 0

    End Function
End Class
Public Class DictionaryBinderFields
    Inherits Dictionary(Of String, DictionaryBinderField)

    Public Overloads Function Add(vKey As String, Optional vRequired As Boolean = False, Optional vDataType As SqlDbType = SqlDbType.VarChar,
        Optional vDestinationField As String = Nothing,
        Optional vLength As Integer = 0) As DictionaryBinderField

        Dim pOut As DictionaryBinderField

        pOut = New DictionaryBinderField
        pOut.Key = vKey
        pOut.Required = vRequired
        pOut.DataType = vDataType
        pOut.DestinationField = vDestinationField
        pOut.Length = vLength
        MyBase.Add(vKey, pOut)

        Return pOut

    End Function
End Class

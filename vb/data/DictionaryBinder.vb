Public Class DictionaryBinder

    ReadOnly mFields As DictionaryBinderFields
    Public Source As Dictionary(Of String, String)

    Public Sub New()

        mFields = New DictionaryBinderFields

    End Sub

    Default Public ReadOnly Property Item(vKey As String) As DictionaryBinderField
        Get

            Return mFields(vKey)

        End Get
    End Property

    Public ReadOnly Property Items As DictionaryBinderFields
        Get

            Return mFields

        End Get
    End Property

    Public Function CreateInsertUpdate(ByRef oError As String) As InsertUpdate

        Return CreateInsertUpdate(Source, oError)

    End Function

    Public Function CreateInsertUpdate(ByVal vDR As DataRow, ByRef oError As String) As InsertUpdate

        Return CreateInsertUpdate(JsonDictionary.RowToDictionary(vDR), oError)

    End Function

    Private Function CreateInsertUpdate(ByVal vSource As Dictionary(Of String, String), ByRef oError As String) As InsertUpdate

        Dim pIU As InsertUpdate
        Dim pValue As String

        pIU = New InsertUpdate
        For Each pField As DictionaryBinderField In mFields.Values
            If pField.Validate(vSource, pValue, oError, False, pField.Length) = False Then
                Return Nothing
            End If
            If pValue IsNot Nothing Then
                pIU(pField.DestinationFieldResolved) = pValue
            End If
        Next pField

        Return pIU

    End Function

    Public Function CreateDataTable() As DataTable

        Dim pOut As DataTable

        pOut = New DataTable
        For Each pItem As DictionaryBinderField In mFields.Values
            pOut.Columns.Add(pItem.DestinationFieldResolved, SQLDBType2Type(pItem.DataType))
        Next pItem

        Return pOut

    End Function

    Public Function CreateDataRow(ByVal vDT As DataTable, Optional ByRef oError As String = Nothing) As DataRow

        Dim pDR As DataRow
        Dim pValue As String

        pDR = vDT.NewRow
        For Each pField As DictionaryBinderField In mFields.Values
            If pField.Validate(Source, pValue, oError, True, pField.Length) = False Then
                Return Nothing
            End If
            If pValue IsNot Nothing Then
                pDR(pField.DestinationFieldResolved) = CTypeDynamic(pValue, SQLDBType2Type(pField.DataType))
            End If
        Next pField

        Return pDR

    End Function

    Public Function Validate(ByRef oError As String) As Boolean

        oError = ""
        For Each pField As DictionaryBinderField In mFields.Values
            If pField.Validate(Source, Nothing, oError, True, pField.Length) = False Then
                If pField.DataType = SqlDbType.UniqueIdentifier _
                    AndAlso Source.ContainsKey(pField.Key) _
                    AndAlso CStr2(Source(pField.Key)).Trim.ToLower() = "null" Then

                    Source.Remove(pField.Key)
                    If pField.Validate(Source, Nothing, oError, True, pField.Length) = False Then
                        Return False
                    End If
                Else
                    Return False
                End If
            End If
        Next pField

        Return True

    End Function
End Class

Public Class Finder

    Public SqlFrom As String
    Public SqlWhere As String
    ''' <summary>
    ''' must include the "GROUP BY" phrase
    ''' </summary>
    ''' <remarks></remarks>
    Public SqlGroup As String
    Public SqlHaving As String
    ''' <summary>
    ''' include the "ORDER BY" phrase
    ''' </summary>
    ''' <remarks></remarks>
    Public SqlOrder As String

    Dim mFields As New FinderFields
    Dim mSqlSelect As String

    Public ProData As ProData
    Public SearchString As String = ""
    Public MaxRecords As Integer ' = 500

    Public Enum eSortMode
        None
        Sql
        FieldName
    End Enum

    Public ReadOnly Property Fields() As FinderFields
        Get

            Return mFields

        End Get
    End Property

    Public Function Requery() As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        If mFields.Count = 0 Then
            Throw New Exception("Must have at least 1 FindField")
        End If

        pSql = CreateSql()
        pDT = ProData.Execute(pSql)

        Return pDT

    End Function

    ''' <summary>
    ''' Creates an SQL based on the textbox text and the combobox field
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateSql(Optional ByVal vSortMode As eSortMode = eSortMode.None) As String

        Dim pField As FinderField
        Dim pWhere As String = ""
        Dim pStr As String
        Dim pFindField As String = ""
        Dim pStrs() As String
        Dim p As Integer

        If CStr2(SearchString).Length > 0 Then
            For Each pField In mFields.Values
                If pField.Searchable Then

                    If CStr2(pField.Expression).Length > 0 Then
                        pStr = pField.Expression
                    Else
                        pStr = pField.FieldName
                    End If

                    If pFindField.Length > 0 Then pFindField &= " + "
                    If pField.MDT = eMajorDataType.String Then
                        pFindField &= "ISNULL(" & pStr & ", '')"
                    Else
                        pFindField &= "ISNULL(CONVERT(varchar, " & pStr & "), '')"
                    End If
                End If
            Next pField

            pStrs = SplitGroup(LeanString(OptimizeKeywords(SearchString)), " ", """").ToArray

            For p = 0 To pStrs.Length - 1
                If pWhere.Length > 0 Then pWhere &= " AND "
                pWhere &= pFindField & " LIKE " & ReadyString("%" & pStrs(p) & "%")
            Next p

            If CStr(SqlWhere & "").Length > 0 Then
                pWhere = SqlWhere & " AND (" & pWhere & ")"
            ElseIf pWhere.Length > 0 Then
                pWhere = " WHERE " & pWhere
            Else
                Return ""
            End If
        Else
            pWhere = SqlWhere
        End If

        CreateSqlSelect()
        pStr = mSqlSelect & " " & _
            SqlFrom & " " & _
            pWhere & " " & _
            SqlGroup & " " & _
            If(CStr2(SqlOrder).Length > 0, " " & SqlOrder, "") & _
            SqlHaving
        If vSortMode = eSortMode.Sql Then
            pStr &= " " & SqlOrder
        ElseIf vSortMode = eSortMode.FieldName Then
            pStr = "SELECT * FROM (" & pStr & ") t0 " & SqlOrder
        End If

        Return pStr

    End Function

    Private Sub CreateSqlSelect()

        Dim pSelect As String = ""
        Dim pStr As String
        Dim pField As FinderField

        For Each pField In mFields.Values
            pStr = ""
            If CStr(pField.Expression & "").Length Then
                pStr = pField.Expression & " AS "
            End If
            pStr &= "[" & pField.FieldName & "]"
            If pSelect.Length Then pSelect &= ", "
            pSelect &= pStr
        Next pField

        If MaxRecords = 0 Then
            mSqlSelect = "SELECT TOP 100 PERCENT " & pSelect
        Else
            mSqlSelect = "SELECT TOP " & MaxRecords & " " & pSelect
        End If

    End Sub

    Public Function GetPaging(ByVal vSql As String, ByVal vStartRecord As Integer, ByVal vMaxRecords As Integer) As DataTable

        Return ProData.Execute(GetPagingSql(vSql, vStartRecord, vMaxRecords))

    End Function

    Public Function GetPagingSql(ByVal vSql As String, ByVal vStartRecord As Integer, ByVal vMaxRecords As Integer) As String

        Dim pPattern, pPattern2 As String
        Dim pOrderMatch As Match
        Dim pOrders() As String
        Dim pOrder As String
        Dim pOrderClauseInner As String = ""
        Dim pOrderClauseMiddle As String = ""
        Dim pOrderClauseOuter As String = ""
        Dim pOrderSql As OrderSql
        Dim pField As FinderField

        If vStartRecord < 0 Then Throw New ArgumentException("vStartRecord must be zero or greater", "vStartRecord")

        If CStr2(SqlOrder).Length = 0 Then
            Throw New ArgumentNullException("SqlOrder")
        End If

        pPattern = "^" & _
            " select \s+ (?<Top>(?:top)? \s+ \d+ \s+ (?:percent)?)? " & _
            " (?<Body> .+) " & _
            " \s*" & _
            " \z"

        pPattern2 = " ^ " & _
            " \s* (?<Field>\w+) " & _
            " (?<Sort> (\s+ (?:desc|asc))?) " & _
            " \s* \z"

        '// cache order fields and expressions
        pOrders = SqlOrder.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries)
        For Each pOrder In pOrders
            For Each pField In mFields.Values
                pOrderMatch = Regex.Match(pOrder, pPattern2, RegexOptions.IgnorePatternWhitespace Or RegexOptions.IgnoreCase)
                If pOrderMatch.Groups("Field").Success _
                    AndAlso pField.FieldName.Trim.ToLower = pOrderMatch.Groups("Field").Value.Trim.ToLower Then

                    pOrderSql = New OrderSql
                    'no need to use pField.Expression coz SQL Server now accepts alias on ORDER clause
                    'pOrderSql.Expression = pField.Expression
                    pOrderSql.Expression = pField.FieldName
                    pOrderSql.Field = pOrderMatch.Groups("Field").Value
                    pOrderSql.Sort = pOrderMatch.Groups("Sort").Value.Trim.ToUpper
                    If pOrderSql.Sort = "ASC" Then pOrderSql.Sort = ""

                    '// Create inner order clause
                    If pOrderClauseInner.Length > 0 Then pOrderClauseInner &= ", "
                    pOrderClauseInner &= (pOrderSql.Expression & " " & pOrderSql.Sort).Trim

                    '// Create middle order clause
                    If pOrderClauseMiddle.Length Then pOrderClauseMiddle &= ", "
                    pOrderClauseMiddle &= Trim(pOrderSql.Field & " " & IIf(pOrderSql.Sort = "DESC", "", "DESC")).Trim

                    '// Create outer order clause
                    If pOrderClauseOuter.Length > 0 Then pOrderClauseOuter &= ", "
                    pOrderClauseOuter &= (pOrderSql.Field & " " & pOrderSql.Sort).Trim

                    Exit For
                End If
            Next pField
        Next pOrder

        Dim pSqlInner As String
        Dim pSqlOuter As String
        Dim pSqlMiddle As String

        '// Create inner sql
        'pSqlInner = Regex.Replace(vSql, pPattern, "SELECT TOP " & vMaxRecords + vStartRecord - 1 & " ${Body} ORDER BY ", _
        '    RegexOptions.IgnorePatternWhitespace Or RegexOptions.IgnoreCase) & pOrderClauseInner
        'no need to append pOrderClauseInner coz ${Body} contains ORDER clause
        pSqlInner = Regex.Replace(vSql, pPattern, "SELECT TOP " & vMaxRecords + vStartRecord - 1 & " ${Body}", _
            RegexOptions.IgnorePatternWhitespace Or RegexOptions.IgnoreCase)

        '// Create middle sql
        pSqlMiddle = "SELECT TOP " & vMaxRecords & " * FROM (" & pSqlInner & ") t1" & _
            " ORDER BY " & pOrderClauseMiddle

        '// Create outer sql
        pSqlOuter = "SELECT * FROM (" & pSqlMiddle & ") t2" & _
            " ORDER BY " & pOrderClauseOuter

        Return pSqlOuter

    End Function

    Public Function GetPaging2(ByVal vSql As String, ByVal vPage As Integer, ByVal vMaxRecords As Integer) As DataTable

        Return ProData.Execute(GetPagingSql2(vSql, vPage, vMaxRecords))

    End Function

    Public Function GetPagingSql2(ByVal vSql As String, ByVal vPage As Integer, ByVal vMaxRecords As Integer) As String

        Return GetPagingSql(vSql, (vPage - 1) * vMaxRecords + 1, vMaxRecords)

    End Function

    Private Class OrderSql
        Public Field As String
        Public Sort As String
        Dim mExpression As String

        Public Property Expression() As String
            Get

                If CStr2(mExpression).Length > 0 Then
                    Return mExpression
                Else
                    Return Field
                End If

            End Get
            Set(ByVal value As String)

                mExpression = value

            End Set
        End Property
    End Class
End Class

Public Class FinderField

    Public FieldName As String
    ''' <summary>
    ''' If this is not blank, ProFind will use this property instead of FieldName
    ''' </summary>
    ''' <remarks></remarks>
    Public Expression As String
    Public Format As String
    ''' <summary>
    ''' Major data type
    ''' </summary>
    ''' <remarks></remarks>
    Dim mMDT As eMajorDataType = eMajorDataType.String
    Dim mDataType As SqlDbType
    ''' <summary>
    ''' Determines if to be shown on the field combobox
    ''' </summary>
    ''' <remarks></remarks>
    Public Searchable As Boolean = True

    Public ReadOnly Property MDT() As eMajorDataType
        Get

            Return mMDT

        End Get
    End Property

    Public Property DBType() As SqlDbType
        Get

            Return mDataType

        End Get
        Set(ByVal value As SqlDbType)

            mDataType = value
            mMDT = MajorDataType(value)

        End Set
    End Property
End Class

Public Class FinderFields
    Inherits Generic.Dictionary(Of String, FinderField)

    Shadows Function Add(ByRef rFindField As FinderField) As FinderField

        If Not rFindField.FieldName Is Nothing AndAlso rFindField.FieldName.Length Then
            MyBase.Add(rFindField.FieldName, rFindField)
        Else
            MyBase.Add(Nothing, rFindField)
        End If

        Return rFindField

    End Function

    Shadows Function Add(ByVal vFieldName As String, ByVal vDBType As SqlDbType) As FinderField

        Dim pFindField As New FinderField

        pFindField.FieldName = vFieldName
        pFindField.DBType = vDBType

        MyBase.Add(vFieldName, pFindField)

        Return pFindField

    End Function

    Default Shadows Property Item(ByVal vIndex As Integer) As FinderField
        Get

            Return MyBase.Item(vIndex)

        End Get
        Set(ByVal Value As FinderField)

            MyBase.Item(vIndex) = Value

        End Set
    End Property

    Default Shadows Property Item(ByVal vKey As String) As FinderField
        Get

            Return MyBase.Item(vKey)

        End Get
        Set(ByVal Value As FinderField)

            MyBase.Item(vKey) = Value

        End Set
    End Property

#If _MyType = "Web" Then
    'Public Function ToJson() As JsonNode

    '    Dim pOut As JsonNode
    '    Dim pKey As String
    '    Dim pValue As JsonNode

    '    pOut = New JsonNode
    '    For Each pKey In Me.Keys
    '        pValue = New JsonNode
    '        pValue.Add("Name", Me(pKey).FieldName)
    '        pValue.Add("SqlType", Me(pKey).DBType.ToString)
    '        pOut.Add(pKey, pValue)
    '    Next pKey

    '    Return pOut

    'End Function
#End If
End Class

Public Enum DisplayStyle
    Label
    CheckBox
    LabelButton
    TextBox
    TextBoxButton
    Hidden
    Link
End Enum
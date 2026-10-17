Public Class DictionaryBinderField

    Public Key As String
    Public Required As Boolean
    Public DataType As SqlDbType
    Public AllowZero As Boolean
    Public AllowNegative As Boolean
    Public DestinationField As String
    Public Length As Integer

    Public Sub New()

        DataType = SqlDbType.VarChar

    End Sub

    Public Function Validate(ByVal vSource As Dictionary(Of String, String), ByRef oOutput As String, ByRef oError As String, vDontQuote As Boolean, vMaxLength As Integer) As Boolean

        oError = Nothing
        If vSource.ContainsKey(Key) = False Then
            If Required Then
                oError = Key & " is required."
                Return False
            Else
                oOutput = Nothing
                Return True
            End If
        End If
        oOutput = vSource(Key)
        If oOutput Is Nothing Then
            If Required Then
                oError = Key & " is required."
                Return False
            Else
                Return True
            End If
        End If
        If CStr2(oOutput) = "" Then
            If Required = False Then
                oOutput = Nothing
                Return True
            End If
        End If

        Return VerifyType.ParseData(oOutput, Key, DataType, Not Required, oOutput, oError, , , vDontQuote, vMaxLength)
        'Select Case MajorDataType(DataType)
        '    Case eMajorDataType.Date, eMajorDataType.String
        '        oOutput = ReadyString(oOutput)

        '    Case eMajorDataType.Numeric

        '    Case Else
        '        Throw New InvalidCastException

        'End Select

    End Function

    Public ReadOnly Property DestinationFieldResolved As String
        Get

            If CStr2(DestinationField).Trim() = "" Then
                Return Key
            Else
                Return DestinationField
            End If

        End Get
    End Property
End Class

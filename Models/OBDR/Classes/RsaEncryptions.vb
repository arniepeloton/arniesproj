Imports System.Security.Cryptography
Imports System.IO
Imports System.Text

Public Class RsaEncryptions
    Implements InterfaceRsaEncryptions

    Private Structure KeyInformation
        Public publicKey As String
        Public privateKey As String
        Public keySourcePath As String
    End Structure
    Private keyInformations As KeyInformation
    Private textEncoder As UTF8Encoding
    Private rsaServiceProvider As RSACryptoServiceProvider
    Private exceptionError As Exception

    Public Sub New()
        keyInformations = New KeyInformation
        rsaServiceProvider = New RSACryptoServiceProvider
    End Sub

#Region "Property"

    Public Property setGetPrivateKey As String Implements InterfaceRsaEncryptions.setGetPrivateKey
        Get
            Return keyInformations.privateKey
        End Get
        Set(ByVal value As String)
            keyInformations.privateKey = value
        End Set
    End Property

    Public Property setGetPublicKey As String Implements InterfaceRsaEncryptions.setGetPublicKey
        Get
            Return keyInformations.publicKey
        End Get
        Set(ByVal value As String)
            keyInformations.publicKey = value
        End Set
    End Property

    Public Property setGetError As System.Exception Implements InterfaceRsaEncryptions.setGetError
        Get
            Return exceptionError
        End Get
        Set(ByVal value As System.Exception)
            exceptionError = value
        End Set
    End Property

#End Region

#Region "Sub / Function"
    Public Function checkSecurityKeys() As Boolean Implements InterfaceRsaEncryptions.checkSecurityKeys
        Try
            Dim secretPath As String = "\SecurityKeys.SYS"
            'Dim secretPath As String = "C:\SecurityKeys.SYS"
            Dim streamWriter As StreamWriter
            Dim streamReader As StreamReader
            Dim temporatyStorage As String()

            If File.Exists(secretPath) Then
                'Existing then read keys
                streamReader = File.OpenText(secretPath)
                With streamReader
                    temporatyStorage = .ReadLine().ToString.Split(": ")
                    setGetPrivateKey = Trim(temporatyStorage(1))
                    temporatyStorage = .ReadLine().ToString.Split(": ")
                    setGetPublicKey = Trim(temporatyStorage(1))
                    .Close()
                End With
                Return True
            Else
                'create key file
                generateSecurityKeys()
                streamWriter = File.CreateText(secretPath)
                With streamWriter
                    .WriteLine("PrivateKeys: " & keyInformations.privateKey)
                    .WriteLine("PublicKeys: " & keyInformations.publicKey)
                    .Flush()
                    .Close()
                End With
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function generateSecurityKeys() As Boolean Implements InterfaceRsaEncryptions.generateSecurityKeys
        setGetPublicKey = rsaServiceProvider.ToXmlString(False)
        setGetPrivateKey = rsaServiceProvider.ToXmlString(True)
        Return True
    End Function

    Public Function decryptMessage(ByVal encryptedMessage As String) As Object Implements InterfaceRsaEncryptions.decryptMessage
        Dim textToBytes As Byte()
        Dim textEncoder As New UTF8Encoding

        Try
            rsaServiceProvider.FromXmlString(keyInformations.privateKey)
            textToBytes = Convert.FromBase64String(encryptedMessage)
            textToBytes = rsaServiceProvider.Decrypt(textToBytes, False)
            Return textEncoder.GetString(textToBytes)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function encryptMessage(ByVal plainMessage As String, ByVal senderPublicKey As String) As Object Implements InterfaceRsaEncryptions.encryptMessage
        Dim textToBytes As Byte()
        Dim textEncoder As New UTF8Encoding

        Try
            rsaServiceProvider.FromXmlString(senderPublicKey)
            textToBytes = textEncoder.GetBytes(plainMessage)
            textToBytes = rsaServiceProvider.Encrypt(textToBytes, False)
            Return Convert.ToBase64String(textToBytes)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function encryptMessage(ByVal byteMessage As Byte(), ByVal senderPublicKey As String) As Byte()
        Dim textToBytes As Byte()
        Dim textEncoder As New UTF8Encoding

        Try
            rsaServiceProvider.FromXmlString(senderPublicKey)
            textToBytes = rsaServiceProvider.Encrypt(byteMessage, False)
            Return textToBytes
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function decryptMessage(ByVal encryptedBytes As Byte()) As Byte()
        Dim textToBytes As Byte()
        Dim textEncoder As New UTF8Encoding

        Try
            rsaServiceProvider.FromXmlString(keyInformations.privateKey)
            textToBytes = rsaServiceProvider.Decrypt(encryptedBytes, False)
            Return textToBytes
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region

    
End Class

Public Interface InterfaceRsaEncryptions
    'Propery
    Property setGetPublicKey As String
    Property setGetPrivateKey As String
    Property setGetError As Exception

    'Sub / Function
    'Local Keys
    Function checkSecurityKeys() As Boolean
    Function encryptMessage(ByVal plainMessage As String, ByVal senderPublicKey As String) As Object
    Function decryptMessage(ByVal encryptedMessage As String) As Object
    Function generateSecurityKeys() As Boolean
End Interface

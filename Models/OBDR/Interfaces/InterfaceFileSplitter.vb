Public Interface InterfaceFileSplitter
    'Property
    Property setGetSourceDirectory As String
    Property setGetSourceFile As String
    Property setGetSourceFileSize As Long
    Property setGetDestinationDirectory As String
    Property setGetDestinationFile As String
    Property setGetSourceSplittedFileSize As Long
    Property setGetSourceExpectedSplitFileSize As Long
    Property setGetSourceByteDirectory As Dictionary(Of Integer, Byte())
    'Sub / Functions
    Function splitFileBySize() As Dictionary(Of Integer, Byte())
    Function mergeFileBySize() As Byte()
    'Events
End Interface

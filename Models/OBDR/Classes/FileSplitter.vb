Imports System.IO

Public Class FileSplitter
    Implements InterfaceFileSplitter

    Private Structure FileInformation
        Public sourceDirectory As String
        Public sourceFile As String
        Public sourceFileSize As Long
        Public destinationDirectory As String
        Public destinationFile As String
        Public sourceFileBytes() As Byte
        Public destinationFileBytes() As Byte
        Public sourceSplittedFileSize As Long
        Public sourceExpectedSplitFileSize As Long
        Public sourceSplittedEndFileSize As Long
        Public destinationSplittedFileSize As Long
        Public sourceByteDirectory As Dictionary(Of Integer, Byte())
    End Structure
    Private fileInformations As FileInformation

    Public Sub New()
        fileInformations = New FileInformation
        fileInformations.sourceByteDirectory = New Dictionary(Of Integer, Byte())
    End Sub

#Region "Property"
    Public Property setGetSourceFileSize As Long Implements InterfaceFileSplitter.setGetSourceFileSize
        Get
            Return fileInformations.sourceFileSize
        End Get
        Set(ByVal value As Long)
            fileInformations.sourceFileSize = value
        End Set
    End Property

    Public Property setGetDestinationDirectory As String Implements InterfaceFileSplitter.setGetDestinationDirectory
        Get
            Return fileInformations.destinationDirectory
        End Get
        Set(ByVal value As String)
            fileInformations.destinationDirectory = value
        End Set
    End Property

    Public Property setGetDestinationFile As String Implements InterfaceFileSplitter.setGetDestinationFile
        Get
            Return fileInformations.destinationFile
        End Get
        Set(ByVal value As String)
            fileInformations.destinationFile = value
        End Set
    End Property

    Public Property setGetSourceDirectory As String Implements InterfaceFileSplitter.setGetSourceDirectory
        Get
            Return fileInformations.sourceDirectory
        End Get
        Set(ByVal value As String)
            fileInformations.sourceDirectory = value
        End Set
    End Property

    Public Property setGetSourceFile As String Implements InterfaceFileSplitter.setGetSourceFile
        Get
            Return fileInformations.sourceFile
        End Get
        Set(ByVal value As String)
            fileInformations.sourceFile = value
        End Set
    End Property

    Public Property setGetSourceSplittedFileSize As Long Implements InterfaceFileSplitter.setGetSourceSplittedFileSize
        Get
            Return fileInformations.sourceSplittedFileSize
        End Get
        Set(ByVal value As Long)
            fileInformations.sourceSplittedFileSize = value
        End Set
    End Property

    Public Property setGetSourceExpectedSplitFileSize As Long Implements InterfaceFileSplitter.setGetSourceExpectedSplitFileSize
        Get
            Return fileInformations.sourceExpectedSplitFileSize
        End Get
        Set(ByVal value As Long)
            fileInformations.sourceExpectedSplitFileSize = value
        End Set
    End Property
    Public Property setGetSourceByteDirectory As System.Collections.Generic.Dictionary(Of Integer, Byte()) Implements InterfaceFileSplitter.setGetSourceByteDirectory
        Get
            Return fileInformations.sourceByteDirectory
        End Get
        Set(ByVal value As System.Collections.Generic.Dictionary(Of Integer, Byte()))
            fileInformations.sourceByteDirectory = value
        End Set
    End Property
#End Region

#Region "Sub / Functions"
    Public Function mergeFileBySize() As Byte() Implements InterfaceFileSplitter.mergeFileBySize
        Dim indexCounter As Integer = 0
        Dim mergeBytes() As Byte = Nothing
        Dim currentIndex As Long
        Dim currentSizeIndex As Long

        ReDim mergeBytes(fileInformations.sourceFileSize - 1)
        With fileInformations
            While indexCounter < setGetSourceByteDirectory.Count
                If indexCounter = 0 Then
                    currentIndex = 0
                    currentSizeIndex += setGetSourceByteDirectory(indexCounter).Length
                Else
                    currentIndex += setGetSourceByteDirectory(indexCounter - 1).Length
                    currentSizeIndex += setGetSourceByteDirectory(indexCounter).Length
                End If
                mergeBytes.ConstrainedCopy(setGetSourceByteDirectory(indexCounter), 0, mergeBytes, currentIndex, setGetSourceByteDirectory(indexCounter).Length)
                indexCounter += 1
            End While
        End With
        Return mergeBytes
    End Function

    Public Function splitFileBySize() As System.Collections.Generic.Dictionary(Of Integer, Byte()) Implements InterfaceFileSplitter.splitFileBySize
        Dim temporaryFileBytes() As Byte
        Dim sizeCounter As Long = 0
        Dim fullsourcePath As String = setGetSourceDirectory & setGetSourceFile

        With fileInformations
            .sourceFileBytes = File.ReadAllBytes(fullsourcePath)
            .sourceFileSize = .sourceFileBytes.Length
            setGetSourceSplittedFileSize = Math.Floor(.sourceFileSize / setGetSourceExpectedSplitFileSize)
            .sourceSplittedEndFileSize = .sourceFileSize Mod setGetSourceExpectedSplitFileSize

            While sizeCounter < setGetSourceSplittedFileSize
                ReDim temporaryFileBytes(setGetSourceExpectedSplitFileSize - 1)
                .sourceFileBytes.Copy(.sourceFileBytes, (sizeCounter * setGetSourceExpectedSplitFileSize), temporaryFileBytes, 0, setGetSourceExpectedSplitFileSize)
                .sourceByteDirectory.Add(sizeCounter, temporaryFileBytes)
                sizeCounter += 1
            End While

            If .sourceSplittedEndFileSize <> 0 Then
                ReDim temporaryFileBytes(.sourceSplittedEndFileSize - 1)
                .sourceFileBytes.Copy(.sourceFileBytes, (sizeCounter * setGetSourceExpectedSplitFileSize), temporaryFileBytes, 0, .sourceSplittedEndFileSize)
                .sourceByteDirectory.Add(sizeCounter, temporaryFileBytes)
            End If

            Return .sourceByteDirectory
        End With
    End Function
#End Region
End Class

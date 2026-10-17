Imports System
Imports System.Threading
Imports System.ComponentModel
Imports System.IO.Ports

Public Class SMS

    Private WithEvents SMSPort As SerialPort

    Private SMSThread As Thread

    Private ReadThread As Thread

    Shared _Continue As Boolean = False

    Shared _ContSMS As Boolean = False

    Private _Wait As Boolean = False

    Shared _ReadPort As Boolean = False

    Public Event Sending(ByVal Done As Boolean)

    Public Event DataReceived(ByVal Message As String)

    Private ID As String

    Public Sub ConnectToDevice(COMPORT As String)

        'initialize all values
        Dim COMMPORT As String = COMPORT

        SMSPort = New SerialPort

        Try
            With SMSPort

                'For Each p As String In IO.Ports.SerialPort.GetPortNames
                '    Dim sp As New IO.Ports.SerialPort(p, 9600, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)
                '    sp.ReadTimeout = 1000
                '    sp.Open()
                '    sp.WriteLine("FOO")
                '    Try
                '        sp.ReadLine()
                '        COMMPORT = sp.PortName
                '        'good port here
                '        Exit For
                '    Catch ex As TimeoutException
                '        Stop
                '    End Try
                '    sp.Close()
                'Next

                .PortName = COMMPORT

                .BaudRate = 19200

                .Parity = Parity.None

                .DataBits = 8

                .StopBits = StopBits.One

                .Handshake = Handshake.RequestToSend

                .DtrEnable = True

                .RtsEnable = True

                .NewLine = vbCrLf

                .Open()
            End With
        Catch ex As Exception

        End Try


    End Sub

    Public Function SendSMS(message As String, sender As String) As Boolean

        'SaveSmS(message, sender)
        Dim dtMessages As DataTable

        ConnectToDevice("COM4")

        Try
            If SMSPort.IsOpen = True Then
                'sending AT commands

                dtMessages = GetAllUnsentMessages()

                For i As Integer = 0 To dtMessages.Rows.Count - 1

                    SMSPort.WriteLine("AT")
                    Thread.Sleep(1000)
                    SMSPort.WriteLine("AT+CMGF=1" & vbCrLf) 'set command message format to text mode(1)
                    Thread.Sleep(1000)
                    SMSPort.Write("AT+CMGS=" & Chr(34) & sender & Chr(34) & vbCrLf)
                    Thread.Sleep(1000)
                    SMSPort.Write(message & Chr(26))
                    Thread.Sleep(5000)

                    Dim pSql As String

                    pSql = "Update TransSmsMessages set SenttoModemTries=1 where ID = " & ReadyString(dtMessages.Rows(i).Item("ID"))

                Next

                Close()

            End If
        Catch ex As Exception
            AppendToLog(ex.Message)
        End Try
        

    End Function

    Public Function GetPortNames()

        ' Get a list of serial port names.
        Dim ports As String() = SerialPort.GetPortNames()

        Console.WriteLine("The following serial ports were found:")

        ' Display each port name to the console.
        Dim port As String
        For Each port In ports
            SMSPort.WriteLine("AT")
            Thread.Sleep(1000)
            SMSPort.WriteLine("AT+CMGF=1" & vbCrLf) 'set command message format to text mode(1)
            Thread.Sleep(1000)
            'SMSPort.Write("AT+CMGS=" & Chr(34) & sender & Chr(34) & vbCrLf)
            Thread.Sleep(1000)
            'SMSPort.Write(message & Chr(26))
            Thread.Sleep(5000)
            'ConnectToDevice("COM4")

            Close()

            Console.WriteLine(port)
        Next port

        Console.ReadLine()

    End Function

    Public Function SaveSmS(Message As String, Sender As String) As Boolean

        Dim pSql As String
        Dim pIU As InsertUpdate


        ID = Guid.NewGuid.ToString

        pIU = New InsertUpdate("TransSmsMessages")

        pIU("SendTo") = ReadyString(Sender)
        pIU("SendFrom") = ReadyString(WebSession.Instance.PeopleUserID)
        pIU("Message") = ReadyString(Message)
        pIU("LastUpdated") = "GETDATE()"
        'pIU("ModemID") = ReadyString(vDateTo)

        pSql = pIU.GetInsert

        Common.WebRequest.ProData.Execute(pSql)

    End Function

    Public Function GetAllUnsentMessages() As DataTable
        Dim pSql As String
        Dim dtMessages As DataTable

        pSql = "Select * from TransSmsMessages where SenttoModemTries = 0"

        dtMessages = Common.WebRequest.ProData.Execute(pSql)

        Return dtMessages

    End Function

    Public Sub Open()

        If Not (SMSPort.IsOpen = True) Then

            SMSPort.Open()

        End If

    End Sub

    Public Sub Close()

        If SMSPort.IsOpen = True Then

            SMSPort.Close()

        End If

    End Sub


    Sub CompileTransaction(ID As String)
        Dim pSql As String

        
    End Sub
End Class

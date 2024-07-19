Imports System.Threading
Imports CyberGearVb.nsConstants
Imports Peak.Can.Basic

Public Class PcanReceiver
    Private receiveEvent As EventWaitHandle
    Private receiveThread As Thread
    Private isRunning As Boolean
    Private channel As PcanChannel
    Private messageReceivedHandler As Action(Of PcanMessage)

    Public Sub New(channel As PcanChannel, messageReceivedHandler As Action(Of PcanMessage))
        Me.channel = channel
        Me.messageReceivedHandler = messageReceivedHandler
        receiveEvent = New EventWaitHandle(False, EventResetMode.AutoReset)
    End Sub

    Public Function Start() As Boolean
        If System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) Then
            ' On Windows operating systems, set the receive event directly.
            If Api.SetValue(channel, PcanParameter.ReceiveEvent, CType(receiveEvent.SafeWaitHandle.DangerousGetHandle().ToInt32(), UInteger)) <> PcanStatus.OK Then
                Console.WriteLine($"Se produjo un error al configurar los eventos de recepcion en el canal: {channel}")
                Api.Uninitialize(channel)
                Return False
            End If
        Else
            ' On non-Windows operating systems, obtain the receive event handle and set it
            Dim eventHandle As UInteger
            If Api.GetValue(channel, PcanParameter.ReceiveEvent, eventHandle) <> PcanStatus.OK Then
                Console.WriteLine($"Se produjo un error al configurar los eventos de recepcion en el canal: {channel}")
                Api.Uninitialize(channel)
                Return False
            End If

            receiveEvent.SafeWaitHandle.Close()
            receiveEvent.SafeWaitHandle = New Microsoft.Win32.SafeHandles.SafeWaitHandle(New IntPtr(eventHandle), False)
        End If

        ' Iniciar hilo de recepcion
        isRunning = True
        receiveThread = New Thread(AddressOf ReceiveThreads)
        receiveThread.Start()

        Console.WriteLine($"{channel} ha sido configurado para la recepcion de eventos")
        Return True
    End Function

    Public Sub [Stop]()
        ' Detener el hilo y limpiar
        isRunning = False
        If receiveThread IsNot Nothing AndAlso receiveThread.IsAlive Then
            receiveThread.Join()
        End If
        Api.Uninitialize(channel)
        Console.WriteLine($"{channel} Se ha detenido")
    End Sub

    Private Sub ReceiveThreads()
        While isRunning
            ' Esperar la señal del evento           
            If receiveEvent.WaitOne(50) Then
                Dim canMessage As PcanMessage = Nothing
                Dim canTimestamp As ULong

                ' Lee y procesa todos los mensajes CAN en el buffer de recepcion.
                While Api.Read(channel, canMessage, canTimestamp) = PcanStatus.OK
                    ' Procesa el mensaje recibido
                    'Console.WriteLine($"Mensaje Recibido: ID=0x{canMessage.ID:X} Data= {BitConverter.ToString(canMessage.Data)}")
                    'Console.WriteLine($"TimeStamp: {canTimestamp}")
                    ' Parse the received message
                    Dim responseType As Byte = CByte((canMessage.ID >> 24) And &HFF)
                    If responseType = CmdModes.SINGLE_PARAM_READ Then
                        Thread.Sleep(30)
                        Dim result = BusCan.AnalitzarMissatgeLecturaParametreUnic(canMessage.Data, canMessage.ID)
                        Console.WriteLine(result.Value)
                    Else
                        Dim result = BusCan.AnalitzarMissatgeRebut(canMessage.Data, canMessage.ID)
                    End If
                    ' Access and print the fields of the ParsedMessage struct
                    'Console.WriteLine($"Feedback del Motor: Motor CAN ID: {result.MotorCanId}, Position: {result.Position} rad, Velocity: {result.Velocity} rad/s, Torque: {result.Torque} Nm")
                    If canMessage IsNot Nothing AndAlso messageReceivedHandler IsNot Nothing Then
                        messageReceivedHandler.Invoke(canMessage)
                    End If
                End While

                ' Reestablecer el evento
                receiveEvent.Reset()
            End If
        End While
    End Sub
End Class

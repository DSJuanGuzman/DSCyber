Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports Peak.Can.Basic
Imports System.Diagnostics
Imports System.Threading

''' <summary>
''' Receptor para mensajes CAN usando el canal Pcan.
''' </summary>
''' <remarks>
''' Maneja la recepción de mensajes CAN y la configuración del evento de recepción.
''' </remarks>
Friend Class PcanReceiver
    Private receiveEvent As EventWaitHandle
    Private receiveThread As Thread
    Private isRunning As Boolean
    Private channel As PcanChannel

    Public Sub New(channel As PcanChannel)
        Me.channel = channel
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
                    Console.WriteLine($"Mensaje Recibido: ID=0x{canMessage.ID:X} Data= {BitConverter.ToString(canMessage.Data)}")
                    Console.WriteLine($"TimeStamp: {canTimestamp}")

                    ' Parse the received message
                    Dim result = BusCan.ParseReceivedMsg(canMessage.Data, canMessage.ID)

                    ' Access and print the fields of the ParsedMessage struct
                    Console.WriteLine($"Feedback del Motor: Motor CAN ID: {result.MotorCanId}, Position: {result.Position} rad, Velocity: {result.Velocity} rad/s, Torque: {result.Torque} Nm")
                End While

                ' Reestablecer el evento
                receiveEvent.Reset()
            End If
        End While
    End Sub
End Class

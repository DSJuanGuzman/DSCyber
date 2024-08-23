
Imports Peak.Can.Basic
Imports System.Threading
Imports Dll100PortCyberGear
Imports CyberGearVb.Struct
Imports CyberGearVb.nsConstants

Friend Class BusCan
    Implements IBusCan

    Private MasterCANID As UInteger 'CANID
    Private channel As PcanChannel 'Canal de Comunicación CAN
    Public Event MessageReceived As Action(Of PcanMessage)


    Public Sub New(masterCANID As UInteger)
        ' Inicializa El Constructor
        Me.MasterCANID = masterCANID
        Me.channel = InicializarCanal()
    End Sub

    ''' <summary>  q2
    ''' Inicia el canal de comunicacion CANBus y la recepcion de mensajes
    ''' </summary>
    Private Function InicializarCanal() As PcanChannel
        Dim channel As PcanChannel = PcanChannel.Usb01

        ' 1000k bit/s
        Dim result As PcanStatus = Api.Initialize(channel, Bitrate.Pcan1000)
        If result <> PcanStatus.OK Then
            Dim errorText As String = String.Empty
            Api.GetErrorText(result, errorText)
            Console.WriteLine(errorText)
        Else
            Console.WriteLine($"El hardware representado por el canal {channel} se ha inicializado correctamente")
            Dim receiver As New PcanReceiver(channel, AddressOf HandleMessage)
            If receiver.Start() Then
                Console.WriteLine("El Receptor ha iniciado, Si deseas detener el proceso de recepcion, por favor llama el metodo 'Receiver.Stop()'")
            Else
                Console.WriteLine("Fallo en la inicializacion")
            End If
        End If
        Return channel
    End Function


    Public Sub FinalitzarCanal() Implements IBusCan.FinalitzarCanal
        Dim result = Api.Uninitialize(channel)
        If Not result = PcanStatus.OK Then
            Dim errorText = String.Empty
            Api.GetErrorText(result, errorText)
            Console.WriteLine(errorText)
        Else
            Console.WriteLine($"El hardware representado por el canal {channel} se ha cerrado correctamente")
        End If

    End Sub

    ''' <summary>
    ''' Crea instancias de IMotor
    ''' </summary>
    Public Function fuxIMotor(vparIDispositiu As IDispositiu) As IMotor Implements IBusCan.fuxIMotor
        Return New MotorCyberGear(Me, CType(vparIDispositiu.senCodi(), UInteger))
    End Function

    Public Function fuxIMotor(vparCANID As Integer) As IMotor Implements IBusCan.fuxIMotor
        Return New MotorCyberGear(Me, vparCANID)
    End Function

    ''' <summary>
    ''' Crea una lista de la interfaz IDispositiu
    ''' </summary>
    Public Function secDispositius() As List(Of IDispositiu) Implements IBusCan.secDispositius
        Dim motorCANIDs As List(Of UInteger) = ObtenirIDsDispositius()
        Dim dispositius As New List(Of IDispositiu)()
        For Each id In motorCANIDs
            dispositius.Add(New Dispositiu(CInt(id)))
        Next
        Return dispositius
    End Function

    ''' <summary>
    ''' Recibe el objeto del evento y obtiene el Id de los dispositivos conectados al CANBus
    ''' </summary>
    Private Function ObtenirIDsDispositius() As List(Of UInteger)
        Dim deviceIDs As New List(Of UInteger)()
        Dim receivedMessages As New List(Of PcanMessage)()

        ' Evento para manejar los mensajes recibidos
        AddHandler Me.MessageReceived, Sub(msg)
                                           SyncLock receivedMessages
                                               receivedMessages.Add(msg)
                                           End SyncLock
                                       End Sub

        ' Enviar mensajes a todos los IDs posibles
        For i As UInteger = 0 To 128
            Dim arbitrationId As UInteger = (CmdModes.MOTOR_ENABLE << 24) Or (MasterCANID << 8) Or i
            Dim data1 As Byte() = {0, 0, 0, 0, 0, 0, 0, 0}
            ' Estructura de un mensaje CAN
            Dim canMessage As New PcanMessage With {
            .ID = arbitrationId,
            .MsgType = MessageType.Extended,
            .DLC = CByte(data1.Length),
            .Data = data1
        }
            ' Escribir el mensaje
            Dim writeStatus As PcanStatus = Api.Write(Me.channel, canMessage)
            If writeStatus <> PcanStatus.OK Then
                Debug.WriteLine($"Failed to send message to ID {arbitrationId}.")
            End If
        Next

        Debug.WriteLine("Messages sent to all IDs, waiting for responses...")

        ' Esperar respuestas
        Dim endTime As DateTime = DateTime.Now.AddSeconds(2) ' Esperar por 5 segundos para respuestas
        While DateTime.Now < endTime
            SyncLock receivedMessages
                While receivedMessages.Count > 0
                    Dim receivedMsg As PcanMessage = receivedMessages(0)
                    receivedMessages.RemoveAt(0)
                    Dim result As ParsedMessage = AnalitzarMissatgeRebut(receivedMsg.Data, receivedMsg.ID)
                    Dim deviceId As UInteger = result.MotorCanId
                    If Not deviceIDs.Contains(deviceId) Then
                        deviceIDs.Add(deviceId)
                    End If
                End While
            End SyncLock
        End While

        RemoveHandler Me.MessageReceived, Nothing
        Return deviceIDs
    End Function

    Public Function SendReceiveCanMessage(MotorCANID As UInteger, cmdMode As UInteger, data1 As Byte()) As CanMessageResult
        Dim arbitrationId As UInteger = (cmdMode << 24) Or (MasterCANID << 8) Or MotorCANID
        Dim canMessage As New PcanMessage With {
            .ID = arbitrationId,
            .MsgType = MessageType.Extended,
            .DLC = CByte(data1.Length),
            .Data = data1
        }

        ' Write the CAN message
        Dim writeStatus As PcanStatus = Api.Write(Me.channel, canMessage)
        If writeStatus <> PcanStatus.OK Then
            Debug.WriteLine("Failed to send the message.")
            Return New CanMessageResult(New Byte() {}, 0)
        End If

        ' Output details of the sent message
        Debug.WriteLine($"Sent message with ID {arbitrationId:X}, data: {BitConverter.ToString(data1)}")
        Thread.Sleep(50)  ' Give the driver some time to send the messages...
        Dim receivedMsg As PcanMessage = Nothing
        Dim timestamp As ULong
        Dim readStatus As PcanStatus = Api.Read(Me.channel, receivedMsg, timestamp)
        ' Check if received a message
        If readStatus = PcanStatus.OK Then
            Dim DB() As Byte = receivedMsg.Data
            Return New CanMessageResult(DB, receivedMsg.ID)
        Else
            Debug.WriteLine("Failed to receive the message or message was not received within the timeout period.")
            Return New CanMessageResult(New Byte() {}, 0)
        End If
    End Function

    ''' <summary>
    ''' Enviar mensajes CAN con Id Personalizado (Datos adicionales)
    ''' </summary>
    Public Function EnviarMissatgeCanPersonalitzat(arbitrationId As UInteger, data1 As Byte()) As MotorData Implements IBusCan.EnviarMissatgeCanPersonalitzat
        Dim receivedMessages As New List(Of PcanMessage)()
        AddHandler Me.MessageReceived, Sub(msg)
                                           SyncLock receivedMessages
                                               receivedMessages.Add(msg)
                                           End SyncLock
                                       End Sub
        Dim canMessage As New PcanMessage With {
            .ID = arbitrationId,
            .MsgType = MessageType.Extended,
            .DLC = CByte(data1.Length),
            .Data = data1
        }
        Dim writeStatus As PcanStatus = Api.Write(Me.channel, canMessage)
        If writeStatus <> PcanStatus.OK Then
            Debug.WriteLine("Failed to send the message.")
            Return New MotorData(0, 0, 0, 0, 0)
        End If
        SyncLock receivedMessages
            While receivedMessages.Count > 0
                Dim receivedMsg As PcanMessage = receivedMessages(0)
                receivedMessages.RemoveAt(0)
                Dim result As ParsedMessage = AnalitzarMissatgeRebut(receivedMsg.Data, receivedMsg.ID)
                Return New MotorData(result.Position, result.Velocity, result.Temp, result.Torque, result.MotorCanId)
            End While
        End SyncLock
        RemoveHandler Me.MessageReceived, Nothing
        Return New MotorData(0, 0, 0, 0, 0)
    End Function

    ''' <summary>
    ''' Enviar mensajes CAN
    ''' </summary>
    ''' <param name="cmdMode"></param>
    ''' <param name="data1"></param>
    Public Sub EnviarMissatgeCan(MotorCANID As UInteger, cmdMode As UInteger, data1 As Byte()) Implements IBusCan.EnviarMissatgeCan
        ' Calcular ID DE Arbitraje
        Dim arbitrationId As UInteger = (cmdMode << 24) Or (MasterCANID << 8) Or MotorCANID
        ' Estructura de un mensaje CAN
        Dim canMessage As New PcanMessage With {
            .ID = arbitrationId,
            .MsgType = MessageType.Extended,
            .DLC = Convert.ToByte(data1.Length),
            .Data = data1
        }
        ' Write the CAN message
        Dim writeStatus As PcanStatus = Api.Write(Me.channel, canMessage)
        If writeStatus <> PcanStatus.OK Then
            Debug.WriteLine("Failed to send the message.")
        End If

        ' Output details of the sent message
        Debug.WriteLine($"Sent message with ID {arbitrationId:X}, data: {BitConverter.ToString(data1)}")
    End Sub


    ''' <summary>
    ''' Recibe un mensaje del buffer y lo lee para obtener los datos del frame de respuesta
    ''' </summary>
    Public Shared Function AnalitzarMissatgeRebut(data As Byte(), arbitration_id As UInteger) As ParsedMessage
        If data.Length >= 6 Then
            Debug.WriteLine($"Received message with ID 0x{arbitration_id:X}")

            ' Escribe el CAN ID del motor
            Dim motor_can_id As Byte = CByte((arbitration_id >> 8) And &HFF)

            'Analiza la posición, Velocidad y torque con chequeo de desbordamiento
            Dim pos As Double
            Dim vel As Double
            Dim torque As Double
            Dim temp As Double

            Try
                pos = Calculate.UToF((CInt(data(0)) << 8) + data(1), Constantes.P_MIN, Constantes.P_MAX)
                vel = Calculate.UToF((CInt(data(2)) << 8) + data(3), Constantes.V_MIN, Constantes.V_MAX)
                torque = Calculate.UToF((CInt(data(4)) << 8) + data(5), Constantes.T_MIN, Constantes.T_MAX)
                temp = ((CInt(data(6)) << 8) + data(7)) / 10
            Catch ex As OverflowException
                Debug.WriteLine($"Overflow error: {ex.Message}")
                Return New ParsedMessage(0, 0, 0, 0, 0)
            End Try

            Debug.WriteLine($"Motor CAN ID: {motor_can_id}, pos: {pos:F2} rad, vel: {vel:F2} rad/s, torque: {torque:F2} Nm, temp: {temp:F2} Celsius")

            Return New ParsedMessage(motor_can_id, pos, vel, torque, temp)
        Else
            Debug.WriteLine("No message received within the timeout period or insufficient data length.")
            Return New ParsedMessage(0, 0, 0, 0, 0)
        End If
        AnalitzarMissatgeLecturaParametreUnic(data, arbitration_id)
    End Function

    Public Shared Function AnalitzarMissatgeLecturaParametreUnic(data As Byte(), arbitration_id As UInteger) As ParsedSingleParameter
        If data.Length >= 8 Then
            Debug.WriteLine($"Received parameter message with ID 0x{arbitration_id:X}")

            ' Extraer el Motor CAN ID de los bits 23 ~ 8 del arbitration_id
            Dim motor_can_id As Byte = CByte((arbitration_id >> 8) And &HFF)

            ' Extraer el índice del parámetro (Bytes 0 ~ 1) asegurando el orden correcto
            Dim index As UInteger = BitConverter.ToUInt16(data, 0)

            ' Extraer el valor del parámetro basado en el índice
            Dim parameter_value As Single

            If index = &H7005 Then
                ' Leer el valor del byte 4 como un entero
                Dim parameter_value_int As Byte = data(4)
                parameter_value = CSng(parameter_value_int)
            Else
                ' Leer los bytes 4 a 7 como un flotante
                parameter_value = BitConverter.ToSingle(data, 4)
            End If

            Debug.WriteLine($"Motor CAN ID: {motor_can_id}, Index: {index}, Parameter Value: {parameter_value}")

            ' Devolver los valores analizados en un objeto ParsedSingleParameter
            Return New ParsedSingleParameter(motor_can_id, index, parameter_value)
        Else
            Debug.WriteLine("Insufficient data length to analyze parameter message.")
            Return New ParsedSingleParameter(0, 0, 0)
        End If
    End Function


    ''' <summary>
    ''' Administra el evento cuando se reciben mensajes nuevos en el buffer
    ''' </summary>
    Private Sub HandleMessage(canMessage As PcanMessage)
        RaiseEvent MessageReceived(canMessage)
    End Sub

End Class

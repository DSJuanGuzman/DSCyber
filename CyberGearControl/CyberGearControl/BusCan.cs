using CyberGear.Struct;
using CyberGear;
using nsConstants;
using Peak.Can.Basic;
using System.Diagnostics;
using System.Threading;
using System;
using Dll100PortCyberGear;
using System.Collections.Generic;

internal class BusCan : IBusCan
{
    uint MasterCANID = 0;//CANID
    private PcanChannel channel;//Canal de Comunicacion CAN

    public BusCan(uint masterCANID)
    {
        // Inicializa El Constructor
        MasterCANID = masterCANID;
        this.channel = InicializarCanal();
    }

    private PcanChannel InicializarCanal()
    {
        PcanChannel channel = PcanChannel.Usb01;

        // 1000k bit/s
        PcanStatus result = Api.Initialize(channel, Bitrate.Pcan1000);
        if (result != PcanStatus.OK)
        {
            Api.GetErrorText(result, out var errorText);
            Console.WriteLine(errorText);
        }
        else
        {
            Console.WriteLine($"El hardware representado por el canal {channel} se ha inicializado correctamente");
            Console.ReadKey();
            PcanReceiver receiver = new PcanReceiver(channel);
            if (receiver.Start())
            {
                Console.WriteLine("El Receptor ha iniciado, Si deseas detener el proceso de recepcion, por favor llama el metodo 'Receiver.Stop()'");
            }
            else
            {
                Console.WriteLine("Fallo en la inicializacion");
            }
        }
        return channel;
    }

    public IMotor fuxIMotor(IDispositiu vparIDispositiu)
    {
        IMotor newMotor = new MotorCyberGear(this,(uint)vparIDispositiu.senCodi());
        return newMotor;
    }
    public List<IDispositiu> secDispositius()
    {
        List<uint> motorCANIDs = ObtenerIDsDispositivos();
        List<IDispositiu> dispositius = new List<IDispositiu>();
        foreach (var id in motorCANIDs)
        {
            dispositius.Add(new Dispositiu((int)id));
        }
        return dispositius;
    }
    private List<uint> ObtenerIDsDispositivos()
    {
        List<uint> deviceIDs = new List<uint>();

        // Enviar un mensaje de difusión para descubrir dispositivos
        uint arbitrationId = 0x7DF; // Mensaje de diagnóstico estándar
        byte[] data = new byte[8] { 0x02, 0x01, 0x00, 0, 0, 0, 0, 0 }; // Solicitud de diagnóstico genérico

        PcanMessage canMessage = new PcanMessage
        {
            ID = arbitrationId,
            MsgType = MessageType.Standard,
            DLC = Convert.ToByte(data.Length),
            Data = data
        };

        // Escribir el mensaje de difusión
        PcanStatus writeStatus = Api.Write(this.channel, canMessage);
        if (writeStatus != PcanStatus.OK)
        {
            Debug.WriteLine("Failed to send the broadcast message.");
            return deviceIDs;
        }

        Debug.WriteLine("Broadcast message sent, waiting for responses...");

        // Leer respuestas
        PcanMessage receivedMsg;
        ulong timestamp;
        DateTime endTime = DateTime.Now.AddSeconds(5); // Esperar por 5 segundos para respuestas
        while (DateTime.Now < endTime)
        {
            PcanStatus readStatus = Api.Read(this.channel, out receivedMsg, out timestamp);
            if (readStatus == PcanStatus.OK)
            {
                uint deviceId = receivedMsg.ID;
                if (!deviceIDs.Contains(deviceId))
                {
                    deviceIDs.Add(deviceId);
                }
            }
            Thread.Sleep(10); // Pequeña pausa para no saturar el CPU
        }

        return deviceIDs;
    }


    public CanMessageResult SendReceiveCanMessage(uint MotorCANID, uint cmdMode, byte[] data1)
    {
        uint arbitrationId = (cmdMode << 24) | (MasterCANID << 8) | MotorCANID;

        PcanMessage canMessage = new PcanMessage
        {
            ID = arbitrationId,
            MsgType = MessageType.Extended,
            DLC = Convert.ToByte(data1.Length),
            Data = data1
        };

        // Write the CAN message
        PcanStatus writeStatus = Api.Write(this.channel, canMessage);
        if (writeStatus != PcanStatus.OK)
        {
            Debug.WriteLine("Failed to send the message.");
            return new CanMessageResult(new byte[0], 0);
        }

        // Output details of the sent message
        Debug.WriteLine($"Sent message with ID {arbitrationId:X}, data: {BitConverter.ToString(data1)}");
        Thread.Sleep(50);  // Give the driver some time to send the messages...
        PcanMessage receivedMsg;
        ulong timestamp;
        PcanStatus readStatus = Api.Read(this.channel, out receivedMsg, out timestamp);
        // Check if received a message
        if (readStatus == PcanStatus.OK)
        {
            byte[] DB = receivedMsg.Data;
            byte[] bytes = DB;

            return new CanMessageResult(bytes, receivedMsg.ID);
        }
        else
        {
            Debug.WriteLine("Failed to receive the message or message was not received within the timeout period.");
            return new CanMessageResult(new byte[0], 0);
        }
    }

    /// <summary>
    /// Enviar mensajes CAN con Id Personalizado (Datos adicionales)
    /// </summary>
    public uint SendCustomCanMessage(uint arbitrationId, byte[] data1)
    {
        PcanMessage canMessage = new PcanMessage
        {
            ID = arbitrationId,
            MsgType = MessageType.Extended,
            DLC = Convert.ToByte(data1.Length),
            Data = data1
        };
        // Write the CAN message
        PcanStatus writeStatus = Api.Write(this.channel, canMessage);
        if (writeStatus != PcanStatus.OK)
        {
            Debug.WriteLine("Failed to send the message.");
        }
        // Output details of the sent message
        Debug.WriteLine($"Sent message with ID {arbitrationId:X}, data: {BitConverter.ToString(data1)}");
        PcanMessage meessage;
        ulong canTimestamp;
        if (Api.Read(channel, out meessage, out canTimestamp) == PcanStatus.OK)
        {
            //Devuelve y asigna La ID del motor Automaticamente
            var result = ParseReceivedMsg(canMessage.Data, canMessage.ID);
            return result.MotorCanId;
        }
        return 0;
    }

    /// <summary>
    /// Enviar mensajes CAN
    /// </summary>
    /// <param name="cmdMode"></param>
    /// <param name="data1"></param>
    public void SendCanMessage(uint MotorCANID, uint cmdMode, byte[] data1)
    {
        // Calcular ID DE Arbitraje
        uint arbitrationId = (cmdMode << 24) | (MasterCANID << 8) | MotorCANID;
        // Estructura de un mensaje CAN
        PcanMessage canMessage = new PcanMessage
        {
            ID = arbitrationId,
            MsgType = MessageType.Extended,
            DLC = Convert.ToByte(data1.Length),
            Data = data1
        };
        // Write the CAN message
        PcanStatus writeStatus = Api.Write(this.channel, canMessage);
        if (writeStatus != PcanStatus.OK)
        {
            Debug.WriteLine("Failed to send the message.");
        }
        // Output details of the sent message
        Debug.WriteLine($"Sent message with ID {arbitrationId:X}, data: {BitConverter.ToString(data1)}");
    }

    public static ParsedMessage ParseReceivedMsg(byte[] data, uint arbitration_id)
    {
        if (data.Length > 0)
        {
            Debug.WriteLine($"Received message with ID 0x{arbitration_id:X}");

            // Escribe el CAN ID del motor
            byte motor_can_id = (byte)((arbitration_id >> 8) & 0xFF);
            //Analiza la pocision, Velocidad y torque
            double pos = Calculate.UToF((data[0] << 8) + data[1], Constants.P_MIN, Constants.P_MAX);
            double vel = Calculate.UToF((data[2] << 8) + data[3], Constants.V_MIN, Constants.V_MAX);
            double torque = Calculate.UToF((data[4] << 8) + data[5], Constants.T_MIN, Constants.T_MAX);

            Debug.WriteLine($"Motor CAN ID: {motor_can_id}, pos: {pos:.2f} rad, vel: {vel:.2f} rad/s, torque: {torque:.2f} Nm");

            return new ParsedMessage(motor_can_id, pos, vel, torque);
        }
        else
        {
            Debug.WriteLine("No message received within the timeout period.");
            return new ParsedMessage(0, 0, 0, 0);
        }
    }
}

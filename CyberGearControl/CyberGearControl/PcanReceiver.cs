using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Peak.Can.Basic;
using System.Diagnostics;
using System.Threading;

namespace CyberGear
{
    internal class PcanReceiver
    {
        private EventWaitHandle receiveEvent;
        private Thread receiveThread;
        private bool isRunning;
        private PcanChannel channel;

        public PcanReceiver(PcanChannel channel)
        {
            this.channel = channel;
            receiveEvent = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public bool Start()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                // On Windows operating systems, set the receive event directly.
                if (Api.SetValue(channel, PcanParameter.ReceiveEvent, (uint)receiveEvent.SafeWaitHandle.DangerousGetHandle().ToInt32()) != PcanStatus.OK)
                {
                    Console.WriteLine($"Se produjo un error al configurar los eventos de recepcion en el canal: {channel}");
                    Api.Uninitialize(channel);
                    return false;
                }
            }
            else
            {
                // On non-Windows operating systems, obtain the receive event handle and set it
                uint eventHandle;
                if (Api.GetValue(channel, PcanParameter.ReceiveEvent, out eventHandle) != PcanStatus.OK)
                {
                    Console.WriteLine($"Se produjo un error al configurar los eventos de recepcion en el canal: {channel}");
                    Api.Uninitialize(channel);
                    return false;
                }

                receiveEvent.SafeWaitHandle.Close();
                receiveEvent.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(new IntPtr(eventHandle), false);
            }

            // Iniciar hilo de recepcion
            isRunning = true;
            receiveThread = new Thread(ReceiveThread);
            receiveThread.Start();

            Console.WriteLine($"{channel} ha sido configurado para la recepcion de eventos");
            return true;
        }

        public void Stop()
        {
            // Detener el hilo y limpiar
            isRunning = false;
            if (receiveThread != null && receiveThread.IsAlive)
            {
                receiveThread.Join();
            }
            Api.Uninitialize(channel);
            Console.WriteLine($"{channel} Se ha detenido");
        }

        private void ReceiveThread()
        {
            while (isRunning)
            {
                // Esperar la señal del evento
                if (receiveEvent.WaitOne(50))
                {
                    PcanMessage canMessage;
                    ulong canTimestamp;

                    //Lee y proceso todos los mensajes CAN en el buffer de recepcion.
                    while (Api.Read(channel, out canMessage, out canTimestamp) == PcanStatus.OK)
                    {
                        // Procesa el mensaje recibido
                        Console.WriteLine($"Mensaje Recibido: ID=0x{canMessage.ID:X} Data= {BitConverter.ToString(canMessage.Data)}");
                        Console.WriteLine($"TimeStamp: {canTimestamp}");

                        // Parse the received message
                        var result = BusCan.ParseReceivedMsg(canMessage.Data, canMessage.ID);

                        // Access and print the fields of the ParsedMessage struct
                        Console.WriteLine($"Feedback del Motor: Motor CAN ID: {result.MotorCanId}, Position: {result.Position} rad, Velocity: {result.Velocity} rad/s, Torque: {result.Torque} Nm");
                    }

                    // Reestablecer eL evento
                    receiveEvent.Reset();
                }
            }
        }
    }
}

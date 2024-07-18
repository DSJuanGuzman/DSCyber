using System;
using System.Linq;
using Peak.Can.Basic;
using System.Diagnostics;
using System.Threading;
using nsConstants;
using Dll100PortCyberGear;

namespace CyberGear
{
    /// <summary>
    /// Gestion de comunicacion por medio de Bus CAN con el motor (CyberGear)
    /// </summary>
    /// <remarks>
    /// Envio,Recepcion y Analisis de mensajes CAN
    /// </remarks>
    public class MotorCyberGear : IMotor
    {
        private readonly IBusCan _busCan;
        private readonly uint MotorCANID;

        public MotorCyberGear(IBusCan busCan, uint motorCANID)
        {
            _busCan = busCan;
            MotorCANID = motorCANID;
        }

        /// <summary>
        /// asignacion de un unico valor en el indice ya especificado
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>

        public void WriteSingleParam(uint index, float value) //Asigna un nuevo valor al parametro dado
        {
            byte[] data_index = BitConverter.GetBytes(index);
            byte[] date_parameter = BitConverter.GetBytes(value);
            //Combina ambas matrices
            byte[] data1 = data_index.Concat(date_parameter).ToArray();

            //Envia el mensaje CAN
            _busCan.SendCanMessage(MotorCANID,(uint)CmdModes.SINGLE_PARAM_WRITE, data1);
        }
        public void WriteSingleParam(uint index, byte byteValue)
        {
            // crea una matriz que contengasolo este valor de byte y agrega al index
            byte[] bs = new byte[] { byteValue };
            bs = bs.Concat(Enumerable.Repeat((byte)0, 3)).ToArray();    
            byte[] data_index = BitConverter.GetBytes(index);
            byte[] data1 = data_index.Concat(bs).ToArray();

            // Enviar mensaje CAN
            _busCan.SendCanMessage(MotorCANID,(uint)CmdModes.SINGLE_PARAM_WRITE, data1);
        }

        /// <summary>
        /// lee un unico parametro del index indicado
        /// </summary>
        /// <param name="index"></param>
        public void ReadSingleParam(uint index)
        {
            byte[] data_index = BitConverter.GetBytes(index);
            byte[] date_parameter = {0, 0, 0, 0};
            byte[] data1 = data_index.Concat(date_parameter).ToArray();
            _busCan.SendCanMessage(MotorCANID, (uint)CmdModes.SINGLE_PARAM_READ, data1);
        }

        /// <summary>
        /// Inicializa el motor
        /// </summary>
        public void EnableMotor()
        {
            byte[] data1 = { };
            _busCan.SendCanMessage(MotorCANID, (uint)CmdModes.MOTOR_ENABLE, data1);
        }
        /// <summary>
        /// Desactiva el motor
        /// </summary>
        public void DisableMotor()
        {
            byte[] data1 = { 0, 0, 0, 0, 0, 0, 0, 0 };
            _busCan.SendCanMessage(MotorCANID, (uint)CmdModes.MOTOR_STOP, data1);
        }
        /// <summary>
        /// El motor ubica su pocision 0.
        /// </summary>

        public void SetMechanicalZero()
        {
            byte[] data1 = { 1 };//Byte[0]=1
            _busCan.SendCanMessage(MotorCANID, (uint)CmdModes.SET_MECHANICAL_ZERO, data1);
        }


        public void SendMotorControlCommand(float torque, float target_angle, float target_velocity, float Kp, float Kd)
        {
            //Enviar instrucciones de control en modo de operacion.
            //Parametros:
            //torque
            //target_angle = Posicion
            //target_velocity
            //Kp= Ganancia Proporcional
            //Kd= Ganancia Derivada

            //Generar los componenetes de la Id de arbitraje de 29 bits
            //uint cmd_mode = CmdModes.MOTOR_CONTROL;
            uint torque_mapped = Calculate.FToU(torque, -12.0, 12.0);// Float to Uint (Calculate.cs)
            uint data2 = torque_mapped;
            // Id de Arbitracion，
            uint arbitrationId = ((uint)CmdModes.MOTOR_CONTROL << 24) | (data2 << 8) | MotorCANID; //Encabezado de la peticion

            // GEnerar Datos de Area 1
            uint target_angle_mapped = Calculate.FToU(target_angle, -4 * Math.PI, 4 * Math.PI);//Angulo Objetivo
            uint target_velocity_mapped = Calculate.FToU(target_velocity, -30.0F, 30.0F);//Velocidad Objetivo
            uint Kp_mapped = Calculate.FToU(Kp, 0.0F, 500.0F);//Ganancia Proporcional
            uint Kd_mapped = Calculate.FToU(Kd, 0.0F, 5.0F);//Ganancia Diferencial

            //Datos a cuerpo de 8 bytes
            byte[] data1 = new byte[8];//Cuerpo de la peticion (Datos)
            Array.Copy(BitConverter.GetBytes(target_angle_mapped), 0, data1, 0, 2);
            Array.Copy(BitConverter.GetBytes(target_velocity_mapped), 0, data1, 2, 2);
            Array.Copy(BitConverter.GetBytes(Kp_mapped), 0, data1, 4, 2);
            Array.Copy(BitConverter.GetBytes(Kd_mapped), 0, data1, 6, 2);//Cada parametro tiene su espacio en el array de bytes 
                                                                         //Byte 0 ~ 1: Target angle [0 ~ 65535] corresponding to (-4π ~ 4π)
                                                                         //Byte 2 ~3: Target angular velocity[0 ~65535] corresponds to(-30rad / s ~30rad / s)
                                                                         //Byte 4 ~5: Kp[0 ~65535] corresponds to(0.0 ~500.0)
                                                                         //Byte 6 ~7: Kd[0 ~65535] corresponds to(0.0 ~5.0)

            _busCan.SendCustomCanMessage(arbitrationId, data1);
        }

    }
}

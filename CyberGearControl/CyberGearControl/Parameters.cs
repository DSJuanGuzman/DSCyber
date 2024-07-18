
namespace nsConstants
{
        public enum ParameterList
        {
            //Parametros modificables ()
            RunMode = 0x7005, //Modo de control de funcionamiento: 0: Operation control mode
            //1: Position mode
            //2: Speed mode
            //3: Current mode
            IqRef = 0x7006, //（tipo: float， unidad: A)
            SpdRef = 0x700A, //（tipo: float，unidad: rad/s）
            ImitTorque = 0x700B, //（tipo: float, unidad: Nm）
            CurKp = 0x7010, //（Tipo: float, Valor predeterminado 0.125）ganancia proporcional
            CurKi = 0x7011, //（Tipo: float, Valor predeterminado 0.0158）ganancia derivada
            CurFiltGain = 0x7014, // filt_gain（Tipo: float, Valor prdeterminado: 0.1）
            LocRef = 0x7016, // (Tipo: float，unidad: rad）
            LimitSpd = 0x7017, //（tipo: float，unidades: rad/s）
            LimitCur = 0x7018 //（tipo: float，unidad: A）
        }
        public enum RunModes
        {
            CONTROL_MODE = 0,
            POSITION_MODE = 1,
            SPEED_MODE = 2,
            CURRENT_MODE = 3
        }
        public enum CmdModes
        {
            GET_DEVICE_ID = 0,
            MOTOR_CONTROL = 1,
            MOTOR_FEEDBACK = 2,
            MOTOR_ENABLE = 3,
            MOTOR_STOP = 4,
            SET_MECHANICAL_ZERO = 6,
            SET_MOTOR_CAN_ID = 7,
            PARAM_TABLE_WRITE = 8,
            SINGLE_PARAM_READ = 17,
            SINGLE_PARAM_WRITE = 18,
            FAULT_FEEDBACK = 21,
            BAUD_RATE_MODIFICATION = 22 //modificar el rango de Baudios de la comunicacion, tener mucha precaucion al modificarlo puede dejar el motor inutilizable

        }   
    public class Constants
    {
        public const double P_MIN = -4 * System.Math.PI;
        public const double P_MAX = 4 * System.Math.PI;
        public const double V_MIN = -30.0;
        public const double V_MAX = 30.0;
        public const double T_MIN = -12.0;
        public const double T_MAX = 12.0;
        public const double KP_MIN = 0.0;
        public const double KP_MAX = 500.0;
        public const double KD_MIN = 0.0;
        public const double KD_MAX = 5.0;
    }


    }


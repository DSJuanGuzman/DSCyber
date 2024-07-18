Namespace nsConstants
    Public Enum ParameterList
        'Parametros modificables ()
        'Modo de control de funcionamiento: 0: Operation control mode
        '1: Position mode
        '2: Speed mode
        '3: Current mode
        RunMode = &H7005 ' Modo de control de funcionamiento
        ' (tipo: float， unidad: A)
        IqRef = &H7006 ' (tipo: float， unidad: A)
        ' (tipo: float，unidad: rad/s)
        SpdRef = &H700A ' (tipo: float，unidad: rad/s)
        ' (tipo: float, unidad: Nm)
        ImitTorque = &H700B ' (tipo: float, unidad: Nm)
        ' (Tipo: float, Valor predeterminado 0.125)ganancia proporcional
        CurKp = &H7010 ' (Tipo: float, Valor predeterminado 0.125)ganancia proporcional
        ' (Tipo: float, Valor predeterminado 0.0158)ganancia derivada
        CurKi = &H7011 ' (Tipo: float, Valor predeterminado 0.0158)ganancia derivada
        ' filt_gain（Tipo: float, Valor prdeterminado: 0.1）
        CurFiltGain = &H7014 ' filt_gain（Tipo: float, Valor prdeterminado: 0.1）
        ' (Tipo: float，unidad: rad）
        LocRef = &H7016 ' (Tipo: float，unidad: rad）
        ' (tipo: float，unidades: rad/s)
        LimitSpd = &H7017 ' (tipo: float，unidades: rad/s）
        ' (tipo: float，unidad: A）
        LimitCur = &H7018 ' (tipo: float，unidad: A）
    End Enum

    Public Enum RunModes
        CONTROL_MODE = 0
        POSITION_MODE = 1
        SPEED_MODE = 2
        CURRENT_MODE = 3
    End Enum

    Public Enum CmdModes
        GET_DEVICE_ID = 0
        MOTOR_CONTROL = 1
        MOTOR_FEEDBACK = 2
        MOTOR_ENABLE = 3
        MOTOR_STOP = 4
        SET_MECHANICAL_ZERO = 6
        SET_MOTOR_CAN_ID = 7
        PARAM_TABLE_WRITE = 8
        SINGLE_PARAM_READ = 17
        SINGLE_PARAM_WRITE = 18
        FAULT_FEEDBACK = 21
        BAUD_RATE_MODIFICATION = 22 'modificar el rango de Baudios de la comunicacion, tener mucha precaucion al modificarlo puede dejar el motor inutilizable
    End Enum

    Public Class Constantes
        Public Const P_MIN As Double = -4 * System.Math.PI
        Public Const P_MAX As Double = 4 * System.Math.PI
        Public Const V_MIN As Double = -30.0
        Public Const V_MAX As Double = 30.0
        Public Const T_MIN As Double = -12.0
        Public Const T_MAX As Double = 12.0
        Public Const KP_MIN As Double = 0.0
        Public Const KP_MAX As Double = 500.0
        Public Const KD_MIN As Double = 0.0
        Public Const KD_MAX As Double = 5.0
    End Class
End Namespace

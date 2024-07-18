Imports System
Imports System.Linq
Imports Peak.Can.Basic
Imports System.Diagnostics
Imports System.Threading
Imports Dll100PortCyberGear
Imports CyberGearVb.nsConstants


''' <summary>
''' Gestion de comunicacion por medio de Bus CAN con el motor (CyberGear)
''' </summary>
''' <remarks>
''' Envio,Recepcion y Analisis de mensajes CAN
''' </remarks>
Friend Class MotorCyberGear
    Implements IMotor

    Private ReadOnly _busCan As IBusCan
    Private ReadOnly MotorCANID As UInteger

    Public Sub New(busCan As IBusCan, motorCANID As UInteger)
        _busCan = busCan
        Me.MotorCANID = motorCANID
    End Sub

    ''' <summary>
    ''' asignacion de un unico valor en el indice ya especificado
    ''' </summary>
    ''' <param name="index"></param>
    ''' <param name="value"></param>
    Public Sub WriteSingleParam(index As UInteger, value As Single) Implements IMotor.WriteSingleParam
        'Asigna un nuevo valor al parametro dado
        Dim data_index As Byte() = BitConverter.GetBytes(index)
        Dim date_parameter As Byte() = BitConverter.GetBytes(value)
        'Combina ambas matrices
        Dim data1 As Byte() = data_index.Concat(date_parameter).ToArray()

        'Envia el mensaje CAN
        _busCan.SendCanMessage(MotorCANID, CType(CmdModes.SINGLE_PARAM_WRITE, UInteger), data1)
    End Sub

    Public Sub WriteSingleParam(index As UInteger, byteValue As Byte) Implements IMotor.WriteSingleParam
        ' crea una matriz que contengasolo este valor de byte y agrega al index
        Dim bs As Byte() = New Byte() {byteValue}
        bs = bs.Concat(Enumerable.Repeat(CByte(0), 3)).ToArray()
        Dim data_index As Byte() = BitConverter.GetBytes(index)
        Dim data1 As Byte() = data_index.Concat(bs).ToArray()

        ' Enviar mensaje CAN
        _busCan.SendCanMessage(MotorCANID, CType(CmdModes.SINGLE_PARAM_WRITE, UInteger), data1)
    End Sub

    ''' <summary>
    ''' lee un unico parametro del index indicado
    ''' </summary>
    ''' <param name="index"></param>
    Public Sub ReadSingleParam(index As UInteger) Implements IMotor.ReadSingleParam
        Dim data_index As Byte() = BitConverter.GetBytes(index)
        Dim date_parameter As Byte() = {0, 0, 0, 0}
        Dim data1 As Byte() = data_index.Concat(date_parameter).ToArray()
        _busCan.SendCanMessage(MotorCANID, CType(CmdModes.SINGLE_PARAM_READ, UInteger), data1)
    End Sub

    ''' <summary>
    ''' Asigna el modo pocision al motor
    ''' </summary>
    Public Sub SetPositionMode() Implements IMotor.SetPositionMode
        WriteSingleParam(CType(ParameterList.RunMode, UInteger), RunModes.POSITION_MODE)
    End Sub

    ''' <summary>
    ''' Asigna el modo de velocidad
    ''' </summary>
    Public Sub SetSpeedMode()
        WriteSingleParam(CType(ParameterList.RunMode, UInteger), RunModes.SPEED_MODE)
    End Sub

    ''' <summary>
    ''' Asigna la velocidad objetivo
    ''' </summary>
    Public Sub SetSpeed(value As Single)
        WriteSingleParam(CType(ParameterList.LimitSpd, UInteger), value)
    End Sub


    ''' <summary>
    ''' Asigna el limite de velocidad
    ''' </summary>
    Public Sub SetLimitSpeed(value As Single) Implements IMotor.SetLimitSpeed
        WriteSingleParam(CType(ParameterList.LimitSpd, UInteger), value)
    End Sub


    ''' <summary>
    ''' Asigna una posicion objetivo en radianes al motor
    ''' </summary>
    Public Sub SetPosition(value As Single) Implements IMotor.SetPosition
        WriteSingleParam(CType(ParameterList.LocRef, UInteger), value)
    End Sub

    ''' <summary>
    ''' Inicializa el motor
    ''' </summary>
    Public Sub EnableMotor() Implements IMotor.EnableMotor
        Dim data1 As Byte() = {}
        _busCan.SendCanMessage(MotorCANID, CType(CmdModes.MOTOR_ENABLE, UInteger), data1)
    End Sub

    ''' <summary>
    ''' Desactiva el motor
    ''' </summary>
    Public Sub DisableMotor() Implements IMotor.DisableMotor
        Dim data1 As Byte() = {0, 0, 0, 0, 0, 0, 0, 0}
        _busCan.SendCanMessage(MotorCANID, CType(CmdModes.MOTOR_STOP, UInteger), data1)
    End Sub

    ''' <summary>
    ''' El motor ubica su pocision 0.
    ''' </summary>
    Public Sub SetMechanicalZero() Implements IMotor.SetMechanicalZero
        Dim data1 As Byte() = {1} 'Byte[0]=1
        _busCan.SendCanMessage(MotorCANID, CType(CmdModes.SET_MECHANICAL_ZERO, UInteger), data1)
    End Sub

    Public Sub SetControlMode() Implements IMotor.SetControlMode
        WriteSingleParam(CType(ParameterList.RunMode, UInteger), RunModes.CONTROL_MODE)
    End Sub
    Public Sub SendMotorControlCommand(torque As Single, target_angle As Single, target_velocity As Single, Kp As Single, Kd As Single) Implements IMotor.SendMotorControlCommand
        'Enviar instrucciones de control en modo de operacion.
        'Parametros:
        'torque
        'target_angle = Posicion
        'target_velocity
        'Kp= Ganancia Proporcional
        'Kd= Ganancia Derivada

        'Generar los componenetes de la Id de arbitraje de 29 bits
        'uint cmd_mode = CmdModes.MOTOR_CONTROL;
        Dim torque_mapped As UInteger = Calculate.FToU(torque, -12.0F, 12.0F) ' Float to Uint (Calculate.cs)
        Dim data2 As UInteger = torque_mapped
        ' Id de Arbitracion，
        Dim arbitrationId As UInteger = (CType(CmdModes.MOTOR_CONTROL, UInteger) << 24) Or (data2 << 8) Or MotorCANID 'Encabezado de la peticion

        ' GEnerar Datos de Area 1
        Dim target_angle_mapped As UInteger = Calculate.FToU(target_angle, -4 * Math.PI, 4 * Math.PI) 'Angulo Objetivo
        Dim target_velocity_mapped As UInteger = Calculate.FToU(target_velocity, -30.0F, 30.0F) 'Velocidad Objetivo
        Dim Kp_mapped As UInteger = Calculate.FToU(Kp, 0.0F, 500.0F) 'Ganancia Proporcional
        Dim Kd_mapped As UInteger = Calculate.FToU(Kd, 0.0F, 5.0F) 'Ganancia Diferencial

        'Datos a cuerpo de 8 bytes
        Dim data1 As Byte() = New Byte(7) {} 'Cuerpo de la peticion (Datos)
        Array.Copy(BitConverter.GetBytes(target_angle_mapped), 0, data1, 0, 2)
        Array.Copy(BitConverter.GetBytes(target_velocity_mapped), 0, data1, 2, 2)
        Array.Copy(BitConverter.GetBytes(Kp_mapped), 0, data1, 4, 2)
        Array.Copy(BitConverter.GetBytes(Kd_mapped), 0, data1, 6, 2) 'Cada parametro tiene su espacio en el array de bytes 
        'Byte 0 ~ 1: Target angle [0 ~ 65535] corresponding to (-4π ~ 4π)
        'Byte 2 ~3: Target angular velocity[0 ~65535] corresponds to(-30rad / s ~30rad / s)
        'Byte 4 ~5: Kp[0 ~65535] corresponds to(0.0 ~500.0)
        'Byte 6 ~7: Kd[0 ~65535] corresponds to(0.0 ~5.0)

        _busCan.SendCustomCanMessage(arbitrationId, data1)
    End Sub

End Class

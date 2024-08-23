Imports System.Threading
Imports Dll100PortCyberGear
Imports Newtonsoft.Json.Linq

Public Class ControlService
    Implements IControlService
    Private ReadOnly _configurationService As IConfigurationService
    Private Property _Motor As IMotor
    Private Property _Motores As List(Of IMotor)

    Public Sub New(configurationService As IConfigurationService)
        _configurationService = configurationService
    End Sub

    Public Sub Activar(id As Integer) Implements IControlService.Activar
        _Motor = _configurationService.IniciarMotor(id)
        _Motor.ActivarMotor()
    End Sub

    Public Sub ActivarMotor(id As Integer) Implements IControlService.ActivarMotor
        Dim motorSeleccionado = _Motores.FirstOrDefault(Function(m) m.SenID = id)
        If motorSeleccionado IsNot Nothing Then
            _Motor = motorSeleccionado
            Console.WriteLine("Motor seleccionado: " + id.ToString())
            _Motor.ActivarMotor()
        Else
            Console.WriteLine("Motor con ID " + id.ToString() + " no encontrado.")
        End If
    End Sub

    Public Function BuscarMotores() As List(Of Integer) Implements IControlService.BuscarMotores
        _Motores = _configurationService.BuscarMotores()
        Dim MotoresDisponibles As New List(Of Integer)
        For Each motor In _Motores
            MotoresDisponibles.Add(motor.SenID)
        Next
        Return MotoresDisponibles
    End Function
    Public Sub EstablecerCeroMecanico() Implements IControlService.EstablecerCeroMecanico
        _Motor.EstablirZeroMecanic()
        Console.WriteLine("Cero Mecanico Establecido")
    End Sub
    Public Sub EstablecerModoPosicion() Implements IControlService.EstablecerModoPosicion
        _Motor.EstablirModePosicio()
        Console.WriteLine("Modo posicion Establecido")
    End Sub
    Public Sub EstablecerLimiteVelocidad(value As Single) Implements IControlService.EstablecerLimiteVelocidad
        _Motor.EstablirLimitVelocitat(value)
        Console.WriteLine("Limite De Velocidad Establecido")
    End Sub
    Public Sub EstablecerPosicion(value As Single) Implements IControlService.EstablecerPosicion
        _Motor.EstablirPosicio(value)
        Console.WriteLine("Posicion Establecida")
    End Sub
    Public Sub EstablecerModoControl() Implements IControlService.EstablecerModoControl
        _Motor.EstablirModeControl()
        Console.WriteLine("Modo De Control Establecido")
    End Sub
    Public Sub EviarComandoControlMotor(torque As Single, target As Single, velocity As Single, Kp As Single, Kd As Single) Implements IControlService.EviarComandoControlMotor
        _Motor.EnviarComandaControlMotor(torque, target, velocity, Kp, Kd)
        Console.WriteLine("Comando de control enviado")
    End Sub
    Public Function LeerParametroUnico(Index As UInteger) As String Implements IControlService.LeerParametroUnico
        Return _Motor.LlegirParametreUnic(Index)
    End Function
    Public Sub EscribirParametroUnico(index As UInteger, value As Single) Implements IControlService.EscribirParametroUnico
        _Motor.EscriureParametreUnic(index, value)
        Console.WriteLine($"Asignando a parametro: {index}")
    End Sub
    Public Sub EscribirParametroUnico(index As UInteger, value As Byte) Implements IControlService.EscribirParametroUnico
        _Motor.EscriureParametreUnic(index, value)
        Console.WriteLine($"Asignando a parametro: {index}")
    End Sub
    Public Sub EstablecerModoVelocidad() Implements IControlService.EstablecerModoVelocidad
        _Motor.EstablirModeVelocitat()
        Console.WriteLine("Modo De Velocidad Establecido")
    End Sub
    Public Sub EstablecerVelocidad(value As Single) Implements IControlService.EstablecerVelocidad
        _Motor.EstablirVelocitat(value)
        Console.WriteLine("Velocidad Establecida")
    End Sub
    Public Sub EstablecerModoCorriente() Implements IControlService.EstablecerModoCorriente
        _Motor.EstablirModeCorrent()
        Console.WriteLine("Modo De Corriente Establecido")
    End Sub
    Public Sub EstablecerComandoCorriente(value As Single) Implements IControlService.EstablecerComandoCorriente
        _Motor.EstablirComandaCorrent(value)
        Console.WriteLine("Comando De Corriente Establecido")
    End Sub
    Public Function ObtenerEstado() As MotorData Implements IControlService.ObtenerEstado
        Dim Motor_Data = _Motor.RebreStatMotor
        Console.WriteLine("Estado:")
        Console.WriteLine("Posicion: " + Motor_Data.Posicion)
        Console.WriteLine("Velocidad: " + Motor_Data.Velocidad)
        Console.WriteLine("Temperatura: " + Motor_Data.Temperatura)
        Console.WriteLine("Torque: " + Motor_Data.Torque)
        Console.WriteLine("CAN ID: " + Motor_Data.CanID)
        Return Motor_Data
    End Function
    Public Sub Desactivar() Implements IControlService.Desactivar
        _Motor.DesactivarMotor()
        Console.WriteLine("Motor Desactivado")
    End Sub
End Class

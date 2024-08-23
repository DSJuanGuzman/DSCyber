Imports Dll100PortCyberGear

Public Interface IControlService
    Sub Activar(id As Integer)
    Sub EstablecerCeroMecanico()
    Sub EstablecerModoPosicion()
    Sub EstablecerLimiteVelocidad(value As Single)
    Sub EstablecerPosicion(value As Single)
    Sub EstablecerModoControl()
    Sub EviarComandoControlMotor(torque As Single, target As Single, velocity As Single, Kp As Single, Kd As Single)
    Function LeerParametroUnico(index As UInteger) As String
    Sub EscribirParametroUnico(index As UInteger, value As Single)
    Sub EscribirParametroUnico(index As UInteger, value As Byte)
    Sub EstablecerModoVelocidad()
    Sub EstablecerVelocidad(value As Single)
    Sub EstablecerModoCorriente()
    Sub EstablecerComandoCorriente(value As Single)
    Function ObtenerEstado() As MotorData
    Sub Desactivar()
    Sub ActivarMotor(id As Integer)
    Function BuscarMotores() As List(Of Integer)
End Interface

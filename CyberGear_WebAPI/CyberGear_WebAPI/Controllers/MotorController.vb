Imports System.Web.Http

Public Class MotorController
    Inherits ApiController

    Private ReadOnly _configurationService As IConfigurationService
    Private ReadOnly _controlService As IControlService

    Public Sub New(configurationService As IConfigurationService, controlService As IControlService)
        _configurationService = configurationService
        _controlService = controlService
    End Sub

    <HttpPost>
    <Route("api/Motor/Iniciar")>
    Public Function IniciarServicio() As IHttpActionResult
        _configurationService.IniciarServicio()
        Return Ok()
    End Function

    <HttpGet>
    <Route("api/Motor/Buscar")>
    Public Function Buscar() As IHttpActionResult
        Return Ok(_controlService.BuscarMotores())
    End Function

    <HttpPost>
    <Route("api/Motor/Activar/{ID}")>
    Public Function Activar(ID As Integer) As IHttpActionResult
        _controlService.Activar(ID)
        Return Ok()
    End Function

    <HttpGet>
    <Route("api/Motor/Estado")>
    Public Function EstadoMotor() As IHttpActionResult
        Return Ok(_controlService.ObtenerEstado())
    End Function

    <HttpPost>
    <Route("api/Motor/Terminar")>
    Public Function TerminarServicio() As IHttpActionResult
        _configurationService.CerrarServicio()
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/Desactivar")>
    Public Function Desactivar() As IHttpActionResult
        _controlService.Desactivar()
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/CeroMecanico")>
    Public Function CeroMecanico() As IHttpActionResult
        _controlService.EstablecerCeroMecanico()
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/ModoPosicion")>
    Public Function ModoPosicion() As IHttpActionResult
        _controlService.EstablecerModoPosicion()
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/LimiteVelocidad/{value}")>
    Public Function LimiteVelocidad(value As Single) As IHttpActionResult
        _controlService.EstablecerLimiteVelocidad(value)
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/Posicion/{value}")>
    Public Function Posicion(value As Single) As IHttpActionResult
        _controlService.EstablecerPosicion(value)
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/ModoControl")>
    Public Function LimiteVelocidad() As IHttpActionResult
        _controlService.EstablecerModoControl()
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/ComandoControl")>
    Public Function LimiteVelocidad(torque As Single, target As Single, velocity As Single, Kp As Single, Kd As Single) As IHttpActionResult
        _controlService.EviarComandoControlMotor(torque, target, velocity, Kp, Kd)
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/LeerParametro/{index}")>
    Public Function LeerParametro(index As UInteger) As IHttpActionResult
        Return Ok(_controlService.LeerParametroUnico(index))
    End Function

    <HttpPost>
    <Route("api/Motor/EscribirParametro")>
    Public Function EscribirParametro(index As UInteger, value As Single) As IHttpActionResult
        _controlService.EscribirParametroUnico(index, value)
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/ModoVelocidad")>
    Public Function ModoVelocidad() As IHttpActionResult
        _controlService.EstablecerModoVelocidad()
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/Velocidad/{value}")>
    Public Function Velocidad(value As Single) As IHttpActionResult
        _controlService.EstablecerVelocidad(value)
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/ModoCorriente")>
    Public Function ModoCorriente() As IHttpActionResult
        _controlService.EstablecerModoCorriente()
        Return Ok()
    End Function

    <HttpPost>
    <Route("api/Motor/Corriente/{value}")>
    Public Function Corriente(value As Single) As IHttpActionResult
        _controlService.EstablecerComandoCorriente(value)
        Return Ok()
    End Function
End Class

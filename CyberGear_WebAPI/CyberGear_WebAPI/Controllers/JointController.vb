Imports System.Web.Http
Imports System.Threading.Tasks ' Para soportar métodos asincrónicos
Imports System.Collections.Concurrent

Public Class JointController
    Inherits ApiController

    Private ReadOnly _controlService As IControlService
    Private ReadOnly _JointService As IJointControlService

    Public Sub New(controlService As IControlService, jointservice As IJointControlService)
        _controlService = controlService
        _JointService = jointservice
    End Sub

    ' Optimización: Convertido a método asincrónico
    <HttpPost>
    <Route("api/Joint/Activar/{Ids}/{configuration}")>
    Public Async Function ActivarJoint(Ids As String, configuration As Integer) As Task(Of IHttpActionResult)
        Dim idsArray As String() = Ids.Split(","c)
        Dim idsList As New List(Of Integer)

        ' Optimización: Se puede paralelizar la validación de IDs
        For Each idStr As String In idsArray
            Dim id As Integer
            If Integer.TryParse(idStr.Trim(), id) Then
                If id >= 0 AndAlso id <= 127 Then
                    idsList.Add(id)
                Else
                    Return BadRequest($"El valor '{idStr}' no está dentro del rango permitido (0-127).")
                End If
            Else
                Return BadRequest($"El valor '{idStr}' no es un número válido.")
            End If
        Next

        ' Optimización: Selección del enum
        Dim jointConfiguration As ConfigurationEnum
        Select Case configuration
            Case 0
                jointConfiguration = ConfigurationEnum.Parallel
            Case 1
                jointConfiguration = ConfigurationEnum.Inverted
            Case 2
                jointConfiguration = ConfigurationEnum.Combined
            Case Else
                Return BadRequest("El valor de configuration debe estar entre 0 y 2.")
        End Select

        ' Optimización: Llamada asincrónica a ActivarJoint
        Dim joint As Joint = Await Task.Run(Function() _JointService.ActivarJoint(idsList, jointConfiguration))
        _JointService.SetJoint(joint)

        Return Ok($"{Ids}, {jointConfiguration}")
    End Function

    ' Optimización: Convertido a método asincrónico
    <HttpPost>
    <Route("api/Joint/ModoPosicion/{Id}")>
    Public Async Function EstablirModoPosicio(Id As Integer) As Task(Of IHttpActionResult)
        ' Asincronía para evitar bloqueo del hilo principal
        Await Task.Run(Sub() _JointService.EstablecerModoPosicion(Id))
        Return Ok("Se ha establecido el Modo Posicion para los motores")
    End Function

    ' Optimización: Convertido a método asincrónico
    <HttpPost>
    <Route("api/Joint/Posicion/{Id}/{Speed}/{Angle}")>
    Public Async Function EstablirPosicio(Id As Integer, Speed As Single, angle As Single) As Task(Of IHttpActionResult)
        ' Asincronía para evitar bloqueo del hilo principal
        Await Task.Run(Sub() _JointService.EstablecerPosicion(Id, Speed, angle))
        Return Ok("Se ha establecido la Posicion para los motores")
    End Function

    <HttpPost>
    <Route("api/Joint/Forward/{Id}/{Speed}")>
    Public Async Function forward(Id As Integer, Speed As Single) As Task(Of IHttpActionResult)
        Await Task.Run(Sub() _JointService.Forward(Id, Speed))
        Return Ok("Se ha establecido la Posicion para los motores")
    End Function

    <HttpPost>
    <Route("api/Joint/Backward/{Id}/{Speed}")>
    Public Async Function Backward(Id As Integer, Speed As Single) As Task(Of IHttpActionResult)
        Await Task.Run(Sub() _JointService.Backward(Id, Speed))
        Return Ok("Se ha establecido la Posicion para los motores")
    End Function

    <HttpGet>
    <Route("api/Joint/GetJoints")>
    Public Async Function GetJoints() As Task(Of IHttpActionResult)
        Try
            Dim articulaciones As ConcurrentDictionary(Of Integer, Joint) = Await Task.Run(Function() _JointService.GetJoints)

            If articulaciones Is Nothing OrElse articulaciones.Count = 0 Then
                Return NotFound()
            End If

            ' Convertir los datos a una lista de DTOs
            Dim jointsDto As List(Of JointDto) = articulaciones.Select(Function(pair) New JointDto With {
                .Id = pair.Key,
                .NumberOfMotors = pair.Value._Motores.Count,
                .Configuration = pair.Value._Configuration.ToString() ' Asegúrate de que Configuration sea convertible a String
            }).ToList()

            Return Ok(jointsDto)
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function

End Class

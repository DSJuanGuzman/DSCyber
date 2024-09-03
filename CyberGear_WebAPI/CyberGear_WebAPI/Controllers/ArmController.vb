Imports System.Web.Http
Imports System.Threading.Tasks

Public Class ArmController
    Inherits ApiController

    Private ReadOnly _armControlService As IArmControlService

    Public Sub New(armControlService As IArmControlService)
        _armControlService = armControlService
    End Sub

    <HttpPost>
    <Route("api/Arm/ActivateAllJoints")>
    Public Async Function ActivateAllJoints() As Task(Of IHttpActionResult)
        Try
            Await Task.Run(Sub() _armControlService.ActivateArm())
            Return Ok("Todas las Articulaciones del brazo han sido activadas.")
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function

    ' Mover el brazo a una posición específica
    <HttpPost>
    <Route("api/Arm/MoveToPosition")>
    Public Async Function MoveJointsToPosition(positions As Dictionary(Of Integer, Tuple(Of Single, Single))) As Task(Of IHttpActionResult)
        Try
            ' Llamada asincrónica para mover el brazo a la posición deseada
            Await Task.Run(Sub() _armControlService.MoveToPosition(positions))
            Return Ok("El brazo se ha movido a la posición deseada.")
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function

    ' Mover el brazo a su posición inicial (de reposo)
    <HttpPost>
    <Route("api/Arm/MoveToInitialPosition")>
    Public Async Function MoveToInitialPosition() As Task(Of IHttpActionResult)
        Try
            ' Llamada asincrónica para mover el brazo a la posición de reposo
            Await Task.Run(Sub() _armControlService.MoveToInitialPosition())
            Return Ok("El brazo se ha movido a la posición inicial (de reposo).")
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function

    ' Activar una articulación específica del brazo
    <HttpPost>
    <Route("api/Arm/ActivateJoint/{jointId}")>
    Public Async Function ActivateJoint(jointId As Integer) As Task(Of IHttpActionResult)
        Try
            ' Llamada asincrónica para activar una articulación específica
            Await Task.Run(Sub() _armControlService.ActivarJoint(jointId))
            Return Ok($"La articulación {jointId} ha sido activada.")
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function

    ' Desactivar todas las articulaciones del brazo
    <HttpPost>
    <Route("api/Arm/DeactivateAllJoints")>
    Public Async Function DeactivateAllJoints() As Task(Of IHttpActionResult)
        Try
            ' Llamada asincrónica para desactivar todas las articulaciones
            Await Task.Run(Sub() _armControlService.DeactivateArm())
            Return Ok("Todas las articulaciones del brazo han sido desactivadas.")
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function

    ' Establecer el modo de posición para todas las articulaciones
    <HttpPost>
    <Route("api/Arm/SetPositionModeForAllJoints")>
    Public Async Function SetPositionModeForAllJoints() As Task(Of IHttpActionResult)
        Try
            ' Llamada asincrónica para establecer el modo de posición en todas las articulaciones
            Await Task.Run(Sub() _armControlService.SetPositionModeForAllJoints())
            Return Ok("El modo de posición se ha establecido para todas las articulaciones del brazo.")
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function

    ' Obtener las articulaciones del brazo
    <HttpGet>
    <Route("api/Arm/GetJoints")>
    Public Async Function GetJoints() As Task(Of IHttpActionResult)
        Try
            Dim joints As List(Of Joint) = Await Task.Run(Function() _armControlService.GetJoints())

            If joints Is Nothing OrElse joints.Count = 0 Then
                Return NotFound()
            End If

            ' Convertir los datos a una lista de DTOs o devolver directamente las articulaciones
            Return Ok(joints)
        Catch ex As Exception
            Return InternalServerError(ex)
        End Try
    End Function


End Class

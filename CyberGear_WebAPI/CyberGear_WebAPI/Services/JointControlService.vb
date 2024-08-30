Imports System.Web.UI.WebControls
Imports Dll100PortCyberGear
Imports System.Collections.Concurrent

Public Class JointControlService
    Implements IJointControlService

    Private Property _Joints As New ConcurrentDictionary(Of Integer, Joint)
    Private ReadOnly _motorCache As New ConcurrentDictionary(Of Integer, IMotor)
    Private ReadOnly _controlService As IControlService
    Private ReadOnly _ConfigurationService As IConfigurationService

    Public Sub New(controlServices As IControlService, configurationService As IConfigurationService)
        _controlService = controlServices
        _ConfigurationService = configurationService
    End Sub

    Public Sub SetJoint(joint As Joint) Implements IJointControlService.SetJoint
        _Joints(joint._JointId) = joint
    End Sub

    Public Function ActivarJoint(Ids As List(Of Integer), configuration As ConfigurationEnum) As Joint Implements IJointControlService.ActivarJoint
        _controlService.BuscarMotores()

        Dim motors As New ConcurrentBag(Of IMotor)

        Parallel.ForEach(Ids, Sub(Id)
                                  Dim motor As IMotor
                                  If Not _motorCache.TryGetValue(Id, motor) Then
                                      motor = _ConfigurationService.IniciarMotor(Id)
                                      _motorCache(Id) = motor
                                  End If
                                  motors.Add(motor)
                                  _controlService.ActivarMotor(Id)
                              End Sub)

        Return New Joint(_Joints.Count + 1, configuration, motors.ToList())
    End Function

    Public Sub DesactivarJoint(Id As Integer) Implements IJointControlService.DesactivarJoint
        ' Optimización: Acceso rápido a Joint mediante diccionario
        If _Joints.ContainsKey(Id) Then
            Dim selectedJoint = _Joints(Id)
            For Each motor In selectedJoint._Motores
                motor.DesactivarMotor()
            Next
        End If
    End Sub

    Public Sub EstablecerModoPosicion(Id As Integer) Implements IJointControlService.EstablecerModoPosicion
        ' Optimización: Acceso rápido a Joint mediante diccionario
        If _Joints.ContainsKey(Id) Then
            Dim selectedJoint = _Joints(Id)
            For Each motor In selectedJoint._Motores
                motor.EstablirModePosicio()
            Next
        End If
    End Sub

    Public Sub EstablecerPosicion(id As Integer, SpeedValue As Single, AngleValue As Single) Implements IJointControlService.EstablecerPosicion
        ' Acceso rápido a Joint mediante diccionario
        If _Joints.ContainsKey(id) Then
            Dim selectedJoint = _Joints(id)
            Dim motors = selectedJoint._Motores
            Dim positions(motors.Count - 1) As Single

            ' Primero calculamos las posiciones necesarias para todos los motores
            For i As Integer = 0 To motors.Count - 1
                If selectedJoint._Configuration = ConfigurationEnum.Parallel Then
                    positions(i) = AngleValue
                ElseIf selectedJoint._Configuration = ConfigurationEnum.Inverted Then
                    If i = 0 Then
                        positions(i) = AngleValue
                    Else
                        positions(i) = AngleValue * -1
                    End If
                End If
            Next

            ' Luego, aplicamos la configuración en paralelo para sincronizar los motores
            Parallel.For(0, motors.Count, Sub(i)
                                              motors(i).EstablirLimitVelocitat(SpeedValue)
                                              motors(i).EstablirPosicio(positions(i))
                                          End Sub)
        End If
    End Sub


End Class

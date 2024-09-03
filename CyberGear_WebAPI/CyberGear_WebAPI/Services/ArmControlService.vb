Public Class ArmControlService
    Implements IArmControlService

    Private ReadOnly _jointControlService As IJointControlService
    Private ReadOnly _joints As New List(Of Joint)

    Public Sub New(jointControlService As IJointControlService)
        _jointControlService = jointControlService
    End Sub

    Public Function CrearJoint(Ids As List(Of Integer), configuration As ConfigurationEnum) As Joint Implements IArmControlService.CrearJoint
        Dim joint As Joint = _jointControlService.CrearJoint(Ids, configuration)
        _joints.Add(joint)
        Return joint
    End Function

    Public Sub ActivarJoint(Id As Integer) Implements IArmControlService.ActivarJoint
        _jointControlService.ActivarJoint(Id)
    End Sub

    Public Sub ActivateArm() Implements IArmControlService.ActivateArm
        For Each joint In _joints
            _jointControlService.ActivarJoint(joint._JointId)
        Next
    End Sub

    Public Sub DeactivateArm() Implements IArmControlService.DeactivateArm
        For Each joint In _joints
            _jointControlService.DesactivarJoint(joint._JointId)
        Next
    End Sub

    Public Sub MoveToInitialPosition() Implements IArmControlService.MoveToInitialPosition
        For Each joint In _joints
            _jointControlService.EstablecerPosicion(joint._JointId, SpeedValue:=1.0F, AngleValue:=0.0F)
        Next
    End Sub

    Public Sub MoveToPosition(positions As Dictionary(Of Integer, Tuple(Of Single, Single))) Implements IArmControlService.MoveToPosition
        For Each kvp In positions
            _jointControlService.EstablecerPosicion(kvp.Key, kvp.Value.Item1, kvp.Value.Item2)
        Next
    End Sub

    Public Sub AddJoint(joint As Joint) Implements IArmControlService.AddJoint
        _joints.Add(joint)
        _jointControlService.SetJoint(joint)
    End Sub

    Public Function GetJoints() As List(Of Joint) Implements IArmControlService.GetJoints
        Return _joints
    End Function

    Public Sub SetJointPosition(jointId As Integer, speed As Single, angle As Single) Implements IArmControlService.SetJointPosition
        _jointControlService.EstablecerPosicion(jointId, speed, angle)
    End Sub

    Public Sub SetPositionModeForAllJoints() Implements IArmControlService.SetPositionModeForAllJoints
        For Each joint In _joints
            _jointControlService.EstablecerModoPosicion(joint._JointId)
        Next
    End Sub

    Public Sub DeactivateJoint(jointId As Integer) Implements IArmControlService.DeactivateJoint
        _jointControlService.DesactivarJoint(jointId)
    End Sub
End Class

Public Class Brazo
    Public Property _Joints As List(Of Joint)
    Public Property _TotalMotors As Integer
    Public Property _PhysicalLimits As Tuple(Of Single, Single)
    Public Property _ArmId As Integer
    Public Property _CurrentPosition As Dictionary(Of Integer, Tuple(Of Single, Single))
    Public Property _RestPosition As Dictionary(Of Integer, Tuple(Of Single, Single))

    Public Sub New(armId As Integer, physicalLimits As Tuple(Of Single, Single))
        _ArmId = armId
        _Joints = New List(Of Joint)()
        _PhysicalLimits = physicalLimits
        _CurrentPosition = New Dictionary(Of Integer, Tuple(Of Single, Single))()
        _RestPosition = New Dictionary(Of Integer, Tuple(Of Single, Single))()
    End Sub
End Class
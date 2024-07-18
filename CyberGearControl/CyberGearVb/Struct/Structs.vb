Namespace Struct
    Public Structure CanMessageResult
        Public ReadOnly Property Data As Byte()
        Public ReadOnly Property Id As UInteger

        Public Sub New(data As Byte(), id As UInteger)
            Me.Data = data
            Me.Id = id
        End Sub
    End Structure

    Public Structure ParsedMessage
        Public ReadOnly Property MotorCanId As Byte
        Public ReadOnly Property Position As Double
        Public ReadOnly Property Velocity As Double
        Public ReadOnly Property Torque As Double

        Public Sub New(motorCanId As Byte, position As Double, velocity As Double, torque As Double)
            Me.MotorCanId = motorCanId
            Me.Position = position
            Me.Velocity = velocity
            Me.Torque = torque
        End Sub
    End Structure
End Namespace

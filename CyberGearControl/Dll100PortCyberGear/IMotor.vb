Public Interface IMotor
    Sub WriteSingleParam(index As UInteger, value As Single)
    Sub WriteSingleParam(index As UInteger, value As Byte)
    Sub ReadSingleParam(index As UInteger)
    Sub DisableMotor()
    Sub EnableMotor()
    Sub SetMechanicalZero()
    Sub SetPositionMode()
    Sub SetLimitSpeed(value As Single)
    Sub SetPosition(value As Single)
    Sub SetControlMode()
    Sub SendMotorControlCommand(torque As Single, target_angle As Single, target_velocity As Single, Kp As Single, Kd As Single)
End Interface

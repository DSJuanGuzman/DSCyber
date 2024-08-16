Imports Dll100PortCyberGear

Public Interface IConfigurationService
    Sub IniciarServicio()
    Sub CerrarServicio()
    Function IniciarMotor(Id As Integer) As IMotor
End Interface

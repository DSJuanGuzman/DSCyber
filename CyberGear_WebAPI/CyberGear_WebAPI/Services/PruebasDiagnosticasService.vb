Public Class PruebasDiagnosticasService
    Public Property _configurationService As IConfigurationService
    Public Property _ControlService As IControlService

    Sub New(configurationService As IConfigurationService, controlService As IControlService)
        _configurationService = configurationService
        _ControlService = controlService
    End Sub
End Class

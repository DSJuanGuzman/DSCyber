Imports Owin
Imports System.Web.Http
Imports Unity
Imports Unity.AspNet.WebApi
Imports Unity.Lifetime

Public Class Startup
    Public Sub Configuration(app As IAppBuilder)
        ' Usar middleware de registro personalizado
        app.Use(Of LoggingMiddleware)()

        ' Configurar Web API para OWIN
        Dim config = New HttpConfiguration()
        ' Configurar rutas de Web API
        ' Configurar la inyección de dependencias con Unity
        Dim container As IUnityContainer = New UnityContainer()
        'Registro de los servicios
        container.RegisterType(Of IConfigurationService, ConfigurationService)(New ContainerControlledLifetimeManager())
        container.RegisterType(Of IControlService, ControlService)(New ContainerControlledLifetimeManager())
        ' Establecer el solucionador de dependencias de Web API con Unity
        config.DependencyResolver = New UnityDependencyResolver(container)
        config.MapHttpAttributeRoutes()
        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional}
        )
        ' Habilitar Web API para ser usada con OWIN
        app.UseWebApi(config)
    End Sub
End Class

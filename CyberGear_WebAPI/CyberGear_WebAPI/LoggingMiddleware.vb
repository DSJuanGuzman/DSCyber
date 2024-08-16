Imports Microsoft.Owin
Imports System.Threading.Tasks

Public Class LoggingMiddleware
    Inherits OwinMiddleware

    Public Sub New(nextMiddleware As OwinMiddleware)
        MyBase.New(nextMiddleware)
    End Sub

    Public Overrides Async Function Invoke(context As IOwinContext) As Task
        ' Log de la solicitud
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Uri}")

        ' Invocar el siguiente middleware en la cadena
        Await [Next].Invoke(context)

        ' Log de la respuesta
        Console.WriteLine($"Response: {context.Response.StatusCode}")
    End Function
End Class


Imports Microsoft.Owin.Hosting

Module Module1
    Sub Main()
        Dim baseAddress As String = "http://localhost:9000/"

        ' Iniciar el servidor OWIN
        Using WebApp.Start(Of Startup)(baseAddress)
            Console.WriteLine($"Server running at {baseAddress}")
            Console.WriteLine("Press [Enter] to exit...")
            Console.ReadLine()
        End Using
    End Sub
End Module

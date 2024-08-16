Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json


Class MainWindow
    Private ReadOnly client As New HttpClient()
    Public Property BaseUri As String = "http://localhost:9000/api/"
    Private Property IsServiceInitialized As Boolean = False
    Private Property IsMotorInitialized As Boolean = False
    Private Property CurrentMode As String = String.Empty
    Public Sub New()
        InitializeComponent()
        ' Selecciona la primera opción por defecto
        OptionsComboBox.SelectedIndex = 0
        Control_Tab.IsEnabled = False
        IniciarMotor_Button.IsEnabled = False
        Detener_Button.IsEnabled = False
        Desactivar_Motor_Button.IsEnabled = False
    End Sub

    Private Sub OptionsComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim selectedOption As String = TryCast(CType(OptionsComboBox.SelectedItem, ComboBoxItem)?.Content, String)
        Select Case selectedOption
            Case "Modo Control"
                MainTabControl.SelectedIndex = 0
            Case "Modo Posicion"
                MainTabControl.SelectedIndex = 1
            Case "Modo Velocidad"
                MainTabControl.SelectedIndex = 2
            Case "Modo Corriente"
                MainTabControl.SelectedIndex = 3
        End Select
    End Sub

    Private Async Sub IniciarServicio(sender As Object, e As RoutedEventArgs)
        If IsServiceInitialized = False Then
            Dim Response As HttpResponseMessage = Await client.PostAsync(BaseUri + "Motor/Iniciar", Nothing)
            If Response.IsSuccessStatusCode Then
                Message_Log.AppendText("Se ha iniciado el servicio.")
                Message_Log.AppendText(Environment.NewLine)
                IsServiceInitialized = True
                IniciarMotor_Button.IsEnabled = True
                Detener_Button.IsEnabled = True
                Iniciar_Servicio_Button.IsEnabled = False
            Else
                Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
                Message_Log.AppendText(Environment.NewLine)
            End If
        End If
        MessageBox.Show("El serivico ya se encuentra inicializado")
    End Sub
    Private Async Sub PuntoZero(sender As Object, e As RoutedEventArgs)
        CurrentMode = String.Empty
        Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/CeroMecanico", Nothing)
        If Response.IsSuccessStatusCode Then
            Message_Log.Text = "Se ha enviado el comando"
        Else
            Message_Log.Text = "Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase
        End If

    End Sub
    Private Async Sub DetenerServicio(sender As Object, e As RoutedEventArgs)
        Detener_Button.IsEnabled = IsServiceInitialized

        Dim Response As HttpResponseMessage = Await client.PostAsync(BaseUri + "Motor/Terminar", Nothing)
        If Response.IsSuccessStatusCode Then
            Message_Log.AppendText("Se ha finalizado el servicio.")
            Message_Log.AppendText(Environment.NewLine)
            IniciarMotor_Button.IsEnabled = False
            Detener_Button.IsEnabled = False
            Iniciar_Servicio_Button.IsEnabled = True
        Else
            Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
        End If
    End Sub
    Private Async Sub ActivarMotor_Button(sender As Object, e As RoutedEventArgs)
        If ID_TextBox.Text IsNot String.Empty Then
            Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/Activar/{ID_TextBox.Text}", Nothing)
            If Response.IsSuccessStatusCode Then
                Message_Log.AppendText("Se ha iniciado el Dispositivo.")
                Message_Log.AppendText(Environment.NewLine)
                IsMotorInitialized = True
                If IsMotorInitialized And IsServiceInitialized Then
                    Control_Tab.IsEnabled = IsMotorInitialized
                    Desactivar_Motor_Button.IsEnabled = True
                End If
            Else
                Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
                Message_Log.AppendText(Environment.NewLine)
            End If
        End If
    End Sub
    Private Async Sub DesactivarMotor_Button(sender As Object, e As RoutedEventArgs)
        If ID_TextBox.Text IsNot String.Empty Then
            Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/Desactivar", Nothing)
            If Response.IsSuccessStatusCode Then
                Message_Log.AppendText("Se ha Desactivado el Dispositivo.")
                Message_Log.AppendText(Environment.NewLine)
                IsMotorInitialized = False
                Control_Tab.IsEnabled = False
                Detener_Button.IsEnabled = True
                IniciarMotor_Button.IsEnabled = True
                Desactivar_Motor_Button.IsEnabled = False
            Else
                Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
                Message_Log.AppendText(Environment.NewLine)
            End If
        End If
    End Sub

    Private Async Sub EstablecerModo(sender As Object, e As RoutedEventArgs)
        Dim selectedOption As String = TryCast(CType(OptionsComboBox.SelectedItem, ComboBoxItem)?.Content, String)

        ' Cambia el contenido de cada pestaña según la opción seleccionada
        Select Case selectedOption
            Case "Modo Control"
                Dim Response As HttpResponseMessage = Await client.PostAsync(BaseUri + "Motor/ModoControl", Nothing)
                If Response.IsSuccessStatusCode Then
                    Message_Log.AppendText("Se ha establecido el modo control.")
                    Message_Log.AppendText(Environment.NewLine)
                    CurrentMode = selectedOption
                Else
                    Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
                    Message_Log.AppendText(Environment.NewLine)
                End If
            Case "Modo Posicion"
                Dim Response As HttpResponseMessage = Await client.PostAsync(BaseUri + "Motor/ModoPosicion", Nothing)
                If Response.IsSuccessStatusCode Then
                    Message_Log.AppendText("Se ha establecido el modo posicion.")
                    Message_Log.AppendText(Environment.NewLine)
                    CurrentMode = selectedOption
                Else
                    Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
                    Message_Log.AppendText(Environment.NewLine)
                End If
            Case "Modo Velocidad"
                Dim Response As HttpResponseMessage = Await client.PostAsync(BaseUri + "Motor/ModoVelocidad", Nothing)
                If Response.IsSuccessStatusCode Then
                    Message_Log.AppendText("Se ha establecido el modo velocidad.")
                    Message_Log.AppendText(Environment.NewLine)
                    CurrentMode = selectedOption
                Else
                    Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
                    Message_Log.AppendText(Environment.NewLine)
                End If
            Case "Modo Corriente"
                Dim Response As HttpResponseMessage = Await client.PostAsync(BaseUri + "Motor/ModoCorriente", Nothing)
                If Response.IsSuccessStatusCode Then
                    Message_Log.AppendText("Se ha establecido el modo corriente.")
                    Message_Log.AppendText(Environment.NewLine)
                    CurrentMode = selectedOption
                Else
                    Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
                    Message_Log.AppendText(Environment.NewLine)
                End If
        End Select
    End Sub

    Private Sub EnviarComando_Button(sender As Object, e As RoutedEventArgs)
        Dim selectedOption As String = CurrentMode
        If selectedOption IsNot String.Empty Then
            Select Case selectedOption
                Case "Modo Control" And MainTabControl.SelectedIndex = 0 And Torque_Text.Text, Posicion_Text.Text, Velocidad_Text.Text, Kp_Text.Text, Kd_Text.Text IsNot String.Empty
                    ComandoControl(Torque_Text.Text, Posicion_Text.Text, Velocidad_Text.Text, Kp_Text.Text, Kd_Text.Text)
                Case "Modo Posicion" And MainTabControl.SelectedIndex = 1 And LimitVelocity_Text.Text, Position_Text.Text IsNot String.Empty
                    Posicion(LimitVelocity_Text.Text, Position_Text.Text)
                Case "Modo Velocidad" And MainTabControl.SelectedIndex = 2 And Speedref_Text.Text IsNot String.Empty
                    Velocidad(Speedref_Text.Text)
                Case "Modo Corriente" And MainTabControl.SelectedIndex = 3 And Corriente_Text.Text IsNot String.Empty
                    Corriente(Corriente_Text.Text)
                Case "Escribir Parametro" And MainTabControl.SelectedIndex = 4 And Index_Text.Text, Value_Text.Text IsNot String.Empty
                    EscribirParametro(Index_Text.Text, Value_Text.Text)
                Case "Leer Parametro" And MainTabControl.SelectedIndex = 5 And ReadIndex_Text.Text IsNot String.Empty
                    LeerParametro(ReadIndex_Text.Text)
            End Select
        End If

    End Sub
    Private Async Sub ComandoControl(torque As Single, target As Single, velocity As Single, Kp As Single, Kd As Single)
        Dim datos As New With {
            torque,
            target,
            velocity,
            Kp,
            Kd
        }
        Dim json As String = Newtonsoft.Json.JsonConvert.SerializeObject(datos)
        Dim contenido As New StringContent(json, Encoding.UTF8, "application/json")
        Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/ComandoControl", contenido)
        If Response.IsSuccessStatusCode Then
            Message_Log.AppendText("Se ha enviado el comando")
            Message_Log.AppendText(Environment.NewLine)
        Else
            Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
        End If
    End Sub
    Private Async Sub Posicion(velocityValue As Single, targetValue As Single)
        Dim Responsev As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor//LimiteVelocidad/{velocityValue}", Nothing)
        Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/Posicion/{targetValue}", Nothing)
        If Response.IsSuccessStatusCode And Responsev.IsSuccessStatusCode Then
            Message_Log.AppendText("Se ha enviado el comando")
            Message_Log.AppendText(Environment.NewLine)
        Else
            Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
            Message_Log.AppendText("Error en la solicitud: " & Responsev.StatusCode & " " & Response.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
        End If
    End Sub
    Private Async Sub Velocidad(value As Single)
        Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/Velocidad/{value}", Nothing)
        If Response.IsSuccessStatusCode Then
            Message_Log.AppendText("Se ha enviado el comando")
            Message_Log.AppendText(Environment.NewLine)
        Else
            Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
        End If
    End Sub
    Private Async Sub Corriente(value As Single)
        Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/Corriente/{value}", Nothing)
        If Response.IsSuccessStatusCode Then
            Message_Log.AppendText("Se ha enviado el comando")
            Message_Log.AppendText(Environment.NewLine)
        Else
            Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
        End If
    End Sub
    Private Async Sub EscribirParametro(index As UInteger, value As Single)
        Dim datos As New With {
            index,
            value
        }
        Dim json As String = Newtonsoft.Json.JsonConvert.SerializeObject(datos)
        Dim contenido As New StringContent(json, Encoding.UTF8, "application/json")
        Dim Response As HttpResponseMessage = Await client.PostAsync($"{BaseUri}/Motor/EscribirParametro", contenido)
        If Response.IsSuccessStatusCode Then
            Message_Log.AppendText($"Se ha escrito sobre el parametro {index}")
            Message_Log.AppendText(Environment.NewLine)
        Else
            Message_Log.AppendText("Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
        End If
    End Sub
    Private Async Sub LeerParametro(index As UInteger)
        Dim respuesta As HttpResponseMessage = Await client.GetAsync($"{BaseUri}/Motor/LeerParametro/{index}")
        If respuesta.IsSuccessStatusCode Then
            Dim respuestaContenido As String = Await respuesta.Content.ReadAsStringAsync()
            Message_Log.AppendText("Solicitud exitosa: " & respuestaContenido)
            Message_Log.AppendText(Environment.NewLine)
            Value_Textbox.Text = respuestaContenido
        Else
            Message_Log.AppendText("Error en la solicitud: " & respuesta.StatusCode & " " & respuesta.ReasonPhrase)
            Message_Log.AppendText(Environment.NewLine)
        End If
    End Sub
End Class

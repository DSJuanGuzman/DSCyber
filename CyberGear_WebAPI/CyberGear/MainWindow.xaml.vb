Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json


Class MainWindow
    Private ReadOnly client As New HttpClient()
    Public Property BaseUri As String = "http://localhost:9000/api/"
    Private Property IsServiceInitialized As Boolean = False
    Private Property IsMotorInitialized As Boolean = False
    Private Property CurrentMode As String = String.Empty
    Public Property _SecMotors As New List(Of Integer)
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

    Private Async Sub Buscar(sender As Object, e As RoutedEventArgs)
        Dim Response As HttpResponseMessage = Await client.GetAsync($"{BaseUri}/Motor/Buscar")
        If Response.IsSuccessStatusCode Then
            Dim jsonString As String = Await Response.Content.ReadAsStringAsync()

            ' Deserializar la respuesta en una lista de enteros
            Dim motores As List(Of Integer) = JsonConvert.DeserializeObject(Of List(Of Integer))(jsonString)

            ' Verificar si la lista no está vacía y asignarla a _SecMotors
            If motores IsNot Nothing AndAlso motores.Any() Then
                _SecMotors = motores
                ListaMotores.ItemsSource = _SecMotors
            Else
                Message_Log.Text = "No se encontraron motores."
            End If
        Else
            Message_Log.Text = "Error en la solicitud: " & Response.StatusCode & " " & Response.ReasonPhrase
        End If
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

    Private Async Sub EnviarComando_Button(sender As Object, e As RoutedEventArgs)
        Dim selectedOption As Integer = MainTabControl.SelectedIndex
        Select Case selectedOption
            Case 0
                Dim torqueValue As Single
                Dim targetValue As Single
                Dim velocityValue As Single
                Dim KpValue As Single
                Dim KdValue As Single

                If Single.TryParse(Torque_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, torqueValue) AndAlso
               Single.TryParse(Posicion_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, targetValue) AndAlso
               Single.TryParse(Velocidad_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, velocityValue) AndAlso
               Single.TryParse(Kp_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, KpValue) AndAlso
               Single.TryParse(Kd_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, KdValue) Then
                    ComandoControl(torqueValue, targetValue, velocityValue, KpValue, KdValue)
                Else
                    Message_Log.Text = "Uno o más valores ingresados no son números válidos."
                End If

            Case 1
                Dim limitSpdValue As Single
                Dim positionValue As Single

                If Single.TryParse(LimitVelocity_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, limitSpdValue) AndAlso
               Single.TryParse(Position_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, positionValue) Then
                    Posicion(limitSpdValue, positionValue)
                Else
                    Message_Log.Text = "Uno o más valores ingresados no son números válidos."
                End If

            Case 2
                Dim velocidadValue As Single

                If Single.TryParse(Speedref_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, velocidadValue) Then
                    Velocidad(velocidadValue)
                Else
                    Message_Log.Text = "El valor ingresado no es un número válido."
                End If

            Case 3
                Dim corrienteValue As Single

                If Single.TryParse(Corriente_Text.Text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, corrienteValue) Then
                    Corriente(corrienteValue)
                Else
                    Message_Log.Text = "El valor ingresado no es un número válido."
                End If

            Case 4
                Dim indexValue As Integer
                Dim value As String

                If Integer.TryParse(Index_Text.Text, indexValue) AndAlso Not String.IsNullOrEmpty(Value_Text.Text) Then
                    EscribirParametro(indexValue, Value_Text.Text)
                Else
                    Message_Log.Text = "El valor ingresado no es un número válido o el valor de texto está vacío."
                End If

            Case 5
                Dim readIndexValue As Integer

                If Integer.TryParse(ReadIndex_Text.Text, readIndexValue) Then
                    LeerParametro(readIndexValue)
                Else
                    Message_Log.Text = "El valor ingresado no es un número válido."
                End If
        End Select
    End Sub

    Private Async Sub ComandoControl(torque As Single, target As Single, velocity As Single, Kp As Single, Kd As Single)
        ' Construir la cadena de consulta
        Dim queryString As String = $"torque={torque}&target={target}&velocity={velocity}&Kp={Kp}&Kd={Kd}"

        ' Crear la URL completa con la cadena de consulta
        Dim requestUri As String = $"{BaseUri}/Motor/ComandoControl?{queryString}"

        ' Realizar la solicitud POST
        Dim response As HttpResponseMessage
        Try
            response = Await client.PostAsync(requestUri, Nothing)

            If response.IsSuccessStatusCode Then
                Message_Log.AppendText("Se ha enviado el comando")
                Message_Log.AppendText(Environment.NewLine)
            Else
                Message_Log.AppendText("Error en la solicitud: " & response.StatusCode & " " & response.ReasonPhrase)
                Message_Log.AppendText(Environment.NewLine)
            End If
        Catch ex As Exception
            Message_Log.AppendText("Error: " & ex.Message)
            Message_Log.AppendText(Environment.NewLine)
        End Try
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

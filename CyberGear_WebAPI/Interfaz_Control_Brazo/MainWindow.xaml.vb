Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports Newtonsoft.Json

Class MainWindow
    Private motorCount As Integer = 1
    Private ReadOnly client As New HttpClient()
    Public Property BaseUri As String = "http://localhost:9000/api/Joint/"
    Public Property BaseUriConfiguration As String = "http://localhost:9000/Motor"

    Private Sub AgregarMotor_Button_Click(sender As Object, e As RoutedEventArgs)
        If motorCount < 4 Then
            motorCount += 1
            Dim newTextBox As New TextBox With {
                .Name = $"Id_{motorCount}",
                .Margin = New Thickness(0, 0, 0, 10)
            }
            MotorStackPanel.Children.Add(newTextBox)
        Else
            MessageBox.Show("No se pueden agregar más de 4 motores.", "Límite alcanzado", MessageBoxButton.OK, MessageBoxImage.Information)
        End If
    End Sub

    Private Async Function ActivarJointAsync(ids As String, configuration As Integer) As Task
        Try
            Dim url As String = $"{BaseUri}Activar/{ids}/{configuration}"
            Dim response As HttpResponseMessage = Await client.PostAsync(url, Nothing)
            If response.IsSuccessStatusCode Then
                MessageBox.Show($"Joint activado con IDs: {ids} y configuración: {configuration}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show($"Error: {Await response.Content.ReadAsStringAsync()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"Exception: {ex.Message}", "Excepción", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Function

    Private Async Function EstablirModoPosicionAsync(id As Integer) As Task
        Try
            Dim url As String = $"{BaseUri}ModoPosicion/{id}"
            Dim response As HttpResponseMessage = Await client.PostAsync(url, Nothing)
            If response.IsSuccessStatusCode Then
                MessageBox.Show($"Modo Posición establecido para el ID: {id}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show($"Error: {Await response.Content.ReadAsStringAsync()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"Exception: {ex.Message}", "Excepción", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Function

    Private Async Function EstablirPosicionAsync(id As Integer, speed As Single, angle As Single) As Task
        Try
            Dim url As String = $"{BaseUri}Posicion/{id}/{speed}/{angle}"
            Dim response As HttpResponseMessage = Await client.PostAsync(url, Nothing)
            If response.IsSuccessStatusCode Then
                MessageBox.Show($"Posición establecida para el ID: {id}, Velocidad: {speed}, Ángulo: {angle}", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show($"Error: {Await response.Content.ReadAsStringAsync()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"Exception: {ex.Message}", "Excepción", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Function

    Private Async Sub Send_Command_Click(sender As Object, e As RoutedEventArgs) Handles Send_Command.Click
        Dim id As Integer = Integer.Parse(CurrentJoint_ComboBox.SelectedItem.ToString())
        Dim speed As Single = Single.Parse(Speed_Textbox.Text)
        Dim angle As Single = -Single.Parse(Position_Textbox.Text)
        Await EstablirPosicionAsync(id, speed, angle)
    End Sub

    Private Async Sub SetMode_Button_Click(sender As Object, e As RoutedEventArgs) Handles SetMode_Button.Click
        Dim id As Integer = Integer.Parse(CurrentJoint_ComboBox.SelectedItem.ToString())

        Await EstablirModoPosicionAsync(id)
    End Sub

    Private Async Sub MoveForward_Button_Click(sender As Object, e As RoutedEventArgs) Handles MoveForward_Button.Click
        Dim id As Integer = Integer.Parse(CurrentJoint_ComboBox.SelectedItem.ToString())
        Dim speed As Single = Single.Parse(Speed_Textbox.Text)
        Dim uri As String = $"{BaseUriConfiguration}/Forward/{id}/{speed}"
        Dim response As HttpResponseMessage = Await client.PostAsync(uri, Nothing)
    End Sub

    Private Async Sub MoveBackward_Button_Click(sender As Object, e As RoutedEventArgs) Handles MoveBackward_Button.Click
        Dim id As Integer = Integer.Parse(CurrentJoint_ComboBox.SelectedItem.ToString())
        Dim speed As Single = Single.Parse(Speed_Textbox.Text)
        Dim uri As String = $"{BaseUriConfiguration}/Backward/{id}/{speed}"
        Dim response As HttpResponseMessage = Await client.PostAsync(uri, Nothing)
    End Sub

    Private Async Sub Create_Button_Click(sender As Object, e As RoutedEventArgs) Handles Create_Button.Click
        Dim ids As String = String.Join(",", MotorStackPanel.Children.OfType(Of TextBox)().Select(Function(tb) tb.Text.Trim()))
        Dim configuration As Integer = Configuration_ComboBox.SelectedIndex

        Await ActivarJointAsync(ids, configuration)
    End Sub

    Private Async Sub Iniciar_Button_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Construir la URI para la solicitud
            Dim uri As String = $"{BaseUriConfiguration}/Iniciar"

            ' Enviar la solicitud POST asincrónicamente
            Dim response As HttpResponseMessage = Await client.PostAsync(uri, Nothing)

            ' Verificar la respuesta
            If response.IsSuccessStatusCode Then
                MessageBox.Show("Servicio iniciado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
            Else
                MessageBox.Show($"Error al iniciar el servicio: {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Async Sub CurrentJoint_ComboBox_DropDownOpened(sender As Object, e As EventArgs)
        Try
            ' Realiza la solicitud al endpoint para obtener las articulaciones
            Dim response As HttpResponseMessage = Await client.GetAsync($"{BaseUri}GetJoints")

            If response.IsSuccessStatusCode Then
                ' Deserializa la respuesta en una lista de JointDto
                Dim jointsDto As List(Of JointDto) = JsonConvert.DeserializeObject(Of List(Of JointDto))(Await response.Content.ReadAsStringAsync())

                ' Limpia los ítems actuales del ComboBox
                CurrentJoint_ComboBox.Items.Clear()

                ' Rellena el ComboBox con los datos de las articulaciones
                For Each joint In jointsDto
                    Dim displayText As String = $"Id de la articulación: {joint.Id}, Número de motores: {joint.NumberOfMotors}, Configuración: {joint.Configuration}"
                    CurrentJoint_ComboBox.Items.Add(displayText)
                Next
            Else
                MessageBox.Show("Error al obtener las articulaciones", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"Excepción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub
End Class

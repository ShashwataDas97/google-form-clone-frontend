Imports System.Net.Http
Imports System.Text
Imports Newtonsoft.Json

Public Class CreateSubmissionForm
    Private stopwatch As Stopwatch
    Private stopwatchRunning As Boolean

    Public Sub New()
        InitializeComponent()
        stopwatch = New Stopwatch()
        stopwatchRunning = False
    End Sub

    ' Event handler for the stopwatch toggle button
    Private Sub btnToggleStopwatch_Click(sender As Object, e As EventArgs) Handles btnToggleStopwatch.Click
        If stopwatchRunning Then
            stopwatch.Stop()
        Else
            stopwatch.Start()
        End If
        stopwatchRunning = Not stopwatchRunning
    End Sub

    ' Event handler for the timer tick
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        txtStopwatchTime.Text = stopwatch.Elapsed.ToString("hh\:mm\:ss")
    End Sub

    ' Event handler for the submit button
    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        ' Create a new submission object
        Dim submission As New Submission() With {
            .Name = txtName.Text,
            .Email = txtEmail.Text,
            .Phone = txtPhoneNumber.Text,
            .github_link = txtGitHubLink.Text,
            .stopwatch_time = txtStopwatchTime.Text
        }

        ' Serialize the submission object to JSON
        Dim json As String = JsonConvert.SerializeObject(submission)

        ' Create an HttpClient instance
        Using client As New HttpClient()
            ' Set up the request content
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Try
                ' Send the POST request
                Dim response As HttpResponseMessage = Await client.PostAsync("http://localhost:3000/submit", content)

                ' Check the response status code
                If response.IsSuccessStatusCode Then
                    MessageBox.Show("Submission successful!")
                    ResetForm()
                Else
                    MessageBox.Show("Submission failed. Status code: " & response.StatusCode)
                End If
            Catch ex As HttpRequestException
                MessageBox.Show("Request error: " & ex.Message)
            Catch ex As Exception
                MessageBox.Show("General error: " & ex.Message)
            End Try
        End Using
    End Sub

    ' Method to reset the form
    Private Sub ResetForm()
        ' Clear the input fields
        txtName.Clear()
        txtEmail.Clear()
        txtPhoneNumber.Clear()
        txtGitHubLink.Clear()
        txtStopwatchTime.Clear()

        ' Reset the stopwatch
        stopwatch.Reset()
        stopwatchRunning = False
        txtStopwatchTime.Text = "00:00:00"
    End Sub

    ' Override method to process custom keyboard shortcuts
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = (Keys.Control Or Keys.S) Then
            btnSubmit.PerformClick()
            Return True
        ElseIf keyData = (Keys.Control Or Keys.T) Then
            btnToggleStopwatch.PerformClick()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
End Class

' Define the Submission class
Public Class Submission
    Public Property name As String
    Public Property email As String
    Public Property phone As String
    Public Property github_link As String
    Public Property stopwatch_time As String
End Class

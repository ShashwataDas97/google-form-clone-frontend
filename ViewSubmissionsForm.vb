Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Text
Imports System.Threading.Tasks
Public Class ViewSubmissionsForm
    Private submissions As List(Of Submission)
    Private currentIndex As Integer

    Public Sub New()
        InitializeComponent()
        currentIndex = 0
        submissions = New List(Of Submission)()
        ' Start the asynchronous loading process without blocking the constructor
        LoadSubmissionsAsync()
    End Sub

    Private Async Sub LoadSubmissionsAsync()
        Await LoadSubmissions()
        DisplaySubmission()
    End Sub

    Private Async Function LoadSubmissions() As Task
        Try
            Using client As New HttpClient()
                Dim index As Integer = 0
                While True
                    Dim response As HttpResponseMessage = Await client.GetAsync($"http://localhost:3000/read?index={index}")
                    If response.IsSuccessStatusCode Then
                        Dim json As String = Await response.Content.ReadAsStringAsync()
                        Dim submission As Submission = JsonConvert.DeserializeObject(Of Submission)(json)
                        submissions.Add(submission)
                        index += 1
                    Else
                        Exit While
                    End If
                End While
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred while loading submissions: " & ex.Message)
        End Try
    End Function

    Private Sub DisplaySubmission()
        If submissions.Count > 0 AndAlso currentIndex >= 0 AndAlso currentIndex < submissions.Count Then
            Dim submission As Submission = submissions(currentIndex)
            txtName.Text = submission.name
            txtEmail.Text = submission.email
            txtPhoneNumber.Text = submission.phone
            txtGitHubLink.Text = submission.github_link
            txtStopwatchTime.Text = submission.stopwatch_time
        Else
            txtName.Text = "N/A"
            txtEmail.Text = "N/A"
            txtPhoneNumber.Text = "N/A"
            txtGitHubLink.Text = "N/A"
            txtStopwatchTime.Text = "00:00:00"
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnPrevious.Click
        'MessageBox.Show("Previous button working successfully")
        If currentIndex > 0 Then
            currentIndex -= 1
            DisplaySubmission()
        End If
    End Sub

    Private Sub btnNext_Click(sender As Object, e As EventArgs) Handles btnNext.Click
        'MessageBox.Show("Next button working successfully")
        If currentIndex < submissions.Count - 1 Then
            currentIndex += 1
            DisplaySubmission()
        End If
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = (Keys.Control Or Keys.P) Then
            btnPrevious.PerformClick()
            Return True
        ElseIf keyData = (Keys.Control Or Keys.N) Then
            btnNext.PerformClick()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
End Class
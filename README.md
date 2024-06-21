# google-form-clone-frontend
### Step 1 : Set Up Project :
1. **Open Visual Studio and Create a New Project :**
   - Select "Visual Basic" -> "Windows Forms App (.NET Framework)"
   - Name the project "GoogleFormsClone"

### Step 2 : Design the Main Form :
1. **Main Form Layout :**
   - Add two buttons: 'btnViewSubmissions' and 'btnCreateNewSubmission'
   - Set their properties accordingly :
     - 'btnViewSubmissions' : Text = "VIEW SUBMISSIONS ( CTRL + V )", Name = "btnViewSubmissions"
     - 'btnCreateNewSubmission' : Text = "CREATE NEW SUBMISSION ( CTRL + N )", Name = "btnCreateNewSubmission"
    
### Step 3 : Create Forms for Viewing and Creating Submissions :
1. **View Submissions Form :**
   - Add a new Windows Form to the project and name it 'ViewSubmissionsForm'.
   - Add two buttons: 'btnPrevious' and 'btnNext'
   - Add labels and readonly textboxes to display submission details.
2. **Create New Submission Form :**
   - Add a new Windows Form to the project and name it 'CreateSubmissionForm'.
   - Add textboxes for Name, Email, Phone Number, GitHub Link.
   - Add a button for toggling the stopwatch: 'btnToggleStopwatch'
   - Add a button for submitting: 'btnSubmit'
   - Add a readonly textbox to display stopwatch time.
  
### Step 4 : Implement Functionality for Main Form :
1. **Main Form Code :**
   
       Public Class Form1
             Private Sub btnViewSubmissions_Click(sender As Object, e As EventArgs) Handles btnViewSubmissions.Click
                 Dim viewForm As New ViewSubmissionsForm()
                 viewForm.ShowDialog()
             End Sub
         
             Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnCreateNewSubmission.Click
                 Dim createForm As New CreateSubmissionForm()
                 createForm.ShowDialog()
             End Sub
         End Class

### Step 5 : Implement Functionality for Viewing Submissions :
1. **View Submissions Form Code :**
   
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
         End Class

### Step 6 : Implement Functionality for Creating Submissions :
1. **Create Submission Form Code :**
   
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
         
             Private Sub btnToggleStopwatch_Click(sender As Object, e As EventArgs) Handles btnToggleStopwatch.Click
                 If stopwatchRunning Then
                     stopwatch.Stop()
                 Else
                     stopwatch.Start()
                 End If
                 stopwatchRunning = Not stopwatchRunning
             End Sub
         
             Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
                 txtStopwatchTime.Text = stopwatch.Elapsed.ToString("hh\:mm\:ss")
             End Sub
         
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
         End Class
         
         ' Define the Submission class
         Public Class Submission
             Public Property name As String
             Public Property email As String
             Public Property phone As String
             Public Property github_link As String
             Public Property stopwatch_time As String
         End Class


### Step 7 : Implement Keyboard Shortcuts :
1. **Main Form Keyboard Shortcuts :**
   
       Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
             If keyData = (Keys.Control Or Keys.V) Then
                 btnViewSubmissions.PerformClick()
                 Return True
             ElseIf keyData = (Keys.Control Or Keys.N) Then
                 btnCreateNewSubmission.PerformClick()
                 Return True
             End If
             Return MyBase.ProcessCmdKey(msg, keyData)
         End Function
2. **View Submissions Form Keyboard Shortcuts :**
   
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
3. **Create Submission Form Keyboard Shortcuts :**
   
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

### Step 8 : Run and Test the Application :
1. **Run the Backend Server :**
   
       npx nodemon src/index.ts
2. **Run the Windows App :**
   - Build and run the Visual Basic project in Visual Studio.
   - Test each functionality (View Submissions, Create New Submission, and Keyboard Shortcuts).

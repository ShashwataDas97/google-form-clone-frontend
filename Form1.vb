Public Class Form1
    ' Event handler for viewing submissions
    Private Sub btnViewSubmissions_Click(sender As Object, e As EventArgs) Handles btnViewSubmissions.Click
        Dim viewForm As New ViewSubmissionsForm()
        viewForm.ShowDialog()
    End Sub

    ' Event handler for creating a new submission
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnCreateNewSubmission.Click
        Dim createForm As New CreateSubmissionForm()
        createForm.ShowDialog()
    End Sub


    ' Override method to process custom keyboard shortcuts
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        ' Check if the pressed key combination is Ctrl + V
        If keyData = (Keys.Control Or Keys.V) Then
            ' Trigger the view submissions button click event
            btnViewSubmissions.PerformClick()
            Return True
            ' Check if the pressed key combination is Ctrl + N
        ElseIf keyData = (Keys.Control Or Keys.N) Then
            ' Trigger the create new submission button click event
            btnCreateNewSubmission.PerformClick()
            Return True
        End If
        ' Call the base class method for other key combinations
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function
End Class

Imports System.Net.Http
Imports System.Text.Json
Imports System.Text
Imports System.Diagnostics

Public Class CreateSubmissionForm
    Private stopwatch As Stopwatch
    Private isRunning As Boolean

    Private Sub CreateSubmissionForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        stopwatch = New Stopwatch()
        isRunning = False

        Dim submitShortcut As New KeysConverter().ConvertFromString("Ctrl+S")

        Me.KeyPreview = True
        AddHandler Me.KeyDown, AddressOf CreateSubmissionForm_KeyDown

        btnSubmit.Text &= " (" & submitShortcut & ")"
    End Sub

    Private Sub btnStopwatchToggle_Click(sender As Object, e As EventArgs) Handles btnStopwatchToggle.Click

        If isRunning Then
            stopwatch.Stop()
        Else
            stopwatch.Start()
        End If
        isRunning = Not isRunning
        UpdateStopwatchDisplay()
    End Sub

    Private Sub UpdateStopwatchDisplay()
        Dim elapsed = stopwatch.Elapsed
        LabelStopwatch.Text = String.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds)
    End Sub

    Private Sub View_submission_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Control AndAlso e.KeyCode = Keys.T Then
            btnStopwatchToggle.PerformClick()
        ElseIf e.Control AndAlso e.KeyCode = Keys.S Then
            btnSubmit.PerformClick()
        End If
    End Sub

    Private Async Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click

        Try
            Dim submission As New Submission() With {
                .name = txtName.Text,
                .email = txtEmail.Text,
                .phone = txtPhone.Text,
                .github_link = txtGitHub.Text,
                .stopwatch_time = stopwatch.Elapsed.ToString("hh\:mm\:ss")
            }

            Using client As New HttpClient()
                Dim jsonContent As String = JsonSerializer.Serialize(submission)
                Dim content As New StringContent(jsonContent, Encoding.UTF8, "application/json")

                Dim response As HttpResponseMessage = Await client.PostAsync("http://localhost:3000/submit", content)
                If response.IsSuccessStatusCode Then
                    MessageBox.Show("Submission successful")
                Else
                    Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                    MessageBox.Show("Submission failed: " & responseBody)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("An error occurred: " & ex.Message)
        End Try
    End Sub

    Private Sub CreateSubmissionForm_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Control AndAlso e.KeyCode = Keys.S Then

            btnSubmit.PerformClick()
        End If
    End Sub
End Class

Public Class Submission
    Public Property name As String
    Public Property email As String
    Public Property phone As String
    Public Property github_link As String
    Public Property stopwatch_time As String
End Class

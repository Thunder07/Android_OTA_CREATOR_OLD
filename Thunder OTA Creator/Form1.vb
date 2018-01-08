Imports System.Security.Cryptography
Imports System.Threading
Public Class Form1
    Private Shared P1 As String
    Private Shared P2 As String
    Private Shared NewN As String
    Private Shared DNO As Integer = 0
    Private Shared DON As Integer = 0
    Private Shared T1 As Thread
    Private Shared T2 As Thread
    Private Shared ok As Date

    
    Private Sub Process(sender As Object, e As EventArgs) Handles Button2.Click
        Try : T1.Abort() : Catch ex As Exception : End Try
        Try : T2.Abort() : Catch ex As Exception : End Try
        ok = Date.Now
        TextBox2.Text = TextBox2.Text.Trim
        If ComboBox1.SelectedIndex = -1 Or ComboBox2.SelectedIndex = -1 Or TextBox2.Text = "" Then
            MsgBox("Please Choose Fill-In The Options Before Proceeding")
            Exit Sub
        End If
        TextBox2.Enabled = False
        ComboBox1.Enabled = False : ComboBox2.Enabled = False
        Button1.Enabled = False : Button2.Enabled = False : Button3.Enabled = False
        TextBox3.Enabled = False : TextBox4.Enabled = False
        TextBox3.Text = TextBox3.Text.Trim : TextBox4.Text = TextBox4.Text.Trim
        P1 = ComboBox1.Text
        P2 = ComboBox2.Text
        NewN = TextBox2.Text
        Dim t As New Thread(AddressOf Compare)
        t.Start()
        ProgressBar1.Style = ProgressBarStyle.Marquee
        ProgressBar1.Value = 1
    End Sub

    Private Sub Compare()
       Dim ss As String = Application.StartupPath & "\ROM\"
        Dim path1 As String = ss & P1
        Dim path2 As String = ss & P2
        T1 = New Thread(AddressOf DNOD)
        T2 = New Thread(AddressOf DOND)
        t1.Name = "T1"
        t2.Name = "T2"

        t1.Start(New IO.DirectoryInfo(path1))
        t2.Start(New IO.DirectoryInfo(path2))
        ' folderDON(New IO.DirectoryInfo(path1))
        'folderDNO(New IO.DirectoryInfo(path2))
        'If t1.IsAlive Then
        '    t1.Join()
        'End If
        'If t2.IsAlive Then
        '    t2.Join()
        'End If


    End Sub

    Private Function compareFileMd5(ByVal filename As String, ByVal oldp As String, ByVal newp As String) As Boolean

        ' get all the file contents
        Dim File1() As Byte = System.IO.File.ReadAllBytes(oldp & "\" & filename)
        ' create a new md5 object
        Dim Md5 As New MD5CryptoServiceProvider()
        ' compute the hash
        Dim byteHash1() As Byte = Md5.ComputeHash(File1)
        ' return the value in base 64

        Dim File2() As Byte = System.IO.File.ReadAllBytes(newp & "\" & filename)
        ' compute the hash
        Dim byteHash2() As Byte = Md5.ComputeHash(File2)
        ' return the value in base 64
        If Convert.ToBase64String(byteHash2) = Convert.ToBase64String(byteHash1) Then
            Return False
        End If
        Return True
    End Function

    Private Function compareOLD2NEW(ByVal filename As String, ByVal oldp As String, ByVal newp As String) As Boolean
        If My.Computer.FileSystem.FileExists(oldp & "\" & filename) Then
            If My.Computer.FileSystem.FileExists(newp & "\" & filename) Then
                If compareFileMd5(filename, oldp, newp) Then
                    Return True
                End If
            Else
                Return False
            End If
        End If
        Return False
    End Function
    Private Function compareNEW2OLD(ByVal filename As String, ByVal oldp As String, ByVal newp As String) As Boolean
        If My.Computer.FileSystem.FileExists(newp & "\" & filename) Then
            If My.Computer.FileSystem.FileExists(oldp & "\" & filename) Then
                If compareFileMd5(filename, oldp, newp) Then
                    Return True
                End If
            Else
                Return True
            End If
        End If
        Return False
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ComboBox1.Items.Clear()
        ComboBox2.Items.Clear()
        Try
            Dim di As New IO.DirectoryInfo(Application.StartupPath + "\ROM\")
            Dim diar1() As IO.DirectoryInfo = di.GetDirectories()
            For x = 0 To diar1.Length() - 1
                ComboBox1.Items.Add(diar1(x).Name())
                ComboBox2.Items.Add(diar1(x).Name())
            Next
        Catch ex As Exception
        End Try
    End Sub


    

    Private Sub folderDNO(ByVal Folder As IO.DirectoryInfo)
        For Each Folder2 As IO.DirectoryInfo In Folder.GetDirectories
            For Each File As IO.FileInfo In Folder2.GetFiles
                If compareNEW2OLD(File.Name, Folder2.FullName, Folder2.FullName.Replace(P1, P2)) Then
                    DNO = 0
AnotherGo:          Try
                        My.Computer.FileSystem.CopyFile(File.FullName.ToString.Replace(P1, P2), File.FullName.ToString.Replace(P1, NewN).Replace("ROM", "OTA"), True)
                    Catch ex As Exception
                        DNO += 1
                        If DNO < 5 Then
                            Thread.Sleep(5000)
                            GoTo AnotherGo
                        End If
                        '''''Report the issue and skip
                        Dim del As New TBD(AddressOf TB)
                        del.Invoke("Failed To Copy " & File.FullName.Replace(Application.StartupPath & "\ROM\", "") & vbNewLine)
                    End Try
                End If
            Next
            folderDNO(Folder2)
        Next
    End Sub
    Private Sub folderDON(ByVal Folder As IO.DirectoryInfo)
        For Each Folder2 As IO.DirectoryInfo In Folder.GetDirectories
            For Each File As IO.FileInfo In Folder2.GetFiles
                If compareOLD2NEW(File.Name, Folder2.FullName, Folder2.FullName.Replace(P1, P2)) Then
                    DON = 0
AnotherGo:          Try
                        My.Computer.FileSystem.CopyFile(File.FullName, File.FullName.ToString.Replace(P2, NewN).Replace("ROM", "OTA"), True)
                    Catch ex As Exception
                        DON += 1
                        If DON < 5 Then
                            Thread.Sleep(5000)
                            GoTo AnotherGo
                        End If
                        '''''Report the issue and skip
                        Dim del As New TBD(AddressOf TB)
                        del.Invoke("Failed To Copy " & File.FullName.Replace(Application.StartupPath & "\ROM\", "") & vbNewLine)
                    End Try
                End If
            Next
            folderDON(Folder2)
        Next
    End Sub
    Private Sub DNOD(ByVal Folder As IO.DirectoryInfo)
        folderDNO(Folder)
        If T2.IsAlive = False Then
            Dim del As New FinishD(AddressOf Finish)
            Me.Invoke(del)
        End If
    End Sub
    Private Sub DOND(ByVal Folder As IO.DirectoryInfo)
        folderDON(Folder)
        If T1.IsAlive = False Then
            Dim del As New FinishD(AddressOf Finish)
            Me.Invoke(del)
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try : My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\ROM\") : Catch ex As Exception : End Try
        Try : My.Computer.FileSystem.CreateDirectory(Application.StartupPath & "\OTA\") : Catch ex As Exception : End Try
    End Sub
    Private Sub Delay(ByVal dblSecs As Double)
        Const OneSec As Double = 1.0# \ (1440.0# * 60.0#)
        Dim dblWaitTil As Date
        Now.AddSeconds(OneSec)
        dblWaitTil = Now.AddSeconds(OneSec).AddSeconds(dblSecs)
        Do Until Now > dblWaitTil
            Application.DoEvents() ' Allow windows messages to be processed
        Loop

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        TextBox1.Clear()
    End Sub

   
    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        TextBox2.Text = TextBox2.Text.Replace("\", "").Replace("/", "").Replace("*", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("""", "").Replace("|", "")
        '\ / * : ? " <> |
        TextBox2.Select(TextBox2.Text.Length, 0)
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Try
            TextBox1.SelectionStart = TextBox1.Text.Length - 1
            TextBox1.SelectionLength = 0
            TextBox1.ScrollToCaret()
        Catch ex As Exception
        End Try
    End Sub

    Private Delegate Sub TBD(ByRef text As String)
    Private Sub TB(ByRef text As String)
        TextBox1.Text &= text
    End Sub
    Private Delegate Sub FinishD()
    Private Sub Finish()
        Excpetion()
        Dim ok2 As TimeSpan = Date.Now.Subtract(ok)
        TextBox1.Text &= "Process Took " & ok2.Minutes & " Minutes And " & ok2.Seconds & " Seconds" & vbNewLine
        TextBox2.Enabled = True
        ComboBox1.Enabled = True : ComboBox2.Enabled = True
        Button1.Enabled = True : Button2.Enabled = True : Button3.Enabled = True
        TextBox3.Enabled = True : TextBox4.Enabled = True
        P1 = Nothing
        P2 = Nothing
        NewN = Nothing
        ProgressBar1.Value = 100
        ProgressBar1.Style = ProgressBarStyle.Blocks
    End Sub

    Private Sub Excpetion()
        TextBox3.Text = TextBox3.Text.Trim
        TextBox4.Text = TextBox4.Text.Trim
        Dim path As String = Application.StartupPath & "\ROM\" & ComboBox2.Text & "\"
        If TextBox3.Text.Length > 0 Then
            Dim tmpfile() As String
            Try
                tmpfile = TextBox3.Text.Split(vbNewLine)
            Catch ex As Exception
                tmpfile = {TextBox3.Text}
            End Try
            For Each file In tmpfile
                file = file.Trim
                DON = 0
AnotherGo:      Try
                    My.Computer.FileSystem.CopyFile(path & file, path.ToString.Replace(P2, NewN).Replace("ROM", "OTA") & file, True)
                Catch ex As Exception
                    DON += 1
                    If DON < 5 Then
                        Thread.Sleep(5000)
                        GoTo AnotherGo
                    End If
                    '''''Report the issue and skip
                    Dim del As New TBD(AddressOf TB)
                    del.Invoke("Failed To Copy " & file & vbNewLine)
                End Try
            Next
        End If
        If TextBox4.Text.Length > 0 Then
            Dim tmpdir() As String
            Try
                tmpdir = TextBox4.Text.Split(vbNewLine)
            Catch ex As Exception
                tmpdir = {TextBox4.Text}
            End Try
            For Each direc In tmpdir
                direc = direc.Trim
                DON = 0
AnotherGo2:     Try
                    My.Computer.FileSystem.CopyDirectory(path & direc, path.ToString.Replace(P2, NewN).Replace("ROM", "OTA") & direc, True)
                Catch ex As Exception
                    DON += 1
                    If DON < 5 Then
                        Thread.Sleep(5000)
                        GoTo AnotherGo2
                    End If
                    '''''Report the issue and skip
                    Dim del As New TBD(AddressOf TB)
                    del.Invoke("Failed To Copy " & direc & vbNewLine)
                End Try
            Next
        End If

    End Sub


    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Diagnostics.Process.Start("http://forum.xda-developers.com/donatetome.php?u=618483")
    End Sub
End Class
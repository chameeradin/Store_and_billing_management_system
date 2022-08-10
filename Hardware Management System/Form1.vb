Imports System.Data.SqlClient
Public Module User
    Public Usr As String
End Module


Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim connection As New SqlConnection("server=DESKTOP-VE5V42R; Database = hmsDB; Trusted_Connection = true")
        Dim command As New SqlCommand("select * from users where usersname = '" & txtUsername.Text & "' and password = '" & txtPassword.Text & "'", connection)


        Dim adapter As New SqlDataAdapter(command)
        Dim table As New DataTable()
        adapter.Fill(table)

        If table.Rows.Count() <= 0 Then
            MessageBox.Show("Username or Password Invalid", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Dim roll As String
            roll = table.Rows.Item(0).ItemArray(3).ToString().Trim()
            If roll = "Admin" Then
                Usr = "Admin"

                Form2.Show()
                Me.Hide()
            ElseIf roll = "Cashier" Then
                Usr = "Cashier"

                Form5.Show()
                Me.Hide()
            Else roll = "Storekeeper"
                Usr = "Storekeeper"

                Form4.Show()
                Me.Hide()

            End If
        End If
    End Sub
End Class
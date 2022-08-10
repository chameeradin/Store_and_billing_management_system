Imports System.Data.SqlClient

Public Class Form7
    Dim connection As New SqlConnection("server=DESKTOP-VE5V42R; Database = hmsDB; Trusted_Connection = true")
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If TextBox1.Text = String.Empty Then
            MessageBox.Show("Category Name Required !", "Wanning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim command As New SqlCommand("insert into category (cat_name) values ('" & TextBox1.Text & "')", connection)
        connection.Open()
        command.ExecuteNonQuery()
        connection.Close()
        MessageBox.Show("Successfully Added !", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        TextBox1.Clear()

        Call AddDataToDropDown()
    End Sub

    Function AddDataToDropDown()

        Dim command As New SqlCommand("select * from category", connection)

        Dim adapter As New SqlDataAdapter(command)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        BindingSource1.DataSource = dt
        ComboBox1.ValueMember = "cat_id"
        ComboBox1.DisplayMember = "cat_name"
        ComboBox1.DataSource = BindingSource1
        Exit Function
    End Function

    Private Sub Form7_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call AddDataToDropDown()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim command As New SqlCommand("delete from category where cat_id = " & ComboBox1.SelectedValue, connection)
        connection.Open()
        command.ExecuteNonQuery()
        connection.Close()
        Call AddDataToDropDown()

    End Sub

    Private Sub Form7_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Form4.AddDataToDropDown()
    End Sub
End Class
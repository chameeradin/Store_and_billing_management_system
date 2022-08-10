Imports System.Data.SqlClient
Public Class Form3
    Dim connection As New SqlConnection("server=DESKTOP-VE5V42R; Database = hmsDB; Trusted_Connection = true")

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Hide()
        Form2.Show()
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call LoadDataSource()
        Call ActionEnable()
        Call AddItemToCombo()

    End Sub
    Function LoadDataSource()
        Dim command As New SqlCommand("select * from users", connection)

        Dim adapter As New SqlDataAdapter(command)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        BindingSource1.DataSource = dt
        DataGridView1.DataSource = BindingSource1
        DataGridView1.Columns(2).Visible = False
        DataGridView1.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill

    End Function

    Public Function AddItemToCombo()
        Dim da As New SqlDataAdapter("select * from Rolls", connection)
        Dim dt As New DataTable()
        da.Fill(dt)
        BindingSource3.DataSource = dt
        ComboBox1.DataSource = BindingSource3
        ComboBox1.DisplayMember = "roll_name"
        ComboBox1.ValueMember = "roll_name"
    End Function
    Private Sub Btn1_Click(sender As Object, e As EventArgs) Handles Btn1.Click
        If Not ValidateControls() Then
            MessageBox.Show("Please Fillout All Fields !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Try
            Dim command As New SqlCommand("insert into users (usersname
                                                          ,password
                                                          ,roll)
                                       values (@usersname, @password, @roll)", connection)
            command.Parameters.AddWithValue("@usersname", TxtBox2.Text)
            command.Parameters.AddWithValue("@password", TxtBox3.Text)
            command.Parameters.AddWithValue("@roll", ComboBox1.SelectedValue)
            connection.Open()
            command.ExecuteNonQuery()
            connection.Close()
            Call LoadDataSource()
            MessageBox.Show("Successfully Added !", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        Finally
            Call ClearTextBoxes()
            connection.Close()
        End Try
    End Sub

    Public Function ValidateControls() As Boolean
        Dim validate As Boolean = True
        If ComboBox1.SelectedIndex = -1 Then
            validate = False
        ElseIf TxtBox2.TextLength = 0 Then
            validate = False
        ElseIf TxtBox3.TextLength = 0 Then
            validate = False
        Else
            Return validate
        End If

        Return validate

    End Function

    Function ClearTextBoxes()
        TxtBox2.Clear()
        TxtBox3.Clear()
        If ComboBox1.SelectedIndex <> -1 Then
            ComboBox1.SelectedIndex = 0
        End If
    End Function

    Private Sub TxtBox2_KeyUp(sender As Object, e As KeyEventArgs) Handles TxtBox2.KeyUp
        Dim command As New SqlCommand("select usersname from users where usersname = @val", connection)
        command.Parameters.AddWithValue("@val", TxtBox2.Text)
        connection.Open()
        Dim reslt As Object = command.ExecuteScalar()
        If Not IsNothing(reslt) Then
            MessageBox.Show("Username is Allready Exit !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TxtBox2.Clear()
        End If

        connection.Close()
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ActionDesable()
        FillControls(DataGridView1.CurrentRow.Index)
    End Sub

    Public Function ActionDesable()
        Btn2.Enabled = True
        Btn3.Enabled = True
        Btn1.Enabled = False
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ActionEnable()
        ClearTextBoxes()
    End Sub
    Public Function ActionEnable()
        Btn2.Enabled = False
        Btn3.Enabled = False
        Btn1.Enabled = True
    End Function

    Public Function FillControls(index As Integer)
        With DataGridView1.Rows(index)
            ComboBox1.SelectedValue = .Cells(3).Value.ToString()
            TxtBox2.Text = .Cells(1).Value.ToString()
            TxtBox3.Text = .Cells(2).Value.ToString()
            ID.Text = .Cells(0).Value.ToString()
        End With
    End Function

    Private Sub Btn2_Click(sender As Object, e As EventArgs) Handles Btn2.Click
        Dim command As New SqlCommand("update users set usersname = @username
                                                          ,password = @password
                                                          ,roll = @roll
                                      where id = @id", connection)

        command.Parameters.AddWithValue("@username", TxtBox2.Text)
        command.Parameters.AddWithValue("@password", TxtBox3.Text)
        command.Parameters.AddWithValue("@id", ID.Text)
        command.Parameters.AddWithValue("@roll", ComboBox1.SelectedValue)
        connection.Open()
        command.ExecuteNonQuery()
        connection.Close()
        Call LoadDataSource()
        MessageBox.Show("Successfully Updated !", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ActionEnable()
        ClearTextBoxes()
    End Sub

    Private Sub Btn3_Click(sender As Object, e As EventArgs) Handles Btn3.Click
        Dim command As New SqlCommand("delete from users where id = @id", connection)
        command.Parameters.AddWithValue("@id", ID.Text)
        connection.Open()
        command.ExecuteNonQuery()
        connection.Close()
        Call LoadDataSource()
        MessageBox.Show("Successfully Deleted !", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ActionEnable()
        ClearTextBoxes()
    End Sub
End Class
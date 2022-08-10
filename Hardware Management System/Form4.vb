Imports System.Data.SqlClient

Public Class Form4
    Dim connection As New SqlConnection("server=DESKTOP-VE5V42R; Database = hmsDB; Trusted_Connection = true")
    Private Sub Btn4_Click(sender As Object, e As EventArgs) Handles Btn4.Click
        If Usr = "Admin" Then
            Me.Close()
        ElseIf Usr = "Storekeeper" Then
            Me.Close()
            Form1.Show()
            Form1.txtUsername.Clear()
            Form1.txtPassword.Clear()

        End If
    End Sub

    Private Sub addCats_Click(sender As Object, e As EventArgs) Handles addCats.Click
        Form7.ShowDialog()
    End Sub

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call ActionEnable()
        Call LoadDataSource()

    End Sub

    Function LoadDataSource()
        Dim command As New SqlCommand("SPselectStore", connection)
        command.CommandType = CommandType.StoredProcedure
        Dim adapter As New SqlDataAdapter(command)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        BindingSource1.DataSource = dt
        DataGridView1.DataSource = BindingSource1
        DataGridView1.Columns(6).Visible = False
        Call AddDataToDropDown()
    End Function

    Function AddDataToDropDown()

        Dim command As New SqlCommand("select * from category", connection)

        Dim adapter As New SqlDataAdapter(command)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        BindingSource2.DataSource = dt
        ComboBox1.ValueMember = "cat_id"
        ComboBox1.DisplayMember = "cat_name"
        ComboBox1.DataSource = BindingSource2
        Exit Function
    End Function

    Private Sub Btn1_Click(sender As Object, e As EventArgs) Handles Btn1.Click

        If Not ValidateControls() Then
            MessageBox.Show("Please Fillout All Fields !", "Warning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Try
            Dim command As New SqlCommand("insert into store (item_name
                                                          ,size
                                                          ,quntity
                                                          ,price
                                                          ,cat_id)
                                       values (@item_name, @size, @quntity, @price, @cat_id)", connection)
            command.Parameters.AddWithValue("@item_name", TextBox2.Text)
            command.Parameters.AddWithValue("@cat_id", ComboBox1.SelectedValue)
            command.Parameters.AddWithValue("@size", TextBox3.Text)
            command.Parameters.AddWithValue("@quntity", TextBox4.Text)
            command.Parameters.AddWithValue("@price", TextBox5.Text)
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

    Function ClearTextBoxes()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox5.Clear()
        ID.Clear()
        If ComboBox1.SelectedIndex <> -1 Then
            ComboBox1.SelectedIndex = 0
        End If
    End Function

    Public Function ActionDesable()
        Btn2.Enabled = True
        Btn3.Enabled = True
        Btn1.Enabled = False
    End Function

    Public Function ActionEnable()

        Btn1.Enabled = True
        Btn2.Enabled = False
        Btn3.Enabled = False

        ClearTextBoxes()
    End Function

    Public Function FillControls(index As Integer)
        With DataGridView1.Rows(index)
            ComboBox1.SelectedValue = .Cells(6).Value
            TextBox2.Text = .Cells(1).Value.ToString()
            TextBox3.Text = .Cells(2).Value.ToString()
            TextBox4.Text = .Cells(3).Value.ToString()
            TextBox5.Text = .Cells(4).Value.ToString()
            ID.Text = .Cells(0).Value.ToString()
        End With
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ActionEnable()
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        ActionDesable()
        FillControls(DataGridView1.CurrentRow.Index)
    End Sub

    Private Sub Btn2_Click(sender As Object, e As EventArgs) Handles Btn2.Click
        Dim command As New SqlCommand("update store set item_name = @item_name
                                                          ,size = @size
                                                          ,quntity = @quntity
                                                          ,price = @price
                                                          ,cat_id = @cat_id
                                      where item_code = @id", connection)

        command.Parameters.AddWithValue("@item_name", TextBox2.Text)
        command.Parameters.AddWithValue("@cat_id", ComboBox1.SelectedValue)
        command.Parameters.AddWithValue("@size", TextBox3.Text)
        command.Parameters.AddWithValue("@quntity", TextBox4.Text)
        command.Parameters.AddWithValue("@price", TextBox5.Text)
        command.Parameters.AddWithValue("@id", ID.Text)
        connection.Open()
        command.ExecuteNonQuery()
        connection.Close()
        Call LoadDataSource()
        MessageBox.Show("Successfully Updated !", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ActionEnable()

    End Sub

    Private Sub Btn3_Click(sender As Object, e As EventArgs) Handles Btn3.Click
        Dim command As New SqlCommand("delete from store where item_code = @id", connection)
        command.Parameters.AddWithValue("@id", ID.Text)
        connection.Open()
        command.ExecuteNonQuery()
        connection.Close()
        Call LoadDataSource()
        MessageBox.Show("Successfully Deleted !", "Succeed", MessageBoxButtons.OK, MessageBoxIcon.Information)
        ActionEnable()
    End Sub

    Public Function ValidateControls() As Boolean
        Dim validate As Boolean = True
        If ComboBox1.SelectedIndex = -1 Then
            validate = False
        ElseIf TextBox2.TextLength = 0 Then
            validate = False
        ElseIf TextBox3.TextLength = 0 Then
            validate = False
        ElseIf TextBox4.TextLength = 0 Then
            validate = False
        ElseIf TextBox5.TextLength = 0 Then
            validate = False
        Else
            Return validate
        End If

        Return validate

    End Function

End Class








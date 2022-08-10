Imports System.Data.SqlClient

Public Class Form5

    Dim connection As New SqlConnection("server=DESKTOP-VE5V42R; Database = hmsDB; Trusted_Connection = true")
    Dim valied As Boolean = False
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Usr = "Admin" Then
            Me.Close()
        ElseIf Usr = "Cashier" Then
            Me.Close()
            Form1.Show()
            Form1.txtUsername.Clear()
            Form1.txtPassword.Clear()
        End If
    End Sub
    'CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalSeconds)
    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCategories()
        TextBox9.Text = CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalSeconds)
    End Sub

    Public Function LoadCategories()
        Dim command As New SqlCommand("select * from category", connection)

        Dim adapter As New SqlDataAdapter(command)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        Dim dr As DataRow = dt.NewRow
        dr("cat_name") = "Select Category"
        dt.Rows.InsertAt(dr, 0)
        BindingSource1.DataSource = dt

        ComboBox1.DataSource = BindingSource1
        ComboBox1.ValueMember = "cat_id"
        ComboBox1.DisplayMember = "cat_name"
    End Function

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex < 1 Then
            Return
        End If
        Dim command As New SqlCommand("select * from store where cat_id=@code", connection)
        command.Parameters.AddWithValue("@code", ComboBox1.SelectedValue)
        Dim adapter As New SqlDataAdapter(command)
        Dim dt1 As New DataTable()
        adapter.Fill(dt1)
        Dim dr As DataRow = dt1.NewRow
        dr("item_name") = "Select Item"
        dt1.Rows.InsertAt(dr, 0)
        BindingSource2.DataSource = dt1
        ComboBox2.ValueMember = "item_code"
        ComboBox2.DisplayMember = "item_name"
        ComboBox2.DataSource = BindingSource2

    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If ComboBox2.SelectedIndex < 1 Then
            Return
        End If

        Dim command As New SqlCommand("select * from store where item_code=@code", connection)
        command.Parameters.AddWithValue("@code", ComboBox2.SelectedValue)
        connection.Open()
        Dim dr As SqlDataReader = command.ExecuteReader()
        dr.Read()
        TextBox4.Text = dr("item_code")
        TextBox7.Text = dr("price")
        connection.Close()


    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        If TextBox7.Text = String.Empty Then
            MessageBox.Show("Price is Emplty !", "Wanning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If (Asc(e.KeyChar) <= 47 Or Asc(e.KeyChar) >= 59) And Not Asc(e.KeyChar) = 8 Then

            e.Handled = True
            Return
        End If
    End Sub

    Private Sub TextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyUp

        If (Not TextBox2.Text = String.Empty) And (Not TextBox1.Text = String.Empty) Then
            TextBox8.Text = (Convert.ToDouble(TextBox7.Text) * Convert.ToInt32(TextBox1.Text) - Convert.ToInt32(TextBox2.Text)).ToString()

        ElseIf Not TextBox1.Text = String.Empty Then
            TextBox8.Text = (Double.Parse(TextBox7.Text) * Integer.Parse(TextBox1.Text)).ToString()
        End If

    End Sub

    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox2.KeyPress
        If TextBox7.Text = String.Empty Then
            MessageBox.Show("Price is Emplty !", "Wanning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If (Asc(e.KeyChar) <= 47 Or Asc(e.KeyChar) >= 59) And Not Asc(e.KeyChar) = 8 Then

            e.Handled = True
            Return
        End If
    End Sub

    Private Sub TextBox2_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyUp
        If (Not TextBox1.Text = String.Empty) And (Not TextBox2.Text = String.Empty) Then
            TextBox8.Text = (Convert.ToDouble(TextBox7.Text) * Convert.ToInt32(TextBox1.Text) - Convert.ToInt32(TextBox2.Text)).ToString()

        ElseIf Not TextBox2.Text = String.Empty Then
            TextBox8.Text = (Double.Parse(TextBox7.Text) * Integer.Parse(TextBox1.Text)).ToString()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not valied Then
            MessageBox.Show("Please Add Valied Quntity !", "Wanning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        If TextBox8.Text = String.Empty Then
            MessageBox.Show("Fill All Fields !", "Wanning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim dgvr As DataGridViewRow = DataGridView1.Rows(0).Clone()

        dgvr.Cells(0).Value = TextBox9.Text
        dgvr.Cells(1).Value = TextBox4.Text
        dgvr.Cells(2).Value = ComboBox2.Text
        dgvr.Cells(3).Value = TextBox7.Text
        dgvr.Cells(4).Value = TextBox1.Text
        dgvr.Cells(5).Value = If(TextBox2.Text = String.Empty, 0, TextBox2.Text)
        dgvr.Cells(6).Value = TextBox8.Text
        DataGridView1.Rows.Add(dgvr)

        ClearControls()
        Calculate()

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        DataGridView1.Rows.RemoveAt(DataGridView1.SelectedRows(0).Index)
        Calculate()
        Button6.Enabled = False
    End Sub

    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        Button6.Enabled = True
    End Sub

    Private Function ClearControls()
        TextBox4.Clear()
        TextBox7.Clear()
        TextBox1.Clear()
        TextBox2.Clear()
        TextBox8.Clear()
        ComboBox2.SelectedIndex = 0
    End Function

    Private Function Calculate()
        Dim Total As Double = 0
        Dim Discount As Double = 0
        Dim NItem As Double = 0
        Dim TAmount As Double = 0

        For index As Integer = 0 To (DataGridView1.Rows.Count - 2)
            With DataGridView1.Rows(index)
                Total = Total + .Cells(3).Value
                Discount = Discount + .Cells(5).Value
                NItem = NItem + 1
                TAmount = TAmount + .Cells(6).Value
            End With
        Next
        TextBox3.Text = TAmount + Discount
        TextBox5.Text = Discount
        TextBox6.Text = NItem
        RichTextBox1.Text = TAmount
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        If Usr = "Admin" Then
            Me.Close()
        ElseIf Usr = "Cashier" Then
            Me.Close()
            Form1.Show()
            Form1.txtUsername.Clear()
            Form1.txtPassword.Clear()

        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'do stuff
        If DataGridView1.Rows.Count <= 1 Then
            MessageBox.Show("Please Add Items !", "Wanning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        InsertDataToTable()
        PrintDocument1.Print()

        TextBox9.Text = CLng(DateTime.UtcNow.Subtract(New DateTime(1970, 1, 1)).TotalSeconds)
        DataGridView1.Rows.Clear()
        TextBox3.Clear()
        TextBox5.Clear()
        TextBox6.Clear()
        RichTextBox1.Clear()

    End Sub

    Public Function InsertDataToTable()
        For index As Integer = 0 To (DataGridView1.Rows.Count - 2)
            With DataGridView1.Rows(index)
                Dim command As New SqlCommand("Insert into invoices (invoiceNo,iteamCode,price,quntity,discount,finalPrice,DateCreate) Values (@invoiceNo,@iteamCode,@price,@quntity,@discount,@finalPrice,@DateCreate)", connection)
                Dim command_1 As New SqlCommand("Update store set quntity=quntity-@qty where item_code = @code", connection)
                command_1.Parameters.AddWithValue("@qty", .Cells(4).Value)
                command_1.Parameters.AddWithValue("@code", .Cells(1).Value)

                command.Parameters.AddWithValue("@invoiceNo", .Cells(0).Value)
                command.Parameters.AddWithValue("@iteamCode", .Cells(1).Value)
                command.Parameters.AddWithValue("@price", .Cells(3).Value)
                command.Parameters.AddWithValue("@quntity", .Cells(4).Value)
                command.Parameters.AddWithValue("@discount", .Cells(5).Value)
                command.Parameters.AddWithValue("@finalPrice", .Cells(6).Value)
                command.Parameters.AddWithValue("@DateCreate", DateTimePicker1.Value.ToString("yyyy/MM/dd"))


                connection.Open()
                command.ExecuteNonQuery()
                command_1.ExecuteNonQuery()
                connection.Close()

            End With
        Next
    End Function

    Private Sub TextBox1_Leave(sender As Object, e As EventArgs) Handles TextBox1.Leave
        Dim command As New SqlCommand("Select quntity from store where item_code=@code", connection)
        command.Parameters.AddWithValue("@code", TextBox4.Text)
        connection.Open()
        Dim curruntQty As Integer = command.ExecuteScalar()
        connection.Close()
        If (curruntQty < Convert.ToInt32(TextBox4.Text)) Then
            MessageBox.Show("Quntity Is Not Available !", "Wanning !", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            valied = False
            Return
        End If
        valied = True
    End Sub

    'Remove Below 2 Lines Here ...
    'Private mRow As Integer = 0
    'Private newpage As Boolean = True

    Private Sub PrintDocument1_PrintPage(sender As Object, e As Printing.PrintPageEventArgs) Handles PrintDocument1.PrintPage

        'Past And Modify Like This ...
        Dim mRow As Integer = 0
        Dim newpage As Boolean = True

        ' sets it to show '...' for long text
        Dim fmt As StringFormat = New StringFormat(StringFormatFlags.LineLimit)
        fmt.LineAlignment = StringAlignment.Center
        fmt.Trimming = StringTrimming.EllipsisCharacter
        Dim y As Int32 = e.MarginBounds.Top
        Dim rc As Rectangle
        Dim x As Int32
        Dim h As Int32 = 0
        Dim row As DataGridViewRow

        ' print the header text for a new page
        '   use a grey bg just like the control
        e.Graphics.DrawString("Alpha Hardware (PVT) Ltd. Invoice No : " & TextBox9.Text & " Date : " & DateTimePicker1.Value.ToString("yyyy/MM/dd"), New Font("Segoe UI Semibold", 13), Brushes.Black, 100, 50)

        If newpage Then
            row = DataGridView1.Rows(mRow)
            x = e.MarginBounds.Left
            For Each cell As DataGridViewCell In row.Cells
                ' since we are printing the control's view,
                ' skip invidible columns
                If cell.Visible Then
                    rc = New Rectangle(x, y, cell.Size.Width, cell.Size.Height)

                    e.Graphics.FillRectangle(Brushes.LightGray, rc)
                    e.Graphics.DrawRectangle(Pens.Black, rc)

                    ' reused in the data pront - should be a function
                    Select Case DataGridView1.Columns(cell.ColumnIndex).DefaultCellStyle.Alignment
                        Case DataGridViewContentAlignment.BottomRight,
                         DataGridViewContentAlignment.MiddleRight
                            fmt.Alignment = StringAlignment.Far
                            rc.Offset(-1, 0)
                        Case DataGridViewContentAlignment.BottomCenter,
                        DataGridViewContentAlignment.MiddleCenter
                            fmt.Alignment = StringAlignment.Center
                        Case Else
                            fmt.Alignment = StringAlignment.Near
                            rc.Offset(2, 0)
                    End Select

                    e.Graphics.DrawString(DataGridView1.Columns(cell.ColumnIndex).HeaderText,
                                            DataGridView1.Font, Brushes.Black, rc, fmt)
                    x += rc.Width
                    h = Math.Max(h, rc.Height)
                End If
            Next
            y += h

        End If
        newpage = False

        ' now print the data for each row
        Dim thisNDX As Int32
        For thisNDX = mRow To DataGridView1.RowCount - 1
            ' no need to try to print the new row
            If DataGridView1.Rows(thisNDX).IsNewRow Then Exit For

            row = DataGridView1.Rows(thisNDX)
            x = e.MarginBounds.Left
            h = 0

            ' reset X for data
            x = e.MarginBounds.Left

            ' print the data
            For Each cell As DataGridViewCell In row.Cells
                If cell.Visible Then
                    rc = New Rectangle(x, y, cell.Size.Width, cell.Size.Height)

                    ' SAMPLE CODE: How To 
                    ' up a RowPrePaint rule
                    'If Convert.ToDecimal(row.Cells(5).Value) < 9.99 Then
                    '    Using br As New SolidBrush(Color.MistyRose)
                    '        e.Graphics.FillRectangle(br, rc)
                    '    End Using
                    'End If

                    e.Graphics.DrawRectangle(Pens.Black, rc)

                    Select Case DataGridView1.Columns(cell.ColumnIndex).DefaultCellStyle.Alignment
                        Case DataGridViewContentAlignment.BottomRight,
                         DataGridViewContentAlignment.MiddleRight
                            fmt.Alignment = StringAlignment.Far
                            rc.Offset(-1, 0)
                        Case DataGridViewContentAlignment.BottomCenter,
                        DataGridViewContentAlignment.MiddleCenter
                            fmt.Alignment = StringAlignment.Center
                        Case Else
                            fmt.Alignment = StringAlignment.Near
                            rc.Offset(2, 0)
                    End Select

                    e.Graphics.DrawString(cell.FormattedValue.ToString(),
                                      DataGridView1.Font, Brushes.Black, rc, fmt)

                    x += rc.Width
                    h = Math.Max(h, rc.Height)
                End If

            Next
            y += h
            ' next row to print
            mRow = thisNDX + 1

            If y + h > e.MarginBounds.Bottom Then
                e.HasMorePages = True
                ' mRow -= 1   causes last row to rePrint on next page
                newpage = True
                Return
            End If
        Next
        e.Graphics.DrawString("Sub Total : Rs." & TextBox3.Text & ".00 / Discount : Rs. " & TextBox5.Text & ".00 / Total Amount : Rs. " & RichTextBox1.Text & ".00", New Font("Segoe UI Semibold", 13), Brushes.Black, 100, 700)
    End Sub
End Class
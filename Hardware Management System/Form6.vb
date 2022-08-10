Imports System.Data.SqlClient

Public Class Form6

    Dim connection As New SqlConnection("server=DESKTOP-VE5V42R; Database = hmsDB; Trusted_Connection = true")
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Hide()
        Form2.Show()
    End Sub
    Public Function loadInvoiceId(dte As String)
        Dim da As New SqlDataAdapter("Select distinct invoiceNo from invoices where DateCreate=@date", connection)
        da.SelectCommand.Parameters.AddWithValue("@date", dte)
        Dim ds As New DataSet()
        da.Fill(ds)
        Dim dr As DataRow = ds.Tables(0).NewRow
        dr("invoiceNo") = "Select Invoice Number"
        ds.Tables(0).Rows.InsertAt(dr, 0)
        BindingSource1.DataSource = ds.Tables(0)

        ComboBox1.DataSource = BindingSource1
        ComboBox1.DisplayMember = "invoiceNo"
        ComboBox1.ValueMember = "invoiceNo"

    End Function
    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadInvoiceId(DateTimePicker1.Value.ToString("yyyy/MM/dd"))
    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        ComboBox1.SelectedIndex = 0

        If DailyReportGrid.Rows.Count > 0 Then
            DailyReportGrid.DataSource = Nothing
        End If

        loadInvoiceId(DateTimePicker1.Value.ToString("yyyy/MM/dd"))
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex > 0 Then
            loadDataToDailyReportGrid()
        End If
    End Sub

    Public Function loadDataToDailyReportGrid()
        Dim da As New SqlDataAdapter("Select iteamCode as [Item Code], price as Price, quntity as Quntity, discount as Discount, finalPrice as [Final  Price] from invoices where invoiceNo=@invoiceNo", connection)
        da.SelectCommand.Parameters.AddWithValue("@invoiceNo", ComboBox1.SelectedValue)
        Dim ds As New DataSet()
        da.Fill(ds)
        BindingSource2.DataSource = ds.Tables(0)
        DailyReportGrid.DataSource = BindingSource2
        DailyReportGrid.RowHeadersVisible = False
        DailyReportGrid.Columns(1).Width = 200
        DailyReportGrid.Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        Calculate()
    End Function

    Private Function Calculate()
        Dim Total As Double = 0
        Dim Discount As Double = 0
        Dim NItem As Double = 0
        Dim TAmount As Double = 0

        For index As Integer = 0 To (DailyReportGrid.Rows.Count - 2)
            With DailyReportGrid.Rows(index)
                Total = Total + .Cells(1).Value
                Discount = Discount + .Cells(3).Value
                NItem = NItem + 1
                TAmount = TAmount + .Cells(4).Value
            End With
        Next
        TextBox3.Text = TAmount + Discount
        TextBox5.Text = Discount
        TextBox6.Text = NItem
        TextBox2.Text = TAmount
    End Function

    Private Sub DailyReportGrid_RowsRemoved(sender As Object, e As DataGridViewRowsRemovedEventArgs) Handles DailyReportGrid.RowsRemoved
        ClearControls()
    End Sub

    Public Function ClearControls()
        TextBox3.Clear()
        TextBox6.Clear()
        TextBox5.Clear()
        TextBox2.Clear()
    End Function

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedIndex = 1 Then
            loadDailyReport(DateTimePicker2.Value.ToString("yyyy/MM/dd"))
        End If
    End Sub

    Public Function loadDailyReport(dte As String)
        Dim da1 As New SqlDataAdapter("select invoiceNo, count(invoiceNo) as [Total Items], sum(price)  as [Sub Total], sum(discount) as [Total Discount], sum(finalPrice) as [Final Price] from invoices where DateCreate = @date group by invoiceNo ", connection)
        da1.SelectCommand.Parameters.AddWithValue("@date", dte)
        Dim ds1 As New DataSet()
        da1.Fill(ds1)
        DataGridView1.DataSource = ds1.Tables(0)
        DataGridView1.Columns(0).Width = 200
        DataGridView1.Columns(4).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        DataGridView1.RowHeadersVisible = False
        Calculate_1()
    End Function

    Public Function Calculate_1()
        TextBox4.Text = DataGridView1.Rows.Count - 1
        Dim dailyIncome As Double = 0
        If DataGridView1.Rows.Count > 0 Then
            For index As Integer = 0 To (DataGridView1.Rows.Count - 2)
                With DataGridView1.Rows(index)
                    dailyIncome = dailyIncome + .Cells(4).Value
                End With
            Next
            TextBox1.Text = dailyIncome
        End If

    End Function

    Private Sub DateTimePicker2_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker2.ValueChanged
        loadDailyReport(DateTimePicker2.Value.ToString("yyyy/MM/dd"))
    End Sub
End Class
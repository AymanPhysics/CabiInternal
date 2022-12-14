Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO
Imports System.Windows.Forms

Public Class CalcSalary
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Hdr As String = ""
    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If Val(txtMonth.Text) = 0 OrElse Val(txtYear.Text) = 0 Then Return

        Dim rpt As New ReportViewer
        Select Case Flag
            Case 1
                MyEmpId = Val(EmpId.Text)
                MyMonth = Val(txtMonth.Text)
                MyYear = Val(txtYear.Text)
                Button2.IsEnabled = False
                BackgroundWorker1.RunWorkerAsync()
                Return
            Case 2
                rpt.Rpt = "SalaryHistoryShifts.rpt"
            Case 3
                rpt.Rpt = "SalaryHistoryNotShifts.rpt"
            Case 4
                LoadLog()
                Return
            Case 5
                rpt.Rpt = "SalaryHistoryAllTax.rpt"
            Case 6
                If bm.ShowDeleteMSG("إغلاق الوردية لا يمكنك من إعادة فتحها مرة أخرى" & vbCrLf & vbCrLf & "هل أنت متأكد من إغلاق الوردية؟") Then

                    If Md.ShowShiftForEveryStore Then
                        dt = bm.ExcuteAdapter("CloseShiftForEveryStore", New String() {"StoreId"}, New String() {Md.DefaultStore})
                    Else
                        bm.ExcuteNonQuery("exec CloseShift")
                    End If

                    bm.ShowMSG("تم إغلاق الوردية")
                    Try
                        'Forms.Application.Restart()
                        'Application.Current.Shutdown()
                    Catch ex As Exception
                    End Try
                End If
                Return
            Case 7
                Dim maintbl As String = "ServiceGroups"
                rpt.Rpt = IIf(maintbl = "", "PrintTbl.rpt", "PrintTbl2.rpt")
                rpt.paraname = {"Header", "@tbl", "@maintbl", "@mainfield"}
                rpt.paravalue = {CType(Parent, Page).Title, "ServiceTypes", maintbl, "ServiceGroupId"}
                rpt.Show()
                Return
            Case 8
                rpt.Rpt = "Employees.rpt"
            Case 9
                rpt.Rpt = "AllCallers.rpt"
        End Select

        rpt.paraname = New String() {"@AccNo", "@Month", "@Period", "@Year", "Header"}
        rpt.paravalue = New String() {TaxAccNo.Text.Trim, Val(txtMonth.Text), Val(txtMonth.Text), Val(txtYear.Text), CType(Parent, Page).Title}
        rpt.Show()

    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.Addcontrol_MouseDoubleClick({EmpId, TaxAccNo})
        LoadResource()
        Dim MyNow As DateTime = bm.MyGetDate()
        txtMonth.Text = MyNow.Month
        txtYear.Text = MyNow.Year

        If Flag <> 1 Then
            lblEmpId.Visibility = Windows.Visibility.Hidden
            EmpId.Visibility = Windows.Visibility.Hidden
            EmpName.Visibility = Windows.Visibility.Hidden
        End If

        If Flag = 6 Then
            GG.Children.Clear()
            Button2.Content = "إغلاق الوردية"
        ElseIf Flag = 7 Then
            GG.Children.Clear()
        ElseIf Flag = 8 Then
            GG.Children.Clear()
        ElseIf Flag = 9 Then
            GG.Children.Clear()
        End If


        If Flag = 5 Then
            TaxAccNo.Text = bm.ExecuteScalar("select dbo.GetTaxAcc()")
            TaxAccNo_LostFocus(Nothing, Nothing)
            Select Case MyNow.Month
                Case Is <= 3
                    txtMonth.Text = 1
                Case Is <= 6
                    txtMonth.Text = 2
                Case Is <= 9
                    txtMonth.Text = 3
                Case Else
                    txtMonth.Text = 4
            End Select
        End If
    End Sub
    Private Sub LoadResource()

        lblTaxAcc.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "TaxAcc")
        lblFromDate.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Month")
        lblFromDate_Copy.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Year")

        Select Case Flag
            Case 1
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "Calculate")
            Case 2, 3, 7, 8, 9
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "View Report")
            Case 4
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "Import Attendance")
            Case 5
                Button2.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "View Report")
                lblFromDate.SetResourceReference(System.Windows.Controls.Label.ContentProperty, "Period")
        End Select

        If Flag <> 5 Then
            lblTaxAcc.Visibility = Windows.Visibility.Hidden
            TaxAccNo.Visibility = Windows.Visibility.Hidden
            TaxAccName.Visibility = Windows.Visibility.Hidden
        End If
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtMonth.KeyDown, txtYear.KeyDown, TaxAccNo.KeyDown, EmpId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub LoadLog()
        Dim oo As New OpenFileDialog
        oo.Filter = "1_attlog.dat|1_attlog.dat"
        oo.FileName = "1_attlog.dat"
        If oo.ShowDialog() = DialogResult.Cancel Then Return
        Dim path As String = oo.FileName
        If Not File.Exists(path) Then
            bm.ShowMSG("Invalid path")
            Return
        End If

        Dim st As New StreamReader(path)
        Dim s As String = ""

        Dim AttendanceLogDT As New DataTable
        AttendanceLogDT.Columns.Add("EmpId")
        AttendanceLogDT.Columns.Add("DayDate")
        AttendanceLogDT.Columns.Add("State")

        Try
            While True
                s = st.ReadLine()
                If Val(s.Substring(10, 4)) = Val(txtYear.Text) AndAlso Val(s.Substring(15, 2)) = Val(txtMonth.Text) Then
                    AttendanceLogDT.Rows.Add({s.Substring(1, 8), s.Substring(10, 19), s.Substring(32, 1)})
                End If
            End While
        Catch ex As Exception
        End Try
        If bm.ExcuteNonQuery("SaveAttandanceLog", {"AttendanceLog"}, {AttendanceLogDT}, {SqlDbType.Structured}) Then
            bm.ShowMSG("Saved Successfuly")
        Else
            bm.ShowMSG("Faild to be Saved")
        End If

    End Sub

    Private Sub TaxAccNo_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles TaxAccNo.LostFocus
        bm.AccNoLostFocus(TaxAccNo, TaxAccName, , , )
    End Sub

    Private Sub TaxAccNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles TaxAccNo.KeyUp
        bm.AccNoShowHelp(TaxAccNo, TaxAccName, e, , , )
    End Sub



    Dim MyEmpId As Integer = 0, MyMonth As Integer = 0, MyYear As Integer = 0
    Dim WithEvents BackgroundWorker1 As New System.ComponentModel.BackgroundWorker
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Select Case Flag
            Case 1
                bm.ExcuteNonQuery("CalcSalary", New String() {"EmpId", "Month", "Year"}, New String() {MyEmpId, MyMonth, MyYear})
        End Select
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        bm.ShowMSG("Done Successfuly")
        Button2.IsEnabled = True
    End Sub

    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        If bm.ShowHelp("Employees", EmpId, EmpName, e, "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees") Then
            EmpId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub EmpId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EmpId.LostFocus
        If Val(EmpId.Text.Trim) = 0 Then
            EmpId.Clear()
            EmpName.Clear()
            Return
        End If
        bm.LostFocus(EmpId, EmpName, "select " & Resources.Item("CboName") & " Name from Employees where Id=" & EmpId.Text.Trim())
    End Sub


End Class
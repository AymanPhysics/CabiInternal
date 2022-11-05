Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO

Public Class RPT25
    Dim bm As New BasicMethods
    Dim dt As New DataTable

    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click

        Dim rpt As New ReportViewer
        Select Case Flag
            Case 1
                rpt.Rpt = "Loans.rpt"
            Case 2
                rpt.Rpt = "DirectBonus.rpt"
            Case 3
                rpt.Rpt = "DirectCut.rpt"
            Case 4
                rpt.Rpt = "LeaveRequests.rpt"
            Case 5
                rpt.Rpt = "LeaveRequests2.rpt"
            Case 6
                rpt.Rpt = "LoansStatus.rpt"
            Case 7
                rpt.Rpt = "EmpShifts.rpt"
            Case 8, 10
                MyFromDate = FromDate.SelectedDate
                MyToDate = ToDate.SelectedDate
                Button2.IsEnabled = False
                BackgroundWorker1.RunWorkerAsync()
                Return
            Case 9
                rpt.Rpt = "NurseShiftSummary.rpt"
            Case 11
                Dim ff As New Plan With {.MyFromDate = FromDate.SelectedDate, .MyToDate = ToDate.SelectedDate}
                Dim frm As New Window With {.WindowState = WindowState.Maximized, .WindowStyle = WindowStyle.None, .Content = ff}
                frm.Show()
                Return
            Case 12
                rpt.Rpt = "EmpOutcome.rpt"
            Case 13
                rpt.Rpt = "CloseShift.rpt"
                If IsDetailed.IsChecked Then
                    rpt.Rpt = "CloseShiftDetailed.rpt"
                End If
            Case 14
                rpt.Rpt = "AllServicesTypes.rpt"
        End Select

        rpt.paraname = New String() {"@EmpId", "@FromDate", "@ToDate", "Header", "IsDetailed"}
        rpt.paravalue = New String() {Val(EmpId.Text), FromDate.SelectedDate, ToDate.SelectedDate, CType(Parent, Page).Title, IIf(IsDetailed.IsChecked, 1, 0)}
        rpt.Show()

    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()
        bm.Addcontrol_MouseDoubleClick({EmpId})
        Dim MyNow As DateTime = bm.MyGetDate()
        FromDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        ToDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        IsDetailed.Visibility = Visibility.Hidden
        Select Case Flag
            Case 6
                FromDate.Visibility = Windows.Visibility.Hidden
                lblFromDate.Visibility = Windows.Visibility.Hidden
                ToDate.Visibility = Windows.Visibility.Hidden
                lblToDate.Visibility = Windows.Visibility.Hidden
            Case 8
                FromDate.SelectedDate = New DateTime(MyNow.Year - 1, 12, 31, 0, 0, 0)
                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
                lblFromDate.Content = "أول المدة في"
                lblToDate.Content = "آخر المدة في"
            Case 10
                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
                'lblFromDate.Visibility = Windows.Visibility.Hidden
                'FromDate.Visibility = Windows.Visibility.Hidden
                lblFromDate.Content = "في تاريخ"
                FromDate.SelectedDate = New DateTime(MyNow.Year - 1, 12, 31, 0, 0, 0)
                lblToDate.Visibility = Windows.Visibility.Hidden
                ToDate.Visibility = Windows.Visibility.Hidden
            Case 11
                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
            Case 13
                IsDetailed.Visibility = Visibility.Visible
            Case 14
                FromDate.Visibility = Windows.Visibility.Hidden
                lblFromDate.Visibility = Windows.Visibility.Hidden
                ToDate.Visibility = Windows.Visibility.Hidden
                lblToDate.Visibility = Windows.Visibility.Hidden

                lblEmpId.Visibility = Windows.Visibility.Hidden
                EmpId.Visibility = Windows.Visibility.Hidden
                EmpName.Visibility = Windows.Visibility.Hidden
        End Select
    End Sub
    Private Sub LoadResource()

        
        lblEmpId.SetResourceReference(Label.ContentProperty, "Employee")
        lblFromDate.SetResourceReference(Label.ContentProperty, "From Date")
        lblToDate.SetResourceReference(Label.ContentProperty, "To Date")

        Select Case Flag
            Case 8, 10
                Button2.SetResourceReference(Button.ContentProperty, "Calculate")
            Case 11
                Button2.SetResourceReference(Button.ContentProperty, "View")
            Case Else
                Button2.SetResourceReference(Button.ContentProperty, "View Report")
        End Select
        
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        Dim str As String = "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees where 1=1 "
        str &= IIf(Flag = 7, " and SalaryOrShifts=1", "")
        str &= IIf(Flag = 12, " and Doctor=0", "")
        str &= IIf(Flag = 13, " and Doctor=0", "")

        If bm.ShowHelp("Employees", EmpId, EmpName, e, str) Then
            EmpId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub EmpId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EmpId.LostFocus
        If Val(EmpId.Text.Trim) = 0 Then
            EmpId.Clear()
            EmpName.Clear()
            Return
        End If

        Dim str As String = "select " & Resources.Item("CboName") & " Name from Employees where Id=" & EmpId.Text.Trim()
        str &= IIf(Flag = 7, " and SalaryOrShifts=1", "")
        str &= IIf(Flag = 12, " and Doctor=0", "")
        str &= IIf(Flag = 13, " and Doctor=0", "")

        bm.LostFocus(EmpId, EmpName, str)
    End Sub

    Dim MyFromDate As Date, MyToDate As Date
    Dim WithEvents BackgroundWorker1 As New ComponentModel.BackgroundWorker
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Select Case Flag
            Case 8
                bm.ExcuteNonQuery("CalcAvgCost", New String() {"FromDate", "ToDate"}, New String() {bm.ToStrDate(MyFromDate), bm.ToStrDate(MyToDate)})
                bm.ExcuteNonQuery("CalcItemsBalCostGroup", New String() {"FromDate", "ToDate"}, New String() {bm.ToStrDate(MyFromDate), bm.ToStrDate(MyToDate)})
            Case 10
                'bm.ExcuteNonQuery("CalcImportMessagesOpennedOnly", {}, {})
                bm.ExcuteNonQuery("CalcImportMessageCostSubAll", {"DeliveredDate"}, {bm.ToStrDate(MyFromDate)})
        End Select
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        bm.ShowMSG("Done Successfuly")
        Button2.IsEnabled = True
    End Sub


End Class
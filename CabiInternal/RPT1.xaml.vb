Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO

Public Class RPT1
    Dim bm As New BasicMethods
    Dim dt As New DataTable

    Public Flag As Integer = 0
    Public Cancel As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click

        Dim rpt As New ReportViewer
        Select Case Flag
            Case 1, 4
                rpt.Rpt = "CallCenterDetailed.rpt"
            Case 2
                rpt.Rpt = "CallCenterTotal.rpt"
            Case 3
                rpt.Rpt = "CallCenterNetIncome.rpt"
            Case 5
                rpt.Rpt = "CallCenterKnownUs.rpt"
            Case 6
                rpt.Rpt = "CallCenterBrief.rpt"
            Case 7
                rpt.Rpt = "EmpOutCome1.rpt"
            Case 8
                rpt.Rpt = "EmpComplaints.rpt"
            Case 9
                rpt.Rpt = "CallCenterShiftClosing.rpt"
        End Select

        rpt.paraname = New String() {"@FromDate", "@ToDate", "@EmpId", "@DeliverymanId", "@CategoryId", "@CallerId", "Header", "@Line", "@Cancel", "@Shift", "@CarId", "@CarOtherId"}
        rpt.paravalue = New String() {FromDate.SelectedDate, ToDate.SelectedDate, Val(EmpId.Text), Val(DeliverymanId.Text), Val(CategoryId.SelectedValue), CallerId.Text.Trim, CType(Parent, Page).Title, Val(OrderNo.Text), Cancel, Val(ShiftId.SelectedValue), Val(CarId.SelectedValue), Val(CarOtherId.Text)}
        rpt.Show()

    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()

        If Flag = 7 Then
            lblDeliverymanId.Visibility = Windows.Visibility.Hidden
            DeliverymanId.Visibility = Windows.Visibility.Hidden
            DeliverymanName.Visibility = Windows.Visibility.Hidden
             
            lblCallerId.Visibility = Windows.Visibility.Hidden
            CallerId.Visibility = Windows.Visibility.Hidden
            CallerName.Visibility = Windows.Visibility.Hidden

            lblOrderNo.Visibility = Windows.Visibility.Hidden
            OrderNo.Visibility = Windows.Visibility.Hidden
             
        ElseIf Flag = 8 Then
            lblDeliverymanId.Visibility = Windows.Visibility.Hidden
            DeliverymanId.Visibility = Windows.Visibility.Hidden
            DeliverymanName.Visibility = Windows.Visibility.Hidden

            lblCategoryId.Visibility = Windows.Visibility.Hidden
            CategoryId.Visibility = Windows.Visibility.Hidden

            lblOrderNo.Visibility = Windows.Visibility.Hidden
            OrderNo.Visibility = Windows.Visibility.Hidden 

            lblOrderNo.Visibility = Windows.Visibility.Hidden
            OrderNo.Visibility = Windows.Visibility.Hidden

            lblCarId.Visibility = Windows.Visibility.Hidden
            CarId.Visibility = Windows.Visibility.Hidden
        End If

        bm.FillCombo("CallCenterCategories", CategoryId, "")
        CategoryId_LostFocus(Nothing, Nothing)
        bm.FillCombo("Shifts", ShiftId, "")
        bm.Addcontrol_MouseDoubleClick({EmpId, DeliverymanId, CallerId, CarOtherId})
        Dim MyNow As DateTime = bm.MyGetDate()
        FromDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        ToDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
        IsDetailed.Visibility = Visibility.Hidden
    End Sub
    Private Sub LoadResource()


        lblEmpId.SetResourceReference(Label.ContentProperty, "Employee")
        lblDeliverymanId.SetResourceReference(Label.ContentProperty, "Deliveryman")
        lblFromDate.SetResourceReference(Label.ContentProperty, "From Date")
        lblToDate.SetResourceReference(Label.ContentProperty, "To Date")
        Button2.SetResourceReference(Button.ContentProperty, "View Report")
        lblShiftId.SetResourceReference(Label.ContentProperty, "Shift")
         
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyDown, DeliverymanId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub
    Private Sub EmpId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles EmpId.KeyUp
        Dim str As String = "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees where 1=1 "
        'str &= IIf(Flag = 13, " and Doctor=0", "")

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
        'str &= IIf(Flag = 7, " and SalaryOrShifts=1", "") 
        bm.LostFocus(EmpId, EmpName, str)
    End Sub
    Private Sub DeliverymanId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles DeliverymanId.KeyUp
        Dim str As String = "Select cast(Id as varchar(10))Id," & Resources.Item("CboName") & " Name from Employees where Deliveryman=1 "
        'str &= IIf(Flag = 13, " and Doctor=0", "")

        If bm.ShowHelp("Deliverymen", DeliverymanId, DeliverymanId, e, str) Then
            DeliverymanId_LostFocus(sender, Nothing)
        End If
    End Sub
    Private Sub DeliverymanId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DeliverymanId.LostFocus
        If Val(DeliverymanId.Text.Trim) = 0 Then
            DeliverymanId.Clear()
            DeliverymanName.Clear()
            Return
        End If
        Dim str As String = "select " & Resources.Item("CboName") & " Name from Employees where Deliveryman=1 and Id=" & DeliverymanId.Text.Trim()
        'str &= IIf(Flag = 7, " and SalaryOrShifts=1", "") 
        bm.LostFocus(DeliverymanId, DeliverymanName, str)
    End Sub

    Private Sub CallerId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CallerId.KeyUp
        Dim str As String = "Select distinct cast(CallerId as varchar(100))Id,CallerName Name from CallCenter where 1=1 "
        If bm.ShowHelp("Callers", CallerId, CallerName, e, str) Then
            CallerId_LostFocus(sender, Nothing)
        End If
    End Sub

    Private Sub CallerId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CallerId.LostFocus
        If Val(CallerId.Text.Trim) = 0 Then
            CallerId.Clear()
            CallerName.Clear()
            Return
        End If
        Dim str As String = "select top 1 CallerName Name from CallCenter where CallerId='" & CallerId.Text.Trim() & "'"
        'str &= IIf(Flag = 7, " and SalaryOrShifts=1", "") 
        bm.LostFocus(CallerId, CallerName, str)
    End Sub

    Private Sub CategoryId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CategoryId.LostFocus
        bm.FillCombo("select Id,Name,dbo.GetCarState(CategoryId,Id)State From Cars where CategoryId=" & Val(CategoryId.SelectedValue) & " union select 0 Id,'-' Name,'#FFFFFFFF' union select -1 Id,'External' Name,'#FFFFFFFF'", CarId)
        CarId_LostFocus(Nothing, Nothing)
    End Sub

    Private Sub CarId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CarId.LostFocus
    If Val(CarId.SelectedValue) = -1 Then
            lblCarOtherId.Visibility = Windows.Visibility.Visible
            CarOtherId.Visibility = Windows.Visibility.Visible
            CarOtherName.Visibility = Windows.Visibility.Visible
        Else
            lblCarOtherId.Visibility = Windows.Visibility.Hidden
            CarOtherId.Visibility = Windows.Visibility.Hidden
            CarOtherName.Visibility = Windows.Visibility.Hidden
            CarOtherId.Clear()
            CarOtherName.Clear()
        End If
    End Sub

    Private Sub CarOtherId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CarOtherId.LostFocus
        If Val(CategoryId.SelectedValue) = 0 Then
            CarOtherId.Clear()
            CarOtherName.Clear()
            Return
        End If
        bm.LostFocus(CarOtherId, CarOtherName, "select Name from CarOthers where Id=" & CarOtherId.Text.Trim() & " and CategoryId=" & Val(CategoryId.SelectedValue))
    End Sub

    Private Sub CarOtherId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CarOtherId.KeyUp
        bm.ShowHelp("External Cars", CarOtherId, CarOtherName, e, "select cast(Id as varchar(100)) Id,Name from CarOthers  where CategoryId=" & Val(CategoryId.SelectedValue), "CarOthers", {"CategoryId"}, {Val(CategoryId.SelectedValue)})
    End Sub

End Class
Imports System.Data

Public Class EmpOutcome
    Public MainTableName As String = "Employees"
    Public MainSubId As String = "Id"
    Public MainSubName As String = "Name"

    Public TableName As String = "EmpOutcome"
    Public MainId As String = "EmpId"
    Public MainId2 As String = "DayDate"
    Public MainId3 As String = "Shift"
    Public SubId As String = "Id"

    Dim dt As New DataTable
    Dim bm As New BasicMethods

    Dim m As MainWindow = Application.Current.MainWindow
    Public Flag As Integer = 0
    Public WithImage As Boolean = False
    Public ReLoadMenue As Boolean = False

    Private Sub BasicForm2_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()
        If WithImage Then
            btnSetImage.Visibility = Visibility.Visible
            btnSetNoImage.Visibility = Visibility.Visible
            Image1.Visibility = Visibility.Visible
        End If

        bm.FillCombo("Shifts", Shift, "")
        bm.FillCombo(MainTableName, CboMain, "")
        bm.FillCombo("CallCenterCategories", CategoryId, "")
        CategoryId_LostFocus(Nothing, Nothing)
        CarId_LostFocus(Nothing, Nothing)
        bm.Fields = {MainId, MainId2, MainId3, SubId, "ToName", "Value", "Notes", "CategoryId", "CarId", "DeliverymanId", "CarOtherId"}
        bm.control = {CboMain, DayDate, Shift, txtID, txtName, Value, Notes, CategoryId, CarId, DeliverymanId, CarOtherId}
        bm.KeyFields = {MainId, MainId2, MainId3, SubId}

        bm.Table_Name = TableName
        CboMain.SelectedValue = Md.UserName
        CboMain.IsEnabled = False ' Md.Manager
        DayDate.IsEnabled = False ' Md.Manager
        btnNew_Click(sender, e)
    End Sub

    Sub FillControls()
        bm.FillControls()
        CategoryId_LostFocus(Nothing, Nothing)
        bm.FillControls()
        CarId_LostFocus(Nothing, Nothing)
        CarOtherId_LostFocus(Nothing, Nothing)
    End Sub

    Private Sub btnLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLast.Click
        bm.FirstLast(New String() {MainId, MainId2, MainId3, SubId}, "Max", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        bm.NextPrevious(New String() {MainId, MainId2, MainId3, SubId}, New String() {CboMain.SelectedValue.ToString, bm.ToStrDate(DayDate.SelectedDate), Shift.SelectedValue, txtID.Text}, "Next", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If (txtName.Text.Trim = "" AndAlso DeliverymanId.SelectedValue = 0) Or Value.Text.Trim = "" Or CboMain.SelectedValue.ToString = 0 Then
            Return
        End If
        Value.Text = Val(Value.Text)

        bm.DefineValues()
        If Not bm.Save(New String() {MainId, MainId2, MainId3, SubId}, New String() {CboMain.SelectedValue.ToString, bm.ToStrDate(DayDate.SelectedDate), Shift.SelectedValue, txtID.Text.Trim}) Then Return

        btnNew_Click(sender, e)
    End Sub

    Private Sub btnFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFirst.Click
        bm.FirstLast(New String() {MainId, MainId2, MainId3, SubId}, "Min", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        'bm.ClearControls()
        ClearControls()
    End Sub

    Sub ClearControls()
        Try
            bm.ClearControls()
            CategoryId_LostFocus(Nothing, Nothing)
            CarId_LostFocus(Nothing, Nothing)
            If WithImage Then bm.SetNoImage(Image1)
            DayDate.SelectedDate = Md.CurrentDate
            Shift.SelectedValue = Md.CurrentShiftId
            txtID.Text = bm.ExecuteScalar("select max(" & SubId & ")+1 from " & TableName & " where " & MainId & "='" & CboMain.SelectedValue.ToString & "' and " & MainId2 & "='" & bm.ToStrDate(DayDate.SelectedDate) & "' and " & MainId3 & "='" & Shift.SelectedValue & "'")
            If txtID.Text = "" Then txtID.Text = "1"
            txtName.Focus()

        Catch
        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExcuteNonQuery("delete from " & TableName & " where " & SubId & "='" & txtID.Text.Trim & "' and " & MainId & " ='" & CboMain.SelectedValue.ToString & "' and " & MainId2 & "='" & bm.ToStrDate(DayDate.SelectedDate) & "' and " & MainId3 & "='" & Shift.SelectedValue & "'")
            btnNew_Click(sender, e)
        End If
    End Sub

    Private Sub btnPrevios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrevios.Click
        bm.NextPrevious(New String() {MainId, MainId2, MainId3, SubId}, New String() {CboMain.SelectedValue.ToString, bm.ToStrDate(DayDate.SelectedDate), Shift.SelectedValue, txtID.Text}, "Back", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub
    Dim lv As Boolean = False
    Private Sub txtID_Leave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.LostFocus
        If lv Then
            Return
        End If
        lv = True

        bm.DefineValues()
        Dim dt As New DataTable
        bm.RetrieveAll(New String() {MainId, MainId2, MainId3, SubId}, New String() {CboMain.SelectedValue.ToString, bm.ToStrDate(DayDate.SelectedDate), Shift.SelectedValue, txtID.Text.Trim}, dt)
        If dt.Rows.Count = 0 Then
            Dim s As String = txtID.Text
            ClearControls()
            txtID.Text = s
            lv = False
            Return
        End If
        FillControls()
        lv = False
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub txtID2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Value.KeyDown
        bm.MyKeyPress(sender, e, True)
    End Sub

    Private Sub CboMain_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CboMain.SelectionChanged
        ClearControls()
    End Sub


    Private Sub btnSetImage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSetImage.Click
        bm.SetImage(Image1)
    End Sub

    Private Sub btnSetNoImage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSetNoImage.Click
        bm.SetNoImage(Image1, False, True)
    End Sub



    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        btnFirst.SetResourceReference(Button.ContentProperty, "First")
        btnNext.SetResourceReference(Button.ContentProperty, "Next")
        btnPrevios.SetResourceReference(Button.ContentProperty, "Previous")
        btnLast.SetResourceReference(Button.ContentProperty, "Last")

        LblId.SetResourceReference(Label.ContentProperty, "Id")
    End Sub


    Private Sub CategoryId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CategoryId.LostFocus
        bm.FillCombo("select Id,Name,dbo.GetCarState(CategoryId,Id)State From Cars where CategoryId=" & Val(CategoryId.SelectedValue) & " union select 0 Id,'-' Name,'#FFFFFFFF' union select -1 Id,'External' Name,'#FFFFFFFF'", CarId)
        CarId_LostFocus(Nothing, Nothing)

        bm.FillCombo("select Id,Name,dbo.GetEmpState(CategoryId,Id)State From Employees where Deliveryman=1 and Stopped=0 and CategoryId=" & Val(CategoryId.SelectedValue) & " union select 0 Id,'-' Name,'#FFFFFFFF'", DeliverymanId)

    End Sub

    Private Sub CarId_LostFocus(sender As Object, e As RoutedEventArgs) Handles CarId.LostFocus
        Try
            Dim x As Integer = Val(bm.ExecuteScalar("select DriverId from Cars where CategoryId=" & CategoryId.SelectedValue.ToString & " and Id=" & CarId.SelectedValue.ToString))
            If x > 0 Then DeliverymanId.SelectedValue = x
        Catch
        End Try
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

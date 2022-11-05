Imports System.Data

Public Class Cars
    Public MainTableName As String = "CallCenterCategories"
    Public MainSubId As String = "Id"
    Public MainSubName As String = "Name"

    Public TableName As String = "Cars"
    Public MainId As String = "CategoryId"
    Public SubId As String = "Id"
    Public SubName As String = "Name"

    Dim dt As New DataTable
    Dim bm As New BasicMethods

    Public Flag As Integer = 0

    Private Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()

        bm.Fields = {MainId, SubId, SubName, "CarGroupId", "CarTypeId", "ColorId", "DriverId", "Ratio"}
        bm.control = {CboMain, txtID, txtName, CarGroupId, CarTypeId, ColorId, DriverId, Ratio}
        bm.KeyFields = {MainId, SubId}

        bm.Table_Name = TableName
        bm.FillCombo(MainTableName, CboMain, "")

        CboMain.SelectedValue = Flag
        CboMain.Visibility = Windows.Visibility.Hidden
        lblMain.Visibility = Windows.Visibility.Hidden

        btnNew_Click(sender, e)

        lblRatio.Visibility = Windows.Visibility.Hidden
        Ratio.Visibility = Windows.Visibility.Hidden

    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        CType(Application.Current.MainWindow, MainWindow).TabControl1.Items.Remove(Me.Parent)
    End Sub

    Private Sub btnLast_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLast.Click
        bm.FirstLast(New String() {MainId, SubId}, "Max", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNext.Click
        bm.NextPrevious(New String() {MainId, SubId}, New String() {CboMain.SelectedValue.ToString, txtID.Text}, "Next", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If txtName.Text.Trim = "" Or CboMain.SelectedValue.ToString = 0 Then
            txtName.Focus()
            Return
        End If
        bm.DefineValues()
        If Not bm.Save(New String() {MainId, SubId}, New String() {CboMain.SelectedValue.ToString, txtID.Text.Trim}) Then Return
        btnNew_Click(sender, e)
    End Sub

    Private Sub btnFirst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFirst.Click

        bm.FirstLast(New String() {MainId, SubId}, "Min", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        ClearControls()
    End Sub

    Sub ClearControls()
        bm.ClearControls()

        CarGroupName.Clear()
        CarTypeName.Clear()
        ColorName.Clear()
        DriverName.Clear()

        txtID.Text = bm.ExecuteScalar("select max(" & SubId & ")+1 from " & TableName & " where " & MainId & "='" & CboMain.SelectedValue.ToString & "'")
        If txtID.Text = "" Then txtID.Text = "1"

        txtName.Focus()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG("MsgDelete") Then
            bm.ExcuteNonQuery("delete from " & TableName & " where " & SubId & "='" & txtID.Text.Trim & "' and " & MainId & " ='" & CboMain.SelectedValue.ToString & "'")
            btnNew_Click(sender, e)
        End If
    End Sub

    Private Sub btnPrevios_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrevios.Click
        bm.NextPrevious(New String() {MainId, SubId}, New String() {CboMain.SelectedValue.ToString, txtID.Text}, "Back", dt)
        If dt.Rows.Count = 0 Then Return
        FillControls()
    End Sub

    Sub FillControls()
        bm.FillControls()
        CarGroupId_LostFocus(Nothing, Nothing)
        CarTypeId_LostFocus(Nothing, Nothing)
        ColorId_LostFocus(Nothing, Nothing)
        DriverId_LostFocus(Nothing, Nothing)
    End Sub

    Dim lv As Boolean = False

    Private Sub txtID_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyUp
        If bm.ShowHelp(CType(Parent, Page).Title, txtID, txtName, e, "select cast(Id as varchar(100)) Id,Name from " & TableName & " where " & MainId & "='" & CboMain.SelectedValue.ToString & "'") Then
            txtID_LostFocus(Nothing, Nothing)
        End If
    End Sub

    Private Sub txtID_LostFocus(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtID.LostFocus
        If lv Then
            Return
        End If
        lv = True

        bm.DefineValues()
        Dim dt As New DataTable
        bm.RetrieveAll(New String() {MainId, SubId}, New String() {CboMain.SelectedValue.ToString, txtID.Text.Trim}, dt)
        If dt.Rows.Count = 0 Then
            Dim s As String = txtID.Text
            ClearControls()
            txtID.Text = s
            txtName.Focus()
            lv = False
            Return
        End If
        FillControls()
        lv = False
        txtName.SelectAll()
        txtName.Focus()
        txtName.SelectAll()
        txtName.Focus()
        'txtName.Text = dt(0)("Name")
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles txtID.KeyDown, CarGroupId.KeyDown, CarTypeId.KeyDown, ColorId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub CboMain_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CboMain.SelectionChanged
        ClearControls()
    End Sub

    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

        btnFirst.SetResourceReference(Button.ContentProperty, "First")
        btnNext.SetResourceReference(Button.ContentProperty, "Next")
        btnPrevios.SetResourceReference(Button.ContentProperty, "Previous")
        btnLast.SetResourceReference(Button.ContentProperty, "Last")

        lblMain.SetResourceReference(Label.ContentProperty, "Group")
        lblId.SetResourceReference(Label.ContentProperty, "Id")
        LblName.SetResourceReference(Label.ContentProperty, "Name")

    End Sub


    Private Sub CarGroupId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CarGroupId.LostFocus
        bm.LostFocus(CarGroupId, CarGroupName, "select Name from CarGroups where Id=" & CarGroupId.Text.Trim())
    End Sub

    Private Sub CarGroupId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CarGroupId.KeyUp
        bm.ShowHelp(lblCarGroupId.Content, CarGroupId, CarGroupName, e, "select cast(Id as varchar(100)) Id,Name from CarGroups", "CarGroups")
    End Sub


    Private Sub CarTypeId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CarTypeId.LostFocus
        bm.LostFocus(CarTypeId, CarTypeName, "select Name from CarTypes where Id=" & CarTypeId.Text.Trim() & " and CarGroupId=" & Val(CarGroupId.Text))
    End Sub

    Private Sub CarTypeId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles CarTypeId.KeyUp
        bm.ShowHelp(lblCarTypeId.Content, CarTypeId, CarTypeName, e, "select cast(Id as varchar(100)) Id,Name from CarTypes where CarGroupId=" & Val(CarGroupId.Text), "CarTypes", {"CarGroupId"}, {Val(CarGroupId.Text)})
    End Sub


    Private Sub ColorId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ColorId.LostFocus
        bm.LostFocus(ColorId, ColorName, "select Name from Colors where Id=" & ColorId.Text.Trim())
    End Sub

    Private Sub ColorId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles ColorId.KeyUp
        bm.ShowHelp(lblColorId.Content, ColorId, ColorName, e, "select cast(Id as varchar(100)) Id,Name from Colors", "Colors")
    End Sub

    Private Sub DriverId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles DriverId.LostFocus
        bm.LostFocus(DriverId, DriverName, "select Name from Employees where Id=" & DriverId.Text.Trim() & " and Deliveryman=1 and Stopped=0")
    End Sub

    Private Sub DriverId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles DriverId.KeyUp
        bm.ShowHelp(lblDriverId.Content, DriverId, DriverName, e, "select cast(Id as varchar(100)) Id,Name from Employees where Deliveryman=1 and Stopped=0")
    End Sub

End Class

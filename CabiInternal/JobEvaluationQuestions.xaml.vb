Imports System.Data

Public Class JobEvaluationQuestions
    Public TableDetailsName As String = "JobEvaluationQuestions"

    Dim dt As New DataTable
    Dim bm As New BasicMethods

    WithEvents G As New MyGrid
    Dim m As MainWindow = Application.Current.MainWindow
    Public IsCommon As Boolean = False

    Public Sub BasicForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        LoadWFH()
        btnNew_Click(sender, e)
        If IsCommon Then
            lblMainJob.Visibility = Windows.Visibility.Hidden
            MainJobId.Visibility = Windows.Visibility.Hidden
            MainJobName.Visibility = Windows.Visibility.Hidden
            FillGrid()
        End If
    End Sub

    Structure GC
        Shared GroupName As String = "GroupName"
        Shared Id As String = "Id"
        Shared Name As String = "Name"
    End Structure

    Private Sub LoadWFH()
        WFH.Child = G

        G.Columns.Clear()
        G.ForeColor = System.Drawing.Color.DarkBlue

        G.Columns.Add(GC.GroupName, "المجموعة")
        G.Columns.Add(GC.Id, "المسلسل")
        G.Columns.Add(GC.Name, "السؤال")

        G.Columns(GC.Name).FillWeight = 500
        G.Columns(GC.Id).ReadOnly = True
        G.AllowUserToDeleteRows = True

        AddHandler G.CellBeginEdit, AddressOf G_CellBeginEdit
        AddHandler G.RowsAdded, AddressOf G_RowsAdded
        AddHandler G.RowsRemoved, AddressOf G_RowsRemoved
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        If Not IsCommon AndAlso MainJobId.Text.Trim = "" Then
            Return
        End If

        G.EndEdit()
        If Not bm.SaveGrid(G, TableDetailsName, New String() {"MainJobId"}, New String() {MainJobId.Text}, New String() {"GroupName", "Id", "Name"}, New String() {GC.GroupName, GC.Id, GC.Name}, New VariantType() {VariantType.String, VariantType.Integer, VariantType.String}, New String() {GC.GroupName, GC.Id, GC.Name}) Then Return

        btnNew_Click(sender, e)
    End Sub


    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        MainJobId.Clear()
        MainJobName.Clear()
        FillGrid()
    End Sub


    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG() Then
            bm.ExcuteNonQuery("delete from " & TableDetailsName & " where MainJobId='" & MainJobId.Text.Trim & "'")
            btnNew_Click(sender, e)
        End If
    End Sub


    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles MainJobId.KeyDown
        bm.MyKeyPress(sender, e)
    End Sub

    Private Sub txtID2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
        bm.MyKeyPress(sender, e, True)
    End Sub


    Private Sub LoadResource()
        btnSave.SetResourceReference(Button.ContentProperty, "Save")
        btnDelete.SetResourceReference(Button.ContentProperty, "Delete")
        btnNew.SetResourceReference(Button.ContentProperty, "New")

    End Sub

    Private Sub FillGrid()
        G.Rows.Clear()
        If Val(MainJobId.Text) = 0 AndAlso Not IsCommon Then Return

        dt = bm.ExcuteAdapter("select * from " & TableDetailsName & " where MainJobId=" & Val(MainJobId.Text))
        For i As Integer = 0 To dt.Rows.Count - 1
            G.Rows.Add({dt.Rows(i)(GC.GroupName), dt.Rows(i)(GC.Id), dt.Rows(i)(GC.Name)})
        Next
        G.Rows(G.Rows.Count - 1).Cells(GC.Id).Value = G.Rows.Count
    End Sub

    Private Sub G_PreviewKeyDown(sender As Object, e As Forms.PreviewKeyDownEventArgs)
        If e.KeyCode = Forms.Keys.Delete AndAlso G.CurrentRow.Index >= 0 AndAlso bm.ShowDeleteMSG() Then
            G.Rows.Remove(G.CurrentRow)
        End If
    End Sub

    Private Sub MainJobId_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles MainJobId.KeyUp
        If bm.ShowHelp("MainJobs", MainJobId, MainJobName, e, "select cast(Id as varchar(100)) Id,Name from MainJobs", "MainJobs") Then
            MainJobId_LostFocus(Nothing, Nothing)
        End If
    End Sub

    Private Sub MainJobId_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MainJobId.LostFocus
        bm.LostFocus(MainJobId, MainJobName, "select Name from MainJobs where Id=" & MainJobId.Text.Trim())
        FillGrid()

    End Sub

    Private Sub G_RowsAdded(sender As Object, e As Forms.DataGridViewRowsAddedEventArgs)
        G.Rows(e.RowIndex).Cells(GC.Id).Value = e.RowIndex + 1
    End Sub

    Private Sub G_RowsRemoved(sender As Object, e As Forms.DataGridViewRowsRemovedEventArgs)
        For i As Integer = e.RowIndex To G.Rows.Count - 1
            G.Rows(i).Cells(GC.Id).Value = i + 1
        Next
    End Sub

    Private Sub G_CellBeginEdit(sender As Object, e As Forms.DataGridViewCellCancelEventArgs)
        Try
            If e.RowIndex > 0 AndAlso (G.CurrentRow.Cells(GC.GroupName).Value Is Nothing OrElse G.CurrentRow.Cells(GC.GroupName).Value = "") Then
                G.CurrentRow.Cells(GC.GroupName).Value = G.Rows(e.RowIndex - 1).Cells(GC.GroupName).Value.ToString
            End If
        Catch ex As Exception
        End Try
    End Sub


End Class

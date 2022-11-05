Imports System.Data

Public Class KMHelp
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Dim dv As New DataView
    Public Header As String = ""
    Public Line As Integer
    Public IsSaved As Boolean = False
    Public Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        Banner1.StopTimer = True
        Banner1.Header = Header

        Try
            dt = bm.ExcuteAdapter("select Id,Name,'' KM from OrderStates order by Id")
            dt.TableName = "tbl"

            DataGridView1.Foreground = System.Windows.Media.Brushes.Black
            dv.Table = dt
            DataGridView1.ItemsSource = dv
            DataGridView1.Columns(1).Width = 300
            DataGridView1.SelectedIndex = 0
            
            DataGridView1.Columns(0).IsReadOnly = True
            DataGridView1.Columns(1).IsReadOnly = True
        Catch
        End Try
        DataGridView1.CanUserAddRows = False
        DataGridView1.CanUserDeleteRows = False
        DataGridView1.CanUserSortColumns = False
        DataGridView1.BeginEdit()

    End Sub

    Private Sub LoadResource()
        'lblName.SetResourceReference(Label.ContentProperty, "Name")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        DataGridView1.CommitEdit()
        Dim str As String = ""
        For i As Integer = 0 To DataGridView1.Items.Count - 1
            str &= " insert CallCenterStateKM(Line,State,KM,UserName,MyGetDate) select " & Line & "," & Val(DataGridView1.Items(i)(0).ToString) & "," & Val(DataGridView1.Items(i)(2).ToString) & "," & Md.UserName & ",GetDate()"
        Next
        If bm.ExcuteNonQuery(str) Then
            IsSaved = True
            Close()
        End If
    End Sub
     
End Class
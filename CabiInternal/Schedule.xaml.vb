Imports System.IO
Imports System.Data.SqlClient
Imports System.Data
Imports System.ComponentModel

Public Class Schedule
    Public Flag As Integer = 1
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Dim dt2 As New DataTable

    Private Sub Schedule_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If bm.TestIsLoaded(Me) Then Return

        LoadAll()
    End Sub

    Private Sub LoadAll()
        dt2 = bm.ExcuteAdapter("select Id,dbo.GetEmpArName(EmpId)EmpName,Notes,MyLine,dbo.ToStrDate(Daydate)Daydate,MyGetDate from Schedule order by MyGetDate desc")
        dt2.TableName = "tbl"

        Dim dv2 As New DataView
        dv2.Table = dt2
        DataGridView2.ItemsSource = dv2
        Try

            DataGridView2.Columns(dt2.Columns("Id").Ordinal).Visibility = Windows.Visibility.Hidden
            DataGridView2.Columns(dt2.Columns("MyLine").Ordinal).Visibility = Windows.Visibility.Hidden
            DataGridView2.Columns(dt2.Columns("Notes").Ordinal).Width = 200
            DataGridView2.Columns(dt2.Columns("EmpName").Ordinal).Header = "المستخدم"
            DataGridView2.Columns(dt2.Columns("Notes").Ordinal).Header = "البيان"
            DataGridView2.Columns(dt2.Columns("Daydate").Ordinal).Header = "اليوم"
            DataGridView2.Columns(dt2.Columns("MyGetDate").Ordinal).Header = "وقت التحديث"

        Catch ex As Exception

        End Try

        If Calendar1.SelectedDate Is Nothing Then Calendar1.SelectedDate = bm.MyGetDate
        Calendar1_SelectedDatesChanged(Nothing, Nothing)
    End Sub

    Private Sub Calendar1_SelectedDatesChanged(sender As Object, e As SelectionChangedEventArgs) Handles Calendar1.SelectedDatesChanged
        dt = bm.ExcuteAdapter("select Id,dbo.GetEmpArName(EmpId)EmpName,Notes,MyLine,MyGetDate from Schedule where DayDate='" & bm.ToStrDate(Calendar1.SelectedDate) & "' order by MyGetDate desc")
        dt.TableName = "tbl"

        Dim dv As New DataView
        dv.Table = dt
        DataGridView1.ItemsSource = dv

        Try

            DataGridView1.Columns(dt.Columns("Id").Ordinal).Visibility = Windows.Visibility.Hidden
            DataGridView1.Columns(dt.Columns("MyLine").Ordinal).Visibility = Windows.Visibility.Hidden

            DataGridView1.Columns(dt.Columns("Notes").Ordinal).Width = 450

            DataGridView1.Columns(dt.Columns("EmpName").Ordinal).Header = "المستخدم"
            DataGridView1.Columns(dt.Columns("Notes").Ordinal).Header = "البيان"
            DataGridView1.Columns(dt.Columns("MyGetDate").Ordinal).Header = "الوقت"
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DataGridView1_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DataGridView1.SelectionChanged
        If DataGridView1.SelectedItem Is Nothing Then
            MyLine = 0
            Notes.Clear()
            Return
        End If
        MyLine = DataGridView1.SelectedItem("MyLine")
        Notes.Text = DataGridView1.SelectedItem("Notes")
    End Sub

    Private Sub DataGridView2_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles DataGridView2.SelectionChanged
        If DataGridView2.SelectedItem Is Nothing Then
            MyLine = 0
            Notes.Clear()
            Return
        End If
        Calendar1.SelectedDate = DateTime.Parse(DataGridView2.SelectedItem("Daydate"))
        MyLine = DataGridView2.SelectedItem("MyLine")
        Notes.Text = DataGridView2.SelectedItem("Notes")
    End Sub

    Dim MyLine As Integer = 0
    Private Sub btnNew_Click(sender As Object, e As RoutedEventArgs) Handles btnNew.Click
        Schedule_Loaded(Nothing, Nothing)
        
        MyLine = 0
        Notes.Clear()
        Notes.Focus()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As RoutedEventArgs) Handles btnDelete.Click
        If bm.ShowDeleteMSG("MsgDelete") Then
            bm.ExcuteNonQuery("delete Schedule where DayDate='" & bm.ToStrDate(Calendar1.SelectedDate) & "' and MyLine=" & MyLine)
            btnNew_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As RoutedEventArgs) Handles btnSave.Click
        If MyLine = 0 Then
            bm.ExcuteNonQuery("insert Schedule(DayDate,Id,EmpId,UserName,MyGetDate,Notes) select '" & bm.ToStrDate(Calendar1.SelectedDate) & "',(select isnull(MAX(Id),0)+1 from Schedule where DayDate='" & bm.ToStrDate(Calendar1.SelectedDate) & "')," & Md.UserName & "," & Md.UserName & ",GetDate(),'" & Notes.Text.Trim.Replace("'", "''") & "'")
        Else
            bm.ExcuteNonQuery("update Schedule set EmpId=" & Md.UserName & ",UserName=" & Md.UserName & ",MyGetDate=GetDate(),Notes='" & Notes.Text.Trim.Replace("'", "''") & "' where DayDate='" & bm.ToStrDate(Calendar1.SelectedDate) & "' and MyLine=" & MyLine)
        End If
        btnNew_Click(Nothing, Nothing)
    End Sub

End Class

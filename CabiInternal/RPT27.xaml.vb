Imports System.Data

Public Class RPT27

    Public MyEndType As Integer = 0
    Public IsIncomeStatement As Integer = 0

    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If Level.SelectedIndex < 1 Then
            bm.ShowMSG("برجاء تحديد المستوى")
            Level.Focus()
            Return
        End If

        Dim rpt As New ReportViewer

        rpt.paraname = New String() {"@EndType", "@Level", "@IsIncomeStatement", "@ToDate", "Header"}
        rpt.paravalue = New String() {MyEndType, Level.SelectedIndex, IsIncomeStatement, ToDate.SelectedDate, CType(Parent, Page).Title}
        Select Case Flag
            Case 1, 2
                rpt.Rpt = "AccountEnd.rpt"
            Case 3
                rpt.Rpt = "AccountEnd2.rpt"
        End Select
        rpt.Show()
    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()
        FillLevel()
        Select Case Flag
            Case 2, 3
                'lblLevel.Visibility = Windows.Visibility.Hidden
                'Level.Visibility = Windows.Visibility.Hidden
        End Select
        Dim MyNow As DateTime = bm.MyGetDate()
        ToDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
    End Sub

    Private Sub LoadResource()
        Button2.SetResourceReference(Button.ContentProperty, "View Report")
        lblToDate.SetResourceReference(Label.ContentProperty, "To Date")
    End Sub

    Private Sub FillLevel()
        Try
            Level.Items.Clear()
            Dim x As Integer = Val(bm.ExecuteScalar("select min(Level) from chart where SubType=1"))
            Dim y As Integer = Val(bm.ExecuteScalar("select max(Level) from chart where SubType=1"))
            Level.Items.Add("-")
            For i As Integer = 1 To x
                Level.Items.Add(New ComboBoxItem With {.Content = i})
            Next
            For i As Integer = x + 1 To y
                Level.Items.Add(New ComboBoxItem With {.Content = i, .Background = System.Windows.Media.Brushes.Red})
            Next
            Level.SelectedIndex = x
        Catch
        End Try
    End Sub

End Class
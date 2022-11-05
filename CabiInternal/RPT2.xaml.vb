Imports System.Data

Public Class RPT2
    Public MyLinkFile As Integer = 0
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Flag As Integer = 0
    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        If MainLinkFile.Visibility = Windows.Visibility.Visible AndAlso MainLinkFile.IsEnabled AndAlso MainLinkFile.SelectedIndex = 0 Then
            bm.ShowMSG("برجاء تحديد " & lblMainLinkFile.Content)
            MainLinkFile.Focus()
            Return
        End If
        If MyLinkFile = 0 AndAlso Val(MainAccNo.Text) = 0 Then
            bm.ShowMSG("برجاء تحديد الحساب العام")
            MainAccNo.Focus()
            Return
        End If
        If MyLinkFile = 0 AndAlso Val(SubAccNo.Text) = 0 AndAlso SubAccNo.IsEnabled Then
            bm.ShowMSG("برجاء تحديد الحساب الفرعى")
            SubAccNo.Focus()
            Return
        End If
        If MyLinkFile > 0 AndAlso Val(SubAccNo.Text) = 0 AndAlso SubAccNo.Visibility = Windows.Visibility.Visible Then
            bm.ShowMSG("برجاء تحديد الكود")
            SubAccNo.Focus()
            Return
        End If

        Dim rpt As New ReportViewer
        Dim RPTFlag1 As Integer = 2
        RPTFlag1 = IIf(MyLinkFile = 1, 3, RPTFlag1)
        RPTFlag1 = IIf(MyLinkFile = 13, 4, RPTFlag1)

        rpt.paraname = New String() {"@MainAccNo", "MainAccName", "@SubAccNo", "SubAccName", "@FromDate", "@ToDate", "Header", "@Detailed", "@DetailedInvoice", "@LinkFile", "@ToId", "@RPTFlag1", "@RPTFlag2"}
        rpt.paravalue = New String() {MainAccNo.Text, MainAccName.Text, Val(SubAccNo.Text), SubAccName.Text, FromDate.SelectedDate, ToDate.SelectedDate, CType(Parent, Page).Title.Trim & " " & IIf(MainLinkFile.SelectedIndex > 0, MainLinkFile.Text, ""), IIf(Detailed.IsChecked, 1, 0), IIf(DetailedInvoice.IsChecked, 1, 0), MyLinkFile, Val(SubAccNo.Text), RPTFlag1, 0}
        Select Case Flag
            Case 1
                rpt.Rpt = "AccountMotion.rpt"
                If Detailed.IsChecked AndAlso (MyLinkFile = 5 OrElse MyLinkFile = 6) Then rpt.Rpt = "AccountMotionBanks.rpt"
                If DetailedInvoice.IsChecked Then
                    rpt.Rpt = "AccountMotion2.rpt"
                 End If
            Case 2
                rpt.Rpt = "AccountMotionBanks2.rpt"
            Case 3
                rpt.Rpt = "Assistant.rpt"
        End Select
        rpt.Show()
    End Sub

    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me, True) Then Return
        LoadResource()
        bm.Addcontrol_MouseDoubleClick({MainAccNo, SubAccNo})
        bm.FillCombo("LinkFile", MainLinkFile, "", , True)

        If Flag = 2 Then
            Detailed.Visibility = Windows.Visibility.Hidden
        ElseIf Flag = 3 Then
            Detailed.Visibility = Windows.Visibility.Hidden
            lblSubAcc.Visibility = Windows.Visibility.Collapsed
            SubAccNo.Visibility = Windows.Visibility.Collapsed
            SubAccName.Visibility = Windows.Visibility.Collapsed
        End If

        If MyLinkFile = -1 Then
            MyLinkFile = 0
            MainLinkFile.SelectedIndex = 0
            MainLinkFile.Visibility = Windows.Visibility.Hidden
            lblMainLinkFile.Visibility = Windows.Visibility.Hidden
        ElseIf MyLinkFile = 0 Then
            lblMainAcc.Visibility = Windows.Visibility.Hidden
            MainAccNo.Visibility = Windows.Visibility.Hidden
            MainAccName.Visibility = Windows.Visibility.Hidden
        End If

        If Flag = 2 Then
            MainLinkFile.Visibility = Windows.Visibility.Hidden
            lblMainLinkFile.Visibility = Windows.Visibility.Hidden
        End If

        If MyLinkFile > 0 And Flag <> 3 Then
            lblMainAcc.Visibility = Windows.Visibility.Collapsed
            MainAccNo.Visibility = Windows.Visibility.Collapsed
            MainAccName.Visibility = Windows.Visibility.Collapsed
        End If
        If Flag = 3 Then
            lblMainAcc.Visibility = Windows.Visibility.Visible
            MainAccNo.Visibility = Windows.Visibility.Visible
            MainAccName.Visibility = Windows.Visibility.Visible
        End If
        
        If Flag <> 1 Then
            DetailedInvoice.Visibility = Windows.Visibility.Hidden
        End If

        Dim MyNow As DateTime = bm.MyGetDate()
        FromDate.SelectedDate = New DateTime(MyNow.Year, 1, 1, 0, 0, 0)
        ToDate.SelectedDate = New DateTime(MyNow.Year, MyNow.Month, MyNow.Day, 0, 0, 0)
    End Sub

    Dim lop As Boolean = False
    Private Sub SubAccNo_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles SubAccNo.LostFocus
        If lop Then Return
        If MyLinkFile = 0 Then
            If Val(MainAccNo.Text) = 0 Or Not SubAccNo.IsEnabled Then
                SubAccNo.Clear()
                SubAccName.Clear()
                Return
            End If
            dt = bm.ExcuteAdapter("select * from LinkFile where Id=(select C.LinkFile from Chart C where C.Id='" & MainAccNo.Text & "')")
            bm.LostFocus(SubAccNo, SubAccName, "select Name from " & dt.Rows(0)("TableName") & " where Id=" & SubAccNo.Text.Trim() & " and AccNo='" & MainAccNo.Text & "'")
        Else
            If Val(SubAccNo.Text) = 0 Then
                SubAccNo.Clear()
                SubAccName.Clear()
                Return
            End If
            dt = bm.ExcuteAdapter("select * from LinkFile where Id=" & MyLinkFile)
            bm.LostFocus(SubAccNo, SubAccName, "select Name from Fn_EmpPermissions(" & MyLinkFile & "," & Md.UserName & ") where Id=" & SubAccNo.Text.Trim(), , True)
            If MyLinkFile > 0 Then
                bm.LostFocus(SubAccNo, MainAccNo, "select AccNo from " & dt.Rows(0)("TableName") & " where Id=" & SubAccNo.Text.Trim())
                lop = True
                MainAccNo_LostFocus(Nothing, Nothing)
                lop = False
            End If

        End If
    End Sub
    Private Sub SubAccNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles SubAccNo.KeyUp
        If MyLinkFile = 0 Then
            dt = bm.ExcuteAdapter("select * from LinkFile where Id=(select C.LinkFile from Chart C where C.Id='" & MainAccNo.Text & "')")
            If dt.Rows.Count > 0 AndAlso bm.ShowHelp(dt.Rows(0)("TableName"), SubAccNo, SubAccName, e, "select cast(Id as varchar(100)) Id,Name from Fn_EmpPermissions(" & dt.Rows(0)("Id") & "," & Md.UserName & ") where AccNo='" & MainAccNo.Text & "'") Then
                SubAccNo_LostFocus(Nothing, Nothing)
            End If
        Else
            dt = bm.ExcuteAdapter("select * from LinkFile where Id=" & MyLinkFile)
            If dt.Rows.Count > 0 AndAlso bm.ShowHelp(dt.Rows(0)("TableName"), SubAccNo, SubAccName, e, "select cast(Id as varchar(100)) Id,Name from Fn_EmpPermissions(" & MyLinkFile & "," & Md.UserName & ")") Then
                SubAccNo_LostFocus(Nothing, Nothing)
            End If
        End If
    End Sub


    Private Sub MainAccNo_LostFocus(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MainAccNo.LostFocus
        bm.AccNoLostFocus(MainAccNo, MainAccName, , MyLinkFile, )

        SubAccNo.IsEnabled = MainAccNo.Visibility <> Windows.Visibility.Visible OrElse MyLinkFile > 0 OrElse bm.ExcuteAdapter("select * from LinkFile where Id=(select C.LinkFile from Chart C where C.Id='" & MainAccNo.Text & "')").Rows.Count > 0
        SubAccNo_LostFocus(Nothing, Nothing)
    End Sub

    Private Sub MainAccNo_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles MainAccNo.KeyUp
        bm.AccNoShowHelp(MainAccNo, MainAccName, e, , MyLinkFile, )
    End Sub


    Private Sub LoadResource()
        Button2.SetResourceReference(Button.ContentProperty, "View Report")
        lblFromDate.SetResourceReference(Label.ContentProperty, "From Date")
        lblToDate.SetResourceReference(Label.ContentProperty, "To Date")
        lblMainAcc.SetResourceReference(Label.ContentProperty, "Main AccNo")
        lblSubAcc.SetResourceReference(Label.ContentProperty, "Sub AccNo")
        Detailed.SetResourceReference(CheckBox.ContentProperty, "Detailed")
        DetailedInvoice.SetResourceReference(CheckBox.ContentProperty, "Detailed With Invoice")
    End Sub

    Private Sub Detailed_Checked(sender As Object, e As RoutedEventArgs) Handles DetailedInvoice.Checked, Detailed.Checked
        If sender Is DetailedInvoice And Detailed.IsChecked = True Then Detailed.IsChecked = False
        If sender Is Detailed And DetailedInvoice.IsChecked = True Then DetailedInvoice.IsChecked = False
    End Sub

    Private Sub MainLinkFile_LostFocus(sender As Object, e As RoutedEventArgs) Handles MainLinkFile.LostFocus
        MyLinkFile = MainLinkFile.SelectedValue
        MainAccNo_LostFocus(Nothing, Nothing)
        SubAccNo_LostFocus(Nothing, Nothing)
    End Sub

End Class
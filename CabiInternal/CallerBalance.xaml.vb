Imports System
Imports System.Threading
Public Class CallerBalance
    Dim bm As New BasicMethods
    Public Ok As Boolean
    Public CurrentCallerId As String, CurrentCallerName As String

    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        LoadResource()
        Ok = False
        CallerId.Content = CurrentCallerId
        CallerName.Content = CurrentCallerName
        OldBal.Content = Val(bm.ExecuteScalar("select top 1 CurrentBal from CallerBalance where CallerId='" & CurrentCallerId & "' order by Line Desc"))
        AddBal_TextChanged(Nothing, Nothing)
    End Sub

    Private Sub btnNo_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnNo.Click
        Ok = False
        Close()
    End Sub

    Private Sub btnYes_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnYes.Click
        If Not bm.ExcuteNonQuery("insert CallerBalance(CallerId,CallerName,OldBal,AddBal,SubstractBal,CurrentBal,Notes,UserName,MyGetDate)select '" & CallerId.Content & "','" & CallerName.Content & "','" & Val(OldBal.Content) & "','" & Val(AddBal.Text) & "','" & Val(SubstractBal.Text) & "','" & Val(CurrentBal.Content) & "','" & Notes.Text.Replace("'", "''") & "','" & Md.UserName & "',GetDate()") Then Return
        Ok = True
        Close()
    End Sub

    Private Sub LoadResource()
        btnYes.SetResourceReference(Button.ContentProperty, "Yes")
        btnNo.SetResourceReference(Button.ContentProperty, "No")

    End Sub

    Private Sub AddBal_TextChanged(sender As Object, e As TextChangedEventArgs) Handles AddBal.TextChanged, SubstractBal.TextChanged
        CurrentBal.Content = Val(OldBal.Content) + Val(AddBal.Text) - Val(SubstractBal.Text)
    End Sub

    Private Sub txtID_KeyPress2(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles OldBal.KeyDown, SubstractBal.KeyDown
        bm.MyKeyPress(sender, e, True)
    End Sub
End Class

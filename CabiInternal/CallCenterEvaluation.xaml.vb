Imports System.Data
Imports Microsoft.Office.Interop
Imports System.IO
Imports System.Threading.Tasks

Public Class CallCenterEvaluation
    Dim bm As New BasicMethods
    Dim dt As New DataTable
    Public Hdr As String = ""
    Public CurrentLine As Integer = 0
    Public IsEvaluated As Boolean = False
    Private Sub UserControl_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        If bm.TestIsLoaded(Me) Then Return
        bm.Addcontrol_MouseDoubleClick({})
        LoadResource()

        MyList.DataContext = bm.ExcuteAdapter("select * from CustomerServiceQuestions")
        Notes.Text = bm.ExecuteScalar("select Notes from CallCenterEvaluationMaster where Line=" & CurrentLine & "")
    End Sub

    Private Sub LoadResource()
        btnSave.SetResourceReference(System.Windows.Controls.Button.ContentProperty, "Save")
    End Sub

    Private Sub txtID_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
        bm.MyKeyPress(sender, e)
    End Sub

    Public Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSave.Click
        Dim str As String = "update CallCenter set UserNameEvaluated=" & Md.UserName & ",IsEvaluated=1,EvaluatedTime=GETDATE() where Line=" & CurrentLine
        If NotAnswered.IsChecked Then str = ""
        str &= "   delete CallCenterEvaluationMaster where Line=" & CurrentLine & "   insert CallCenterEvaluationMaster(Line, Notes, UserName, MyGetDate) select " & CurrentLine & ",'" & Notes.Text.Replace("'", "''") & "'," & Md.UserName & ",GetDate()   "
        If Not NotAnswered.IsChecked Then
            For i As Integer = 0 To MyList.Items.Count - 1
                Dim x As Integer = GetCheckedRdo(i)
                If x = 0 Then
                    bm.ShowMSG("برجاء تحديد اختيار بالسؤال رقم " & (i + 1))
                    Return
                End If
                str &= "  insert CallCenterEvaluationDetails(Line, RdoId, RdoValue, UserName, MyGetDate) select " & CurrentLine & "," & GetRdoTag(i) & "," & x & "," & Md.UserName & ",GetDate()"
            Next
        End If
        If bm.ExcuteNonQuery(str) Then
            IsEvaluated = True
            CType(Parent, Window).Close()
        End If
    End Sub

    Private Function GetRdoTag(i As Integer) As Integer
        Dim row As ListBoxItem = CType(MyList.ItemContainerGenerator.ContainerFromIndex(i), ListBoxItem)
        Dim myContentPresenter As ContentPresenter = FindVisualChild(Of ContentPresenter)(row)
        Dim template As DataTemplate = myContentPresenter.ContentTemplate
        Return CType(template.FindName("rdo5", myContentPresenter), RadioButton).Tag
    End Function

    Private Function GetCheckedRdo(i As Integer) As Integer
        Dim row As ListBoxItem = CType(MyList.ItemContainerGenerator.ContainerFromIndex(i), ListBoxItem)
        Dim myContentPresenter As ContentPresenter = FindVisualChild(Of ContentPresenter)(row)
        Dim template As DataTemplate = myContentPresenter.ContentTemplate
        If CType(template.FindName("rdo1", myContentPresenter), RadioButton).IsChecked Then
            Return 1
        ElseIf CType(template.FindName("rdo2", myContentPresenter), RadioButton).IsChecked Then
            Return 2
        ElseIf CType(template.FindName("rdo3", myContentPresenter), RadioButton).IsChecked Then
            Return 3
        ElseIf CType(template.FindName("rdo4", myContentPresenter), RadioButton).IsChecked Then
            Return 4
        ElseIf CType(template.FindName("rdo5", myContentPresenter), RadioButton).IsChecked Then
            Return 5
        Else
            Return 0
        End If
    End Function

  Private Function FindVisualChild(Of childItem As DependencyObject)(obj As DependencyObject) As childItem
        For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
            Dim child As DependencyObject = VisualTreeHelper.GetChild(obj, i)
            If child IsNot Nothing AndAlso TypeOf child Is childItem Then
                Return DirectCast(child, childItem)
            Else
                Dim childOfChild As childItem = FindVisualChild(Of childItem)(child)
                If childOfChild IsNot Nothing Then
                    Return childOfChild
                End If
            End If
        Next
        Return Nothing
    End Function


End Class
using MyLib;

public class UpdateQuestionDialog : CreateQuestionDialog
{
    public UpdateQuestionDialog()
    {
        this.Title = "Edit question";
    }

    public void SetQuestion (Question a)
    {
        this.questionTextInput.Text = a.questionText;
    }

}
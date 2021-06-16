using MyLib;

public class UpdateAnswerDialog : CreateAnswerDialog
{
    public UpdateAnswerDialog()
    {
        this.Title = "Edit answer";
    }

    public void SetAnswer (Answer a)
    {
        this.answerTextInput.Text = a.answerText;
        this.qId.Text = a.questionId.ToString();
    }

}
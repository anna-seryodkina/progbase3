using ConsoleProject;

public class UpdateQuestionDialog : CreateQuestionDialog
{
    public UpdateQuestionDialog()
    {
        this.Title = "Edit question";
    }

    public void SetQuestion (Question a)
    {
        // this.activityTypeRadioGr.Text = a.type;
        // this.activityNameInput.Text = a.name;
        // this.commentTextView.Text = a.comment;
        // this.distanceInput.Text = a.distance.ToString();
    }

}
using MyLib;

public class UpdateAnswerDialog : CreateAnswerDialog
{
    public UpdateAnswerDialog()
    {
        this.Title = "Edit answer";
    }

    public void SetAnswer (Answer a)
    {
        // this.activityTypeRadioGr.Text = a.type;
        // this.activityNameInput.Text = a.name;
        // this.commentTextView.Text = a.comment;
        // this.distanceInput.Text = a.distance.ToString();
    }

}
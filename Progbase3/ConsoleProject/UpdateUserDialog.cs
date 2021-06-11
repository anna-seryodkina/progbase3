using MyLib;

public class UpdateUserDialog : CreateUserDialog
{
    public UpdateUserDialog()
    {
        this.Title = "Edit user";
    }

    public void SetUser (User a)
    {
        // this.activityTypeRadioGr.Text = a.type;
        // this.activityNameInput.Text = a.name;
        // this.commentTextView.Text = a.comment;
        // this.distanceInput.Text = a.distance.ToString();
    }

}
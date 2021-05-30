using Terminal.Gui;
using System;
using ConsoleProject;

public class CreateAnswerDialog : Dialog
{
    public bool canceledA;

    // protected RadioGroup activityTypeRadioGr;
    // protected TextField activityNameInput;
    // protected TextView commentTextView;
    // protected TextField distanceInput;


    public CreateAnswerDialog()
    {
        this.Title = "create new activity";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnQuestionCancelled;

        Button okBtn = new Button("Ok")
        {
            X = Pos.Right(cancelBtn) + 2,
            Y = Pos.Top(cancelBtn),
        };
        okBtn.Clicked += OnQuestionSubmit;

        this.AddButton(cancelBtn);
        this.AddButton(okBtn);


        int rightColumnX = 20;

        // Label activityTypeLbl = new Label(2, 2, "Type:");
        // activityTypeRadioGr = new RadioGroup()
        // {
        //     X = rightColumnX, Y = Pos.Top(activityTypeLbl),
        //     RadioLabels = new NStack.ustring[]{"walking", "running", "cycling", "swimming", "other"},
        // };
        // this.Add(activityTypeLbl, activityTypeRadioGr);



        // Label activityNameLbl = new Label(2, 8, "Name:");
        // activityNameInput = new TextField("")
        // {
        //     X = rightColumnX, Y = Pos.Top(activityNameLbl), Width = 40,
        // };
        // this.Add(activityNameLbl, activityNameInput);


        // Label distanceLbl = new Label(2, 10, "Distance:");
        // distanceInput = new TextField("")
        // {
        //     X = rightColumnX, Y = Pos.Top(distanceLbl), Width = 40,
        // };
        // this.Add(distanceLbl, distanceInput);


        // Label commentLbl = new Label(2, 12, "Comment:");
        // commentTextView = new TextView()
        // {
        //     X = rightColumnX, Y = Pos.Top(commentLbl),
        //     Width = 40,
        //     Height = 7,
        //     Text =  "",
        // };
        // this.Add(commentLbl, commentTextView);

    }

    public Answer GetAnswer()
    {
        return new Answer()
        {
            // type = Program.activities[activityTypeRadioGr.SelectedItem],
            // name = activityNameInput.Text.ToString(),
            // comment = commentTextView.Text.ToString(),
            // distance = Convert.ToInt32(distanceInput.Text.ToString()),
        };
    }

    private void OnQuestionCancelled()
    {
        this.canceledA = true;
        Application.RequestStop();
    }

    private void OnQuestionSubmit()
    {
        this.canceledA = false;
        Application.RequestStop();
        // OpenActivityDialog dialog = new OpenActivityDialog();
        // dialog.SetActivity(GetActivity());
        // Application.Run(dialog);
    }
}
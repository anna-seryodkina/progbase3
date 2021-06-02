using Terminal.Gui;
using System;
using ConsoleProject;

public class CreateAnswerDialog : Dialog
{
    public bool canceledA;

    protected TextField answerTextInput;


    public CreateAnswerDialog()
    {
        this.Title = "create new activity";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnAnswerCancelled;

        Button okBtn = new Button("Ok")
        {
            X = Pos.Right(cancelBtn) + 2,
            Y = Pos.Top(cancelBtn),
        };
        okBtn.Clicked += OnAnswerSubmit;

        this.AddButton(cancelBtn);
        this.AddButton(okBtn);


        int rightColumnX = 20;

        Label answerTextLbl = new Label(2, 2, "Answer:");
        answerTextInput = new TextField("")
        {
            X = rightColumnX, Y = Pos.Top(answerTextLbl), Width = 40,
        };
        this.Add(answerTextLbl, answerTextInput);

    }

    public Answer GetAnswer()
    {
        return new Answer()
        {
            answerText = answerTextInput.Text.ToString(),
        };
    }

    private void OnAnswerCancelled()
    {
        this.canceledA = true;
        Application.RequestStop();
    }

    private void OnAnswerSubmit()
    {
        this.canceledA = false;
        Application.RequestStop();
        // OpenActivityDialog dialog = new OpenActivityDialog();
        // dialog.SetActivity(GetActivity());
        // Application.Run(dialog);
    }
}
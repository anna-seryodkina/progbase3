using Terminal.Gui;
using System;
using MyLib;

public class CreateQuestionDialog : Dialog
{
    public bool canceledQ;

    protected TextView questionTextInput;


    public CreateQuestionDialog()
    {
        this.Title = "create new question";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCancelled;

        Button okBtn = new Button("Ok")
        {
            X = Pos.Right(cancelBtn) + 2,
            Y = Pos.Top(cancelBtn),
        };
        okBtn.Clicked += OnSubmit;

        this.AddButton(cancelBtn);
        this.AddButton(okBtn);


        int rightColumnX = 20;

        Label questionTextLbl = new Label(2, 2, "Question:");
        questionTextInput = new TextView()
        {
            X = rightColumnX, Y = Pos.Top(questionTextLbl), Width = 40, Height = 15,
        };
        this.Add(questionTextLbl, questionTextInput);
    }

    public Question GetQuestion()
    {
        return new Question()
        {
            questionText = questionTextInput.Text.ToString(),
        };
    }

    private void OnCancelled()
    {
        this.canceledQ = true;
        Application.RequestStop();
    }

    private void OnSubmit()
    {
        this.canceledQ = false;
        Application.RequestStop();
    }
}
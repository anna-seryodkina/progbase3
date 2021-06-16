using Terminal.Gui;
using System;
using MyLib;

public class CreateAnswerDialog : Dialog
{
    public bool canceledA;

    protected TextView answerTextInput;
    protected TextField qId;


    public CreateAnswerDialog()
    {
        this.Title = "create new answer";
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

        Label label = new Label(2, 2, "Question id:");
        qId = new TextField()
        {
            X = rightColumnX, Y = Pos.Top(label), Width = 40,
        };
        this.Add(label, qId);

        Label answerTextLbl = new Label(2, 4, "Answer:");
        answerTextInput = new TextView()
        {
            X = rightColumnX, Y = Pos.Top(answerTextLbl), Width = 40,
            Height = 15,
        };
        this.Add(answerTextLbl, answerTextInput);

    }

    public Answer GetAnswer()
    {
        Answer answer = new Answer();
        try
        {
            answer.answerText = answerTextInput.Text.ToString();
            answer.questionId = int.Parse(qId.Text.ToString());
        }
        catch
        {
            MessageBox.ErrorQuery("oops", "incorrect data.", "OK");
        }
        return answer;
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
    }
}
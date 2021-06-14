using Terminal.Gui;
using System;
using MyLib;

public class OpenAnswerDialog : Dialog
{
    public bool deletedA;
    public bool updatedA;
    protected TextView answerTextInput;

    protected Answer answer;


    public OpenAnswerDialog()
    {
        this.Title = "open answer";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        Button updateBtn = new Button(2, 15, "Update");
        updateBtn.Clicked += OnUpdateAnswer;

        Button deleteBtn = new Button("Delete")
        {
            X = Pos.Right(updateBtn) + 2,
            Y = Pos.Top(updateBtn),
        };
        deleteBtn.Clicked += OnDeleteAnswer;
        this.Add(updateBtn, deleteBtn);

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnBackA;
        this.AddButton(backBtn);


        int rightColumnX = 20;

        Label answerTextLbl = new Label(2, 2, "Answer:");
        answerTextInput = new TextView()
        {
            X = rightColumnX, Y = Pos.Top(answerTextLbl), Width = 40,
            Height = 15, ReadOnly = true,
        };
        this.Add(answerTextLbl, answerTextInput);
    }

    public void SetAnswer(Answer answer)
    {
        this.answer = answer;
        this.answerTextInput.Text = answer.answerText;
    }

    public Answer GetAnswer()
    {
        return this.answer;
    }

    private void OnBackA() // should return to main window !
    {
        Application.RequestStop();
    }

    private void OnUpdateAnswer()
    {
        UpdateAnswerDialog dialog = new UpdateAnswerDialog();
        dialog.SetAnswer(this.answer);

        Application.Run(dialog);
        if(!dialog.canceledA)
        {
            Answer updatedAnswer = dialog.GetAnswer();
            this.updatedA = true;
            this.SetAnswer(updatedAnswer);
            Application.RequestStop();
        }
    }

    private void OnDeleteAnswer()
    {
        int index = MessageBox.Query("Delete question", "Are you sure?", "No", "Yes");
        if(index == 1)
        {
            this.deletedA = true;
            Application.RequestStop();
        }
    }

}
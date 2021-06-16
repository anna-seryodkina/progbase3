using Terminal.Gui;
using System;
using MyLib;

public class OpenAnswerDialog : Dialog
{
    public bool deletedA;
    public bool updatedA;
    protected TextView answerTextInput;
    private User currentUser;

    protected Answer answer;


    public OpenAnswerDialog(User currentUser)
    {
        this.Title = "open answer";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        this.currentUser = currentUser;

        Button updateBtn = new Button(2, 16, "Update");
        updateBtn.Clicked += OnUpdateAnswer;

        Button deleteBtn = new Button("Delete")
        {
            X = 2,
            Y = Pos.Top(updateBtn) + 2,
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

    private void OnBackA()
    {
        Application.RequestStop();
    }

    private void OnUpdateAnswer()
    {
        if(currentUser.id != answer.authorId)
        {
            MessageBox.ErrorQuery("oops", "access denied.", "OK");
            return;
        }
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
        if(!currentUser.isModerator && currentUser.id != answer.authorId)
        {
            MessageBox.ErrorQuery("oops", "access denied.", "OK");
            return;
        }
        int index = MessageBox.Query("Delete question", "Are you sure?", "No", "Yes");
        if(index == 1)
        {
            this.deletedA = true;
            Application.RequestStop();
        }
    }

}
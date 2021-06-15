using Terminal.Gui;
using System;
using MyLib;

public class OpenQuestionDialog : Dialog
{
    public bool deletedQ;
    public bool updatedQ;
    protected TextView questionTextInput;
    private User currentUser;

    protected Question question;


    public OpenQuestionDialog(User currentUser)
    {
        this.Title = "open question";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        this.currentUser = currentUser;

        if(!currentUser.isModerator && currentUser.id != question.authorId)
        {
            MessageBox.ErrorQuery("oops", "access denied.", "OK");
            return;
        }

        Button updateBtn = new Button(2, 16, "Update");
        updateBtn.Clicked += OnUpdateQuestion;

        Button deleteBtn = new Button("Delete")
        {
            X = 2,
            Y = Pos.Top(updateBtn) + 2,
        };
        deleteBtn.Clicked += OnDeleteQuestion;
        this.Add(updateBtn, deleteBtn);

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnBackQ;
        this.AddButton(backBtn);


        int rightColumnX = 20;

        Label questionTextLbl = new Label(2, 2, "Question:");
        questionTextInput = new TextView()
        {
            X = rightColumnX, Y = Pos.Top(questionTextLbl), Width = 40, Height = 15,
            ReadOnly = true,
        };
        this.Add(questionTextLbl, questionTextInput);

    }

    public void SetQuestion(Question question)
    {
        this.question = question;
        this.questionTextInput.Text = question.questionText;
    }

    public Question GetQuestion()
    {
        return this.question;
    }

    private void OnBackQ() // should return to main window !
    {
        Application.RequestStop();
    }

    private void OnUpdateQuestion()
    {
        UpdateQuestionDialog dialog = new UpdateQuestionDialog();
        dialog.SetQuestion(this.question);

        Application.Run(dialog);
        if(!dialog.canceledQ)
        {
            Question updatedQuestion = dialog.GetQuestion();
            this.updatedQ = true;
            this.SetQuestion(updatedQuestion);
            Application.RequestStop();
        }
    }

    private void OnDeleteQuestion()
    {
        int index = MessageBox.Query("Delete question", "Are you sure?", "No", "Yes");
        if(index == 1)
        {
            this.deletedQ = true;
            Application.RequestStop();
        }
    }

}
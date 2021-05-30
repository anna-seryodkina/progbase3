using Terminal.Gui;
using System;
using ConsoleProject;

public class CreateQuestionDialog : Dialog
{
    public bool canceledQ;

    // protected TextField loginInput;
    // protected TextField fullnameInput;
    // protected CheckBox isModeratorCheckBox;


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

        // Label loginLabel = new Label(2, 2, "Login:");
        // loginInput = new TextField("")
        // {
        //     X = rightColumnX, Y = Pos.Top(loginLabel), Width = 40,
        // };
        // this.Add(loginLabel, loginInput);


        // Label fullnameLabel = new Label(2, 4, "Fullname:");
        // fullnameInput = new TextField("")
        // {
        //     X = rightColumnX, Y = Pos.Top(fullnameLabel), Width = 40,
        // };
        // this.Add(fullnameLabel, fullnameInput);


        // Label isModeratorLabel = new Label(2, 6, "Is moderator:");
        // isModeratorCheckBox = new CheckBox("")
        // {
        //     X = rightColumnX, Y = Pos.Top(isModeratorLabel), Width = 40,
        // };
        // this.Add(isModeratorLabel, isModeratorCheckBox);

    }

    public Question GetQuestion()
    {
        return new Question()
        {
            // login = loginInput.Text.ToString(),
            // fullname = fullnameInput.Text.ToString(),
            // isModerator = isModeratorCheckBox.Checked,
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
        // OpenActivityDialog dialog = new OpenActivityDialog();
        // dialog.SetActivity(GetQuestion());
        // Application.Run(dialog);
    }
}
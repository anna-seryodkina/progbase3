using Terminal.Gui;
using System;
using MyLib;

public class CreateUserDialog : Dialog
{
    public bool canceledU;

    protected TextField loginInput;
    protected TextField fullnameInput;
    protected CheckBox isModeratorCheckBox;


    public CreateUserDialog()
    {
        this.Title = "create new user";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        Button cancelBtn = new Button("Cancel");
        cancelBtn.Clicked += OnCreateDialogCancelled;

        Button okBtn = new Button("Ok")
        {
            X = Pos.Right(cancelBtn) + 2,
            Y = Pos.Top(cancelBtn),
        };
        okBtn.Clicked += OnCreateUserSubmit;

        this.AddButton(cancelBtn);
        this.AddButton(okBtn);


        int rightColumnX = 20;

        Label loginLabel = new Label(2, 2, "Login:");
        loginInput = new TextField("")
        {
            X = rightColumnX, Y = Pos.Top(loginLabel), Width = 40,
        };
        this.Add(loginLabel, loginInput);


        Label fullnameLabel = new Label(2, 4, "Fullname:");
        fullnameInput = new TextField("")
        {
            X = rightColumnX, Y = Pos.Top(fullnameLabel), Width = 40,
        };
        this.Add(fullnameLabel, fullnameInput);


        Label isModeratorLabel = new Label(2, 6, "Is moderator:");
        isModeratorCheckBox = new CheckBox("")
        {
            X = rightColumnX, Y = Pos.Top(isModeratorLabel), Width = 40,
        };
        this.Add(isModeratorLabel, isModeratorCheckBox);

    }

    public User GetUser()
    {
        return new User()
        {
            login = loginInput.Text.ToString(),
            fullname = fullnameInput.Text.ToString(),
            isModerator = isModeratorCheckBox.Checked,
        };
    }

    private void OnCreateDialogCancelled()
    {
        this.canceledU = true;
        Application.RequestStop();
    }

    private void OnCreateUserSubmit()
    {
        this.canceledU = false;
        Application.RequestStop();
        // OpenActivityDialog dialog = new OpenActivityDialog();
        // dialog.SetActivity(GetUser());
        // Application.Run(dialog);
    }
}
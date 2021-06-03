using Terminal.Gui;
using System;
using ConsoleProject;

public class OpenUserDialog : Dialog
{
    public bool deleted;
    public bool updated;
    protected TextField loginInput;
    protected TextField fullnameInput;
    // protected CheckBox isModeratorCheckBox;
    protected TextField isModeratorTextField;

    protected User user;


    public OpenUserDialog()
    {
        this.Title = "open user";
        this.Width = Dim.Percent(80);
        this.Height = Dim.Percent(80);

        Button updateBtn = new Button(2, 15, "Update");
        updateBtn.Clicked += OnUpdate;

        Button deleteBtn = new Button("Delete")
        {
            X = Pos.Right(updateBtn) + 2,
            Y = Pos.Top(updateBtn),
        };
        deleteBtn.Clicked += OnDeleteUser;
        this.Add(updateBtn, deleteBtn);

        Button backBtn = new Button("Back");
        backBtn.Clicked += OnBack;
        this.AddButton(backBtn);


        int rightColumnX = 20;

        Label loginLabel = new Label(2, 2, "Login:");
        loginInput = new TextField("")
        {
            X = rightColumnX, Y = Pos.Top(loginLabel), Width = 40,
            ReadOnly = true,
        };
        this.Add(loginLabel, loginInput);


        Label fullnameLabel = new Label(2, 4, "Fullname:");
        fullnameInput = new TextField("")
        {
            X = rightColumnX, Y = Pos.Top(fullnameLabel), Width = 40,
            ReadOnly = true,
        };
        this.Add(fullnameLabel, fullnameInput);


        Label isModeratorLabel = new Label(2, 6, "Is moderator:");
        // isModeratorCheckBox = new CheckBox("")
        // {
        //     X = rightColumnX, Y = Pos.Top(isModeratorLabel), Width = 40,
        // };
        // this.Add(isModeratorLabel, isModeratorCheckBox);

        isModeratorTextField = new TextField("")
        {
            X = rightColumnX, Y = Pos.Top(fullnameLabel), Width = 40,
            ReadOnly = true,
        };
        this.Add(isModeratorLabel, isModeratorTextField);

    }

    public void SetUser(User user)
    {
        this.user = user;
        this.loginInput.Text = user.login;
        this.fullnameInput.Text = user.fullname;
        // this.isModeratorCheckBox.Checked = user.isModerator;
        this.isModeratorTextField.Text = user.isModerator.ToString();
    }

    public User GetUser()
    {
        return this.user;
    }

    private void OnBack() // should return to main window !
    {
        Application.RequestStop();
    }

    private void OnUpdate()
    {
        UpdateUserDialog dialog = new UpdateUserDialog();
        dialog.SetUser(this.user);

        Application.Run(dialog);
        if(!dialog.canceledU)
        {
            User updatedUser = dialog.GetUser();
            this.updated = true;
            this.SetUser(updatedUser);
            Application.RequestStop();
        }
    }

    private void OnDeleteUser()
    {
        int index = MessageBox.Query("Delete user", "Are you sure?", "No", "Yes");
        if(index == 1)
        {
            this.deleted = true;
            Application.RequestStop();
        }
    }

}
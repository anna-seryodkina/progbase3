using Terminal.Gui;

namespace ConsoleProject
{
    public class LogInWindow : Window
    {
        protected TextField loginInput;
        protected TextField passwordInput;
        protected TextField fullnameInput;
        public bool canceled;
        public LogInWindow()
        {
            this.X = 0;
            this.Y = 0;
            this.Width = Dim.Fill();
            this.Height = Dim.Fill() - 1;

            int rightColumnX = 20;

            Label loginLabel = new Label(2, 6, "Login:");
            loginInput = new TextField("")
            {
                X = rightColumnX, Y = Pos.Top(loginLabel), Width = 40,
            };
            this.Add(loginLabel, loginInput);

            Label passwordLabel = new Label(2, 8, "Password:");
            passwordInput = new TextField("")
            {
                X = rightColumnX, Y = Pos.Top(passwordLabel), Width = 40,
                Secret = true,
            };
            this.Add(passwordLabel, passwordInput);

            Button cancelBtn = new Button("Cancel")
            {
                X = 2, Y = 12,
            };
            cancelBtn.Clicked += OnCancel;

            Button okBtn = new Button("Ok")
            {
                X = Pos.Right(cancelBtn) + 2,
                Y = Pos.Top(cancelBtn),
            };
            okBtn.Clicked += OnOk;

            this.Add(cancelBtn);
            this.Add(okBtn);
        }

        public void OnCancel()
        {
            this.canceled = true;
            Application.RequestStop();
        }

        public void OnOk()
        {
            this.canceled = false;
            Application.RequestStop();
        }

        public string[] GetLoginInfo()
        {
            return new string[]
            {
                loginInput.Text.ToString(),
                passwordInput.Text.ToString(),
            };
        }
    }
}
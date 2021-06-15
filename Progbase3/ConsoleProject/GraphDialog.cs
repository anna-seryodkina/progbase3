using Terminal.Gui;

namespace ConsoleProject
{
    public class GraphDialog : Dialog
    {
        private Label fromLabel;
        public DateField fromDateField;
        private Label toLabel;
        public DateField toDateField;
        public bool canceled;

        public GraphDialog()
        {
            this.Title = "graph";

            fromLabel = new Label(2, 2, "From: ");

            fromDateField = new DateField()
            {
                X = Pos.Right(fromLabel) + 2,
                Y = Pos.Top(fromLabel),
                Date = System.DateTime.Now,
                IsShortFormat = false,
            };
            this.Add(fromLabel, fromDateField);

            toLabel = new Label(2, 4, "To: ");

            toDateField = new DateField()
            {
                X = Pos.Right(toLabel) + 2,
                Y = Pos.Top(toLabel),
                Date = System.DateTime.Now,
                IsShortFormat = false,
            };
            this.Add(toLabel, toDateField);


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
        }
        private void OnCancelled()
        {
            this.canceled = true;
            Application.RequestStop();
        }

        private void OnSubmit()
        {
            this.canceled = false;
            Application.RequestStop();
        }

    }
}
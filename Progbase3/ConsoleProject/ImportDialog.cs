using Terminal.Gui;

namespace ConsoleProject
{
    public class ImportDialog : OpenDialog
    {
        public ImportDialog()
        {
            this.Title = "Open import directory";
            this.Message ="Open?";
            this.CanChooseDirectories = true;
            this.CanChooseFiles = false;
        }
    }
}
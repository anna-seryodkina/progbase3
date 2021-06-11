using Terminal.Gui;

namespace ConsoleProject
{
    public class ExportDialog : OpenDialog
    {
        public ExportDialog()
        {
            this.Title = "Open export directory";
            this.Message ="Open?";
            this.CanChooseDirectories = true;
            this.CanChooseFiles = false;
        }
    }
}
using System;
using Microsoft.Data.Sqlite;
using Terminal.Gui;
using MyLib;

namespace ConsoleProject
{
    class Program
    {
        static SqliteConnection connection;
        static UserRepository userRepository;
        static AnswerRepository answerRepository;
        static QuestionRepository questionRepository;
        static void Main(string[] args)
        {
            string databaseFileName = "../../data/db.db";
            connection = new SqliteConnection($"Data Source={databaseFileName}");

            userRepository = new UserRepository(connection);
            answerRepository = new AnswerRepository(connection);
            questionRepository = new QuestionRepository(connection);

            //
            // ----------------- FOR TESTS -----------------
            // string dir = "./../../data/zip";
            // User user = userRepository.GetExportData(1);
            // Im_Ex_port ex = new Im_Ex_port();
            // ex.Export(user, dir);
            // ---------------------------------------------
            // DateTime from = DateTime.Parse("2010/09/01");
            // DateTime to = DateTime.Parse("2011/05/25");
            // userRepository.GetGraph(2, from, to);


            Application.Init();

            Toplevel top = Application.Top; 

            // MainWindow win = new MainWindow();
            // win.SetRepository(userRepository);

            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem("_Import...", "", OnImport),
                    new MenuItem ("_Export...", "", OnExport),
                    new MenuItem ("_Exit", "", OnQuit)
                }),
                new MenuBarItem ("_Help", new MenuItem [] {
                    new MenuItem("_About", "", OnAbout)
                }),
            });

            top.Add(menu);

            Application.Run();
        }

        static void OnExport()
        {
            ExportDialog dialog = new ExportDialog();
            Application.Run(dialog);
            string dir = "";
            if(!dialog.Canceled)
            {
                dir = dialog.FilePath.ToString();
            }
            long userId = 0; // get from GUI (logined user's id)
            User user = userRepository.GetExportData(userId);
            Im_Ex_port exporter = new Im_Ex_port(connection);
            exporter.Export(user, dir);
        }

        static void OnImport()
        {
            ImportDialog dialog = new ImportDialog();
            Application.Run(dialog);

            string path = "";
            if(!dialog.Canceled)
            {
                path = dialog.FilePath.ToString();
            }

            Im_Ex_port importer = new Im_Ex_port(connection);
            importer.Import(path);
        }

        static void OnAbout()
        {
            string information = "hi :)\nthis program allows you to manage your data\n(\\__/)\n( . .)\n/ >< \\\n   ";

            int index = MessageBox.Query("info", information, "Ok");
            if(index == 1)
            {
                Application.RequestStop();
            }
        }

        static void OnQuit()
        {
            Application.RequestStop();
        }
    }
}

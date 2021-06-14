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
        static TextField currentUserTextField;
        static Button answerListButton;
        static Button questionListButton;
        static User currentUser;
        static void Main(string[] args)
        {
            string databaseFileName = "../../data/db.db";
            connection = new SqliteConnection($"Data Source={databaseFileName}");

            userRepository = new UserRepository(connection);
            answerRepository = new AnswerRepository(connection);
            questionRepository = new QuestionRepository(connection);

            Application.Init();

            Toplevel top = Application.Top; 

            MenuBar menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem("_Import...", "", OnImport),
                    new MenuItem ("_Export...", "", OnExport),
                    new MenuItem ("_Graph...", "", OnGraph),
                    new MenuItem ("_Exit", "", OnQuit)
                }),
                new MenuBarItem ("_Help", new MenuItem [] {
                    new MenuItem("_About", "", OnAbout)
                }),
            });

            Window win = new Window()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1,
            };

            Button registrationButton = new Button("Sign In")
            {
                X = 2,
                Y = 4,
            };
            registrationButton.Clicked += OnRegistration;
            win.Add(registrationButton);


            Button loginButton = new Button("Log In")
            {
                X = Pos.Right(registrationButton) + 2,
                Y = Pos.Top(registrationButton),
            };
            loginButton.Clicked += OnLogin;
            win.Add(loginButton);

            Label currentUserLabel = new Label("Current user: ")
            {
                X = Pos.Right(registrationButton),
                Y = Pos.Top(registrationButton) + 4,
            };
            win.Add(currentUserLabel);

            currentUserTextField = new TextField("")
            {
                X = Pos.Right(currentUserLabel) + 2,
                Y = Pos.Top(currentUserLabel),
                Width = 15,
                Height = 30,
                ReadOnly = true,
            };
            if(currentUser == null)
            {
                currentUserTextField.Text = "no one";
            }
            else
            {
                currentUserTextField.Text = currentUser.fullname;
            }
            win.Add(currentUserTextField);

            answerListButton = new Button("Answers")
            {
                X = Pos.Percent(50),
                Y = Pos.Bottom(win) - 8,
                Visible = false,
            };
            answerListButton.Clicked += OnAnswerListClicked;
            win.Add(answerListButton);

            questionListButton = new Button("Questions")
            {
                X = Pos.X(answerListButton),
                Y = Pos.Y(answerListButton) + 2,
                Visible = false,
            };
            questionListButton.Clicked += OnQuestionListClicked;
            win.Add(questionListButton);


            top.Add(menu, win);
            Application.Run();
        }

        static void OnQuestionListClicked()
        {
            OpenQuestionListDialog dialog = new OpenQuestionListDialog();
            dialog.SetRepository(questionRepository);
            Application.Run(dialog);
        }

        static void OnAnswerListClicked()
        {
            OpenAnswerListDialog dialog = new OpenAnswerListDialog();
            dialog.SetRepository(answerRepository);
            Application.Run(dialog);
        }

        static void OnRegistration()
        {
            RegistrationWindow window = new RegistrationWindow();
            Application.Run(window);
            if(window.canceled)
            {
                return;
            }
            string[] info = window.GetRegInfo();
            string login = info[0];
            string password = info[1];
            string fullname = info[2];

            Authentication authentication = new Authentication(connection);
            bool result = authentication.Registration(login, password, fullname);
            if(result)
            {
                // added new user
            }
            else
            {
                // user already exists
            }
        }

        static void OnLogin()
        {
            LogInWindow window = new LogInWindow();
            Application.Run(window);
            if(window.canceled)
            {
                return;
            }
            string[] info = window.GetLoginInfo();
            string login = info[0];
            string password = info[1]; 

            Authentication authentication = new Authentication(connection);
            User user = authentication.Login(login, password);
            if(user == null)
            {
                // no such user
            }
            else
            {
                currentUser = user;
                currentUserTextField.Text = currentUser.fullname;
                answerListButton.Visible = true;
                questionListButton.Visible = true;
            }

            // if (currentUser.id == ...authorId) // користувач є автором питання/відповіді
            // { може створювати видаляти редагувати те що належить йому }
            // if (currentUser.isModerator) { може видаляти будь які питання та відповіді }
        }

        static void OnGraph()
        {
            // dialog
            long userId = currentUser.id;
            DateTime from = new DateTime(); // get from dialog
            DateTime to = new DateTime(); // get from dialog
            userRepository.GetGraph(userId, from, to);
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
            long userId = currentUser.id;
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

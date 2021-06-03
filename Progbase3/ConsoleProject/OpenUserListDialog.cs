using System.Collections.Generic;
using Terminal.Gui;

namespace ConsoleProject
{
    public class OpenUserListDialog : Dialog
    {
        private ListView allUsersListView;

        private UserRepository userRepo;
        private Label totalPagesLbl;
        private Label pageLbl;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label emptyLabel;

        private int pageSize = 5;
        private int pageNum = 1;
        public OpenUserListDialog()
        {
            this.Title = "Users list";

            prevPageBtn = new Button(2, 4, "Prev");
            prevPageBtn.Clicked += OnPrevPageButton;

            pageLbl = new Label("?")
            {
                X = Pos.Right(prevPageBtn) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            totalPagesLbl = new Label("?")
            {
                X = Pos.Right(pageLbl) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            nextPageBtn = new Button("Next")
            {
                X = Pos.Right(totalPagesLbl) + 2,
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextPageButton;
            this.Add(prevPageBtn, pageLbl, totalPagesLbl, nextPageBtn);


            Rect frame = new Rect(4, 8, 40, 200);
            allUsersListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allUsersListView.OpenSelectedItem += OnOpenActivity;
            FrameView frameView = new FrameView("Users")
            {
                X = 2,
                Y = 6,
                Width = Dim.Fill() - 4,
                Height = pageSize + 2,
            };
            frameView.Add(allUsersListView);
            this.Add(frameView);

            Button createNewUserBtn = new Button(2, 16, "create new user");
            createNewUserBtn.Clicked += OnCreateButtonClicked;
            this.Add(createNewUserBtn);

            emptyLabel = new Label("Database is empty")
            {
                X = 4, Y = 14, Visible = false,
            };
            this.Add(emptyLabel);
        }

        public void SetRepository(UserRepository repo)
        {
            this.userRepo = repo;
            this.UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            int totalPages = (int)userRepo.GetTotalPages(pageSize);
            if(pageNum > totalPages && pageNum > 1)
            {
                pageNum = totalPages;
            }
            this.pageLbl.Text = pageNum.ToString();

            if (totalPages == 0)
            {
                emptyLabel.Visible = true;
            }
            else
            {
                emptyLabel.Visible = false;
            }
            
            if (totalPages == 0)
            {
                totalPages = 1;
            }
            this.totalPagesLbl.Text = totalPages.ToString();

            this.allUsersListView.SetSource(userRepo.GetPage(this.pageNum, this.pageSize));

            prevPageBtn.Visible = (pageNum != 1);
            nextPageBtn.Visible = (pageNum != totalPages );
        }

        private void OnOpenActivity(ListViewItemEventArgs args)
        {
            User value = (User)args.Value;
            OpenUserDialog dialog = new OpenUserDialog();
            dialog.SetUser(value);

            Application.Run(dialog);

            if(dialog.deleted)
            {
                bool result = userRepo.DeleteUser(value.id);
                if(result)
                {
                    int pages = (int)userRepo.GetTotalPages(pageSize);
                    if(pageNum > pages && pageNum > 1)
                    {
                        pageNum -= 1;
                    }
                    this.UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete user", "Can not delete user", "Ok");
                }
            }

            if(dialog.updated)
            {
                bool result = userRepo.Update(value.id, dialog.GetUser());
                if(result)
                {
                    allUsersListView.SetSource(userRepo.GetPage(pageNum, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Update user", "Can not update user", "Ok");
                }
            }
        }

        public void OnCreateButtonClicked()
        {
            CreateUserDialog dialog = new CreateUserDialog();
            Application.Run(dialog);

            if(!dialog.canceledU)
            {
                User user = dialog.GetUser();
                long userId = userRepo.InsertUser(user);
                user.id = userId;

                UpdateCurrentPage();
            }
        }

        private void OnNextPageButton()
        {
            int pages = (int)userRepo.GetTotalPages(pageSize);
            if(pageNum >= pages)
            {
                return;
            }
            this.pageNum += 1;
            this.UpdateCurrentPage();
        }

        private void OnPrevPageButton()
        {
            if(pageNum <= 1)
            {
                return;
            }
            this.pageNum -= 1;
            this.UpdateCurrentPage();
        }
    }
}
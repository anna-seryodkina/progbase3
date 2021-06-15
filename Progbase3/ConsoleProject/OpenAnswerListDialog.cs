using System.Collections.Generic;
using Terminal.Gui;
using MyLib;

namespace ConsoleProject
{
    public class OpenAnswerListDialog : Dialog
    {
        private ListView allAnswersListView;
        private AnswerRepository answerRepo;
        private Label totalPagesLbl;
        private Label pageLbl;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label emptyLabel;
        private TextField searchInput;
        private Button searchButton;
        private string forSearch;
        private Button closeButton;
        private bool onSearchMode;

        private int pageSize = 5;
        private int pageNum = 1;
        public OpenAnswerListDialog()
        {
            this.Title = "Answer list";

            closeButton = new Button("Close");
            closeButton.Clicked += OnCloseClicked;
            this.AddButton(closeButton);

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
            allAnswersListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allAnswersListView.OpenSelectedItem += OnOpenAnswer;
            FrameView frameView = new FrameView("Questions")
            {
                X = 2,
                Y = 6,
                Width = Dim.Fill() - 4,
                Height = pageSize + 2,
            };
            frameView.Add(allAnswersListView);
            this.Add(frameView);

            emptyLabel = new Label("Database is empty") 
            {
                X = 4, Y = 14, Visible = false,
            };
            this.Add(emptyLabel);

            Button createNewUserBtn = new Button(2, 16, "create new answer");
            createNewUserBtn.Clicked += OnCreateButtonClicked;
            this.Add(createNewUserBtn);

            searchInput = new TextField("")
            {
                X = 2, Y = 18, Width = 40,
            };

            searchButton = new Button("Search")
            {
                X = Pos.Right(searchInput) + 2,
                Y = Pos.Top(searchInput),
            };
            searchButton.Clicked += OnSearch;
            this.Add(searchInput, searchButton);
        }

        private void OnCloseClicked()
        {
            Application.RequestStop();
        }

        private void OnSearch()
        {
            forSearch = searchInput.Text.ToString();
            if(forSearch == "")
            {
                onSearchMode = false;
                this.UpdateCurrentPage();
                return;
            }

            this.onSearchMode = true;

            this.allAnswersListView.SetSource(answerRepo.GetPageSearch(forSearch, pageNum, pageSize));
        }

        public void SetRepository(AnswerRepository repo)
        {
            this.answerRepo = repo;
            this.UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            int totalPages = 0;
            if(onSearchMode)
            {
                totalPages = (int)answerRepo.GetTotalPagesSearch(forSearch, pageSize);
            }
            else
            {
                totalPages = (int)answerRepo.GetTotalPages(pageSize);
            }
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

            if(onSearchMode)
            {
                this.allAnswersListView.SetSource(answerRepo.GetPageSearch(forSearch, pageNum, pageSize));
            }
            else
            {
                this.allAnswersListView.SetSource(answerRepo.GetPage(this.pageNum, this.pageSize));
            }

            prevPageBtn.Visible = (pageNum != 1);
            nextPageBtn.Visible = (pageNum != totalPages );
        }

        private void OnOpenAnswer(ListViewItemEventArgs args)
        {
            Answer value = (Answer)args.Value;
            OpenAnswerDialog dialog = new OpenAnswerDialog();
            dialog.SetAnswer(value);

            Application.Run(dialog);

            if(dialog.deletedA)
            {
                bool result = answerRepo.Delete(value.id);
                if(result)
                {
                    int pages = (int)answerRepo.GetTotalPages(pageSize);
                    if(pageNum > pages && pageNum > 1)
                    {
                        pageNum -= 1;
                    }
                    this.UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete answer", "Can not delete answer", "Ok");
                }
            }

            if(dialog.updatedA)
            {
                bool result = answerRepo.Update(value.id, dialog.GetAnswer());
                if(result)
                {
                    allAnswersListView.SetSource(answerRepo.GetPage(pageNum, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Update answer", "Can not update answer", "Ok");
                }
            }
        }

        public void OnCreateButtonClicked()
        {
            CreateAnswerDialog dialog = new CreateAnswerDialog();
            Application.Run(dialog);

            if(!dialog.canceledA)
            {
                Answer answer = dialog.GetAnswer();
                long questionId = answerRepo.Insert(answer);
                answer.id = questionId;

                UpdateCurrentPage();
            }
        }

        private void OnNextPageButton()
        {
            int totalPages = 0;
            if(onSearchMode)
            {
                totalPages = (int)answerRepo.GetTotalPagesSearch(forSearch, pageSize);
            }
            else
            {
                totalPages = (int)answerRepo.GetTotalPages(pageSize);
            }
            if(pageNum >= totalPages)
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
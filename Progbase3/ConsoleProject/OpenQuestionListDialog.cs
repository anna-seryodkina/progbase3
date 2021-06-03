using System.Collections.Generic;
using Terminal.Gui;

namespace ConsoleProject
{
    public class OpenQuestionListDialog : Dialog
    {
        private ListView allQuestionsListView;

        private QuestionRepository questionRepo;
        private Label totalPagesLbl;
        private Label pageLbl;
        private Button prevPageBtn;
        private Button nextPageBtn;
        private Label emptyLabel;

        private int pageSize = 5;
        private int pageNum = 1;
        public OpenQuestionListDialog()
        {
            this.Title = "Question list";

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
            allQuestionsListView = new ListView(new List<User>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allQuestionsListView.OpenSelectedItem += OnOpenQuestion;
            FrameView frameView = new FrameView("Questions")
            {
                X = 2,
                Y = 6,
                Width = Dim.Fill() - 4,
                Height = pageSize + 2,
            };
            frameView.Add(allQuestionsListView);
            this.Add(frameView);

            Button createNewUserBtn = new Button(2, 16, "create new question");
            createNewUserBtn.Clicked += OnCreateButtonClicked;
            this.Add(createNewUserBtn);

            emptyLabel = new Label("Database is empty")
            {
                X = 4, Y = 14, Visible = false,
            };
            this.Add(emptyLabel);
        }

        public void SetRepository(QuestionRepository repo)
        {
            this.questionRepo = repo;
            this.UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            int totalPages = (int)questionRepo.GetTotalPages(pageSize);
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

            this.allQuestionsListView.SetSource(questionRepo.GetPage(this.pageNum, this.pageSize));

            prevPageBtn.Visible = (pageNum != 1);
            nextPageBtn.Visible = (pageNum != totalPages );
        }

        private void OnOpenQuestion(ListViewItemEventArgs args)
        {
            Question value = (Question)args.Value;
            OpenQuestionDialog dialog = new OpenQuestionDialog();
            dialog.SetQuestion(value);

            Application.Run(dialog);

            if(dialog.deletedQ)
            {
                bool result = questionRepo.Delete(value.id);
                if(result)
                {
                    int pages = (int)questionRepo.GetTotalPages(pageSize);
                    if(pageNum > pages && pageNum > 1)
                    {
                        pageNum -= 1;
                    }
                    this.UpdateCurrentPage();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete question", "Can not delete question", "Ok");
                }
            }

            if(dialog.updatedQ)
            {
                bool result = questionRepo.Update(value.id, dialog.GetQuestion());
                if(result)
                {
                    allQuestionsListView.SetSource(questionRepo.GetPage(pageNum, pageSize));
                }
                else
                {
                    MessageBox.ErrorQuery("Update question", "Can not update question", "Ok");
                }
            }
        }

        public void OnCreateButtonClicked()
        {
            CreateQuestionDialog dialog = new CreateQuestionDialog();
            Application.Run(dialog);

            if(!dialog.canceledQ)
            {
                Question question = dialog.GetQuestion();
                long questionId = questionRepo.Insert(question);
                question.id = questionId;

                UpdateCurrentPage();
            }
        }

        private void OnNextPageButton()
        {
            int pages = (int)questionRepo.GetTotalPages(pageSize);
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
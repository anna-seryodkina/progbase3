using System;
using Microsoft.Data.Sqlite;

namespace ConsoleProject
{
    class Program
    {
        static UserRepository userRepository;
        static AnswerRepository answerRepository;
        static QuestionRepository questionRepository;

        static void Main(string[] args)
        {
            string databaseFileName = "db.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");

            userRepository = new UserRepository(connection);
            answerRepository = new AnswerRepository(connection);
            questionRepository = new QuestionRepository(connection);

            //
            Console.WriteLine("> generate data? [yes|no]");
            string input = Console.ReadLine();
            if(input == "yes")
            {
                GenerateData();
            }
            else if(input == "no")
            {
                return;
            }
            else
            {
                Console.WriteLine(">> incorrect input.");
                return;
            }
        }

        private static void GenerateData()
        {
            Console.WriteLine("> enter type: [user|answer|question]");
            string typeInput = Console.ReadLine();

            switch(typeInput)
            {
                case "user":
                    AddUsersToDB();
                    break;
                case "answer":
                    AddAnswersToDB();
                    break;
                case "question":
                    AddQuestionsToDB();
                    break;
                default:
                    Console.WriteLine(">> incorrect input.");
                    break;
            }
        }

        private static void AddUsersToDB()
        {
            User[] users = GenerateUsersData();
            foreach(User user in users)
            {
                long userId = userRepository.InsertUser(user);
            }
        }

        public static int GetN()
        {
            Console.WriteLine("> enter number of entities:");
            string input = Console.ReadLine();
            int n;
            if(!int.TryParse(input, out n))
            {
                Console.WriteLine(">> incorrect input.");
                return -1;
            }
            return n;
        }

        private static User[] GenerateUsersData()
        {
            int n = GetN();

            User[] users = new User[n];

            // get time interval

            Random random = new Random();

            for(int i = 0; i < n; i++)
            {
                User u = new User();
                // set login
                // set fullname
                u.isModerator = random.Next(0, 2);
                // set createdAt
                users[i] = u;
            }
            return users;
        }

        private static void AddAnswersToDB()
        {
            Answer[] answers = GenerateAnswersData();
            foreach(Answer answer in answers)
            {
                long userId = answerRepository.Insert(answer);
            }
        }

        private static Answer[] GenerateAnswersData()
        {
            int n = GetN();

            Answer[] answers = new Answer[n];

            // get time interval

            for(int i = 0; i < n; i++)
            {
                string answerText = GetRandomAnswerText();
                Answer answer = new Answer(answerText);
                // set DateTime createdAt
                answers[i] = answer;
            }
            return answers;
        }

        private static string GetRandomAnswerText()
        {
            string a = "";
            // get answer from file
            return a;
        }

        private static void AddQuestionsToDB()
        {
            Question[] questions = GenerateQuestionsData();
            foreach(Question question in questions)
            {
                long userId = questionRepository.Insert(question);
            }
        }

        private static Question[] GenerateQuestionsData()
        {
            int n = GetN();

            Question[] questions = new Question[n];

            // get time interval

            for(int i = 0; i < n; i++)
            {
                string qText = GetRandomQuestionText();
                Question question = new Question(qText);
                // set DateTime createdAt
                questions[i] = question;
            }
            return questions;
        }

        private static string GetRandomQuestionText()
        {
            string q = "";
            // get question from file
            return q;
        }
    }
}

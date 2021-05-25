using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace ConsoleProject
{
    class Program
    {
        static UserRepository userRepository;
        static AnswerRepository answerRepository;
        static QuestionRepository questionRepository;
        static List<string> logins;
        static List<string> fullnames;
        static List<string> answers;
        static List<string> questions;


        static void Main(string[] args)
        {
            string databaseFileName = "../../data/db.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");

            userRepository = new UserRepository(connection);
            answerRepository = new AnswerRepository(connection);
            questionRepository = new QuestionRepository(connection);
            //
            answers = new List<string>();
            questions = new List<string>();
            logins = new List<string>();
            fullnames = new List<string>();
            GetAnswers("../../data/generator/answers");
            GetQuestions("../../data/generator/questions");
            GetFullnames("../../data/generator/fullnames");
            GetLogins("../../data/generator/logins");
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
            List<User> users = GenerateUsersData();
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

        private static List<User> GenerateUsersData()
        {
            int n = GetN();

            List<User> users = new List<User>();

            // get time interval

            Random random = new Random();

            for(int i = 0; i < n; i++)
            {
                User u = new User();
                int index = random.Next(0, logins.Count);
                u.login = logins[index];
                index = random.Next(0, fullnames.Count);
                u.fullname = fullnames[index];
                u.isModerator = random.Next(0, 2);
                // set createdAt
                users.Add(u);
            }
            return users;
        }

        private static void AddAnswersToDB()
        {
            List<Answer> answers = GenerateAnswersData();
            foreach(Answer answer in answers)
            {
                long userId = answerRepository.Insert(answer);
            }
        }

        private static List<Answer> GenerateAnswersData()
        {
            int n = GetN();

            List<Answer> answers = new List<Answer>();

            // get time interval

            for(int i = 0; i < n; i++)
            {
                string answerText = GetRandomAnswerText();
                Answer answer = new Answer(answerText);
                // set DateTime createdAt
                answers.Add(answer);
            }
            return answers;
        }

        private static string GetRandomAnswerText()
        {
            Random random = new Random();
            int index = random.Next(0, answers.Count);
            return answers[index];
        }

        private static void GetAnswers(string path)
        {
            StreamReader sr = new StreamReader(path);
            string s = "";
            while (true)
            {
                s = sr.ReadLine();
                if (s == null)
                {
                    break;
                }
                answers.Add(s);
            }
            sr.Close();
        }

        private static void GetQuestions(string path)
        {
            StreamReader sr = new StreamReader(path);
            string s = "";
            while (true)
            {
                s = sr.ReadLine();
                if (s == null)
                {
                    break;
                }
                questions.Add(s);
            }
            sr.Close();
        }

        private static void GetLogins(string path)
        {
            StreamReader sr = new StreamReader(path);
            string s = "";
            while (true)
            {
                s = sr.ReadLine();
                if (s == null)
                {
                    break;
                }
                logins.Add(s);
            }
            sr.Close();
        }

        private static void GetFullnames(string path)
        {
            StreamReader sr = new StreamReader(path);
            string s = "";
            while (true)
            {
                s = sr.ReadLine();
                if (s == null)
                {
                    break;
                }
                fullnames.Add(s);
            }
            sr.Close();
        }

        private static void AddQuestionsToDB()
        {
            List<Question> questions = GenerateQuestionsData();
            foreach(Question question in questions)
            {
                long userId = questionRepository.Insert(question);
            }
        }

        private static List<Question> GenerateQuestionsData()
        {
            int n = GetN();

            List<Question> questions = new List<Question>();

            // get time interval

            for(int i = 0; i < n; i++)
            {
                string qText = GetRandomQuestionText();
                Question question = new Question(qText);
                // set DateTime createdAt
                questions.Add(question);
            }
            return questions;
        }

        private static string GetRandomQuestionText()
        {
            Random random = new Random();
            int index = random.Next(0, questions.Count);
            return questions[index];
        }
    }
}

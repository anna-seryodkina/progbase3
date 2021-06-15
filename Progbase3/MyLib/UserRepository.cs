using System;
using System.Collections.Generic;
using static System.Globalization.CultureInfo;
using Microsoft.Data.Sqlite;
using ScottPlot;

namespace MyLib
{
    public class UserRepository
    {
        private SqliteConnection connection;
        public UserRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        private HashSet<DateTime> GetMonths (DateTime fromDate, DateTime toDate)
        {
            HashSet<DateTime> set = new HashSet<DateTime>();

            while(set.Count != 10)
            {
                DateTime randomDate = GenerateDate(fromDate, toDate);
                int daysInMonth = DateTime.DaysInMonth(randomDate.Year, randomDate.Month);
                int c = 0;
                for(int j = 1; j <= daysInMonth; j++)
                {
                    DateTime dt = new DateTime(randomDate.Year, randomDate.Month, j);
                    if(set.Contains(dt))
                    {
                        c++;
                    }
                }
                if(c > 1)
                {
                    continue;
                }
                set.Add(randomDate);
            }

            return set;
        }

        public static DateTime GenerateDate(DateTime startDate, DateTime endDate)
        {
            Random random = new Random();
            TimeSpan dateInterval = endDate - startDate;
            int hours = random.Next(0, 24);
            int minutes = random.Next(0, 60);
            int seconds = random.Next(0, 60);
            int value = random.Next(dateInterval.Days);
            DateTime datetime = startDate.AddDays(value).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            return datetime;
        }

        public void GetGraph(long userId, DateTime fromDate, DateTime toDate)
        {
            string[] labels = new string[10];
            double[] values = new double[10];
            int index = 0;

            HashSet<DateTime> rrrrrr = GetMonths(fromDate, toDate);
            foreach (DateTime date in rrrrrr)
            {
                values[index] = CountValue(date, userId);
                labels[index] = CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month) + ".";
                labels[index] += date.Year.ToString();
                // positions[index] = index+1;

                index++;
            }

            ScottPlot.Plot plt = new ScottPlot.Plot(600, 400);

            var bar = plt.AddBar(values);
            bar.Orientation = Orientation.Vertical;
            plt.Grid(lineStyle: LineStyle.Dot);
            plt.XTicks(labels);
            plt.SetAxisLimits(yMin: 0);

            string path = "./F.png";
            plt.SaveFig(path); 
        }

        public double CountValue (DateTime date, long userId)
        {
            QuestionRepository qRepo = new QuestionRepository(connection);
            AnswerRepository aRepo = new AnswerRepository(connection);

            double result = qRepo.CountQuestions(userId, date) + aRepo.CountAnswers(userId, date);
            return result;
        }

        public long InsertUser(User user)
        {
            connection.Open();
 
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO users (login, fullname, isModerator, createdAt)
                VALUES ($login, $fullname, $isModerator, $createdAt);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$login", user.login);
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$isModerator", user.isModerator);
            command.Parameters.AddWithValue("$createdAt", user.createdAt.ToString("o"));

            long newId = (long)command.ExecuteScalar();

            connection.Close();
            return newId;
        }

        public bool SetPassword(string password, long userId)
        {
            connection.Open();
 
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE users SET password = $password WHERE id = $id
            ";
            command.Parameters.AddWithValue("$password", password);
            command.Parameters.AddWithValue("$id", userId);

            int nChanged = command.ExecuteNonQuery();

            connection.Close();

            return nChanged == 1;
        }

        public bool UserExists(string login)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE login = $login";
            command.Parameters.AddWithValue("$login", login);

            SqliteDataReader reader = command.ExecuteReader();

            bool result = reader.Read();
            this.connection.Close();

            return result;
        }

        public User GetUserById(long userId)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", userId);

            SqliteDataReader reader = command.ExecuteReader();

            if(reader.Read())
            {
                User user = new User();
                user.id = long.Parse(reader.GetString(0));
                user.login = reader.GetString(1);
                user.fullname = reader.GetString(2);
                user.isModerator = reader.GetString(3) != "0";
                user.createdAt = DateTime.Parse(reader.GetString(4));

                this.connection.Close();
                return user;
            }
            else
            {
                this.connection.Close();
                return null;
            }
        }

        public User GetUserByLoginPassword(string login, string password)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT * FROM users
                WHERE login = $login AND password = $password;
            ";
            command.Parameters.AddWithValue("$login", login);
            command.Parameters.AddWithValue("$password", password);

            SqliteDataReader reader = command.ExecuteReader();

            if(reader.Read())
            {
                User user = new User();
                user.id = long.Parse(reader.GetString(0));
                user.login = reader.GetString(1);
                user.fullname = reader.GetString(2);
                user.isModerator = reader.GetString(3) != "0";
                user.createdAt = DateTime.Parse(reader.GetString(4));

                this.connection.Close();
                return user;
            }
            else
            {
                this.connection.Close();
                return null;
            }
        }

        public User GetExportData(long userId)
        {
            QuestionRepository qRepo = new QuestionRepository(connection);
            AnswerRepository aRepo = new AnswerRepository(connection);

            User user = GetUserById(userId);
            user.questions = qRepo.GetAllQuestionsByUserId(userId);
            foreach(Question question in user.questions)
            {
                question.answers = aRepo.GetAllAnswersByQuestionId(question.id);
            }
            user.answers = aRepo.GetAllAnswersByUserId(userId);
            return user;
        }

        public List<int> GetIdList()
        {
            List<int> list = new List<int>();

            this.connection.Open();
 
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT id FROM users";

            SqliteDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                list.Add(int.Parse(reader.GetString(0)));
            }

            this.connection.Close();

            return list;
        }
    }
}

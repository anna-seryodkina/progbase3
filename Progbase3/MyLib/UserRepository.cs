using System;
using System.Collections.Generic;
using static System.Globalization.CultureInfo;
using Microsoft.Data.Sqlite;
using ScottPlot;

namespace ConsoleProject
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
            var rand = new Random();
            var dateInterval = endDate - startDate;
            var hours = rand.Next(0, 24);
            var minutes = rand.Next(0, 60);
            var seconds = rand.Next(0, 60);
            var value = rand.Next(dateInterval.Days);
            var dt = startDate.AddDays(value).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            return dt;
        }

        public void GetGraph(long userId, DateTime fromDate, DateTime toDate) // userId or User user ?
        {
            // int totalMonths = toDate.Month - fromDate.Month;

            // QuestionRepository qRepo = new QuestionRepository(connection);
            // List<DateTime> dateTimes = qRepo.GetQuestionDatetimes(userId, fromDate, toDate);

            // double[] positions = new double[10];
            string[] labels = new string[10]; // names of the months
            double[] values = new double[10];
            int index = 0;

            // for(int i = fromDate.Month; i < toDate.Month; i++)
            // {
            //     int h = 0; // num of questions in 1 month
            //     foreach(DateTime dateTime in dateTimes)
            //     {
            //         if(dateTime.Month == i)
            //         {
            //             h++;
            //         }
            //     }
            //     values[index] = h;
            //     positions[index] = index;
            //     index++;
            // }


            HashSet<DateTime> rrrrrr = GetMonths(fromDate, toDate);
            foreach (DateTime date in rrrrrr)
            {
                values[index] = CountValue(date, userId);
                labels[index] = CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(date.Month) + " | ";
                labels[index] += date.Year.ToString();
                // positions[index] = index+1;

                index++;
            }

            ScottPlot.Plot plt = new ScottPlot.Plot(600, 400);
            //
            var bar = plt.AddBar(values);
            bar.Orientation = Orientation.Vertical;
            plt.Grid(lineStyle: LineStyle.Dot);
            plt.XTicks(labels);
            plt.SetAxisLimits(yMin: 0);
            //
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

        public List<User> GetPageSearch(string forSearch, int pageNumber, int pageSize)
        {
            if(pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            List<User> list = new List<User>();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT * FROM users
                WHERE fullname LIKE '%' || $value || '%'
                LIMIT $pageS OFFSET ($pageN-1)*$pageS;
            ";
            command.Parameters.AddWithValue("$value", forSearch);
            command.Parameters.AddWithValue("$pageN", pageNumber);
            command.Parameters.AddWithValue("$pageS", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                User user = new User();
                user.id = int.Parse(reader.GetString(0));
                user.login = reader.GetString(1);
                user.fullname = reader.GetString(2);
                user.isModerator = reader.GetString(3) != "0";
                user.createdAt = DateTime.Parse(reader.GetString(4));
                list.Add(user);
            }
            reader.Close();

            connection.Close();
            return list;
        }

        public long GetTotalPagesSearch(string forSearch, int pageSize)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT COUNT(*) FROM users
                WHERE fullname LIKE '%' || $value || '%';
            ";
            command.Parameters.AddWithValue("$value", forSearch);
            long numOfRows = (long)command.ExecuteScalar();

            connection.Close();

            long pages = 0;
            if(numOfRows%pageSize != 0)
            {
                pages = numOfRows/pageSize + 1;
            }
            else
            {
                pages = numOfRows/pageSize;
            }
            return pages;
        }

        public bool UserExists(string username)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username = $username";
            command.Parameters.AddWithValue("$username", username);

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

        public User GetByUsername(string username)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE username = $username";
            command.Parameters.AddWithValue("$username", username);

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

        public bool Update(long userId, User user)
        {
            throw new NotImplementedException();
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

        public bool DeleteUser(long userID)
        {
            this.connection.Open();
 
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", userID);

            int nChanged = command.ExecuteNonQuery();
            this.connection.Close();

            if (nChanged == 0)
            {
                return false;
            }
            else 
            {
                return true;
            }
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

        public long GetTotalPages(int pageSize)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT COUNT(*) FROM users;
            ";
            long numOfRows = (long)command.ExecuteScalar();

            connection.Close();

            long pages = 0;
            if(numOfRows%pageSize != 0)
            {
                pages = numOfRows/pageSize + 1;
            }
            else
            {
                pages = numOfRows/pageSize;
            }
            return pages;
        }

        public List<User> GetPage(int pageNumber, int pageSize)
        {
            if(pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            List<User> list = new List<User>();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT * FROM users LIMIT $pageS OFFSET ($pageN-1)*$pageS;
            ";
            command.Parameters.AddWithValue("$pageN", pageNumber);
            command.Parameters.AddWithValue("$pageS", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                User user = new User();

                user.id = int.Parse(reader.GetString(0));
                user.login = reader.GetString(1);
                user.fullname = reader.GetString(2);
                user.isModerator = reader.GetString(3) != "0";
                user.createdAt = DateTime.Parse(reader.GetString(4));
                list.Add(user);
            }
            reader.Close();

            connection.Close();
            return list;
        }
    }
}

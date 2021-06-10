using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ConsoleProject
{
    public class QuestionRepository
    {
        private SqliteConnection connection;
        public QuestionRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public List<DateTime> GetQuestionDatetimes(long userId, DateTime fromDate, DateTime toDate)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT createdAt FROM questions
                WHERE createdAt >= $fromDate
                and createdAt <= $toDate
                and authorId = $userId
            ";
            command.Parameters.AddWithValue("$fromDate", fromDate);
            command.Parameters.AddWithValue("$toDate", toDate);
            command.Parameters.AddWithValue("$userId", userId);

            SqliteDataReader reader = command.ExecuteReader();
            List<DateTime> dates = new List<DateTime>();

            while (reader.Read())
            {
                dates.Add(DateTime.Parse(reader.GetString(0)));
            }
            reader.Close();

            this.connection.Close();

            return dates;
        }

        public int CountQuestions(long userId, DateTime date)
        {
            this.connection.Open();

            int countQuestions = 0;
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            for(int i = 1; i <= daysInMonth; i++)
            {
                SqliteCommand command = connection.CreateCommand();
                // command.CommandText = 
                // @"
                //     SELECT COUNT(*) From questions
                //     WHERE authorId = $userId
                //     AND createdAt = $date
                // ";
                command.CommandText = 
                @"
                    SELECT COUNT(*) From questions
                    WHERE authorId = $userId
                    AND createdAt LIKE $date || '%'
                ";
                DateTime newDate = new DateTime(date.Year, date.Month, i);
                string isoDate = newDate.ToString("o");
                isoDate = isoDate.Split("T")[0];
                command.Parameters.AddWithValue("$date", isoDate);
                command.Parameters.AddWithValue("$userId", userId);

                int n = Convert.ToInt32(command.ExecuteScalar());
                countQuestions += n;
            }
            this.connection.Close();

            return countQuestions;
        }

        public List<Question> GetPageSearch(string forSearch, int pageNumber, int pageSize)
        {
            if(pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            List<Question> list = new List<Question>();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT * FROM questions
                WHERE questionText LIKE '%' || $value || '%'
                LIMIT $pageS OFFSET ($pageN-1)*$pageS;
            ";
            command.Parameters.AddWithValue("$value", forSearch);
            command.Parameters.AddWithValue("$pageN", pageNumber);
            command.Parameters.AddWithValue("$pageS", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Question q = new Question();
                q.id = int.Parse(reader.GetString(0));
                q.questionText = reader.GetString(1);
                q.authorId = int.Parse(reader.GetString(2));
                q.helpfulAnswerId = int.Parse(reader.GetString(3));
                q.createdAt = DateTime.Parse(reader.GetString(4));
                list.Add(q);
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
                SELECT COUNT(*) FROM questions
                WHERE questionText LIKE '%' || $value || '%';
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

        public long Insert(Question question)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO questions (questionText, createdAt, authorId, helpfulAnswerId)
                VALUES ($questionText, $createdAt, $authorId, $helpfulAnswerId);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$questionText", question.questionText);
            command.Parameters.AddWithValue("$createdAt", question.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$authorId", question.authorId);
            command.Parameters.AddWithValue("$helpfulAnswerId", question.helpfulAnswerId);

            long newId = (long)command.ExecuteScalar();

            this.connection.Close();
            return newId;
        }

        public bool QuestionExists(long questionId)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM questions WHERE id = $id";
            command.Parameters.AddWithValue("$id", questionId);

            SqliteDataReader reader = command.ExecuteReader();

            bool result = reader.Read();
            this.connection.Close();

            return result;
        }


        public List<Question> GetAllQuestionsByUserId(long userId)
        {
            List<Question> qList = new List<Question>();

            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM questions WHERE authorId = $id";
            command.Parameters.AddWithValue("$id", userId);

            SqliteDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                Question q = new Question();
                q.id = int.Parse(reader.GetString(0));
                q.questionText = reader.GetString(1);
                q.authorId = int.Parse(reader.GetString(2));
                q.helpfulAnswerId = int.Parse(reader.GetString(3));
                q.createdAt = DateTime.Parse(reader.GetString(4));
                qList.Add(q);
            }
            this.connection.Close();
            return qList;
        }

        public bool Update(long questionId, Question question) // доробити !
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE questions
                SET questionText = $questionText
                WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", questionId);
            command.Parameters.AddWithValue("$questionText", question.questionText);

            int nChanged = command.ExecuteNonQuery();

            connection.Close();

            return nChanged == 1;
        }

        public bool Delete(long questionId)
        {
            this.connection.Open();
 
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM questions WHERE id = $id";
            command.Parameters.AddWithValue("$id", questionId);

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

        public List<int> GetQuestionIdList()
        {
            List<int> list = new List<int>();

            this.connection.Open();
 
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT id FROM questions";

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
                SELECT COUNT(*) FROM questions;
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

        public List<Question> GetPage(int pageNumber, int pageSize)
        {
            if(pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            List<Question> list = new List<Question>();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT * FROM questions LIMIT $pageS OFFSET ($pageN-1)*$pageS;
            ";
            command.Parameters.AddWithValue("$pageN", pageNumber);
            command.Parameters.AddWithValue("$pageS", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Question q = new Question();
                q.id = int.Parse(reader.GetString(0));
                q.questionText = reader.GetString(1);
                q.authorId = int.Parse(reader.GetString(2));
                q.helpfulAnswerId = int.Parse(reader.GetString(3));
                q.createdAt = DateTime.Parse(reader.GetString(4));
                list.Add(q);
            }
            reader.Close();

            connection.Close();
            return list;
        }
    }
}

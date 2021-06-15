using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace MyLib
{
    public class AnswerRepository
    {
        private SqliteConnection connection;
        public AnswerRepository(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public int CountAnswers(long userId, DateTime date)
        {
            this.connection.Open();

            int countAnswers = 0;
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            for(int i = 1; i <= daysInMonth; i++)
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = 
                @"
                    SELECT COUNT(*) From answers
                    WHERE authorId = $userId
                    AND createdAt LIKE $date || '%'
                ";
                DateTime newDate = new DateTime(date.Year, date.Month, i);
                string isoDate = newDate.ToString("o");
                isoDate = isoDate.Split("T")[0];
                command.Parameters.AddWithValue("$date", isoDate);
                command.Parameters.AddWithValue("$userId", userId);

                int n = Convert.ToInt32(command.ExecuteScalar());
                countAnswers += n;
            }
            this.connection.Close();

            return countAnswers;
        }

        public List<Answer> GetPageSearch(string forSearch, int pageNumber, int pageSize)
        {
            if(pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            List<Answer> list = new List<Answer>();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT * FROM answers
                WHERE answerText LIKE '%' || $value || '%'
                LIMIT $pageS OFFSET ($pageN-1)*$pageS;
            ";
            command.Parameters.AddWithValue("$value", forSearch);
            command.Parameters.AddWithValue("$pageN", pageNumber);
            command.Parameters.AddWithValue("$pageS", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Answer a = new Answer();
                a.id = int.Parse(reader.GetString(0));
                a.answerText = reader.GetString(1);
                a.authorId = int.Parse(reader.GetString(2));
                a.questionId = int.Parse(reader.GetString(3));
                a.createdAt = DateTime.Parse(reader.GetString(4));
                list.Add(a);
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
                SELECT COUNT(*) FROM answers
                WHERE answerText LIKE '%' || $value || '%';
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

        public bool AnswerExists(long answerId)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM answers WHERE id = $id";
            command.Parameters.AddWithValue("$id", answerId);

            SqliteDataReader reader = command.ExecuteReader();

            bool result = reader.Read();
            this.connection.Close();

            return result;
        }

        public long Insert(Answer answer)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO answers (answerText, createdAt, authorId, questionId)
                VALUES ($answerText, $createdAt, $authorId, $questionId);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$answerText", answer.answerText);
            command.Parameters.AddWithValue("$createdAt", answer.createdAt.ToString("o"));
            command.Parameters.AddWithValue("$authorId", answer.authorId.ToString());
            command.Parameters.AddWithValue("$questionId", answer.questionId);

            long newId = (long)command.ExecuteScalar();

            this.connection.Close();
            return newId;
        }

        public Answer GetAnswerById(long answerId)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM answers WHERE id = $id";
            command.Parameters.AddWithValue("$id", answerId);

            SqliteDataReader reader = command.ExecuteReader();

            if(reader.Read())
            {
                Answer a = new Answer();
                a.id = int.Parse(reader.GetString(0));
                a.answerText = reader.GetString(1);
                a.authorId = int.Parse(reader.GetString(2));
                a.questionId = int.Parse(reader.GetString(3));
                a.createdAt = DateTime.Parse(reader.GetString(4));

                this.connection.Close();
                return a;
            }
            else
            {
                this.connection.Close();
                return null;
            }
        }

        public List<Answer> GetAllAnswersByUserId(long userId)
        {
            List<Answer> aList = new List<Answer>();

            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM answers WHERE authorId = $id";
            command.Parameters.AddWithValue("$id", userId);

            SqliteDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                Answer a = new Answer();
                a.id = int.Parse(reader.GetString(0));
                a.answerText = reader.GetString(1);
                a.authorId = int.Parse(reader.GetString(2));
                a.questionId = int.Parse(reader.GetString(3));
                a.createdAt = DateTime.Parse(reader.GetString(4));
                aList.Add(a);
            }
            this.connection.Close();
            return aList;
        }

        public List<Answer> GetAllAnswersByQuestionId(long questionId)
        {
            List<Answer> aList = new List<Answer>();

            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM answers WHERE questionId = $id";
            command.Parameters.AddWithValue("$id", questionId);

            SqliteDataReader reader = command.ExecuteReader();

            while(reader.Read())
            {
                Answer a = new Answer();
                a.id = int.Parse(reader.GetString(0));
                a.answerText = reader.GetString(1);
                a.authorId = int.Parse(reader.GetString(2));
                a.questionId = int.Parse(reader.GetString(3));
                a.createdAt = DateTime.Parse(reader.GetString(4));
                aList.Add(a);
            }
            this.connection.Close();
            return aList;
        }

        public bool Update(long answerId, Answer answer)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE answers
                SET answerText = $text
                WHERE id = $id
            ";
            command.Parameters.AddWithValue("$id", answerId);
            command.Parameters.AddWithValue("$text", answer.answerText);

            int nChanged = command.ExecuteNonQuery();

            connection.Close();

            return nChanged == 1;
        }

        public bool Delete(long answerId)
        {
            this.connection.Open();
 
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM answers WHERE id = $id";
            command.Parameters.AddWithValue("$id", answerId);

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

        public long GetTotalPages(int pageSize)
        {
            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT COUNT(*) FROM answers;
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

        public List<Answer> GetPage(int pageNumber, int pageSize)
        {
            if(pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber));
            }
            List<Answer> list = new List<Answer>();

            connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT * FROM answers LIMIT $pageS OFFSET ($pageN-1)*$pageS;
            ";
            command.Parameters.AddWithValue("$pageN", pageNumber);
            command.Parameters.AddWithValue("$pageS", pageSize);

            SqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Answer a = new Answer();
                a.id = int.Parse(reader.GetString(0));
                a.answerText = reader.GetString(1);
                a.authorId = int.Parse(reader.GetString(2));
                a.questionId = int.Parse(reader.GetString(3));
                a.createdAt = DateTime.Parse(reader.GetString(4));
                list.Add(a);
            }
            reader.Close();

            connection.Close();
            return list;
        }
    }
}

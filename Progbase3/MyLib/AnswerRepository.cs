using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ConsoleProject
{
    public class AnswerRepository
    {
        private SqliteConnection connection;
        public AnswerRepository(SqliteConnection connection)
        {
            this.connection = connection;
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
                Answer answer = new Answer();
                answer.id = long.Parse(reader.GetString(0));
                answer.answerText = reader.GetString(1);

                this.connection.Close();
                return answer;
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
            throw new NotImplementedException();
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

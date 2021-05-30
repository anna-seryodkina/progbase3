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

        public long Insert(Question question)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO questions (questionText, createdAt)
                VALUES ($questionText, $createdAt);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$questionText", question.questionText);
            command.Parameters.AddWithValue("$createdAt", question.createdAt.ToString("o"));
            // command.Parameters.AddWithValue("$userId", );

            long newId = (long)command.ExecuteScalar();

            this.connection.Close();
            return newId;
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

        public void Update()
        {
            throw new NotImplementedException();
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
    }
}

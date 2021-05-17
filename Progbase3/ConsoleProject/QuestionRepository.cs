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
                INSERT INTO questions (questionText, createdAt, userId)
                VALUES ($questionText, $createdAt, $userId);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$questionText", question.questionText);
            command.Parameters.AddWithValue("$createdAt", question.createdAt.ToString("o"));
            // command.Parameters.AddWithValue("$userId", );

            long newId = (long)command.ExecuteScalar();

            this.connection.Close();
            return newId;
        }

        public List<Question> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Read()
        {
            throw new NotImplementedException();
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
    }
}

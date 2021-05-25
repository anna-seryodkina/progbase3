using System;
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

        public long Insert(Answer answer)
        {
            this.connection.Open();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO answers (answerText, createdAt)
                VALUES ($answerText, $createdAt);
            
                SELECT last_insert_rowid();
            ";
            command.Parameters.AddWithValue("$answerText", answer.answerText);
            command.Parameters.AddWithValue("$createdAt", answer.createdAt.ToString("o"));

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
                string text = reader.GetString(1);
                Answer answer = new Answer(text);
                answer.id = long.Parse(reader.GetString(0));

                this.connection.Close();
                return answer;
            }
            else
            {
                this.connection.Close();
                return null;
            }
        }

        public void Read()
        {
            throw new NotImplementedException();
        }

        public void Update()
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
    }
}

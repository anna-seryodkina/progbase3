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

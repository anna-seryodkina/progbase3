using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace ConsoleProject
{
    public class UserRepository
    {
        private SqliteConnection connection;
        public UserRepository(SqliteConnection connection)
        {
            this.connection = connection;
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
                user.isModerator = int.Parse(reader.GetString(3));
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
                user.isModerator = int.Parse(reader.GetString(3));
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

        public void Update()
        {
            throw new NotImplementedException();
        }

        public User GetQAndAByUserId_Export(long userId, QuestionRepository qRepo, AnswerRepository aRepo)
        {
            User user = new User();
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
    }
}

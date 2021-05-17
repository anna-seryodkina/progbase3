using System;

namespace ConsoleProject
{
    public class User
    {
        public long id;
        public string login;
        public string fullname;
        public int isModerator;
        public DateTime createdAt;

        public User()
        {
            this.createdAt = DateTime.Now;
        }
    }
}

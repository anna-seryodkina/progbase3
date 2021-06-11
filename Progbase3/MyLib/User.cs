using System;
using System.Collections.Generic;

namespace MyLib
{
    public class User
    {
        public long id;
        public string login;
        public string fullname;
        public bool isModerator;
        public DateTime createdAt;
        public List<Answer> answers;
        public List<Question> questions;

        public User()
        {
            this.createdAt = DateTime.Now;
        }
    }
}

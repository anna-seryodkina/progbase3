using System.Collections.Generic;

namespace MyLib
{
    public class Question
    {
        public long id;
        public string questionText;
        public System.DateTime createdAt;
        public long authorId;
        public long helpfulAnswerId;
        public User author;
        public List<Answer> answers;

        public Question()
        {
            this.createdAt = System.DateTime.Now;
        }
    }
}

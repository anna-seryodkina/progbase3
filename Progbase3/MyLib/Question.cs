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

        public override string ToString()
        {
            string subString = "";
            if(questionText.Length <= 12)
            {
                subString = questionText;
            }
            else
            {
                subString = questionText.Substring(0, 12) + "...";
            }
            return $"[{id}] {subString}";
        }
    }
}

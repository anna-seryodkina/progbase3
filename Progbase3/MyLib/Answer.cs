namespace MyLib
{
    public class Answer
    {
        public long id;
        public string answerText;
        public System.DateTime createdAt;
        public long authorId;
        public long questionId;
        public User author;
        public Question question;

        public Answer()
        {
            this.createdAt = System.DateTime.Now;
        }

        public override string ToString()
        {
            string subString = "";
            if(answerText.Length <= 12)
            {
                subString = answerText;
            }
            else
            {
                subString = answerText.Substring(0, 12) + "...";
            }
            return $"[{id}] {subString}";
        }
    }
}

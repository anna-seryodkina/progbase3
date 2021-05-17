namespace ConsoleProject
{
    public class Answer
    {
        public long id;
        public string answerText;
        public System.DateTime createdAt;
        // public long userId;
        // public long questionId;

        public Answer(string text) // (string text, long questionId, long userId)
        {
            // this.questionId = questionId;
            // this.userId = userId;
            this.answerText = text;
            this.createdAt = System.DateTime.Now;
        }
    }
}

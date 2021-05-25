namespace ConsoleProject
{
    public class Question
    {
        public long id;
        public string questionText;
        public System.DateTime createdAt; // ???
        // public long userId;
        // public long helpfulAnswerId;

        public Question(string text) // (long userId, string text)
        {
            // this.userId = userId;
            this.questionText = text;
            this.createdAt = System.DateTime.Now;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using Microsoft.Data.Sqlite;

namespace ConsoleProject
{
    public class Root<T>
    {
        public List<T> root;
    }

    public class Im_Ex_port
    {
        public SqliteConnection connection;
        public string filenameUsersAnswer;
        public string filenameQuestion;
        string filenameQuestionAnswers;
        public void Import(string path)
        {
            QuestionRepository questionRepository = new QuestionRepository(connection);
            AnswerRepository answerRepository = new AnswerRepository(connection);

            string importPath = "";

            ZipFile.ExtractToDirectory(@path, @importPath);

            Root<Question> questionRoot = Deserialize<Question>(importPath + "/" + this.filenameQuestion);
            Root<Answer> answersListQRoot = Deserialize<Answer>(importPath + "/" + this.filenameQuestionAnswers);

            List<Question> questions = questionRoot.root;
            List<Answer> answers = answersListQRoot.root;

            //
            foreach(Answer answer in answers)
            {
                if(!answerRepository.AnswerExists(answer.id))
                {
                    answerRepository.Insert(answer);
                }
            }
            //
            foreach(Question question in questions)
            {
                if(!questionRepository.QuestionExists(question.id))
                {
                    questionRepository.Insert(question);
                    question.answers = answerRepository.GetAllAnswersByQuestionId(question.id);
                }
            }

        }

        public void Export(User user, string filenameUsersAnswer, string filenameQuestion, string filenameQuestionAnswers)
        {
            this.filenameUsersAnswer = filenameUsersAnswer;
            this.filenameQuestion = filenameQuestion;
            this.filenameQuestionAnswers = filenameQuestionAnswers;

            if(!System.IO.File.Exists(filenameUsersAnswer))
            {
                throw new Exception($"File does not exist: {filenameUsersAnswer}"); // not exception шоб не падало!
            }

            if(!System.IO.File.Exists(filenameQuestion))
            {
                throw new Exception($"File does not exist: {filenameQuestion}"); // not exception шоб не падало!
            }

            if(!System.IO.File.Exists(filenameQuestionAnswers))
            {
                throw new Exception($"File does not exist: {filenameQuestionAnswers}"); // not exception шоб не падало!
            }

            // users_answers.xml
            Root<Answer> usersAnswersRoot = new Root<Answer>();
            usersAnswersRoot.root = user.answers;
            Serialize<Answer>(filenameUsersAnswer, usersAnswersRoot);

            // questions_answers.xml
            Root<Answer> questionAnswersRoot = new Root<Answer>();
            foreach(Question question in user.questions)
            {
                foreach(Answer answer in question.answers)
                {
                    questionAnswersRoot.root.Add(answer);
                }
                question.answers = default; // to not serialize answers into questions.xml ??????????
            }
            Serialize<Answer>(filenameQuestionAnswers, questionAnswersRoot);

            // questions.xml
            Root<Question> qRoot = new Root<Question>();
            qRoot.root = user.questions;
            Serialize<Question>(filenameQuestion, qRoot);

            // заархівувати файли!!!
        }

        private void Serialize<T>(string filename, Root<T> root)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Root<T>));
            //
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = System.Xml.NewLineHandling.Entitize;
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(filename, settings);
            //
            ser.Serialize(writer, root);
            writer.Close();
        }

        public Root<T> Deserialize<T>(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Root<T>));
            StreamReader reader = new StreamReader(filename);
            Root<T> root = (Root<T>)ser.Deserialize(reader);
            reader.Close();
            return root;
        }
    }
}
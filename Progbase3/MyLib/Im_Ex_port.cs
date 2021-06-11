using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using Microsoft.Data.Sqlite;

namespace MyLib
{
    public class Root<T>
    {
        public List<T> root;
    }

    public class Im_Ex_port
    {
        public SqliteConnection connection;
        public string filenameUsersAnswer = "users_answers.xml";
        public string filenameQuestion = "questions.xml";
        public string filenameQuestionAnswers = "questions_answers.xml";

        public Im_Ex_port(SqliteConnection connection)
        {
            this.connection = connection;
        }

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

        public void Export(User user, string exportDirectoryName)
        {
            string usersAnswersPath = exportDirectoryName + "/" + this.filenameUsersAnswer;
            string questionsPath = exportDirectoryName + "/" + this.filenameQuestion;
            string questionAnswersPath = exportDirectoryName + "/" + this.filenameQuestionAnswers;

            // users_answers.xml
            Root<Answer> usersAnswersRoot = new Root<Answer>();
            usersAnswersRoot.root = user.answers;
            Serialize<Answer>(usersAnswersPath, usersAnswersRoot);

            // questions_answers.xml
            Root<Answer> questionAnswersRoot = new Root<Answer>();
            foreach(Question question in user.questions)
            {
                foreach(Answer answer in question.answers)
                {
                    questionAnswersRoot.root.Add(answer);
                }
                question.answers = default; // to not serialize answers into questions.xml
            }
            Serialize<Answer>(questionAnswersPath, questionAnswersRoot);

            // questions.xml
            Root<Question> qRoot = new Root<Question>();
            qRoot.root = user.questions;
            Serialize<Question>(questionsPath, qRoot);



            // archive
            string startPath = @exportDirectoryName;
            string zipPath = @exportDirectoryName + "/archive.zip";

            ZipFile.CreateFromDirectory(startPath, zipPath);

            File.Delete(usersAnswersPath);
            File.Delete(questionsPath);
            File.Delete(questionAnswersPath);
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
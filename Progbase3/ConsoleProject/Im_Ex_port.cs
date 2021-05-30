using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ConsoleProject
{
    public static class Im_Ex_port
    {
        public static void Import(string path)
        {
            // import magic, яку я не шарю
        }

        public static void Export(User user, string filenameAnswer, string filenameQuestion)
        {
            // навіщо масив з відповідями на всі питаннях користувача ??????????????
            // result: answers file + questions file

            if(!System.IO.File.Exists(filenameAnswer))
            {
                throw new Exception($"File does not exist: {filenameAnswer}"); // not exception шоб не падало!
            }

            if(!System.IO.File.Exists(filenameQuestion))
            {
                throw new Exception($"File does not exist: {filenameQuestion}"); // not exception шоб не падало!
            }

            List<Answer> answersList = user.answers;
            SerializeAnswers(filenameAnswer, answersList); // use ДЖЕНЕРІКИ !!!

            List<Question> questionsList = user.questions;
            SerializeQuestions(filenameQuestion, questionsList); // use ДЖЕНЕРІКИ !!!
        }

        private static void SerializeAnswers(string filename, List<Answer> root)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Answer>));
            //
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = System.Xml.NewLineHandling.Entitize;
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(filename, settings);
            //
            ser.Serialize(writer, root);
            writer.Close();
        }

        private static void SerializeQuestions(string filename, List<Question> root)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<Question>));
            //
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = System.Xml.NewLineHandling.Entitize;
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(filename, settings);
            //
            ser.Serialize(writer, root);
            writer.Close();
        }
    }
}
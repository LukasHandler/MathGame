using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Shared.Data
{
    public static class QuestionAccessor
    {
        public static List<MathQuestion> MathQuestions;

        static QuestionAccessor()
        {
            using (StreamReader r = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"/Resources/questions.json"))
            {
                string fileText = r.ReadToEnd();
                MathQuestions = JsonConvert.DeserializeObject<List<MathQuestion>>(fileText);
            }
        }
    }
}

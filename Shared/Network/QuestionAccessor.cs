//-----------------------------------------------------------------------
// <copyright file="QuestionAccessor.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the question accessor.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// THis class represents the question accessor.
    /// </summary>
    public static class QuestionAccessor
    {
        /// <summary>
        /// Initializes static members of the <see cref="QuestionAccessor"/> class.
        /// </summary>
        static QuestionAccessor()
        {
            using (StreamReader r = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory + @"/Resources/questions.json"))
            {
                string fileText = r.ReadToEnd();
                MathQuestions = JsonConvert.DeserializeObject<List<MathQuestion>>(fileText);
            }
        }

        /// <summary>
        /// Gets or sets the math questions.
        /// </summary>
        /// <value>
        /// The math questions.
        /// </value>
        public static List<MathQuestion> MathQuestions { get; set; }
    }
}

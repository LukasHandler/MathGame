//-----------------------------------------------------------------------
// <copyright file="MathQuestion.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents a math question.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.Data
{
    /// <summary>
    /// This class represents a math question.
    /// </summary>
    public class MathQuestion
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        public int Answer { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time for answering the question.
        /// </value>
        public int Time { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            MathQuestion toCompare = (MathQuestion)obj;

            if (this.Question == toCompare.Question && this.Time == toCompare.Time)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
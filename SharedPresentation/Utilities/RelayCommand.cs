//-----------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the generic relay command implementation from <see href="https://gist.github.com/schuster-rainer/2648922"/>.
// </summary>
//-----------------------------------------------------------------------
namespace Shared.SharedPresentation.Utilities
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// This class represents the generic relay command implementation.
    /// </summary>
    /// <seealso cref="System.Windows.Input.ICommand" />
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The action which should be executed.
        /// </summary>
        private Action<object> execute;

        /// <summary>
        /// The can execute condition.
        /// </summary>
        private Predicate<object> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute condition.</param>
        /// <exception cref="System.ArgumentNullException">If the execute action is null.</exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when can execute changed. Auto detects dependencies.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Determines whether this instance can execute the specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>True if it can be executed.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute(parameter);
        }

        /// <summary>
        /// Executes with specified parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}

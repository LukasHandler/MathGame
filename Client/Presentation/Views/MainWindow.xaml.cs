//-----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Lukas Handler">
//     Lukas Handler
// </copyright>
// <summary>
// This file represents the main window. Contains some logic for UI specific things.
// </summary>
//-----------------------------------------------------------------------
namespace Client.Presentation.Views
{
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for main window. Contains some logic for UI specific things.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            // When the collection of questions changes, always scroll to the last question.
            ((INotifyCollectionChanged)questions.Items).CollectionChanged += this.ScrollToLastQuestion;
        }

        /// <summary>
        /// Scrolls to last question.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void ScrollToLastQuestion(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            // From <see href="http://stackoverflow.com/questions/2337822/wpf-listbox-scroll-to-end-automatically"/>.
            if (VisualTreeHelper.GetChildrenCount(this.questions) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(this.questions, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
                this.questions.SelectedIndex = this.questions.Items.Count - 1;
            }
        }

        /// <summary>
        /// Focus the answer field.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void AnswerFocus(object sender, RoutedEventArgs eventArgs)
        {
            this.AnswerField.Clear();
            this.AnswerField.Focus();
        }
    }
}

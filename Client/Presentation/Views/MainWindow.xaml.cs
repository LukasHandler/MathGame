using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)questions.Items).CollectionChanged += ScrollToLastQuestion;
        }

        private void ScrollToLastQuestion(object sender, NotifyCollectionChangedEventArgs e)
        {
            //http://stackoverflow.com/questions/2337822/wpf-listbox-scroll-to-end-automatically
            if (VisualTreeHelper.GetChildrenCount(this.questions) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(this.questions, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
                this.questions.SelectedIndex = this.questions.Items.Count - 1;
            }
        }

        private void AnswerFocus(object sender, RoutedEventArgs e)
        {
            this.AnswerField.Clear();
            this.AnswerField.Focus();
        }
    }
}

using System;
using System.Diagnostics;
using System.Windows;

namespace ForgeModGenerator.ApplicationModule
{
    public partial class BugReporter : Window
    {
        public BugReporter(Exception exception)
        {
            InitializeComponent();
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            DataContext = exception;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) => Process.Start(AppPaths.Logs);

        private void Close_Click(object sender, RoutedEventArgs e) => Quit();

        private void Send_Click(object sender, RoutedEventArgs e) => SendBugReport();

        private void SendAndClose_Click(object sender, RoutedEventArgs e)
        {
            SendBugReport();
            Quit();
        }

        private void SendBugReport() => Process.Start("https://github.com/Prastiwar/ForgeModGenerator/issues/new?template=bug_report.md");

        private async void Quit()
        {
            await System.Threading.Tasks.Task.Delay(150).ConfigureAwait(false); // to show click effect
            Environment.Exit(0);
        }
    }
}

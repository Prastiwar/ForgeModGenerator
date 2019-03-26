using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ForgeModGenerator.Controls
{
    public class SubmitTextBox : TextBox
    {
        public SubmitTextBox() : base() => PreviewKeyDown += new KeyEventHandler(SubmitTextBox_PreviewKeyDown);

        public static readonly DependencyProperty TextSubmitedCommandProperty =
            DependencyProperty.Register("TextSubmitedCommand", typeof(ICommand), typeof(SubmitTextBox), new PropertyMetadata(null));
        public ICommand TextSubmitedCommand {
            get => (ICommand)GetValue(TextSubmitedCommandProperty);
            set => SetValue(TextSubmitedCommandProperty, value);
        }

        private void SubmitTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SubmitTextBox s = (SubmitTextBox)sender;
                SubmitText(s, s.Text);
            }
        }

        protected void SubmitText(SubmitTextBox sender, string text)
        {
            BindingExpression textBinding = sender.GetBindingExpression(TextProperty);
            if (textBinding != null)
            {
                textBinding.UpdateSource();
            }
            if (CanSubmitText(sender, text))
            {
                BindingExpression submitCommandBinding = sender.GetBindingExpression(TextSubmitedCommandProperty);
                if (submitCommandBinding != null)
                {
                    if (sender.TextSubmitedCommand != null)
                    {
                        sender.TextSubmitedCommand.Execute(text);
                    }
                }
            }
        }

        protected virtual bool CanSubmitText(SubmitTextBox sender, string text)
        {
            BindingExpression textBinding = sender.GetBindingExpression(TextProperty);
            if (textBinding != null)
            {
                if (textBinding.ParentBinding.Mode == BindingMode.TwoWay || textBinding.ParentBinding.Mode == BindingMode.OneWayToSource)
                {
                    textBinding.UpdateSource();
                    return textBinding.HasError;
                }
                else
                {
                    return textBinding.ValidateWithoutUpdate();
                }
            }
            return true;
        }
    }
}

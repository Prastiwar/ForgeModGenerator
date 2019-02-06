using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ForgeModGenerator.UserControls
{
    public class SubmitTextBox : TextBox
    {
        public SubmitTextBox() : base()
        {
            PreviewKeyDown += new KeyEventHandler(SubmitTextBox_PreviewKeyDown);
        }

        private void SubmitTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BindingExpression be = GetBindingExpression(TextProperty);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }
        }
    }
}

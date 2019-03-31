namespace ForgeModGenerator
{
    public class DialogResultAssemblyConverter
    {
        public static DialogResult Convert(System.Windows.Forms.DialogResult result)
        {
            switch (result)
            {
                case System.Windows.Forms.DialogResult.None:
                    return DialogResult.None;
                case System.Windows.Forms.DialogResult.OK:
                    return DialogResult.OK;
                case System.Windows.Forms.DialogResult.Cancel:
                    return DialogResult.Cancel;
                case System.Windows.Forms.DialogResult.Abort:
                    return DialogResult.Abort;
                case System.Windows.Forms.DialogResult.Retry:
                    return DialogResult.Retry;
                case System.Windows.Forms.DialogResult.Ignore:
                    return DialogResult.Ignore;
                case System.Windows.Forms.DialogResult.Yes:
                    return DialogResult.Yes;
                case System.Windows.Forms.DialogResult.No:
                    return DialogResult.No;
                default:
                    return DialogResult.None;
            }
        }

        public static System.Windows.Forms.DialogResult Convert(DialogResult result)
        {
            switch (result)
            {
                case DialogResult.None:
                    return System.Windows.Forms.DialogResult.None;
                case DialogResult.OK:
                    return System.Windows.Forms.DialogResult.OK;
                case DialogResult.Cancel:
                    return System.Windows.Forms.DialogResult.Cancel;
                case DialogResult.Abort:
                    return System.Windows.Forms.DialogResult.Abort;
                case DialogResult.Retry:
                    return System.Windows.Forms.DialogResult.Retry;
                case DialogResult.Ignore:
                    return System.Windows.Forms.DialogResult.Ignore;
                case DialogResult.Yes:
                    return System.Windows.Forms.DialogResult.Yes;
                case DialogResult.No:
                    return System.Windows.Forms.DialogResult.No;
                default:
                    return System.Windows.Forms.DialogResult.None;
            }
        }
    }
}

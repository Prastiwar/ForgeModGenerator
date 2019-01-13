using System;
using System.Windows;

namespace ForgeModGenerator
{
    public static class Log
    {
        public static void Info(object msg)
        {
            Console.WriteLine(msg);
        }

        public static void InfoBox(object msg)
        {
            MessageBox.Show(msg?.ToString());
        }
    }
}

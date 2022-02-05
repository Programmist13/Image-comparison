using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_comparison
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
    }

    static class percent_done
    {
        public static decimal value
        {
            get;
            set;
        }

        public static int photo
        {
            get;
            set;
        }

        public static int all_photo
        {
            get;
            set;
        }

        public static int step_token
        {
            get;
            set;
        }
    }
}

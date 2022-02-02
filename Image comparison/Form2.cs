using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace Image_comparison
{
    
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Thread t = new Thread(Start);
            t.Start();
            /*
            while (true)
            {
                if (Math.Round(percent_done.value) == 100)
                {
                    Thread.Sleep(1000);
                    this.Close();
                    break;
                }
            }*/
        }
        
        void Start()
        {
            while (true)
            {
                progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value=Convert.ToInt32(percent_done.value)));
                PB_label.Invoke((MethodInvoker)(() => PB_label.Text = Convert.ToString(Math.Round(percent_done.value))+"%"));
                if (Math.Round(percent_done.value) == 100)
                {
                    this.Invoke((MethodInvoker)(() => this.Close()));
                    break;
                }
            }
        }
    }
}

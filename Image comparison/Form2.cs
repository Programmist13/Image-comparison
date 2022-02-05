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
            Thread tf2 = new Thread(Step_1);
            tf2.Start();
        }

        void Step_1()
        {
            while (true)
            {
                progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = Convert.ToInt32(percent_done.value)));
                PB_label.Invoke((MethodInvoker)(() => PB_label.Text = Convert.ToString(Math.Round(percent_done.value)) + "%"));
                count_label.Invoke((MethodInvoker)(() => count_label.Text = "Фото " + percent_done.photo + " из " + percent_done.all_photo));
                if (Math.Round(percent_done.value) == 100 || percent_done.step_token > 1 )
                {
                    Step_2();
                    break;
                }
            }
        }

        void Step_2()
        {                
            while (true)
            {
                if (percent_done.step_token == 2 || percent_done.step_token > 2)
                {
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = Convert.ToInt32(percent_done.value)));
                    step_label.Invoke((MethodInvoker)(() => step_label.Text = "Этап 2 из 3. Обработка фото. Сравнение"));
                    count_label.Invoke((MethodInvoker)(() => count_label.Text = "Фото " + percent_done.photo + " из " + percent_done.all_photo));
                    PB_label.Invoke((MethodInvoker)(() => PB_label.Text = Convert.ToString(Math.Round(percent_done.value)) + "%"));
                    if (Math.Round(percent_done.value) == 100)
                    {
                        Step_3();
                        break;
                    }
                }
            }
        }

        void Step_3()
        {
            while (true)
            {
                if (percent_done.step_token == 3 || percent_done.step_token > 3)
                {
                    progressBar1.Invoke((MethodInvoker)(() => progressBar1.Value = Convert.ToInt32(percent_done.value)));
                    step_label.Invoke((MethodInvoker)(() => step_label.Text = "Этап 3 из 3. Сортировка ассицаций по убыванию похожести"));
                    count_label.Invoke((MethodInvoker)(() => count_label.Text = "Фото " + percent_done.photo + " из " + percent_done.all_photo));
                    PB_label.Invoke((MethodInvoker)(() => PB_label.Text = Convert.ToString(Math.Round(percent_done.value)) + "%"));
                    if (Math.Round(percent_done.value) == 100)
                    {
                        this.Invoke((MethodInvoker)(() => this.Close()));
                        break;
                    }
                }
            }
        }
    }
}

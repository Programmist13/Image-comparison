using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


//Сравнить уменьшенное изображение с другим таким же

namespace Image_comparison
{
    public partial class Form1 : Form
    {
        string[] boxfiles;
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;            
            InitializeComponent();
        }

        private void OpenFolder(object sender, EventArgs e)     //открываем папку с изображениями
        {
            folderBrowserDialog1.ShowDialog();                                                      //отображаем диалоговое окно
            Label_folder.Text = folderBrowserDialog1.SelectedPath;                                  //отображаем выбранный путь 
            string[] filebox = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.jpg", SearchOption.AllDirectories);
            var allfiles = new DirectoryInfo(folderBrowserDialog1.SelectedPath);                    //создаём экземпляр класса с указанием пути
            foreach (FileInfo file in allfiles.GetFiles("*.jpg", SearchOption.AllDirectories))      //перебираем все файлы jpg, включая вложенные папки
            {
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(file.FullName));                //отображаем имена файлов стобликом
            }
            boxfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.jpg", SearchOption.AllDirectories);
            listBox1.SelectedIndex = 0;
            
        }

        private void Select_Picture(object sender, EventArgs e)     //показываем выбраное изображение
        {
            Bitmap image;
            image = new Bitmap(boxfiles[listBox1.SelectedIndex]);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            //this.pictureBox1.Size = image.Size;
            pictureBox1.Image = image;
            pictureBox1.Invalidate();
        }

        private void Convert_WB(object sender, EventArgs e)         //преобразовываем в ЧБ
        {
            if (pictureBox3.Image != null) // если изображение в pictureBox1 имеется
            {
                
                Bitmap input = new Bitmap(pictureBox3.Image);               // создаём экземпляр изображения, из pictureBox1
                Bitmap output = new Bitmap(input.Width, input.Height);      // создаём новое изображение по размерам исходного
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb()); // получаем цифровой цветовой код пикселя
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;     //здесь хранится цвет пикселя в оттенках серого
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                pictureBox4.Image = output;
            }
        }

        private void ReSize(object sender, EventArgs e)             //уменьшение изображения
        {
                Bitmap input = new Bitmap(pictureBox1.Image);               
                int newWidth, newHeight;
                int nWidth, nHeight;
                nWidth = 100;
                nHeight = 100;
                var coefH = (double)nHeight / (double)input.Height;
                var coefW = (double)nWidth / (double)input.Width;
                if (coefW >= coefH)
                {
                    newHeight = (int)(input.Height * coefH);
                    newWidth = (int)(input.Width * coefH);
                }
                else
                {
                    newHeight = (int)(input.Height * coefW);
                    newWidth = (int)(input.Width * coefW);
                }

                Bitmap output = new Bitmap(newWidth, newHeight);
                using (var g = Graphics.FromImage(output))
                {
                
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    g.DrawImage(input, 0, 0, newWidth, newHeight);
                    g.Dispose();
                }
            pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox3.Image = output;


        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap input = new Bitmap(pictureBox1.Image);
            int newWidth, newHeight;
            int nWidth, nHeight;
            nWidth = 100;
            nHeight = 100;
            var coefH = (double)nHeight / (double)input.Height;
            var coefW = (double)nWidth / (double)input.Width;
            if (coefW >= coefH)
            {
                newHeight = (int)(input.Height * coefH);
                newWidth = (int)(input.Width * coefH);
            }
            else
            {
                newHeight = (int)(input.Height * coefW);
                newWidth = (int)(input.Width * coefW);
            }

            Bitmap output = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(output))
            {

                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                g.DrawImage(input, 0, 0, newWidth, newHeight);
                g.Dispose();
            }
            pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox5.Image = output;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null) // если изображение в pictureBox1 имеется
            {

                Bitmap input = new Bitmap(pictureBox5.Image);               // создаём экземпляр изображения, из pictureBox1
                Bitmap output = new Bitmap(input.Width, input.Height);      // создаём новое изображение по размерам исходного
                // перебираем в циклах все пиксели исходного изображения
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb()); // получаем цифровой цветовой код пикселя
                        // получаем компоненты цветов пикселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                        float B = (float)(pixel & 0x000000FF); // синий
                                                               // делаем цвет черно-белым (оттенки серого) - находим среднее арифметическое
                        R = G = B = (R + G + B) / 3.0f;
                        // собираем новый пиксель по частям (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляем его в Bitmap нового изображения
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // выводим черно-белый Bitmap в pictureBox2
                pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                pictureBox2.Image = output;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //pictureBox4, picturebox2
            //пиксель может иметь глубину цвета от 0 до 255 (оттенки серого)
            //посчитать количество пикселей в каждой из градаций, диапазон градаций будет указывать на процент схожести снимков
            //если взять на 100% схожести попиксельная точность, то 100% = диапазону 1, а 0% диапазону 0-255, соответственно 50%=128, т.е. 0-128, 129-255 (два диапазона)
            int[] color_base = new int [256];   //pictureBox4
            int[] color_base2 = new int[256];   //pictureBox2
            int[] color_deff = new int[256];    //массив для подсчёта разницы снимков
            float sum_diff = 0;
            float resul = 0;
            Bitmap input = new Bitmap(pictureBox3.Image);               // создаём экземпляр изображения, из pictureBox1
            Bitmap input2 = new Bitmap(pictureBox5.Image);               // создаём экземпляр изображения, из pictureBox1


            //считаем количество каждого цвета пикселя ЧБ снимка 1
            for (int j = 0; j < input.Height; j++)
                for (int i = 0; i < input.Width; i++)
                {
                    UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb()); // получаем цифровой цветовой код пикселя
                    float R = (float)((pixel & 0x00FF0000) >> 16); // красный
                    float G = (float)((pixel & 0x0000FF00) >> 8); // зеленый
                    float B = (float)(pixel & 0x000000FF); // синий
                    R = G = B = (R + G + B) / 3.0f;
                    color_base[(int)(R)] ++;                    
                }

            //считаем количество каждого цвета пикселя ЧБ снимка 2
            for (int j = 0; j < input2.Height; j++)
                for (int i = 0; i < input2.Width; i++)
                {
                    UInt32 pixel = (UInt32)(input2.GetPixel(i, j).ToArgb()); 
                    float R = (float)((pixel & 0x00FF0000) >> 16); 
                    float G = (float)((pixel & 0x0000FF00) >> 8); 
                    float B = (float)(pixel & 0x000000FF); 
                    R = G = B = (R + G + B) / 3.0f;
                    color_base2[(int)(R)]++;
                }

            //считаем разницу по количеству разных цветных пикселей по модулю
            for (int i=0; i<256; i++)
            {                
                color_deff[i] = Math.Abs(color_base2[i] - color_base[i]);
                sum_diff += color_deff[i];                
                resul = Math.Abs(((sum_diff * 100) / 2550000)*100-100);            
            }

            label2.Text = string.Format("Степень оригинальности {0:F2}%", resul);
        }
    }
}
/*
 * Console.WriteLine(string.Format("{0:F1}", 123.233424224));
foreach (string filename in boxfiles)
            {
                listBox1.Items.Add(filename);
            }

Типичное изображение в градациях серого — 256 оттенков серого в диапазоне от 0 (черный) до 255 (белый).
 */
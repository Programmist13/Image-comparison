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
        string[] boxfiles;                  //храним полный путь к каждому файлу
        Bitmap[] image_box_all_small;       //массив для хранения копий изображений уменьшенного масштаба
        int count_images;                   //общее количество снимков
        object[][,] comprasion_list;         //список сопоставлений фотографий
        int[][] temp_count_colors;
        //в массиве массивов первый ряд элементов должен соответсвовать количеству снимков
        /*int[][] nums = new int[3][];
        nums[0] = new int[2] { 1, 2 };          // выделяем память для первого подмассива
        nums[1] = new int[3] { 1, 2, 3 };       // выделяем память для второго подмассива
        nums[2] = new int[5] { 1, 2, 3, 4, 5 }; // выделяем память для третьего подмассива
        */
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;            
            InitializeComponent();
            comboBox1.SelectedIndex = 1;
        }

        private void OpenFolder(object sender, EventArgs e)     //открываем папку с изображениями
        {
            folderBrowserDialog1.ShowDialog();                                                      //отображаем диалоговое окно
            Label_folder.Text = folderBrowserDialog1.SelectedPath;                                  //отображаем выбранный путь 
            boxfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.jpg", SearchOption.AllDirectories);
            var allfiles = new DirectoryInfo(folderBrowserDialog1.SelectedPath);                    //создаём экземпляр класса с указанием пути
            foreach (FileInfo file in allfiles.GetFiles("*.jpg", SearchOption.AllDirectories))      //перебираем все файлы jpg, включая вложенные папки
            {
                listBox1.Items.Add(Path.GetFileNameWithoutExtension(file.FullName));                //отображаем имена файлов стобликом
            }
            listBox1.SelectedIndex = 0;
            count_images = boxfiles.Length;
            temp_count_colors = new int[count_images][];
            comprasion_list = new string[count_images][,];
            image_box_all_small = new Bitmap[count_images];                                      //делаем маленькие копии изображений и сохраняем их в массив
            for (int i=0; i<count_images; i++)                                                   
            {
                ReSize(i);
            }
            splitting();
            //comprasion_difference();

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

        void ReSize(int i)             //уменьшаем изображения и сохраняем их в массив image_box_all_small
        {
                Bitmap input = new Bitmap(boxfiles[i]);               
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
            image_box_all_small[i] = output;
        }

        //метод разбивает каждый маленький снимок по цветам и записывает информацию по количеству цветов в отдельный массив массивов comprasion_list
        void splitting ()
        {            
            for (int i = 0; i < count_images; i++)
            {
                temp_count_colors[i] = new int[256];
                for (int j = 0; j < image_box_all_small[i].Height; j++)
                    for (int k = 0; k < image_box_all_small[i].Width; k++)
                    {
                        UInt32 pixel = (UInt32)(image_box_all_small[i].GetPixel(i, j).ToArgb());
                        float R = (float)((pixel & 0x00FF0000) >> 16);
                        float G = (float)((pixel & 0x0000FF00) >> 8);
                        float B = (float)(pixel & 0x000000FF);
                        R = G = B = (R + G + B) / 3.0f;
                        //nums[0] = new int[2] { 1, 2 };                        
                        temp_count_colors[i][(int)(R)]++;                                //считаем количество цветов
                    }
            }
        }

        //метод считает разницу между снимками в цифровом эквиваленте и записывает для каждого изображения сопоставления
        //формирует список похожести (с указанием в процентах) для каждой фотографии
        //дописать метод comprasion_difference чтобы он умел перебирать фото которые сравнивает с оригинало
        void comprasion_difference()
        {
            int[] color_deff = new int[256];
            int sum_diff;
            int resul;

            for (int i = 0; i < count_images; i++)
            {
                sum_diff = 0;
                comprasion_list[i] = new string[count_images,count_images];
                for (int k = 0; k < 256; k++)
                {                    
                    color_deff[k] = Math.Abs(temp_count_colors[i][k] - color_base2[k]);           //фиксируем разницу по каждому цвету между сравниваемыми снимками
                    sum_diff += color_deff[k];                                          //накапливаем общую разницу между снимками
                    resul = Math.Abs(((sum_diff * 100) / 2550000) * 100 - 100);         //вычисляем общую степень схожести в процентах
                    //если у снимка схожесть более 75% только тогда вносим его в список похожих фотографий
                    if (resul>75)
                    {
                        comprasion_list[i][1,1] = ("путь снимка", resul );
                        //int[,] numbers = { { 1, 2, 3 }, { 4, 5, 6 }};
                    }
                }
            }
        }
        /*void Comprasion(int i)      //сравнение
        {
            //пиксель может иметь глубину цвета от 0 до 255 (оттенки серого)
            //записать какого цвета каждый пиксель
            //если взять на 100% схожести попиксельная точность, то 100% = диапазону 1, а 0% диапазону 0-255, соответственно 50%=128, т.е. 0-128, 129-255 (два диапазона)
            int[] color_base = new int [256];   //массив для подсчёта количества каждого цвета пикселя изображения 1
            int[] color_base2 = new int[256];   //изображение с которым сравниваем
            int[] color_deff = new int[256];    //массив для подсчёта разницы снимков
            float sum_diff = 0;
            float resul = 0;

            //считаем количество каждого цвета пикселя ЧБ изображения 1
            //каждый индекс массива соответствует отдельному цвету от 0 до 255
            for (int j = 0; j < image_box_all_small[i].Height; j++)
                for (int k = 0; k < image_box_all_small[i].Width; k++)
                {
                    UInt32 pixel = (UInt32)(image_box_all_small[i].GetPixel(i, j).ToArgb()); // получаем цифровой цветовой код пикселя
                    float R = (float)((pixel & 0x00FF0000) >> 16);  // красный
                    float G = (float)((pixel & 0x0000FF00) >> 8);   // зеленый
                    float B = (float)(pixel & 0x000000FF);          // синий
                    R = G = B = (R + G + B) / 3.0f;
                    color_base[(int)(R)] ++;                    
                }

            //считаем разницу по количеству разных цветных пикселей по модулю
            //Чем больше непохожесть цвета, тем больше разница изображений
            for (int k=0; k<256; k++)
            {                
                color_deff[k] = Math.Abs(color_base2[k] - color_base[k]);
                sum_diff += color_deff[k];                
                resul = Math.Abs(((sum_diff * 100) / 2550000)*100-100);            
            }

            label2.Text = string.Format("Степень оригинальности {0:F2}%", resul);
        }*/

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
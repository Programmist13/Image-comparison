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
        string[][,] comprasion_list;        //список сопоставлений фотографий
        int[][] temp_count_colors;          //хранит в себе количество цвета каждого снимка
        int origin_level=50;                   //степень оригинальности

        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;            
            InitializeComponent();
            comboBox1.SelectedIndex = 10;            
        }

        private void OpenFolder(object sender, EventArgs e)     //открываем папку с изображениями
        {Repite:
            try
            {                
                listBox1.Items.Clear();
                folderBrowserDialog1.ShowDialog();                                                      //отображаем диалоговое окно
                Label_folder.Text = folderBrowserDialog1.SelectedPath;                                  //отображаем выбранный путь 
                boxfiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.jpg", SearchOption.AllDirectories);
                var allfiles = new DirectoryInfo(folderBrowserDialog1.SelectedPath);                    //создаём экземпляр класса с указанием пути
                foreach (FileInfo file in allfiles.GetFiles("*.jpg", SearchOption.AllDirectories))      //перебираем все файлы jpg, включая вложенные папки
                {
                    listBox1.Items.Add(Path.GetFileNameWithoutExtension(file.FullName));                //отображаем имена файлов стобликом
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Нужно открыть папку", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                goto Repite;
            }


            count_images = boxfiles.Length;
            temp_count_colors = new int[count_images][];
            comprasion_list = new string[count_images][,];
            image_box_all_small = new Bitmap[count_images];                                      //делаем маленькие копии изображений и сохраняем их в массив
            for (int i=0; i<count_images; i++)                                                   
            {
                ReSize(i);
            }
            splitting();
            comprasion_difference();
            listBox1.SelectedIndex = 0;
        }

        private void Select_Picture(object sender, EventArgs e)     //показываем выбраное изображение и список похожих фото
        {
            
            Bitmap image;
            image = new Bitmap(boxfiles[listBox1.SelectedIndex]);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.Image = image;
            pictureBox1.Invalidate();
            out_resul(listBox1.SelectedIndex);
        }

        void ReSize(int i)             //уменьшаем изображения и сохраняем их в массив image_box_all_small
        {
            try
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
            catch (OutOfMemoryException)
            {
                MessageBox.Show("ОЗУ не хватает", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            
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
                        UInt32 pixel = (UInt32)(image_box_all_small[i].GetPixel(k, j).ToArgb());
                        float R = (float)((pixel & 0x00FF0000) >> 16);
                        float G = (float)((pixel & 0x0000FF00) >> 8);
                        float B = (float)(pixel & 0x000000FF);
                        R = G = B = (R + G + B) / 3.0f;                   
                        temp_count_colors[i][(int)(R)]++;                                //считаем количество цветов
                    }
            }
        }

        //метод считает разницу между снимками в цифровом эквиваленте и записывает для каждого изображения сопоставления
        void comprasion_difference()
        {
            int[] color_deff = new int[256];
            decimal sum_diff;
            decimal resul;
            

            for (int i = 0; i < count_images; i++)          //первым for i перебираем все снимки оригинала
            {
                comprasion_list[i] = new string[count_images,2];
                int x = 0;

                for (int j = 0; j < count_images; j++)      //вторым for j перебираем снимки с которыми сравниваем
                {
                    sum_diff = 0;
                    

                    if (i != j)                             //исключавем сравнение фото с самим собой
                    {                     
                        for (int k = 0; k < 256; k++)                                           //for k - перебирает цветовой диапазон каждого снимка
                        {
                            color_deff[k] = Math.Abs(temp_count_colors[i][k] - temp_count_colors[j][k]);            //фиксируем разницу по каждому цвету между сравниваемыми снимками
                            sum_diff += color_deff[k];                                                              //накапливаем общую разницу между снимками
                            resul = Math.Round(Math.Abs(((sum_diff * 100) / 2550000) * 100 - 100),2);                             //вычисляем общую степень схожести в процентах
                                                                                                                    //если у снимка схожесть более 75% только тогда вносим его в список похожих фотографий
                            if (k==255)
                                
                            {
                                comprasion_list[i][x,0] = Convert.ToString(boxfiles[j]);
                                comprasion_list[i][x,1] = Convert.ToString(resul);
                                x++;                                
                            }
                        }
                    }
                    
                }
            }
            sorting();
        }

        //сортировка массива ассицаций похожих фотографий по степени убывания
        void sorting()
        {
            
            //сортировка пузыриком по убыванию значений
            for (int k = 0; k < count_images; k++)
            {
                for (int i = 0; i < count_images; i++)
                {
                    for (int j = 0; j < count_images - 1; j++)
                    {
                        if (Convert.ToSingle(comprasion_list[k][j,1]) < Convert.ToSingle(comprasion_list[k][j + 1,1]))
                        {
                            string temp_patch = comprasion_list[k][j,0];
                            string temp_perc = comprasion_list[k][j,1];
                            comprasion_list[k][j,0] = comprasion_list[k][j + 1,0];
                            comprasion_list[k][j,1] = comprasion_list[k][j + 1,1];
                            comprasion_list[k][j + 1, 0] = temp_patch;
                            comprasion_list[k][j + 1, 1] = temp_perc;

                        }
                    }
                }
            }
        }

        void out_resul(int i)
        {
            listBox2.Items.Clear();

                for (int j = 0; j < comprasion_list.Length; j++)
                {
                    if (comprasion_list[i][j, 0] != null & Convert.ToDecimal(comprasion_list[i][j, 1])>origin_level)
                    {
                        listBox2.Items.Add(Convert.ToString(comprasion_list[i][j, 1]) + "% - " + Convert.ToString(Path.GetFileNameWithoutExtension(comprasion_list[i][j, 0])));
                    }
                }
                if (listBox2.Items.Count != 0)
                {
                    listBox2.SelectedIndex = 0;
                }
            
        }

        private void Select_Picture2(object sender, EventArgs e)
        {
            Bitmap image;
            image = new Bitmap(comprasion_list[listBox1.SelectedIndex][listBox2.SelectedIndex,0]);              
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox2.Image = image;
            pictureBox2.Invalidate();
        }

        private void select_level_origin(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0: origin_level = 100; break;
                case 1: origin_level = 95; break;
                case 2: origin_level = 90; break;
                case 3: origin_level = 85; break;
                case 4: origin_level = 80; break;
                case 5: origin_level = 75; break;
                case 6: origin_level = 70; break;
                case 7: origin_level = 65; break;
                case 8: origin_level = 60; break;
                case 9: origin_level = 55; break;
                case 10: origin_level = 50; break;
                default: origin_level = 75; break;
            }
            out_resul(listBox1.SelectedIndex);
        }
    }
}

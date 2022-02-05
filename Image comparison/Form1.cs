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
using System.Threading;




namespace Image_comparison
{
    public partial class Form1 : Form
    {
        #region initial value
    
        string[] boxfiles;                  //храним полный путь к каждому файлу
        string[] name_box;
        string[] patch_box_adaptive;
        int count_images;                   //общее количество снимков
        string[][,] comprasion_list;        //список сопоставлений фотографий
        int[][] small_count;                //накопитель информации по снимкам
        int origin_level = 50;                //уставка степени оригинальности
        Bitmap origin = new Bitmap(100, 100);
        Bitmap origin_small = new Bitmap(100, 100);                //делаем ссылку для маленького снимка
        Bitmap comp_small = new Bitmap(100, 100);
        Bitmap Picture_2 = new Bitmap(100, 100);
        int x = 0;                          //счётчик сравниваемых фото для comprassion_difference
        int[] color_deff = new int[256];
        #endregion
        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;            
            InitializeComponent();
            comboBox1.SelectedIndex = 10;
        }
        private void OpenF2()                                   //открываем ProgressBar в отдельном потоке
        {
            Form2 f2 = new Form2();
            f2.ShowDialog();
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
            percent_done.step_token = 0;
            count_images = boxfiles.Length;
            name_box = new string[count_images];
            for (int i = 0; i < count_images; i++)
            {
                name_box[i] = listBox1.Items[i].ToString();
            }
            percent_done.all_photo = count_images;
            comprasion_list = new string[count_images][,];
            small_count = new int[count_images][];
            percent_done.value = 0;
            Thread main_thread = new Thread(new ThreadStart(OpenF2));
            main_thread.Start();
            percent_done.step_token = 1;
            for (int i = 0; i < count_images; i++)
            {   
                small_count[i] = new int[256];
                ReSize(i);
                percent_done.value = Decimal.Multiply((Convert.ToDecimal(i+1) / Convert.ToDecimal(count_images)),100);
                percent_done.photo = i+1;
            }
            percent_done.step_token = 2;
            for (int i=0; i<count_images; i++)                                                   
            {
                comprasion_list[i] = new string[count_images, 2];
                percent_done.photo = i + 1;
                percent_done.value = Decimal.Multiply((Convert.ToDecimal(i + 1) / Convert.ToDecimal(count_images)), 100);
                for (int j = 0; j < count_images; j++)
                {
                    if (i != j)
                    {
                        comprasion_difference(i,j);
                    }
                }
                x=0;
            }
            percent_done.step_token = 3;
            sorting();
            listBox1.SelectedIndex = 0;
            percent_done.step_token = 4;             //процесс загрузки, обработки и сортировки фото завершился
        }

        //уменьшаем изображения и сохраняем их в массив image_box_all_small
        void ReSize(int i)             
        {
            try
            {
                    origin = new Bitmap(boxfiles[i]);
                    origin_small = new Bitmap(origin, new Size(100, 100));
                    splitting(origin_small, i);
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Переполнение оперативной памяти", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            
        }

        //метод разбивает каждый маленький снимок по цветам и записывает информацию по количеству цветов в отдельный массив массивов comprasion_list
        void splitting (Bitmap input, int i)
        {           
                for (int j = 0; j < input.Height; j++)
                    for (int k = 0; k < input.Width; k++)
                    {
                        UInt32 pixel = (UInt32)(input.GetPixel(k, j).ToArgb());     //посмотреть что то более быстрое чем GetPixel, сделать многопоточность
                        float R = (float)((pixel & 0x00FF0000) >> 16);
                        float G = (float)((pixel & 0x0000FF00) >> 8);
                        float B = (float)(pixel & 0x000000FF);
                        R = G = B = (R + G + B) / 3.0f;
                        small_count[i][(int)(R)]++;                                //считаем количество цветов оригинала
                    }                      
        }

        //метод считает разницу между снимками в цифровом эквиваленте и записывает для каждого изображения сопоставления      
        void comprasion_difference(int i, int j)
        {
            
            decimal sum_diff;
            decimal resul;                      
            sum_diff = 0;
                for (int k = 0; k < 256; k++)                                                         //for k - перебирает цветовой диапазон каждого снимка
                {
                    color_deff[k] = Math.Abs(small_count[i][k] - small_count[j][k]);            //фиксируем разницу по каждому цвету между сравниваемыми снимками
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

        //сортировка массива ассицаций похожих фотографий по степени убывания
        void sorting()
        {            
            //сортировка пузыриком по убыванию значений
            for (int k = 0; k < count_images; k++)
            {
                percent_done.photo = k + 1;
                percent_done.value = Decimal.Multiply((Convert.ToDecimal(k + 1) / Convert.ToDecimal(count_images)), 100);
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
       
        //показываем выбраное изображение и список похожих фото
        private void Select_Picture(object sender, EventArgs e)    
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
            }
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.Image = Image.FromFile(boxfiles[listBox1.SelectedIndex]);
            pictureBox2.Image = null;
            out_resul(listBox1.SelectedIndex);
        }

        //выводим список ассицаций к выбранному фото
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
            if (pictureBox2.Image != null)
            {
                Picture_2 = null;
                pictureBox2.Image.Dispose();
            }
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            Picture_2 = new Bitmap(comprasion_list[listBox1.SelectedIndex][listBox2.SelectedIndex, 0]);
            pictureBox2.Image = Picture_2;
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
            pictureBox2.Image = null;
            out_resul(listBox1.SelectedIndex);
        }
    }
}

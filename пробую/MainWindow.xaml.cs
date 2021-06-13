using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
//через добавить ссылку в обозревателе решений добавил Windows.Forms

namespace пробую
{
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(); //таймер
        MediaPlayer player = new MediaPlayer(); //сам плеер
        string[] files; //сами треки
        int i; //какой по счету трек
        string[] str5; //название песни
        int[] rnd2; //массив для рандома
        Random rnd = new Random(); //сам рандом
        int j; //запоминает само рандомное число
        bool k = false; //определяет включен ли рандом

        string[] str10; //нужно для проверки папки
        int y; //нужно для проверки папки
        //string[] str6; //для поиска
        public MainWindow()
        {
            InitializeComponent();  //изначальный выбор папки

            i = 0;
            button_r.Content = "Random: off";
           
            
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            //FBD.ShowDialog();
            //if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK) крч я это пичинил
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                y = 0;
                str10 = Directory.GetFiles(FBD.SelectedPath);
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg")
                    {
                        
                        y++;
                    }
                    
                }

                files = new string[y];
                y = 0;
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg")
                    {
                        files[y] = str10[v];
                        y++;
                    }

                }

                //files = Directory.GetFiles(FBD.SelectedPath);

                for (int b = 0; b < files.Length; b++)
                    {
                        str5 = files[b].ToString().Split('\\');
                        lb1.Items.Add(str5[str5.Length - 1]);
                    }
                   
            }
            else
            {
                //MessageBox.Show("Вы ничего не выбрали!");
                Environment.Exit(0);
            }

            //ser.IsEnabled = true;
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            music();
        }

        private void music() //основная функция, которая включает песни
        {
            try
            {
                timer.Stop();
                player.Volume = sl1.Value;

                if (k == false)
                {
                    player.Open(new Uri(files[i], UriKind.Relative));
                    str5 = files[i].Split('\\');
                    song.Content = str5[str5.Length - 1].Remove(str5[str5.Length-1].Length-4);
                    lb1.SelectedIndex = i;
                    button_r.Content = "Random: выкл";
                    player.Play();
                }
                else
                {
                    player.Open(new Uri(files[rnd2[i]], UriKind.Relative));
                    str5 = files[rnd2[i]].Split('\\');
                    song.Content = str5[str5.Length - 1].Remove(str5[str5.Length - 1].Length - 4);
                    lb1.SelectedIndex = rnd2[i];
                    button_r.Content = "Random: вкл";
                    player.Play();         
                }

                p1.Visibility = Visibility.Visible;
                r1.Visibility = Visibility.Hidden;
                timer.Start();
                sl2.Value = 0;          
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка! Возможно вы выбрали пустую папку или в ней нет музыки.");
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) //это нужно чтоб музыка не прерывалась
        {
            player.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //следующая песня
        {
            player.Close();
            if (i == files.Length - 1)
            {
                i = 0;

                ser.Text = "";
                lb4.Items.Clear();
                lb4.Visibility = Visibility.Hidden;
                lb1.Visibility = Visibility.Visible;

                music();
            }
            else
            {
                i++;

                ser.Text = "";
                lb4.Items.Clear();
                lb4.Visibility = Visibility.Hidden;
                lb1.Visibility = Visibility.Visible;

                music();
            }
        }

        private void timerTick(object sender, EventArgs e) //автоматический переход на следующий трек
        {
            try
            {
                /*if (k == false)
                {
                    lb1.SelectedIndex = i;
                }
                else
                {
                    lb1.SelectedIndex = rnd2[i];
                }*/

                sl2.Maximum = TimeSpan.Parse(player.NaturalDuration.ToString()).TotalSeconds;
                sl2.Value++; //он умный, за максимум выйти не может
                lb2.Content = player.Position.ToString().Remove(8);
                try
                {
                    lb3.Content = player.NaturalDuration.ToString().Remove(8);
                }
                catch (Exception)
                {
                    lb3.Content = player.NaturalDuration.ToString();
                }

                if (player.Position == player.NaturalDuration)
                {
                    player.Close();

                    if (i == files.Length - 1)
                    {
                        i = 0;
                        music();
                    }
                    else
                    {
                        i++;
                        music();
                    }
                }
                
            }
            catch (Exception)
            {
                player.Close();

                if (i == files.Length - 1)
                {
                    i = 0;
                    music();
                }
                else
                {
                    i++;
                    music();
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)   //пауза
        {
            player.Pause();
            timer.Stop();
            r1.Visibility = Visibility.Visible;
            p1.Visibility = Visibility.Hidden;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) //продолжить
        {
            player.Play();
            timer.Start();
            p1.Visibility = Visibility.Visible;
            r1.Visibility = Visibility.Hidden;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) //назад
        {
            player.Close();
            if (i == 0)
            {
                i = files.Length-1;

                ser.Text = "";
                lb4.Items.Clear();
                lb4.Visibility = Visibility.Hidden;
                lb1.Visibility = Visibility.Visible;

                music();
            }
            else
            {
                i--;

                ser.Text = "";
                lb4.Items.Clear();
                lb4.Visibility = Visibility.Hidden;
                lb1.Visibility = Visibility.Visible;

                music();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) //изменить папку
        {
            //lb1.Items.Clear();
            //player.Close();
            //timer.Stop();
            //button_r.Content = "Random: выкл";
            //i = 0;
            //k = false;
            
                System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
                //FBD.ShowDialog();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                y = 0;
                str10 = Directory.GetFiles(FBD.SelectedPath);
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg")
                    {

                        y++;
                    }

                }

                files = new string[y];
                y = 0;
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg")
                    {
                        files[y] = str10[v];
                        y++;
                    }

                }

                //files = Directory.GetFiles(FBD.SelectedPath);

                    lb1.Items.Clear();
                    player.Close();
                    timer.Stop();
                    button_r.Content = "Random: выкл";
                    i = 0;
                    k = false;
                song.Content = "";
                lb2.Content = "00:00:00";
                lb3.Content = "00:00:00";

                ser.Text = "";
                lb4.Items.Clear();
                lb4.Visibility = Visibility.Hidden;
                lb1.Visibility = Visibility.Visible;

                for (int b = 0; b < files.Length; b++)
                {
                        str5 = files[b].Split('\\');
                        lb1.Items.Add(str5[str5.Length - 1]);
                }
                    music();
                   
            }
            else
            {
                    MessageBox.Show("Вы ничего не выбрали!");
            }
            
            
        }

        private void sl1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //громкость
        {
            player.Volume = sl1.Value;
        }

        private void sl2_PreviewMouseUp(object sender, MouseButtonEventArgs e) //перемотка
        {
            try
            {
                player.Close();
                timer.Stop();

                player.Volume = sl1.Value;
                if (k == false)
                {
                    player.Open(new Uri(files[i], UriKind.Relative));
                    player.Position = TimeSpan.FromSeconds(sl2.Value);
                    player.Play();
                }
                else
                {
                    player.Open(new Uri(files[rnd2[i]], UriKind.Relative));
                    player.Position = TimeSpan.FromSeconds(sl2.Value);
                    player.Play();
                }
                p1.Visibility = Visibility.Visible;
                r1.Visibility = Visibility.Hidden;
                timer.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка! Возможно вы выбрали пустую папку или в ней нет музыки.");
            }
        }
        
        private void Button_Click_5(object sender, RoutedEventArgs e) //random
        {
            if (k == false)
            {
                k = true;
                rnd2 = new int[files.Length];
                for (int b = 0; b < files.Length; b++)
                {
                    rnd2[b] = -1;
                }

                for (int b = 0; b < files.Length; b++)
                {

                m:
                    j = rnd.Next(files.Length);
                    for (int b2 = 0; b2 < files.Length; b2++)
                    {
                        if (rnd2[b2] == j)
                            goto m;
                    }
                    rnd2[b] = j;
                }

                ser.Text = "";
                lb4.Items.Clear();
                lb4.Visibility = Visibility.Hidden;
                lb1.Visibility = Visibility.Visible;
            }
            else
            {
                k = false;
                i = rnd2[i];

                ser.Text = "";
                lb4.Items.Clear();
                lb4.Visibility = Visibility.Hidden;
                lb1.Visibility = Visibility.Visible;
            }
            music();
        }

        private void lb1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (k == false)
            {
                i = lb1.SelectedIndex;
            }
            else
            {
                i = lb1.SelectedIndex;
                k = false;
            }
            music();
        }

        private void ser_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) //строка поиска
        {
            try
            {
                string searchText = ser.Text.ToLower();

                if (searchText == "")
                {
                    lb4.Items.Clear();
                    lb4.Visibility = Visibility.Hidden;
                    lb1.Visibility = Visibility.Visible;
                }
                else
                {
                    lb4.Visibility = Visibility.Visible;
                    lb1.Visibility = Visibility.Hidden;
                    lb4.Items.Clear();
                    for (int b = 0; b < files.Length; b++)
                    {
                        if (files[b].ToLower().IndexOf(searchText) != -1)
                        {
                            str5 = files[b].ToString().Split('\\');
                            lb4.Items.Add(str5[str5.Length - 1]);
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void lb4_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string str11 = lb4.SelectedItem.ToString();
                for (int b = 0; b < files.Length; b++)
                {

                    if (files[b].ToString().Contains(str11))
                    {
                        i = b;
                        k = false;

                        ser.Text = "";
                        lb4.Items.Clear();
                        lb4.Visibility = Visibility.Hidden;
                        lb1.Visibility = Visibility.Visible;

                        music();
                    }
                }
            }
            catch (Exception) { }
        }
    }
}

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Linq;
//через добавить ссылку в обозревателе решений добавил Windows.Forms

namespace пробую
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer(); //таймер
        DispatcherTimer timer2 = new DispatcherTimer(); //таймер для анимации
        MediaPlayer player = new MediaPlayer(); //сам плеер
        string[] files; //сами треки
        int i; //какой по счету трек
        string[] str5; //название песни
        int[] rnd2; //массив для рандома
        Random rnd = new Random(); //сам рандом
        int j; //запоминает само рандомное число
        bool k = false; //определяет включен ли рандом
        string[] str10 = null; //нужно для проверки папки
        int y; //нужно для проверки папки
        int p; //ждем 5 секунд, чтобыпесня прогрузилась  
        bool r = false; //для конопки повтора
        int t; //для анимации
        bool move_sl2 = true; //для движения главного слайдера
        string selected_directory;

        public MainWindow()
        {
            InitializeComponent();  //изначальный выбор папки

            sl2.ApplyTemplate(); //для главного слайдера
            Thumb thumb2 = (sl2.Template.FindName("PART_Track", sl2) as Track).Thumb; 
            thumb2.MouseEnter += new MouseEventHandler(thumb_MouseEnter2);

            sl1.ApplyTemplate(); //для слайдера громкости
            Thumb thumb = (sl1.Template.FindName("PART_Track", sl1) as Track).Thumb;
            thumb.MouseEnter += new MouseEventHandler(thumb_MouseEnter);

            i = 0;
            button_r.Content = "Random: off";
            h:
            if (Properties.Settings.Default.papka3 == "") // если нет папки по умолчанию
            {
                System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
                if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    str10 = Directory.GetFiles(FBD.SelectedPath);
                    Window1 w1 = new Window1();
                    if (w1.ShowDialog() == true)
                    {
                        Properties.Settings.Default.papka3 = FBD.SelectedPath;
                        Properties.Settings.Default.Save();
                    }
                    selected_directory = FBD.SelectedPath;
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            else // если папка по умолчанию есть
            {
                try
                {
                    str10 = Directory.GetFiles(Properties.Settings.Default.papka3);
                    selected_directory = Properties.Settings.Default.papka3;
                }
                catch
                {
                    MessageBox.Show("Не удалось открыть папку по умолчанию. Возможно она удалена или переименована!");
                    Properties.Settings.Default.papka3 = "";
                    Properties.Settings.Default.Save();
                    goto h;
                }
            }

            y = 0;
            for (int v = 0; v < str10.Length; v++)
            {
                FileInfo fi = new FileInfo(str10[v]);
                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                {
                    y++;
                }
            }

            files = new string[y];
            y = 0;
            for (int v = 0; v < str10.Length; v++) // заполняется список песен
            {
                FileInfo fi = new FileInfo(str10[v]);
                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                {
                    files[y] = str10[v];
                    y++;
                }
            }

            for (int b = 0; b < files.Length; b++)
            {
                str5 = files[b].ToString().Split('\\');
                lb1.Items.Add(str5[str5.Length - 1]);
            }

            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);

            timer2.Tick += new EventHandler(timerTick2);
            timer2.Interval = new TimeSpan(0, 0, 5);
            music();
        }

        private void thumb_MouseEnter2(object sender, MouseEventArgs e) //для работы главного слайдера
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.MouseDevice.Captured == null)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
                args.RoutedEvent = MouseLeftButtonDownEvent;
                (sender as Thumb).RaiseEvent(args);
                move_sl2 = false;
            }
        }

        private void thumb_MouseEnter(object sender, MouseEventArgs e) //для работы слайдера громкости
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.MouseDevice.Captured == null)
            {
                MouseButtonEventArgs args = new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left);
                args.RoutedEvent = MouseLeftButtonDownEvent;
                (sender as Thumb).RaiseEvent(args);
            }
        }

        private void music() //основная функция, которая включает песни
        {
            try
            {
                p = 0;
                timer.Stop();
                timer2.Stop();
                player.Volume = sl1.Value / 200;
                t = 0;
                if (k == false) // если случайное произведение выключено
                {
                    player.Open(new Uri(files[i], UriKind.Relative));
                    str5 = files[i].Split('\\');
                    textbl.Text = str5[str5.Length - 1].Remove(str5[str5.Length - 1].Length - 4);
                    textbl.BeginAnimation(Canvas.LeftProperty, null); //остановка анимации
                    lb1.SelectedIndex = i;
                
                    button_r.Content = "Random: выкл";
                    player.Play();
                }
                else //если случайное воспроизведение включено
                {
                    player.Open(new Uri(files[rnd2[i]], UriKind.Relative));
                    str5 = files[rnd2[i]].Split('\\');
                    textbl.Text = str5[str5.Length - 1].Remove(str5[str5.Length - 1].Length - 4);
                    textbl.BeginAnimation(Canvas.LeftProperty, null);
                    lb1.SelectedIndex = rnd2[i];
                    button_r.Content = "Random: вкл";
                    player.Play();         
                }

                p1.Visibility = Visibility.Visible;
                r1.Visibility = Visibility.Hidden;          
                sl2.Value = 0;
                timer.Start();
                timer2.Start();
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
            if (i >= files.Length - 1)
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

        private void timerTick(object sender, EventArgs e) //автоматический переход на следующий трек
        {
            try
            {
                sl2.Maximum = TimeSpan.Parse(player.NaturalDuration.ToString()).TotalSeconds;
                try
                {
                    lb3.Content = player.NaturalDuration.ToString().Remove(8);
                }
                catch (Exception)
                {
                    lb3.Content = player.NaturalDuration.ToString();
                }
                sl2_move();
                lb2.Content = player.Position.ToString().Remove(8);
                
                if (player.Position == player.NaturalDuration) // если песня закончилась
                {
                    if (r == false)
                    {
                        player.Close();

                        if (i >= files.Length - 1)
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
                    else
                    {
                        player.Close();
                        music();
                    }
                }
            }
            catch (Exception) // если плеер 5 секунд не может воспроизвести песню, то включает следующую
            {
                if (p != 5)
                    p++;
                else
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
                music();
            }
            else
            {
                i--;
                music();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e) //изменить папку
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                y = 0;
                str10 = Directory.GetFiles(FBD.SelectedPath);
                selected_directory = FBD.SelectedPath;
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                    {
                        y++;
                    }
                }

                files = new string[y];
                y = 0;
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                    {
                        files[y] = str10[v];
                        y++;
                    }
                }

                lb1.Items.Clear();
                player.Close();
                timer.Stop();
                button_r.Content = "Random: выкл";
                i = 0;
                k = false;
                textbl.Text = "";
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
        }

        private void sl1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) //громкость
        {
            player.Volume = sl1.Value / 200;
        }

        private void sl2_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //перемотка
        {
            try
            {
                move_sl2 = true;
                player.Volume = sl1.Value / 200;
                player.Position = TimeSpan.FromSeconds(sl2.Value);
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка! Возможно вы выбрали пустую папку или в ней нет музыки.");
            }
        }

        private void Random()
        {
            if (k == true)
            {
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

                for (int b2 = 0; b2 < files.Length; b2++)
                {
                    if (rnd2[b2] == i)
                    {
                        i = b2;
                        break;
                    }
                }

                button_r.Content = "Random: вкл";
            }
            else
            {
                try
                {
                    i = rnd2[i];
                    button_r.Content = "Random: выкл";
                }
                catch
                {
                    MessageBox.Show("Ошибка! Возможно вы выбрали пустую папку или в ней нет музыки.");
                }
                
            }
        }
        
        private void Button_Click_5(object sender, RoutedEventArgs e) //случайное воспроизведение
        {
            if (k == false)
            {
                k = true;
                Random();
            }
            else
            {
                k = false;
                Random();
            }     
        }

        private void lb1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) //выбор песни
        {
            if (k == false)
            {
                i = lb1.SelectedIndex;
            }
            else
            {
                i = lb1.SelectedIndex;
                Random();
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
                    src_button.Visibility = Visibility.Hidden;
                    lb4.Items.Clear();
                    lb4.Visibility = Visibility.Hidden;
                    lb1.Visibility = Visibility.Visible;
                }
                else
                {
                    src_button.Visibility = Visibility.Visible;
                    lb4.Visibility = Visibility.Visible;
                    lb1.Visibility = Visibility.Hidden;
                    lb4.Items.Clear();
                    for (int b = 0; b < files.Length; b++)
                    {
                        if (files[b].ToString().ToLower().Contains(searchText))
                        {
                            str5 = files[b].ToString().Split('\\');
                            lb4.Items.Add(str5[str5.Length - 1]);
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void lb4_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) //выбор песни из поиска
        {
            try
            {
                string str11 = lb4.SelectedItem.ToString();
                for (int b = 0; b < files.Length; b++)
                {
                    if (files[b].ToString().Contains(str11))
                    {
                        i = b;
                        if (k == true)
                        {
                            Random();
                        }
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

        private void MenuItem_Click(object sender, RoutedEventArgs e) //файл, о программе
        {
            Window2 w2 = new Window2();
            w2.ShowDialog();
        }

        private void lb4_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) //запрещаем пкм в списке песен поиск
        {
            e.Handled = true;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) //удалить папку по умолчанию
        {
            Properties.Settings.Default.papka3 = "";
            Properties.Settings.Default.Save();
            MessageBox.Show("Удалено!");
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) //изменение папки по умолчанию
        {
            System.Windows.Forms.FolderBrowserDialog FBD = new System.Windows.Forms.FolderBrowserDialog();
            if (FBD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.papka3 = FBD.SelectedPath;
                Properties.Settings.Default.Save();
                MessageBox.Show("Изменено!");
            }
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) //текущая папка по умолчанию
        {
            if(Properties.Settings.Default.papka3!="")
                MessageBox.Show(Properties.Settings.Default.papka3);
            else
                MessageBox.Show("Нет папки по умолчанию.");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e) //конпка повтора
        {
            r = true;
            rep_off.Visibility = Visibility.Hidden;
            rep_on.Visibility = Visibility.Visible;    
        }

        private void rep_on_Click(object sender, RoutedEventArgs e) //кнопка повтора
        {
            r = false;
            rep_off.Visibility = Visibility.Visible;
            rep_on.Visibility = Visibility.Hidden;
        }

        private void timerTick2(object sender, EventArgs e) //анимация
        {
            if (t == 0)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = 0;
                doubleAnimation.To = str5[str5.Length - 1].Remove(str5[str5.Length - 1].Length - 4).Length * (-1) * 11;
                doubleAnimation.RepeatBehavior = new RepeatBehavior(1);
                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(15));
                textbl.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
                t++;
            }
            else if (t == 3)
            {
                DoubleAnimation doubleAnimation = new DoubleAnimation();
                doubleAnimation.From = 575;
                doubleAnimation.To = 0;
                doubleAnimation.RepeatBehavior = new RepeatBehavior(1);
                doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(15));
                textbl.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
                t++;
            }
            else if (t == 6)
            {
                t = 0;
            }
            else
            {
                t++;
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) //контекст меню свойства песни 
        {
            try
            {
                var index = lb1.SelectedIndex;

                if (k == false)
                {
                    lb1.SelectedIndex = i;
                }
                else
                {
                    lb1.SelectedIndex = rnd2[i];
                }

                FileInfo fi = new FileInfo(files[index]);
                double l = fi.Length;
                l = Math.Round(l / 1048576, 2);

                MessageBox.Show($"Название: {fi.Name}\nРазмер: {l} мб\nРасположение: {fi.Directory}\\{fi.Name}", "Свойства");
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
            
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e) //контекст меню удаления песни 
        {
            try
            {
                var index = lb1.SelectedIndex;
                FileInfo fi2 = new FileInfo(files[index]);
                string dir = fi2.Directory.ToString();

                if (MessageBox.Show($"Вы точно хотите удалить с компьютера {fi2.Name}?",
                        "Удаление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    if (k == false)
                    {
                        if (i != index)
                        {
                            fi2.Delete();

                            y = 0;
                            str10 = Directory.GetFiles(dir);
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    y++;
                                }
                            }

                            files = new string[y];
                            y = 0;
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    files[y] = str10[v];
                                    y++;
                                }
                            }

                            lb1.Items.Clear();
                            button_r.Content = "Random: выкл";
                            k = false;
                            ser.Text = "";
                            lb4.Items.Clear();
                            lb4.Visibility = Visibility.Hidden;
                            lb1.Visibility = Visibility.Visible;

                            for (int b = 0; b < files.Length; b++)
                            {
                                str5 = files[b].Split('\\');
                                lb1.Items.Add(str5[str5.Length - 1]);
                            }

                            if (index < i)
                            {
                                i = i - 1;
                                lb1.SelectedIndex = i;
                            }
                            else
                            {
                                lb1.SelectedIndex = i;
                            }

                            MessageBox.Show("Удалено!");
                        }

                        else
                        {
                            player.Close();
                            timer.Stop();

                            fi2.Delete();

                            y = 0;
                            str10 = Directory.GetFiles(dir);
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    y++;
                                }
                            }

                            files = new string[y];
                            y = 0;
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    files[y] = str10[v];
                                    y++;
                                }
                            }

                            lb1.Items.Clear();
                            button_r.Content = "Random: выкл";
                            k = false;
                            textbl.Text = "";
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

                            i = i - 1;

                            Button_Click(sender, e);

                            MessageBox.Show("Удалено!");
                        }
                    }
                    else
                    {
                        if (index != rnd2[i])
                        {
                            fi2.Delete();

                            y = 0;
                            str10 = Directory.GetFiles(dir);
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    y++;
                                }
                            }

                            files = new string[y];
                            y = 0;
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    files[y] = str10[v];
                                    y++;
                                }
                            }

                            lb1.Items.Clear();
                            button_r.Content = "Random: выкл";
                            k = false;
                            ser.Text = "";
                            lb4.Items.Clear();
                            lb4.Visibility = Visibility.Hidden;
                            lb1.Visibility = Visibility.Visible;

                            for (int b = 0; b < files.Length; b++)
                            {
                                str5 = files[b].Split('\\');
                                lb1.Items.Add(str5[str5.Length - 1]);
                            }

                            i = rnd2[i];

                            if (index < i)
                            {
                                i = i - 1;
                                lb1.SelectedIndex = i;
                            }
                            else
                            {
                                lb1.SelectedIndex = i;
                            }

                            MessageBox.Show("Удалено!");
                        }

                        else
                        {
                            player.Close();
                            timer.Stop();

                            fi2.Delete();

                            y = 0;
                            str10 = Directory.GetFiles(dir);
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    y++;
                                }
                            }

                            files = new string[y];
                            y = 0;
                            for (int v = 0; v < str10.Length; v++)
                            {
                                FileInfo fi = new FileInfo(str10[v]);
                                if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                                {
                                    files[y] = str10[v];
                                    y++;
                                }
                            }

                            lb1.Items.Clear();
                            button_r.Content = "Random: выкл";
                            k = false;
                            textbl.Text = "";
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
                            i = i - 1;

                            Button_Click(sender, e);

                            MessageBox.Show("Удалено!");
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }

        private void lb1_ContextMenuClosing(object sender, ContextMenuEventArgs e) //закрытие контекстного меню
        {
            try
            {
                if (k == false)
                {
                    lb1.SelectedIndex = i;
                }
                else
                {
                    lb1.SelectedIndex = rnd2[i];
                }
            }
            catch
            {
                MessageBox.Show("Ошибка!");
            }
        }

        private void sl2_move() // движение главного слайдера
        {
            if (move_sl2 == true)
            {
                sl2.Value = TimeSpan.Parse(player.Position.ToString()).TotalSeconds;
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e) //кнопка отчистки строки поиска
        {
            src_button.Visibility = Visibility.Hidden;
            ser.Text = "";
            lb4.Items.Clear();
            lb4.Visibility = Visibility.Hidden;
            lb1.Visibility = Visibility.Visible;
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e) //сортировка по дате добавления
        {
            try
            {
                var str11 = new DirectoryInfo(selected_directory).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
                // получаем все файлы в папке, сразу отсортированные
                string song_now = "";

                if(k == false)
                {
                    song_now = files[i];
                }
                else
                {
                    song_now = files[rnd2[i]];
                }
                

                int v5 = 0;
                foreach (var f in str11)
                {
                    str10[v5] = f.FullName; // перезаполняем массивы с песнями
                    v5++;
                }

                y = 0;
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                    {
                        y++;
                    }
                }

                files = new string[y];
                y = 0;
                for (int v = 0; v < str10.Length; v++) // заполняется список песен
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                    {
                        files[y] = str10[v];
                        y++;
                    }
                }

                lb1.Items.Clear();
                for (int b = 0; b < files.Length; b++)
                {
                    str5 = files[b].ToString().Split('\\');
                    lb1.Items.Add(str5[str5.Length - 1]);
                }

                for(int b = 0; b < files.Length; b++)
                {
                    if (files[b] == song_now)
                    {
                        if (k == false)
                        {
                            i = b;
                            lb1.SelectedIndex = i;
                            break;
                        }
                        else
                        {
                            for (int b2 = 0; b2 < files.Length; b2++)
                            {
                                if (rnd2[b2] == b)
                                {
                                    i = b2;
                                    lb1.SelectedIndex = rnd2[b2];
                                    break;
                                }
                            }
                        }


                    }
                }

            }
            catch
            {
                MessageBox.Show("Ошибка! Возможно вы выбрали пустую папку или в ней нет музыки.");
                //MessageBox.Show(ex.Message);
            }   
        }

        private void MenuItem_Click_7(object sender, RoutedEventArgs e) //сортировка по названию
        {
            try 
            {
                str10 = Directory.GetFiles(selected_directory);

                string song_now = "";

                if (k == false)
                {
                    song_now = files[i];
                }
                else
                {
                    song_now = files[rnd2[i]];
                }

                y = 0;
                for (int v = 0; v < str10.Length; v++)
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                    {
                        y++;
                    }
                }

                files = new string[y];
                y = 0;
                for (int v = 0; v < str10.Length; v++) // заполняется список песен
                {
                    FileInfo fi = new FileInfo(str10[v]);
                    if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".flac" || fi.Extension == ".m4a" || fi.Extension == ".m4v" || fi.Extension == ".mp4v" || fi.Extension == ".mpg" || fi.Extension == ".mkv")
                    {
                        files[y] = str10[v];
                        y++;
                    }
                }

                lb1.Items.Clear();
                for (int b = 0; b < files.Length; b++)
                {
                    str5 = files[b].ToString().Split('\\');
                    lb1.Items.Add(str5[str5.Length - 1]);
                }

                for (int b = 0; b < files.Length; b++)
                {
                    if (files[b] == song_now)
                    {
                        if (k == false)
                        {
                            i = b;
                            lb1.SelectedIndex = i;
                            break;
                        }
                        else
                        {
                            for (int b2 = 0; b2 < files.Length; b2++)
                            {
                                if (rnd2[b2] == b)
                                {
                                    i = b2;
                                    lb1.SelectedIndex = rnd2[b2];
                                    break;
                                }
                            }
                        }
                        

                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка! Возможно вы выбрали пустую папку или в ней нет музыки.");
            }
            
        }
    }
}

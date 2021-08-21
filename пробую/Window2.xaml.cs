using System.Windows;

namespace пробую
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            label1.Content = "Version " + Properties.Settings.Default.version; 
        }
    }
}

 
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls; 

namespace Localization.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        { 
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
         

       
    }
}
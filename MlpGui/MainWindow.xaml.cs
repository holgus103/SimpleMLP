using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataParser;
using SimpleMLP;
using MaterialDesignThemes.Wpf;

namespace MlpGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Network network;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                showWaitingDialog();
                // TODO: ask user for classes count
                var trainSet = CsvParser.Parse(dlg.FileName, 3);
                // TODO: permit user to model network and edit parameters
                this.network = new Network(2, 4, 3, 2, 1);
                var t = new Task(() =>
                {
                    this.network.Train(trainSet, 1000);
                    // TODO: update GUI
                });
                t.Start();
            }
        }

        private static void showWaitingDialog()
        {
            var t = new TextBox();
            t.Text = "Please wait, the model is being trained!";
            DialogHost.Show(t);
        }
    }
}

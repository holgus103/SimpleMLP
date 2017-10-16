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
using RDotNet;

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

        private void trainBtnClick(object sender, RoutedEventArgs e)
        {
            var trainSet = this.getSetFromFile();
            if (trainSet == null) return;
            // TODO: permit user to model network and edit parameters
            this.network = new Network(2, 4, 3, 2, 1);
            var t = new Task(() =>
            {
                this.network.Train(trainSet, 1000);
                    // TODO: update GUI
            });
            t.Start();

        }

        private static void showWaitingDialog()
        {
            var t = new TextBox();
            t.Text = "Please wait, the model is being trained!";
            DialogHost.Show(t);
        }

        private void testBtnClick(object sender, RoutedEventArgs e)
        {
            var testSet = this.getSetFromFile();
            var resX = new List<double>(testSet.Count);
            var resY = new List<double>(testSet.Count);
            // TODO: classification
            this.drawChart
            (
                testSet.GroupBy(x => x.Item2.IndexOf(1.0)),
                testSet.Min(x => x.Item1[0]),
                testSet.Max(x => x.Item1[0]),
                testSet.Min(x => x.Item1[1]),
                testSet.Max(x => x.Item1[1])
            );

        }

        private List<Tuple<List<double>, List<double>>> getSetFromFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                //showWaitingDialog();
                // TODO: ask user for classes count
                return CsvParser.Parse(dlg.FileName, 3);
            }
            return null;
        }

        private void drawChart(IEnumerable<IGrouping<int, Tuple<List<double>, List<double>>>> data, double lowX, double highX, double lowY, double highY)
        {
            if (data.Count() == 0) return;
            var colorIndex = 0;
            var colors = new[] { "red", "green", "blue", "yellow" };
            REngine.SetEnvironmentVariables();
            var engine = REngine.GetInstance();
            engine.SetSymbol("lowX", engine.CreateNumeric(lowX));
            engine.SetSymbol("highX", engine.CreateNumeric(highX));
            engine.SetSymbol("lowY", engine.CreateNumeric(lowY));
            engine.SetSymbol("highY", engine.CreateNumeric(highY));
            engine.Evaluate($"plot(1, type=\"n\", xlab=\"X\", ylab=\"Y\", xlim=c(lowX, highX), ylim=c(lowY, highY))");
            foreach (var val in data)
            {
                // get xs
                engine.SetSymbol("x", engine.CreateNumericVector(val.Select(e => e.Item1[0])));
                // get ys
                engine.SetSymbol("y", engine.CreateNumericVector(val.Select(e => e.Item1[1])));
                engine.SetSymbol("color", engine.CreateCharacterVector(new[] { colors[colorIndex % colors.Length] }));
                engine.Evaluate("points(x, y, col = color)");
                colorIndex++;
            }
        }
    }
}

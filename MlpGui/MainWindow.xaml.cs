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
using System.Threading;

namespace MlpGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Network network;
        private REngine engine;

        public MainWindow()
        {
            this.InitializeComponent();
            REngine.SetEnvironmentVariables();
            this.engine = REngine.GetInstance();
        }

        ~MainWindow()
        {
            this.engine.Dispose();
        }

        private void trainBtnClick(object sender, RoutedEventArgs e)
        {
            double eta;
            double alpha;
            int hiddenNeurons;
            int iterations;
            int classesCount;
            int attributesCount;
            if (!Double.TryParse(this.EtaTb.Text, out eta)) return;
            if (!Double.TryParse(this.AlphaTb.Text, out alpha)) return;
            if (!Int32.TryParse(this.HiddenNeuronsTb.Text, out hiddenNeurons)) return;
            if (!Int32.TryParse(this.IterationsTb.Text, out iterations)) return;
            var trainSet = this.getSetFromFile(out classesCount, out attributesCount);
            if (trainSet == null) return;
            // TODO: permit user to model network and edit parameters
            this.network = new Network(attributesCount, hiddenNeurons, classesCount, eta, alpha);
            var tb = showWaitingDialog();
            Task.Run(() =>
               {
                   var errors = this.network.Train(trainSet, iterations);

                   tb.Dispatcher.Invoke(() =>
                   {
                       this.drawChart
                       (
                           new List<IEnumerable<Tuple<double, double>>>() { Enumerable.Range(1, iterations).Zip(errors, (it, val) => new Tuple<double, double>((double)it, val)) },
                           1,
                           iterations,
                           errors.Min(),
                           errors.Max()
                       );
                       DialogHost.CloseDialogCommand.Execute(null, tb);
                   });
               });

            // TODO: update GUI


        }

        private static TextBox showWaitingDialog()
        {
            var t = new TextBox();
            t.Text = "Please wait, the model is being trained!";
            DialogHost.Show(t);
            return t;
        }

        private void testBtnClick(object sender, RoutedEventArgs e)
        {
            int classesCount, attributesCount;
            var testSet = this.getSetFromFile(out classesCount, out attributesCount);
            var resX = new List<double>(testSet.Count);
            var resY = new List<double>(testSet.Count);

            var lowX = testSet.Min(x => x.Item1[0]);
            var highX = testSet.Max(x => x.Item1[0]);
            var lowY = testSet.Min(x => x.Item1[1]);
            var highY = testSet.Max(x => x.Item1[1]);
            this.drawChart
            (
                testSet.GroupBy(x => x.Item2.IndexOf(1.0)).Select(g => g.Select(x => new Tuple<double, double>(x.Item1[0], x.Item1[1]))),
                lowX,
                highX,
                lowY,
                highY
            );

            var data = testSet.Select(x => new {x = x.Item1[0], y = x.Item1[1], cls = this.network.GetClass(this.network.Predict(x.Item1))});
            this.drawChart(
                data.GroupBy(x => x.cls)
                    .Select(x => x.Select(y => new Tuple<double, double>(y.x, y.y))),
                lowX,
                highX,
                lowY,
                highY
            );

        }

        private List<Tuple<List<double>, List<double>>> getSetFromFile(out int classesCount, out int attributesCount)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                //showWaitingDialog();
                // TODO: ask user for classes count
                return CsvParser.Parse(dlg.FileName, out classesCount, out attributesCount);
            }
            attributesCount = classesCount = 0;
            return null;
        }

        private void drawChart(IEnumerable<IEnumerable<Tuple<double, double>>> data, double lowX, double highX, double lowY, double highY)
        {
            if (data.Count() == 0) return;
            var colorIndex = 0;
            var colors = new[] { "red", "green", "blue", "yellow" };
            this.engine.Evaluate("dev.new()");
            this.engine.SetSymbol("lowX", this.engine.CreateNumeric(lowX));
            this.engine.SetSymbol("highX", this.engine.CreateNumeric(highX));
            this.engine.SetSymbol("lowY", this.engine.CreateNumeric(lowY));
            this.engine.SetSymbol("highY", this.engine.CreateNumeric(highY));
            this.engine.Evaluate($"plot(1, type=\"n\", xlab=\"X\", ylab=\"Y\", xlim=c(lowX, highX), ylim=c(lowY, highY))");
            foreach (var val in data)
            {
                // get xs
                engine.SetSymbol("x", engine.CreateNumericVector(val.Select(e => e.Item1)));
                // get ys
                engine.SetSymbol("y", engine.CreateNumericVector(val.Select(e => e.Item2)));
                engine.SetSymbol("color", engine.CreateCharacterVector(new[] { colors[colorIndex % colors.Length] }));
                engine.Evaluate("points(x, y, col = color)");
                colorIndex++;
            }
        }
    }
}

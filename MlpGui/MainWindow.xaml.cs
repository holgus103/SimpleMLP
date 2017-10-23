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
        NetworkBase network;
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

        private void TrainBtnClick(object sender, RoutedEventArgs e)
        {
            double learningRate;
            double momentum;
            int iterations;
            int classesCount;
            int attributesCount;
            if (!Double.TryParse(this.EtaTb.Text, out learningRate)) return;
            if (!Double.TryParse(this.AlphaTb.Text, out momentum)) return;
            // parse neuron counts separated by commas
            var neurons = this.HiddenNeuronsTb.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
            {
                var res = 0;
                if (!Int32.TryParse(x, out res))
                {
                    return 0;
                }
                return res;
            }).ToList();

            if(neurons.Any(x => x <= 0))
            {
                return;
            }

            if (!Int32.TryParse(this.IterationsTb.Text, out iterations)) return;
            var trainSet = this.GetSetFromFile(out classesCount, out attributesCount);
            if (trainSet == null) return;
            // TODO: permit user to model network and edit parameters
            this.network = new EncogNetwork().BuildNetwork(attributesCount, neurons, classesCount, learningRate, momentum, new ReLUFunction());
            var tb = ShowWaitingDialog();
            Task.Run(() =>
               {
                   var errors = this.network.Train(trainSet, iterations);

                   tb.Dispatcher.Invoke(() =>
                   {
                       this.DrawChart
                       (
                           "Error function",
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

        private static TextBox ShowWaitingDialog()
        {
            var t = new TextBox();
            t.Text = "Please wait, the model is being trained!";
            DialogHost.Show(t);
            return t;
        }

        private void TestBtnClick(object sender, RoutedEventArgs e)
        {
            int classesCount, attributesCount;
            var testSet = this.GetSetFromFile(out classesCount, out attributesCount);
            if (testSet is null)
                return;
            var resX = new List<double>(testSet.Count);
            var resY = new List<double>(testSet.Count);

            var lowX = testSet.Min(x => x.Item1[0]);
            var highX = testSet.Max(x => x.Item1[0]);
            var lowY = testSet.Min(x => x.Item1[1]);
            var highY = testSet.Max(x => x.Item1[1]);
            this.DrawChart
            (
                "Correct results",
                testSet.GroupBy(x => x.Item2.IndexOf(1.0)).Select(g => g.Select(x => new Tuple<double, double>(x.Item1[0], x.Item1[1]))),
                lowX,
                highX,
                lowY,
                highY
            );

            var data = testSet.Select(x => new { x = x.Item1[0], y = x.Item1[1], cls = NetworkBase.GetClass(x.Item2), resCls = NetworkBase.GetClass(this.network.Predict(x.Item1)) });
            var acc = data.Count(x => x.cls == x.resCls) / data.Count() * 100;
            this.AccLbl.Content = acc.ToString();
            this.DrawChart(
                "Trained results",
                data.GroupBy(x => x.resCls)
                    .Select(x => x.Select(y => new Tuple<double, double>(y.x, y.y))),
                lowX,
                highX,
                lowY,
                highY
            );

        }

        private List<Tuple<List<double>, List<double>>> GetSetFromFile(out int classesCount, out int attributesCount)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                //showWaitingDialog();
                // TODO: ask user for classes count
                return CsvParser.Parse(dlg.FileName, out classesCount, out attributesCount).NormalizedData;
            }
            attributesCount = classesCount = 0;
            return null;
        }

        private void DrawChart(string chartName, IEnumerable<IEnumerable<Tuple<double, double>>> data, double lowX, double highX, double lowY, double highY)
        {
            if (data.Count() == 0) return;
            var colorIndex = 0;
            var colors = new[] { "red", "green", "blue", "yellow" };
            this.engine.Evaluate("dev.new()");
            this.engine.SetSymbol("lowX", this.engine.CreateNumeric(lowX));
            //this.engine.SetSymbol("main", this.engine.CreateCharacter("Chart number1"));
            this.engine.SetSymbol("highX", this.engine.CreateNumeric(highX));
            this.engine.SetSymbol("lowY", this.engine.CreateNumeric(lowY));
            this.engine.SetSymbol("highY", this.engine.CreateNumeric(highY));
            this.engine.Evaluate($"plot(1, type=\"n\", xlab=\"X\", ylab=\"Y\", xlim=c(lowX, highX), ylim=c(lowY, highY), main='{chartName}')");
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

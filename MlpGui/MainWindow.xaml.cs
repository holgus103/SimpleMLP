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
using System.IO;
using System.Globalization;

namespace MlpGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetworkBase network;
        private REngine engine;
        private INetwork networkType;

        public MainWindow()
        {
            this.InitializeComponent();
            REngine.SetEnvironmentVariables();
            this.engine = REngine.GetInstance();
            TypeComboBox.Items.Add("Classification");
            TypeComboBox.Items.Add("Regression");
            TypeComboBox.SelectedValue = "Regression";
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
            if (TypeComboBox.SelectedItem is null) return;
            if (TypeComboBox.SelectedValue.ToString() == "Classification")
                networkType = new ClassificationNetwork();
            else
                networkType = new RegressionNetwork();
            int classesCount = 0;
            int attributesCount = 0;
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

            if (neurons.Any(x => x <= 0))
            {
                return;
            }

            if (!Int32.TryParse(this.IterationsTb.Text, out iterations)) return;
            var trainSet = this.GetSetFromFile(ref classesCount, ref attributesCount)?.NormalizedData;
            if (trainSet == null) return;
            // TODO: permit user to model network and edit parameters
            int outputNeurons = classesCount;
            IActivation activationFunction = new SigmoidFunction();
            if (networkType is RegressionNetwork)
            {
                outputNeurons = 1;
                activationFunction = new IdentityFunction();
            }
            this.network = new Network().BuildNetwork(attributesCount, neurons, outputNeurons, learningRate, momentum, activationFunction, networkType);

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
            if (this.network == null) return;
            int classesCount = 0, attributesCount = 0;
            var testSet = this.GetSetFromFile(ref classesCount, ref attributesCount)?.NormalizedData;
            if (testSet is null)
                return;
            var resX = new List<double>(testSet.Count);
            var resY = new List<double>(testSet.Count);

            var lowX = testSet.Min(x => x.Item1[0]);
            var highX = testSet.Max(x => x.Item1[0]);
            var lowY = 0.0;
            var highY = 0.0;
            if(networkType is ClassificationNetwork)
            {
                lowY = testSet.Min(x => x.Item1[1]);
                highY = testSet.Max(x => x.Item1[1]);
            }
            else if(networkType is RegressionNetwork)
            {
                lowY = testSet.Min(x => x.Item2[0]);
                highY = testSet.Max(x => x.Item2[0]);
            }
            if(networkType is ClassificationNetwork)
            {
                this.DrawChart
                (
                    "Correct results",
                    testSet.GroupBy(x => x.Item2.IndexOf(1.0)).Select(g => g.Select(x => new Tuple<double, double>(x.Item1[0], x.Item1[1]))),
                    lowX,
                    highX,
                    lowY,
                    highY
                );
            }
            else
            {
                this.DrawChart
                (
                    "Correct results",
                    testSet.GroupBy(x => x.Item2.IndexOf(1.0)).Select(g => g.Select(x => new Tuple<double, double>(x.Item1[0], x.Item2[0]))),
                    lowX,
                    highX,
                    lowY,
                    highY
                );
            }

            var data = testSet.Select(x => new { x = x.Item1[0], y = x.Item1[1], cls = NetworkBase.GetClass(x.Item2), resCls = NetworkBase.GetClass(this.network.Predict(x.Item1)) });
            if(networkType is RegressionNetwork)
            {
                data = testSet.Select(x => new { x = x.Item1[0], y = this.network.Predict(x.Item1).First(), cls = NetworkBase.GetClass(x.Item2), resCls = NetworkBase.GetClass(this.network.Predict(x.Item2)) });
            }
            var acc = data.Count(x => x.cls == x.resCls) / (double)data.Count() * 100;
            this.DrawChart(
                "Trained results",
                data.GroupBy(x => x.resCls)
                    .Select(x => x.Select(y => new Tuple<double, double>(y.x, y.y))),
                lowX,
                highX,
                lowY,
                highY
            );
            this.AccLbl.Content = acc.ToString();

        }

        private CsvData GetSetFromFile(ref int classesCount, ref int attributesCount, bool attributesOnly = false)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == true)
            {
                //showWaitingDialog();
                // TODO: ask user for classes count

                //return CsvParser.Parse(dlg.FileName, out classesCount, out attributesCount).NormalizedData;
                return networkType.Parse(dlg.FileName, ref classesCount, ref attributesCount, attributesOnly);
                //return CsvParser.Parse(dlg.FileName, ref classesCount, ref attributesCount, attributesOnly);

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

        private void TestBtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (this.network == null) return;
            var classesCount = 0;
            var attributesCount = 0;
            var csvData = this.GetSetFromFile(ref classesCount, ref attributesCount, true);
            if (csvData == null) return;
            var dataSet = csvData.NormalizedData;

            var resX = new List<double>(dataSet.Count);
            var resY = new List<double>(dataSet.Count);

            var lowX = dataSet.Min(x => x.Item1[0]);
            var highX = dataSet.Max(x => x.Item1[0]);
            var lowY = dataSet.Min(x => x.Item1[1]);
            var highY = dataSet.Max(x => x.Item1[1]);

            var data = dataSet.Select(x => new { x = x.Item1[0], y = x.Item1[1], resCls = NetworkBase.GetClass(this.network.Predict(x.Item1)) });

            this.DrawChart(
                "Trained results",
                data.GroupBy(x => x.resCls)
                    .Select(x => x.Select(y => new Tuple<double, double>(y.x, y.y))),
                lowX,
                highX,
                lowY,
                highY
            );

            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "csv files (*.csv)|*.txt|All files (*.*)|*.*";
            var c = new CultureInfo("en-US");
            if (saveFileDlg.ShowDialog() == true)
            {
                File.WriteAllLines
                (
                    saveFileDlg.FileName,
                    data.Zip(csvData.RawData, (f, s) => $"{s.Item1[0].ToString(c)},{s.Item1[1].ToString(c)},{f.resCls + 1}")
                );
            }
        }
    }
}

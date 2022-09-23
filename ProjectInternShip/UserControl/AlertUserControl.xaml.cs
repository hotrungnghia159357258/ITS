using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ProjectInternShip.DBContexts;
using ProjectInternShip.Models;
using System.ComponentModel;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace ProjectInternShip.AlertUC
{
    /// <summary>
    /// Interaction logic for AlertUserControl.xaml
    /// </summary>
    public partial class AlertUserControl : UserControl
    {
        private data_sensor_DBcontexts infor;
        private static BackgroundWorker backgroundWorker;
        public string warningMess = "Humidity is good";

        //private string _operator;
        //private string _aggregationType;
        //private int _thresholdValue;
        //private string _period;
        //private string _frequencyOfEvaluation;

        public AlertUserControl()
        {
            InitializeComponent();
            infor = new data_sensor_DBcontexts();
            initParam();
            createPreview();
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

            backgroundWorker.RunWorkerAsync();

        }

        #region initialize configuration parameter of alert logic
        public void initParam()
        {
            var number = (from param in infor.parameters
                          select param).Count();

            if (number <= 0)
            {
                infor.parameters.Add(new param
                {
                    ID = 1,
                    Operator = "Greater than",
                    AggregationType = "Average",
                    ThresholdValue = 35,
                    Period = "3 minutes",
                    FrequencyOfEvaluation = "Every 1 minute"
                });
                infor.SaveChanges();
            }
        }
        #endregion

        #region check alert 
        static void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            data_sensor_DBcontexts information = new data_sensor_DBcontexts();

            string _operator;
            string _aggregationType;
            int _thresholdValue;
            string _period;
            string _frequencyOfEvaluation;

            #region get parameter from database
            void getParam()
            {
                var p = (from param in information.parameters
                         orderby param.ID descending
                         select param).First();
                _operator = p.Operator;
                _aggregationType = p.AggregationType;
                _thresholdValue = p.ThresholdValue;
                _period = p.Period;
                _frequencyOfEvaluation = p.FrequencyOfEvaluation;
            }
            #endregion

            //convert _period to number of records 
            int convertPeriod(string peri)
            {
                int result = 0;
                switch (_period)
                {
                    case "10 seconds":
                        result = 10;
                        break;
                    case "1 minute":
                        result = 60;
                        break;
                    case "3 minutes":
                        result = 180;
                        break;
                    case "5 minutes":
                        result = 300;
                        break;
                    case "10 minutes":
                        result = 600;
                        break;
                }
                return result;
            }

            //convert _frequancyOfEvaluation to time(s)
            int convertFrequency(string f)
            {
                int result = 1;
                switch (f)
                {
                    case "Every 10 seconds":
                        result = 10;
                        break;
                    case "Every 30 seconds":
                        result = 30;
                        break;
                    case "Every 1 minute":
                        result = 60;
                        break;
                    case "Every 2 minutes":
                        result = 120;
                        break;
                    case "Every 3 minutes":
                        result = 180;
                        break;
                }
                return result;
            }

            bool checkAlert()
            {
                getParam();

                var p = (from d in information.data
                         orderby d.time descending
                         select d).ToList();

                int record = convertPeriod(_period);

                List<data_sensor> pNew;
                if (p.Count() > record)
                {
                    pNew = p.GetRange(0, record);
                }
                else
                {
                    pNew = p;
                }

                double sum = 0;

                foreach (var re in pNew)
                {
                    sum += re.sensor_value_1;
                }

                switch (_aggregationType)
                {
                    case "Average":
                        if (sum / pNew.Count() > _thresholdValue && _operator == "Greater than"
                            || sum / pNew.Count() < _thresholdValue && _operator == "Less than")
                        {
                            return true;
                        }
                        break;
                    case "Sum":
                        if (sum > _thresholdValue && _operator == "Greater than"
                            || sum < _thresholdValue && _operator == "Less than")
                        {
                            return true;
                        }
                        break;

                    case "Max":
                        double max = 0;
                        foreach (var re in pNew)
                        {
                            if (max < re.sensor_value_1) max = re.sensor_value_1;
                        }
                        if (max > _thresholdValue && _operator == "Greater than"
                            || max < _thresholdValue && _operator == "Less than") return true;
                        break;
                    case "Min":
                        double min = 1000;
                        foreach (var re in pNew)
                        {
                            if (min > re.sensor_value_1) min = re.sensor_value_1;
                        }
                        if (min > _thresholdValue && _operator == "Greater than"
                            || min < _thresholdValue && _operator == "Less than") return true;
                        break;
                }
                return false;
            }

            while (true)
            {
                if (checkAlert() == true)
                {
                    if (_operator == "Greater than")
                    {
                        backgroundWorker.ReportProgress(1);
                    }
                    else
                    {
                        backgroundWorker.ReportProgress(2);
                    }
                }       
                else
                {
                    backgroundWorker.ReportProgress(0);
                }
                
                int frequen = convertFrequency(_frequencyOfEvaluation);
                Thread.Sleep(TimeSpan.FromSeconds(frequen));
            }           
        }

        public void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            StackPanel stack = (StackPanel)((Panel)this.Parent).FindName("Warning_panel");
            TextBlock txt = (TextBlock)((Panel)this.Parent).FindName("sensor_2_warning_txt");
            Label lable = (Label)((Panel)this.Parent).FindName("warning");
            if (e.ProgressPercentage > 0)
            {
                if (e.ProgressPercentage == 1) warningMess = "Humidity is too high";
                else warningMess = "Humidity is too low";
                warningMess = "Humidity is too high!";
                stack.Background = new SolidColorBrush(Colors.Yellow);
                lable.Foreground = new SolidColorBrush(Colors.Red);
                txt.Text = warningMess;
                txt.Foreground = new SolidColorBrush(Colors.Red);
            }            
            else
            {
                warningMess = "Humidity is good";
                stack.Background = new SolidColorBrush(Colors.LightGray);
                lable.Foreground = new SolidColorBrush(Colors.Black);
                txt.Text = warningMess;
                txt.Foreground = new SolidColorBrush(Colors.BlueViolet);
            }
                
        }
        #endregion

        #region configure alert logic
        public void btnDone_Click(object sender, RoutedEventArgs e)
        {
            if (Operator.SelectedItem != null && Aggregation_Type.SelectedItem != null && Threshold_Value != null &&
                 Period.SelectedItem != null && Frequency_Of_Evaluation.SelectedItem != null)
            {
                string param1 = Operator.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
                string param2 = Aggregation_Type.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
                string param4 = Period.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();
                string param5 = Frequency_Of_Evaluation.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last();

                infor.parameters.Add(new param
                {
                    Operator = param1,
                    AggregationType = param2,
                    ThresholdValue = Int32.Parse(Threshold_Value.Text),
                    Period = param4,
                    FrequencyOfEvaluation = param5
                }); 
                infor.SaveChanges();

                conditionPreview.Text = $"Whenever the {param2} of humidity is {param1} {Threshold_Value.Text} ";
            }
            else
            {
                notify.Text = "You must complete form before click Done!";
            }
        }
        #endregion

        #region create Condition Preview
        public void createPreview()
        {
            var p = (from param in infor.parameters
                     orderby param.ID descending
                     select param).First();
            string _operator = p.Operator;
            string _aggregationType = p.AggregationType;
            int _thresholdValue = p.ThresholdValue;

            conditionPreview.Text = $"Whenever the {_aggregationType} of humidity is {_operator} {_thresholdValue} ";
        }
        #endregion

        #region event handler
        public void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            ((StatusBar)((Panel)this.Parent).FindName("StatusBar")).Visibility = Visibility.Visible;
            ((Border)((Panel)this.Parent).FindName("Border1")).Visibility = Visibility.Visible;
            ((Border)((Panel)this.Parent).FindName("Border2")).Visibility = Visibility.Visible;
            ((Border)((Panel)this.Parent).FindName("Border3")).Visibility = Visibility.Visible;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.IO.Ports;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Wpf;
using ProjectInternShip.DBContexts;
using ProjectInternShip.Models;
using System.Media;


namespace ProjectInternShip
{

    public partial class MainWindow : Window
    {
        #region  declare variable
        SerialPort p = new SerialPort();
        string data;
        Boolean first = false;
        string sensor_value_A;
        string sensor_value_B;

        #endregion
        #region declare method
        private delegate void text_delegate(string text);
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public SeriesCollection SeriesCollection_1 { get; set; }
        public List<string> Labels_1 { get; set; }
        public Func<int, string> YFormatter_1 { get; set; }

        data_sensor_DBcontexts db = new data_sensor_DBcontexts();
        private MediaPlayer mediaPlayer = new MediaPlayer();
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            time_display_load();
            port_form_load();
            run_charrt();
            load_data_list();
        }

        #region time_display
        private void time_display_load()
        {
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(update_timer);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();

        }
        private void update_timer(object sender, EventArgs e)
        {
            time_display.Content = DateTime.Now.ToString();
        }
        #endregion


        #region serial communication
        private void port_form_load()
        {
            string[] port_com = SerialPort.GetPortNames();
            txtPort.ItemsSource = port_com;
            string[] rate = { "2400", "4800", "9600", "19200", "38400" };
            txtRate.ItemsSource = rate;
            connect_btn.Content = "Connect";
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if ((connect_btn.Content as string) == "Connect")
            {

                if (txtPort.SelectedIndex == -1)
                {
                    MessageBox.Show("Port comm is empty", "Announcement", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (txtRate.Text == "")
                {
                    MessageBox.Show("Rate is empty", "Announcement", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (p.IsOpen == true)
                    {
                        p.Close();
                    }
                    if (p.IsOpen == false)
                    {
                        p.PortName = txtPort.Text;
                        p.BaudRate = Convert.ToInt32(txtRate.Text);
                        if (first == false)
                        {
                            p.DataReceived += new SerialDataReceivedEventHandler(received_ev);
                            first = true;
                        }
                        p.Open();
                        connect_btn.Content = "Disconnect";

                    }
                }
            }
            else
            {
                try
                {
                    p.Close();
                    connect_btn.Content = "Connect";
                    //SeriesCollection[0].Values.Add(double.NaN);
                    //SeriesCollection_1[0].Values.Add(double.NaN);
                    //add_db(DateTime.Now,-9.99,-9);                                    //khi can thi moi add db

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void received_ev(object sender, SerialDataReceivedEventArgs e)      //nhan du lieu arduino kieu bit,chuyen ve float xu li xong lai dua ve string
        {
            //data = p.ReadExisting();
            if (p.IsOpen == true)
            {
                //data = p.ReadByte().ToString();
                data = p.ReadLine().ToString();
                //double t = string_to_float(data) / 1000.0;
                //data = t.ToString();
                if (!string.IsNullOrEmpty(data))
                    Dispatcher.Invoke(DispatcherPriority.Send, new text_delegate(write_data_func), data);
            }
        }

        private void write_data_func(string text)                                   //hien thi du lieu ra UI
        {
            if (p.IsOpen)
            {
                int index_A = text.IndexOf("A");
                sensor_value_A = text.Split('A').Last();
                sensor_value_B = text.Split('A').First();

                //txt_sent.AppendText(cal_string(sensor_value_A) + "PPM"+"\n");
                //txt_sent.AppendText(sensor_value_B+"%"+"\n");
                txt_value_1.Text = cal_string(sensor_value_A) + "PPM";
                txt_value_2.Text = sensor_value_B + "%";

                ////txt_sent.Text += Environment.NewLine;


                Labels.Add(DateTime.Now.ToString("H:MM:ss"));
                double t = string_to_float((cal_string(sensor_value_A)));
                SeriesCollection[0].Values.Add(t);

                Labels_1.Add(DateTime.Now.ToString("H:MM:ss"));
                int z = Int32.Parse(sensor_value_B);
                SeriesCollection_1[0].Values.Add(z);
                add_db(DateTime.Now, t,z);
                load_data_list();
            }                                                                          ////khi can thi moi add db
        }


        double string_to_float(string str)
        {
            double a = Convert.ToDouble(str);
            return a;
        }

        string cal_string(string str)
        {
            double t = string_to_float(str) / 1000.0;
            return t.ToString();

        }
        #endregion

        #region chart region
        public void run_charrt()                                                    //load init chart
        {
            SeriesCollection = new SeriesCollection();
            YFormatter = value => value.ToString("00.000") + "PPM";
            Labels = new List<string>();

            SeriesCollection_1 = new SeriesCollection();
            YFormatter_1 = value => value.ToString("C") + "%";
            Labels_1 = new List<string>();
            add_chart();
            //load_full_database_chart();

            DataContext = this;
            //full_chart(0);
        }
        void add_chart()                                                            //add line series to chart
        {
            //var a = new ChartValues<double>();
            SeriesCollection.Add(new LineSeries
            {
                Title = "Air sensor",
                Values = new ChartValues<double>(),
                LineSmoothness = 0,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 4,
                PointForeground = Brushes.Gray
            });
            SeriesCollection_1.Add(new LineSeries
            {
                Title = "Soil humidity sensor",
                Values = new ChartValues<int>(),
                LineSmoothness = 0,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 4,
                PointForeground = Brushes.Brown
            });

        }
        void load_full_database_chart()                                             //chart displat all data in database
        {
            using (data_sensor_DBcontexts db = new data_sensor_DBcontexts())
            {
                var query = from dt in db.data orderby dt.data_sensor_ID select dt;
                foreach (var item in query)
                {
                    if (item.sensor_value < 0.00)
                    {
                        SeriesCollection[1].Values.Add(double.NaN);
                    }
                    else
                    {
                        SeriesCollection[1].Values.Add(item.sensor_value);
                        Labels.Add(item.time.ToString(("H:MM:ss")));
                    }
                }

            }
            //full_chart(0);
        }

        #endregion
        #region sensor_db
        public void add_db(DateTime t, double value, int value_1)
        {
            using (data_sensor_DBcontexts db = new data_sensor_DBcontexts())
            {
                //var query =from dt in  db.data select dt;
                var sensor = new data_sensor();
                sensor.sensor_value = value;
                sensor.sensor_value_1 = value_1;
                sensor.time = t;
                db.data.Add(sensor);
                db.SaveChanges();                           //co nhieu loai save change0..
            }
        }

        public DateTime display_time()
        {
            DateTime t = new DateTime();

            return t;
        }
        public double display_value()
        {
            double d;
            d = 0;
            return d;
        }
        #endregion
        #region data list region

        private void load_data_list()
        {
            var query = from dt in db.data select dt;
            //var list = query.ToList();
            ListData.ItemsSource = query.ToList();
        }
        #endregion
        #region warning
           
        #endregion






        public void btnBack1_Click(object sender, RoutedEventArgs e)
        {
            StatusBar.Visibility = Visibility.Collapsed;
            Border1.Visibility = Visibility.Collapsed;
            Border2.Visibility = Visibility.Collapsed;
            Border3.Visibility = Visibility.Collapsed;
            rootElement.Children[0].Visibility = Visibility.Visible;
        }

        public void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            StatusBar.Visibility = Visibility.Collapsed;
            Border1.Visibility = Visibility.Collapsed;
            Border2.Visibility = Visibility.Collapsed;
            Border3.Visibility = Visibility.Collapsed;
            rootElement.Children[0].Visibility = Visibility.Collapsed;
            rootElement.Children[1].Visibility = Visibility.Visible;
        }
    }
}

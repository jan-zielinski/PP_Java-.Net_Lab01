using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http; 
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Xml;
using System.Xml.Linq;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Lab01
{
    public partial class MainWindow : Window
    {
        ObservableCollection<Person> people = new ObservableCollection<Person>
        {
            new Person { Name = "Jan", Age = 21 },
            new Person { Name = "Laura", Age = 21 }
        };

        //DB
        Customer model = new Customer();
        public static DataGrid datagrid;
       
        private string imagePath;

        public static HttpClient Client => client;
        private static HttpClient client = new HttpClient();
        private static Random random = new Random();


        public ObservableCollection<Person> Items
        {
            get => people;
        }



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            GetWeatherData();
            LoadDB();

            RunPeriodically(OnTick, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5)).ContinueWith(task => { }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void AddNewPersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ageTextBox.Text, out int Age) && int.Parse(ageTextBox.Text) <= 99 && int.Parse(ageTextBox.Text) > 0)
            {
                people.Add(new Person { Age = int.Parse(ageTextBox.Text), Name = nameTextBox.Text, Picture = (BitmapImage)photoPreview.Source });
                ageTextBox.BorderBrush = System.Windows.Media.Brushes.Gray;
            }
            else
            {
                ageTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Age must be an number!");
                ageTextBox.Text = string.Empty;
            }
        }


        private void AddPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select a photo";
            if (fileDialog.ShowDialog() == true)
            {
                photoPreview.Source = new BitmapImage(new Uri(fileDialog.FileName));
                imagePath = fileDialog.FileName;
            }
        }

        private void EnterWebsite_Click(object sender, RoutedEventArgs e) => EnterWebsite();

        //Dodaje nowa osoba do 'ObservableCollection' przy uzyciu HttpClient. Za pomoca Regex wyciagamy tytul, oraz pierwszy napotkany int
        private async void EnterWebsite()
        {
            try
            {
                string result = await Client.GetStringAsync(websiteTextBox.Text);

                string name = Regex.Match(result, @"<title>\s*(.+?)\s*</title>", RegexOptions.IgnoreCase).Groups["1"].Value;
                //https://www.dotnetperls.com/title-html
                string ageString = Regex.Match(result, "[0-9]+").Value;

                if (int.TryParse(ageString, out int age))
                {
                    people.Add(new Person { Age = age, Name = name });
                }

                else
                {
                    MessageBox.Show("Age must be number");
                }
            }
            catch (Exception ex)
            {
                if (ex is HttpRequestException)
                {
                    MessageBox.Show(ex.Message);
                }

                else
                {
                    MessageBox.Show("Unkown Exception caught");
                }
            }
        }


        private void OnTick() => EnterWebsite();

        //Zadanie które perdiodycznie wywołuje funkcję OnTick
        //Ontick - delegat na wydarzenie wywołujące OnTick
        //dueTime - parametr TimeSpana mowi po jakim czasie zacznie wykonywac sie periodic loop
        //interval - co ile bedzie wykonywane zadanie OnTick (wejscie na strone)
        //
        private async Task RunPeriodically(Action OnTick, TimeSpan dueTime, TimeSpan interval)
        {
            if (dueTime > TimeSpan.Zero)
            {
                await Task.Delay(dueTime);
            }

            while (true)
            {
                OnTick?.Invoke();

                if (interval > TimeSpan.Zero)
                {
                    await Task.Delay(interval);
                }
            }
        }

        private async void GetWeatherData()
        {

            Console.WriteLine("tekst");
            string apiKey = "1b6714e500f0cdd864a8b49ec6ac5e45";
            string apiBaseUrl = "https://api.openweathermap.org/data/2.5/weather";


            await Task.Run(() =>
            {
                using (var client = new WebClient())
                {

                    //string apiCall = apiBaseUrl + "?q=" + "London" + "&apikey=" + apiKey;
                    List<string> cities = new List<string> {
                                "London", "Warsaw", "Paris", "Berlin", "Tokyo" };

                    for (int i = 1; i <= cities.Count; i++)
                    {
                        string city = cities[i - 1];

                        string apiCall = apiBaseUrl + "?q=" + city + "&apikey=" + apiKey;
                        String jsonString = client.DownloadString(apiCall);
                        var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonString);
                        String name = jsonObject["name"].ToString();
                        String temp = jsonObject["main"]["humidity"].ToString();


                        Dispatcher.Invoke(() =>
                        {
                            people.Add(new Person { Age = int.Parse(temp), Name = name });
                        });

                        Thread.Sleep(5);

                    }
                }
            });

        }

        /// <summary>
        /// Uzytkowni ma mozliwosc wyboru dla jakie miasto chce dodac do tabeli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterWeatherApi_Click(object sender, RoutedEventArgs e) => EnterWeatherApi();

        private void EnterWeatherApi()
        {
            string apiKey = "1b6714e500f0cdd864a8b49ec6ac5e45";
            string apiBaseUrl = "https://api.openweathermap.org/data/2.5/weather";

            using (var client = new WebClient())
            {
                string city = cityTextBox.Text;

                string apiCall = apiBaseUrl + "?q=" + city + "&apikey=" + apiKey;
                String jsonString = client.DownloadString(apiCall);
                var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonString);
                String name = jsonObject["name"].ToString();
                String temp = jsonObject["clouds"]["all"].ToString();

                people.Add(new Person { Age = int.Parse(temp), Name = name });

            }
        }

        private void LoadDB()
        {
            using (DBEntities db = new DBEntities())
            {
                dgvCustomer.ItemsSource = db.Customer.ToList<Customer>();
                datagrid = dgvCustomer;
            }
        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            InsertPage Ipage = new InsertPage();
            Ipage.ShowDialog();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            int Id = (dgvCustomer.SelectedItem as Customer).CustomerID;
            UpdatePage Upage = new UpdatePage(Id);
            Upage.ShowDialog();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            using (DBEntities db = new DBEntities())
            {
                int Id = (dgvCustomer.SelectedItem as Customer).CustomerID;
                var deleteMember = db.Customer.Where(c => c.CustomerID == Id).Single();
                db.Customer.Remove(deleteMember);
                db.SaveChanges();
                dgvCustomer.ItemsSource = db.Customer.ToList();
            }
        }

    


    }
}

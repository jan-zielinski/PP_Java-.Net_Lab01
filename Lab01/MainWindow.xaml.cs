﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Lab01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Person> people = new ObservableCollection<Person>
        {
            new Person { Name = "Jan", Age = 21 },
            new Person { Name = "Laura", Age = 21 }
        };
        private string imagePath;

        public ObservableCollection<Person> Items
        {
            get => people;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; //Jest to potrzebne do dataBindingu
        }

        private void AddNewPersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ageTextBox.Text, out int Age) && int.Parse(ageTextBox.Text) <= 99 && int.Parse(ageTextBox.Text) > 0)
            {
                people.Add(new Person { Age = int.Parse(ageTextBox.Text), Name = nameTextBox.Text, Picture = (BitmapImage)photoPreview.Source });
                ageTextBox.BorderBrush = Brushes.Gray;
            }
            else
            {
                ageTextBox.BorderBrush = Brushes.Red;
                ageTextBox.Text = "Age must be an number!";
            }
        }
        
            //Jakies bugi tutaj
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

       
    }

    }

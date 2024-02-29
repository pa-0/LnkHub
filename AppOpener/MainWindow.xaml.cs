using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ApplicationLauncherWidget
{
    public partial class MainWindow : Window
    {
        private const string FilePath = "applications.json";
        private ObservableCollection<ApplicationInfo> applications = new ObservableCollection<ApplicationInfo>();

        public MainWindow()
        {
            InitializeComponent();
            lstApplications.ItemsSource = applications;
            LoadApplications();
        }

        private void LoadApplications()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                applications = JsonConvert.DeserializeObject<ObservableCollection<ApplicationInfo>>(json);
                lstApplications.ItemsSource = applications;
            }
        }

        private void SaveApplications()
        {
            string json = JsonConvert.SerializeObject(applications);
            File.WriteAllText(FilePath, json);
        }

        private void LaunchApplication(string path)
        {
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error launching application: " + ex.Message);
            }
        }

        private void AddApplication(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            applications.Add(new ApplicationInfo { Name = fileName, Path = filePath });
            SaveApplications();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable Files (*.exe)|*.exe";
            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFile = openFileDialog.FileName;
                AddApplication(selectedFile);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lstApplications.SelectedItem != null)
            {
                applications.Remove((ApplicationInfo)lstApplications.SelectedItem);
                SaveApplications();
            }
        }

        private void lstApplications_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (lstApplications.SelectedItem != null)
            {
                LaunchApplication(((ApplicationInfo)lstApplications.SelectedItem).Path);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveApplications();
        }
    }

    public class ApplicationInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}

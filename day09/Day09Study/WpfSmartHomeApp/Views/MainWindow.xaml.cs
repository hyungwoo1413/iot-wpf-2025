﻿using System.Windows;
using System.Windows.Input;
using WpfSmartHomeApp.ViewModels;

namespace WpfSmartHomeApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // 앱 완전종료
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // 앱종료를 막는 기능
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.LoadedCommand.Execute(null);
            }
        }
    }
}
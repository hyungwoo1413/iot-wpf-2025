using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfMqttSub.ViewModels;

namespace WpfMqttSub.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();

            var vm = new MainViewModel(DialogCoordinator.Instance);
            this.DataContext = vm;
            vm.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(vm.LogText))
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        LogBox.CaretPosition = LogBox.Document.ContentEnd;
                        LogBox.ScrollToEnd();
                    });
                }
            };
        }
    }
}
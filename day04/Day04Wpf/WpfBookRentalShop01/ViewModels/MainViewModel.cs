using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Views;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // MahApp.Metro 형태 다이얼로그 코디네이터
        private readonly IDialogCoordinator dialogCoordinator;

        private string _greeting;

        public string Greeting
        {
            get => _greeting;
            set => SetProperty(ref _greeting, value);
        }


        /* ================ Constructor ================ */
        public MainViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator;
            Greeting = "BookRentalShop!";

            Common.LOGGER.Info("도서 대여점 프로그램 실행!");
        }


        /* ================ CurrentSatus ================ */
        private string _currentStatus;

        public string CurrentStatus
        {
            get => _currentStatus;
            set => SetProperty(ref _currentStatus, value);
        }


        /* ================ CurrentView ================ */
        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        #region '화면 기능(이벤트) 처리'


        /* ================ AppExit ================ */
        [RelayCommand]
        public async Task AppExit()
        {
            //MessageBox.Show("종료합니다!");
            //await this.dialogCoordinator.ShowMessageAsync(this, "종료", "종료하시겠습니까?");

            var result = await this.dialogCoordinator.ShowMessageAsync(this, "종료", "종료하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative) // OK 누르면
            {
                Application.Current.Shutdown();
            }
            else
            {
                return;
            }
        }


        /* ================ ShowBookGenre ================ */
        [RelayCommand]
        public void ShowBookGenre()
        {
            var vm = new BookGenreViewModel(this.dialogCoordinator);
            var v = new BookGenreView
            {
                DataContext = vm,
            };
            CurrentView = v;
            CurrentStatus = "책 장르 관리";

            Common.LOGGER.Info("책 장르 관리 실행!");
        }


        /* ================ ShowBooks ================ */
        [RelayCommand]
        public void ShowBooks()
        {
            var vm = new BooksViewModel();
            var v = new BooksView
            {
                DataContext = vm,
            };
            CurrentView = v;
            CurrentStatus = "책 관리";

            Common.LOGGER.Info("책 관리 실행!");
        }

        #endregion
    }
}

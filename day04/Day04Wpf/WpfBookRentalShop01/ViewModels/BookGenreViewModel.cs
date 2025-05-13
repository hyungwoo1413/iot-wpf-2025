using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using WpfBookRentalShop01.Helpers;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class BookGenreViewModel : ObservableObject
    {
        private readonly IDialogCoordinator dialogCoordinator; // MainViewModel과 동일

        private ObservableCollection<Genre> _genres;

        public ObservableCollection<Genre> Genres { 
            get => _genres; 
            set => SetProperty(ref _genres, value); 
        }

        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get => _selectedGenre;
            set
            {
                SetProperty(ref _selectedGenre, value);
                _isUpdate = true;
            }
        }

        private bool _isUpdate;


        /* ================ Constructor ================ */
        public BookGenreViewModel(IDialogCoordinator coordinator)
        {
            this.dialogCoordinator = coordinator;
            InitVariable();
            LoadGridFromDb();
        }


        /* ================ InitVariable ================ */
        private void InitVariable()
        {
            SelectedGenre = new Genre();
            SelectedGenre.Names = string.Empty;
            SelectedGenre.Division = string.Empty;
            _isUpdate = false; // 신규 상태로 변경
        }


        /* ================ LoadGridFromDb ================ */
        private async Task LoadGridFromDb()
        {
            try
            {
                //string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
                string query = "SELECT division, names FROM divtbl";

                ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while(reader.Read())
                    {
                        var division = reader.GetString("division");
                        var names = reader.GetString("names");

                        genres.Add(new Genre { Division = division, Names = names });
                    }
                }
                Genres = genres;
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                //MessageBox.Show(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }
        }


        /* ================ SetInit ================ */
        [RelayCommand]
        public void SetInit()
        {
            InitVariable();
        }


        /* ================ SaveData ================ */
        [RelayCommand]
        public async Task SaveData()
        {
            try
            {
                //string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
                string query = string.Empty;

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();

                    if (_isUpdate) // 기존데이터 수정
                    {
                        query = "UPDATE divtbl SET names = @names WHERE division = @division";
                    }
                    else // 신규추가
                    {
                        query = "INSERT INTO divtbl VALUES (@division, @names)";
                    }

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@division", SelectedGenre.Division);
                    cmd.Parameters.AddWithValue("@names", SelectedGenre.Names);

                    var resultCnt = cmd.ExecuteNonQuery();

                    if (resultCnt > 0)
                    {
                        Common.LOGGER.Info("저장 성공");
                        //MessageBox.Show("저장 성공!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장", "저장 성공");
                    }
                    else
                    {
                        Common.LOGGER.Warn("저장 실패");
                        //MessageBox.Show("저장 실패!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "저장", "저장 실패");
                    }
                }
                //Debug.WriteLine(SelectedGenre.Names);
                //Debug.WriteLine(SelectedGenre.Division);
                //Debug.WriteLine(_isUpdate);
            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                //MessageBox.Show(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            await LoadGridFromDb();
        }


        /* ================ DelData ================ */
        [RelayCommand]
        public async Task DelData()
        {
            if(_isUpdate == false)
            {
                //MessageBox.Show("선택된 데이터가 아니면 삭제할 수 없습니다.");
                await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "선택된 데이터가 아니면 삭제할 수 없습니다.");
                return;
            }

            var result = await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제하시겠습니까?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Negative)
            {
                return;
            }

            try
            {
                //string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
                string query = "DELETE FROM divtbl WHERE division = @division";

                using (MySqlConnection conn = new MySqlConnection(Common.CONNSTR))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@division", SelectedGenre.Division);

                    int resultCnt = cmd.ExecuteNonQuery(); // 한 건 삭제되면 resultCnt = 1, 안지워지면 resultCnt = 0

                    if(resultCnt > 0)
                    {
                        Common.LOGGER.Info($"{SelectedGenre.Division} 삭제 성공");
                        //MessageBox.Show("삭제 성공!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제 성공");
                    }
                    else
                    {
                        Common.LOGGER.Warn("삭제 실패");
                        //MessageBox.Show("삭제 실패!");
                        await this.dialogCoordinator.ShowMessageAsync(this, "삭제", "삭제 실패");
                    }
                }

            }
            catch (Exception ex)
            {
                Common.LOGGER.Error(ex.Message);
                //MessageBox.Show(ex.Message);
                await this.dialogCoordinator.ShowMessageAsync(this, "오류", ex.Message);
            }

            await LoadGridFromDb(); // 저장이 끝난 후 다시 DB내용을 그리드에 그림
        }
    }
}

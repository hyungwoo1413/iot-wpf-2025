using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfBookRentalShop01.Models;

namespace WpfBookRentalShop01.ViewModels
{
    public partial class BookGenreViewModel : ObservableObject
    {
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

        public BookGenreViewModel()
        {
            _isUpdate = false;
            LoadGridFromDb();
        }

        private void LoadGridFromDb()
        {
            string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
            string query = "SELECT division, names FROM divtbl";

            ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
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

        [RelayCommand]
        public void SetInit()
        {
            _isUpdate = false;
            SelectedGenre = null;
        }
        public void SaveData()
        {
        }

        [RelayCommand]
        public void DelData()
        {
            if(_isUpdate == false)
            {
                MessageBox.Show("선택된 데이터가 아니면 삭제할 수 없습니다.");
                return;
            }
            try
            {
                string connectionString = "Server=localhost;Database=bookrentalshop;Uid=root;Pwd=12345;Charset=utf8;";
                string query = "DELETE FROM divtbl WHERE division = @division";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@division", SelectedGenre.Division);

                    int resultCnt = cmd.ExecuteNonQuery(); // 한 건 삭제되면 resultCnt = 1, 안지워지면 resultCnt = 0

                    if(resultCnt > 0)
                    {
                        MessageBox.Show("삭제 성공!");
                    }
                    else
                    {
                        MessageBox.Show("삭제 실패!");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"삭제 중 오류 발생: {ex.Message}");
            }
        }
        //SetInitCommnad
        //SaveDataCommand
        //DelDataCommand
    }
}

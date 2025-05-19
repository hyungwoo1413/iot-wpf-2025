using BusanRestaurantApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusanRestaurantApp.ViewModels
{
    public class GoogleMapViewModel : ObservableObject
    {
        //====================================================

        public GoogleMapViewModel()
        {
            MatjibLocation = "";
        }

        //====================================================

        private BusanItem _selectedMatjibItem;

        public BusanItem SelectedMatjibItem
        {
            get => _selectedMatjibItem;
            set
            {
                SetProperty(ref _selectedMatjibItem, value);
                // 위도(Lattitude = Lat), 경도(Longitude = Lng)
                MatjibLocation = $"https://google.com/maps/place/{SelectedMatjibItem.Lat},{SelectedMatjibItem.Lng}";
            }        
        }

        //====================================================

        private string _matjibLocation;


        public string MatjibLocation
        {
            get => _matjibLocation;
            set => SetProperty(ref _matjibLocation, value);
        }
    }
}

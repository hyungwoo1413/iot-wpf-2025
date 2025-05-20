using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows;
using System.Windows.Threading;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        //================================================
        // 날짜/시간
        private string _currDateTime;

        // 온도/습도
        private double _homeTemp;
        private double _homeHumid;

        // 사람 인지
        private string _detectResult;
        private bool _isDetectOn;

        // 비 감지
        private string _rainResult;
        private bool _isRainOn;

        // 에어컨
        private string _airconResult;
        private bool _isAirconOn;

        // 조명
        private string _lightsResult;
        private bool _isLightsOn;

        //================================================

        private readonly DispatcherTimer _timer;
        public MainViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (sender, e) =>
            {
                CurrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            _timer.Start();
        }

        //================================================
        public string CurrDateTime
        {
            get => _currDateTime;
            set => SetProperty(ref _currDateTime, value);
        }

        public double HomeTemp
        {
            get => _homeTemp;
            set => SetProperty(ref _homeTemp, value);
        }

        public double HomeHumid
        {
            get => _homeHumid;
            set => SetProperty(ref _homeHumid, value);
        }

        public string DetectResult
        {
            get => _detectResult;
            set => SetProperty(ref _detectResult, value);
        }

        public bool IsDetectOn
        {
            get => _isDetectOn;
            set => SetProperty(ref _isDetectOn, value);
        }

        public string RainResult
        {
            get => _rainResult;
            set => SetProperty(ref _rainResult, value);
        }

        public bool IsRainOn
        {
            get => _isRainOn;
            set => SetProperty(ref _isRainOn, value);
        }

        public string AirconResult
        {
            get => _airconResult;
            set => SetProperty(ref _airconResult, value);
        }

        public bool IsAirconOn
        {
            get => _isAirconOn;
            set => SetProperty(ref _isAirconOn, value);
        }

        public string LightsResult
        {
            get => _lightsResult;
            set => SetProperty(ref _lightsResult, value);
        }

        public bool IsLightsOn
        {
            get => _isLightsOn;
            set => SetProperty(ref _isLightsOn, value);
        }

        //================================================

        [RelayCommand]
        public void OnLoaded()
        {
            HomeTemp = 22;
            HomeHumid = 43.5;

            DetectResult = "Detected Human";
            IsDetectOn = true;

            RainResult = "Raining";
            IsRainOn = true;

            AirconResult = "Air Condition";
            IsAirconOn = false;

            LightsResult = "Light";
            IsLightsOn = true;
        }
    }
}

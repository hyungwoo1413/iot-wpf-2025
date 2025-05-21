using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MQTTnet;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfSmartHomeApp.Helpers;
using WpfSmartHomeApp.Models;
using Newtonsoft.Json;

namespace WpfSmartHomeApp.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
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

        // readonly는 생성자에서만 값을 할당. 그외는 불가
        private readonly DispatcherTimer _timer;
        // MQTT용 변수들
        private string TOPIC;
        private IMqttClient mqttClient;
        private string BROKERHOST;

        //================================================

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
        public async Task OnLoaded()
        {
            /* 테스트로 집어넣은 가짜 데이터
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
            */

            // MQTT 접속부터 실행까지
            TOPIC = "pknu/sh01/data"; // publish, subscribe 동시에 사용
            BROKERHOST = "210.119.12.52"; // SmartHome MQTT Broker IP

            var mqttFactory = new MqttClientFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            // MQTT 클라이언트 접속 설정변수
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(BROKERHOST)
                .WithCleanSession(true)
                .Build();

            // MQTT 접속확인 이벤트 메서드 선언
            mqttClient.ConnectedAsync += MqttClient_ConnectedAsync;
            // MQTT 구독메시지 확인 메서드 선언
            mqttClient.ApplicationMessageReceivedAsync += MqttClient_ApplicationMessageReceivedAsync;
            
            await mqttClient.ConnectAsync(mqttClientOptions); // MQTT 브로커에 접속
        }

        private Task MqttClient_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            var topic = arg.ApplicationMessage.Topic; // pknu/sh01/data
            var payload = arg.ApplicationMessage.ConvertPayloadToString(); // byte -> UTF-8 변환

            var data = JsonConvert.DeserializeObject<SensingInfo>(payload);
            Common.LOGGER.Info($"\nLight : {data.L} | Rain : {data.R} | Temp : {data.T} | Humid : {data.H} | Fan : {data.F} | Vulernability : {data.V} | RealLight : {data.RL} | ChaimBell : {data.CB}");

            HomeTemp = data.T;
            HomeHumid = data.H;

            IsDetectOn = data.V == "ON" ? true : false;
            DetectResult = data.V == "ON" ? "Detection State" : "Normal State";

            IsLightsOn = data.RL == "ON" ? true : false;
            LightsResult = data.RL == "ON" ? "Light ON" : "Light OFF";

            IsAirconOn = data.F == "ON" ? true : false;
            AirconResult = data.F == "ON" ? "Aircon ON" : "Aircon OFF";

            IsRainOn = data.R <= 300 ? true : false;
            RainResult = data.R <= 300 ? "Raining" : "No Raining";

            return Task.CompletedTask; // 구독이 종료됨을 알려주는 리턴문
        }

        // MQTT 접속확인 이벤트 메서드
        private async Task MqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Common.LOGGER.Info($"{arg}");
            Common.LOGGER.Info("MQTT Broker 접속 성공");
            // 연결 이후 Subscribe 구독 시작
            await mqttClient.SubscribeAsync(TOPIC);
        }

        public void Dispose()
        {
            // TODO: 나중에 리소스 해제처리 필요
        }
    }
}

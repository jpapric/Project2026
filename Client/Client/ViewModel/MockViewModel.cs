using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Client.ViewModel
{
    public class MockViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer _timer;
        private readonly Random _random = new Random();

        //EAF PROCESS DATA (PLC → L2) 

        private bool _scrapLoading;
        private bool _tappingActive;
        private double _actualTilting;
        private double _materialWeight;
        private double _actualCurrent;
        private double _energyConsumed;
        private double _actualTemperature;
        private bool _furnaceOverfill;
        private bool _tappingError;
        private bool _furnaceEmpty;
        private bool _furnaceOvertemperature;

        //EAF SETPOINTS (L2 → PLC) 

        private bool _loadScrap;
        private bool _tap;
        private double _currentSetpoint;
        private double _tapAngle;
        private bool _reset;

        //CONNECTION STATUS 

        private bool _plcConnected;
        private bool _backendConnected;
        private bool _databaseConnected;

        //UI HELPERS 

        private string _lastEvent;
        private string _currentTime;

        //PUBLIC PROPERTIES  

        public bool ScrapLoading
        {
            get => _scrapLoading;
            set { _scrapLoading = value; OnPropertyChanged(); }
        }

        public bool TappingActive
        {
            get => _tappingActive;
            set { _tappingActive = value; OnPropertyChanged(); }
        }

        public double ActualTilting
        {
            get => _actualTilting;
            set { _actualTilting = value; OnPropertyChanged(); }
        }

        public double MaterialWeight
        {
            get => _materialWeight;
            set { _materialWeight = value; OnPropertyChanged(); }
        }

        public double ActualCurrent
        {
            get => _actualCurrent;
            set { _actualCurrent = value; OnPropertyChanged(); }
        }

        public double EnergyConsumed
        {
            get => _energyConsumed;
            set { _energyConsumed = value; OnPropertyChanged(); }
        }

        public double ActualTemperature
        {
            get => _actualTemperature;
            set { _actualTemperature = value; OnPropertyChanged(); }
        }

        public bool FurnaceOverfill
        {
            get => _furnaceOverfill;
            set { _furnaceOverfill = value; OnPropertyChanged(); }
        }

        public bool TappingError
        {
            get => _tappingError;
            set { _tappingError = value; OnPropertyChanged(); }
        }

        public bool FurnaceEmpty
        {
            get => _furnaceEmpty;
            set { _furnaceEmpty = value; OnPropertyChanged(); }
        }

        public bool FurnaceOvertemperature
        {
            get => _furnaceOvertemperature;
            set { _furnaceOvertemperature = value; OnPropertyChanged(); }
        }


        public bool LoadScrap
        {
            get => _loadScrap;
            set { _loadScrap = value; OnPropertyChanged(); }
        }

        public bool Tap
        {
            get => _tap;
            set { _tap = value; OnPropertyChanged(); }
        }

        public double CurrentSetpoint
        {
            get => _currentSetpoint;
            set { _currentSetpoint = value; OnPropertyChanged(); }
        }

        public double TapAngle
        {
            get => _tapAngle;
            set { _tapAngle = value; OnPropertyChanged(); }
        }

        public bool Reset
        {
            get => _reset;
            set { _reset = value; OnPropertyChanged(); }
        }

        public bool PlcConnected
        {
            get => _plcConnected;
            set { _plcConnected = value; OnPropertyChanged(); }
        }

        public bool BackendConnected
        {
            get => _backendConnected;
            set { _backendConnected = value; OnPropertyChanged(); }
        }

        public bool DatabaseConnected
        {
            get => _databaseConnected;
            set { _databaseConnected = value; OnPropertyChanged(); }
        }

        //UI 
        public string LastEvent
        {
            get => _lastEvent;
            set { _lastEvent = value; OnPropertyChanged(); }
        }

        public string CurrentTime
        {
            get => _currentTime;
            set { _currentTime = value; OnPropertyChanged(); }
        }

        //SIMULATION INTERNALS 

        private bool _loadScrapPulseActive;
        private int _loadScrapPulseMs;
        private const int LoadScrapPulseDuration = 500; // ms, per spec

        private double _simulatedTilt = 0;   // smooth tilt tracking


        public MockViewModel()
        {
            PlcConnected = true;
            BackendConnected = true;
            DatabaseConnected = true;

            MaterialWeight = 0;
            ActualCurrent = 0;
            ActualTilting = 0;
            ActualTemperature = 20;
            EnergyConsumed = 0;
            FurnaceEmpty = true;
            LastEvent = "System ready — furnace empty";

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) // 100ms per spec
            };
            _timer.Tick += OnTick;
            _timer.Start();
        }

        //MAIN SIMULATION TICK 

        private void OnTick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            // Load scrap pulse logic
            // after 500ms. Each pulse adds 1 ton.
            if (_loadScrapPulseActive)
            {
                _loadScrapPulseMs += 100;
                ScrapLoading = true;

                if (_loadScrapPulseMs >= LoadScrapPulseDuration)
                {
                    LoadScrap = false;
                    _loadScrapPulseActive = false;
                    _loadScrapPulseMs = 0;
                    ScrapLoading = false;

                    MaterialWeight = Math.Min(50, MaterialWeight + 1);
                    LastEvent = $"Scrap loaded — weight: {MaterialWeight:F1} T";
                }
            }

            // Smooth tilt tracking
            _simulatedTilt += (TapAngle - _simulatedTilt) * 0.08;
            if (Math.Abs(_simulatedTilt - TapAngle) < 0.05) _simulatedTilt = TapAngle;
            ActualTilting = _simulatedTilt;

            // Tapping / draining 
            if (Tap && ActualTilting > 0.5 && MaterialWeight > 0)
            {
                TappingActive = true;
                double drainRate = (ActualTilting / 15.0) * 0.08; // tons per 100ms tick
                MaterialWeight = Math.Max(0, MaterialWeight - drainRate);
            }
            else
            {
                TappingActive = false;
            }

            //  Current; temperature & energy 
            if (CurrentSetpoint > 0 && MaterialWeight > 0)
            {
                ActualCurrent = CurrentSetpoint + (_random.NextDouble() - 0.5) * CurrentSetpoint * 0.02;

                // Temperature rises proportionally to current
                double heatRate = (ActualCurrent / 50000.0) * 8.0;
                ActualTemperature = Math.Min(1800, ActualTemperature + heatRate);

                // Energy accumulates (kWh): P = U*I
                EnergyConsumed += ActualCurrent * 0.0000003;
            }
            else
            {
                ActualCurrent = 0;
                // Slow cool-down when no current
                if (ActualTemperature > 20)
                    ActualTemperature = Math.Max(20, ActualTemperature - 0.5);
            }

            //  Alarms 
            FurnaceOverfill = MaterialWeight >= 50;
            FurnaceEmpty = MaterialWeight <= 0.05;
            FurnaceOvertemperature = ActualTemperature >= 1750;
            TappingError = Tap && FurnaceEmpty && TapAngle > 0;

            if (FurnaceEmpty) MaterialWeight = 0;

            //  Alarm events 
            if (FurnaceOverfill) LastEvent = "ALARM: Furnace overfill!";
            if (FurnaceOvertemperature) LastEvent = "ALARM: Overtemperature!";
            if (TappingError) LastEvent = "ALARM: Tapping error — furnace empty";

            if (Reset)
            {
                MaterialWeight = 0;
                ActualCurrent = 0;
                ActualTilting = 0;
                _simulatedTilt = 0;
                ActualTemperature = 20;
                EnergyConsumed = 0;
                CurrentSetpoint = 0;
                TapAngle = 0;
                Tap = false;
                LoadScrap = false;
                FurnaceOverfill = false;
                FurnaceEmpty = true;
                TappingError = false;
                FurnaceOvertemperature = false;
                Reset = false;
                LastEvent = "System reset — furnace empty";
            }
        }

        //  COMMANDS called from EAFView.xaml.cs 

        public void TriggerLoadScrap()
        {
            if (_loadScrapPulseActive || FurnaceOverfill) return;
            LoadScrap = true;
            _loadScrapPulseActive = true;
            _loadScrapPulseMs = 0;
        }

        public void SetTap(bool enabled) => Tap = enabled;

        public void TriggerReset() => Reset = true;


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
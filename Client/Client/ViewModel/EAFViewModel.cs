using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Helpers;
using Client.Models;
using Client.Proxies;

namespace Client.ViewModel
{
    public class EAFViewModel : INotifyPropertyChanged
    {
        private readonly EAFProxy _proxy = new EAFProxy();
        private readonly DispatcherTimer _timer;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region PLC Data Fields

        private bool _scrapLoading;
        private bool _tappingActive;
        private float _actualTilting;
        private float _materialWeight;
        private float _actualCurrent;
        private float _energyConsumed;
        private float _actualTemperature;
        private bool _furnaceOverfill;
        private bool _tappingError;
        private bool _furnaceEmpty;
        private bool _furnaceOvertemperature;

        #endregion

        #region PLC Data Properties

        public bool ScrapLoading
        {
            get => _scrapLoading;
            set
            {
                _scrapLoading = value;
                OnPropertyChanged();
            }
        }

        public bool TappingActive
        {
            get => _tappingActive;
            set
            {
                _tappingActive = value;
                OnPropertyChanged();
            }
        }

        public float ActualTilting
        {
            get => _actualTilting;
            set
            {
                _actualTilting = value;
                OnPropertyChanged();
            }
        }

        public float MaterialWeight
        {
            get => _materialWeight;
            set
            {
                _materialWeight = value;
                OnPropertyChanged();
            }
        }

        public float ActualCurrent
        {
            get => _actualCurrent;
            set
            {
                _actualCurrent = value;
                OnPropertyChanged();
            }
        }

        public float EnergyConsumed
        {
            get => _energyConsumed;
            set
            {
                _energyConsumed = value;
                OnPropertyChanged();
            }
        }

        public float ActualTemperature
        {
            get => _actualTemperature;
            set
            {
                _actualTemperature = value;
                OnPropertyChanged();
            }
        }

        public bool FurnaceOverfill
        {
            get => _furnaceOverfill;
            set
            {
                _furnaceOverfill = value;
                OnPropertyChanged();
            }
        }

        public bool TappingError
        {
            get => _tappingError;
            set
            {
                _tappingError = value;
                OnPropertyChanged();
            }
        }

        public bool FurnaceEmpty
        {
            get => _furnaceEmpty;
            set
            {
                _furnaceEmpty = value;
                OnPropertyChanged();
            }
        }

        public bool FurnaceOvertemperature
        {
            get => _furnaceOvertemperature;
            set
            {
                _furnaceOvertemperature = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Setpoints

        private float _currentSetpoint;
        private float _tapAngleSetpoint;

        public float CurrentSetpoint
        {
            get => _currentSetpoint;
            set
            {
                _currentSetpoint = value;
                OnPropertyChanged();
            }
        }

        public float TapAngleSetpoint
        {
            get => _tapAngleSetpoint;
            set
            {
                _tapAngleSetpoint = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Connection

        private bool _isConnected;
        private string _connectionStatus = "Disconnected";

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region History

        private ObservableCollection<EventDto> _events = new ObservableCollection<EventDto>();

        public ObservableCollection<EventDto> Events
        {
            get => _events;
            set { _events = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand LoadScrapCommand { get; }
        public ICommand TapCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand SetCurrentCommand { get; }
        public ICommand SetAngleCommand { get; }
        public ICommand UpdatePlcCommand { get; }

        #endregion

        #region Constructor

        public EAFViewModel()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };

            _timer.Tick += async (s, e) => await PollAsync();

            LoadScrapCommand = new AsyncCommand(LoadScrap);
            TapCommand = new AsyncCommand(Tap);
            ResetCommand = new AsyncCommand(Reset);
            SetCurrentCommand = new AsyncCommand(SetCurrent);
            SetAngleCommand = new AsyncCommand(SetAngle);
            UpdatePlcCommand = new AsyncCommand<PLCDto>(UpdatePlc);
        }

        #endregion

        #region Polling

        public void StartPolling()
        {
            _timer.Start();
        }

        public void StopPolling()
        {
            _timer.Stop();
        }

        private async Task PollAsync()
        {
            try
            {
                EAFDto data = await _proxy.GetEafDataFromPlcAsync();

                if (data == null)
                {
                    IsConnected = false;
                    ConnectionStatus = "No data";
                    return;
                }

                ScrapLoading = data.Scrap_loading;
                TappingActive = data.Tapping_active;
                ActualTilting = data.Actual_tilting;
                MaterialWeight = data.Material_weight/1000f;
                ActualCurrent = data.Actual_current;
                EnergyConsumed = data.Energy_consumed;
                ActualTemperature = data.Actual_temperature;
                FurnaceOverfill = data.Furnace_overfill;
                TappingError = data.Tapping_error;
                FurnaceEmpty = data.Furnace_empty;
                FurnaceOvertemperature = data.Furnace_overtemperature;

                await RefreshEventsAsync();

                IsConnected = true;
                ConnectionStatus = "Connected";
            }
            catch (Exception ex)
            {
                IsConnected = false;
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }
        private async Task RefreshEventsAsync()
        {
            try
            {
                var events = await _proxy.GetEventsAsync();
                if (events == null) return;

                Events = new ObservableCollection<EventDto>(events);
            }
            catch { }
        }

        #endregion

        #region Command Methods

        private async Task LoadScrap()
        {
            try
            {
                await _proxy.LoadScrapAsync();
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }

        private async Task Tap()
        {
            try
            {
                await _proxy.TapAsync();
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }

        private async Task Reset()
        {
            try
            {
                await _proxy.ResetAsync();
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }

        private async Task SetCurrent()
        {
            try
            {
                await _proxy.SetCurrentAsync(CurrentSetpoint);
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }

        private async Task SetAngle()
        {
            try
            {
                await _proxy.SetAngleAsync(TapAngleSetpoint);
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Error: {ex.Message}";
            }
        }

        private async Task UpdatePlc(PLCDto plcDto)
        {
            try { await _proxy.UpdatePlcAsync(plcDto); }
            catch (Exception ex) { ConnectionStatus = $"Error: {ex.Message}"; }
        }
        #endregion


    }
}
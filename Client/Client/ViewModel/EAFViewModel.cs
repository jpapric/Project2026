using Client.Models;
using Client.Proxies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Client.ViewModel
{
    public class EAFViewModel
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

           
        private readonly EAFProxy _eafProxy = new EAFProxy();
        PlcConnectionViewModel plcConnectionTab = new PlcConnectionViewModel();

        /*
        public async Task PlcConnectionTab()
        {
            PLCDto plc = await _eafProxy.GetPlc();
            PlcConnectionViewModel plcConnectionTab = new PlcConnectionViewModel(plc);
        }
        */
    }
}

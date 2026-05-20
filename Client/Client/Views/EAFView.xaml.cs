using Client.Models;
using Client.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static Client.Models.MockModel;

namespace Client.Views
{
    public partial class EAFView : UserControl
    {
        private readonly MockViewModel _vm;

        private readonly DispatcherTimer _animTimer;
        private double _sparkPhase = 0;
        private double _electrodeCurrentY = 10;
        private const double FillBottomTop = 194.0; 
        private const double MaxFillHeight = 162.0;
        private const double ElRestY = 10.0;
        private const double ElActiveY = 108.0;
        private const double ElHeight = 110.0;
        private const double ElTipH = 16.0;

        private const double MaxWeight = 50.0;
        private const double MaxEnergy = 500.0;

        private readonly ObservableCollection<HistoryEvent> _historyEvents = new ObservableCollection<HistoryEvent>();
        private readonly ObservableCollection<AlarmEvent> _alarmEvents = new ObservableCollection<AlarmEvent>();

        private bool _wasOverfill;
        private bool _wasOvertemp;
        private bool _wasTapError;

        private bool _isConnected = false;

        public EAFView()
        {
            InitializeComponent();

            _vm = new MockViewModel();
            DataContext = _vm;

            HistoryGrid.ItemsSource = _historyEvents;
            AlarmGrid.ItemsSource = _alarmEvents;

            LoadScrapBtn.Click += (s, e) =>
            {
                _vm.TriggerLoadScrap();
                LogEvent("INFO", "Load scrap commanded (+1 T)");
            };

            TapBtn.PreviewMouseLeftButtonDown += (s, e) => _vm.SetTap(true);
            TapBtn.PreviewMouseLeftButtonUp += (s, e) => _vm.SetTap(false);
            TapBtn.MouseLeave += (s, e) => _vm.SetTap(false);

            TapSlider.ValueChanged += (s, e) =>
            {
                _vm.TapAngle = TapSlider.Value;
                TapAngleLabel.Text = $"{TapSlider.Value:F1}°";
            };

            CurrentSetpointInput.LostFocus += (s, e) =>
            {
                if (double.TryParse(CurrentSetpointInput.Text, out double val))
                {
                    _vm.CurrentSetpoint = Math.Max(0, val);
                    LogEvent("INFO", $"Current setpoint changed to {val:F0} A");
                }
            };

            LogEvent("SYSTEM", "EAF Control system started");

            _animTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _animTimer.Tick += OnAnimTick;
            _animTimer.Start();
        }

        private void OnAnimTick(object sender, EventArgs e)
        {
            _sparkPhase += 0.18;
            UpdateMetricCards();
            UpdateElectrodes();
            UpdateFurnaceTilt();
            UpdateMaterialFill();
            UpdateStatusIndicators();
            UpdateAlarmBanners();
            DetectAndLogAlarms();
            UpdateAlarmTab();
        }

        private void UpdateMetricCards()
        {
            TiltingValue.Text = _vm.ActualTilting.ToString("F1");
            WeightValue.Text = _vm.MaterialWeight.ToString("F1");
            CurrentValue.Text = ((int)_vm.ActualCurrent).ToString();
            TempValue.Text = ((int)_vm.ActualTemperature).ToString();
        }

        private void UpdateElectrodes()
        {
            bool melting = _vm.ActualCurrent > 0;
            double targetY = melting ? ElActiveY : ElRestY;

            _electrodeCurrentY += (targetY - _electrodeCurrentY) * 0.08;
            if (Math.Abs(_electrodeCurrentY - targetY) < 0.1)
                _electrodeCurrentY = targetY;

            double tipY = _electrodeCurrentY + ElHeight;
            double sparkY = tipY + ElTipH + 2;

            Canvas.SetTop(Electrode1, _electrodeCurrentY);
            ElTip1.Points = new System.Windows.Media.PointCollection
                            {
                                new System.Windows.Point(148, tipY),
                                new System.Windows.Point(166, tipY),
                                new System.Windows.Point(157, tipY + ElTipH)
                            };
            Canvas.SetTop(Spark1, sparkY);

            Canvas.SetTop(Electrode2, _electrodeCurrentY);
            ElTip2.Points = new System.Windows.Media.PointCollection
                            {
                                new System.Windows.Point(271, tipY),
                                new System.Windows.Point(289, tipY),
                                new System.Windows.Point(280, tipY + ElTipH)
                            };
            Canvas.SetTop(Spark2, sparkY);

            Canvas.SetTop(Electrode3, _electrodeCurrentY);
            ElTip3.Points = new System.Windows.Media.PointCollection
                            {
                                new System.Windows.Point(394, tipY),
                                new System.Windows.Point(412, tipY),
                                new System.Windows.Point(403, tipY + ElTipH)
                            };
            Canvas.SetTop(Spark3, sparkY);

            if (melting)
            {
                Spark1.Opacity = (Math.Sin(_sparkPhase + 0.00) + 1.0) / 2.0;
                Spark2.Opacity = (Math.Sin(_sparkPhase + 2.09) + 1.0) / 2.0;
                Spark3.Opacity = (Math.Sin(_sparkPhase + 4.18) + 1.0) / 2.0;
            }
            else
            {
                Spark1.Opacity = 0;
                Spark2.Opacity = 0;
                Spark3.Opacity = 0;
            }
        }

        private void UpdateFurnaceTilt()
        {
            FurnaceTilt.Angle = _vm.ActualTilting;
        }

        private void UpdateMaterialFill()
        {
            double fraction = Math.Max(0, Math.Min(1, _vm.MaterialWeight / MaxWeight));
            double fillHeight = fraction * MaxFillHeight;

            MaterialFill.Height = fillHeight;
            Canvas.SetTop(MaterialFill, FillBottomTop - fillHeight);


            Canvas.SetTop(WeightOverlay,
                Math.Max(FillBottomTop - fillHeight + 6, FillBottomTop - 28));
            WeightOverlay.Text = $"{_vm.MaterialWeight:F1} T  |  {fraction * 100:F0}%";

            double energyPct = Math.Min(100, (_vm.EnergyConsumed / MaxEnergy) * 100);
            EnergyBar.Value = energyPct;
            EnergyLabel.Text = $"{_vm.EnergyConsumed:F0} kWh";
            EnergyPctLabel.Text = $"{energyPct:F0}%";
        }

        private void UpdateStatusIndicators()
        {
            LedScrap.Fill = _vm.ScrapLoading
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                : new SolidColorBrush(Color.FromRgb(68, 68, 68));

            LedTapping.Fill = _vm.TappingActive
                ? new SolidColorBrush(Color.FromRgb(255, 152, 0))
                : new SolidColorBrush(Color.FromRgb(68, 68, 68));

            LoadScrapBtn.IsEnabled = !_vm.FurnaceOverfill;
            LoadScrapBtn.Opacity = _vm.FurnaceOverfill ? 0.4 : 1.0;
        }

        private void UpdateAlarmBanners()
        {
            OverfillBanner.Visibility = _vm.FurnaceOverfill ? Visibility.Visible : Visibility.Collapsed;
            OvertempBanner.Visibility = _vm.FurnaceOvertemperature ? Visibility.Visible : Visibility.Collapsed;
            TapErrorBanner.Visibility = _vm.TappingError ? Visibility.Visible : Visibility.Collapsed;
            EmptyBanner.Visibility = _vm.FurnaceEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DetectAndLogAlarms()
        {
            if (_vm.FurnaceOverfill && !_wasOverfill)
                LogAlarm("FURNACE OVERFILL", "Material weight exceeds 50 T capacity");

            if (_vm.FurnaceOvertemperature && !_wasOvertemp)
                LogAlarm("OVERTEMPERATURE", "Furnace temperature above safe limit");

            if (_vm.TappingError && !_wasTapError)
                LogAlarm("TAPPING ERROR", "Tap commanded but furnace is empty");

            _wasOverfill = _vm.FurnaceOverfill;
            _wasOvertemp = _vm.FurnaceOvertemperature;
            _wasTapError = _vm.TappingError;
        }

        private void UpdateAlarmTab()
        {
            var red = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            var gray = new SolidColorBrush(Color.FromRgb(68, 68, 68));

            AlarmLedOverfill.Fill = _vm.FurnaceOverfill ? red : gray;
            AlarmLedOvertemp.Fill = _vm.FurnaceOvertemperature ? red : gray;
            AlarmLedTapError.Fill = _vm.TappingError ? red : gray;

            int activeCount = 0;
            foreach (var a in _alarmEvents)
                if (a.Status == "ACTIVE") activeCount++;

            AlarmCountText.Text = activeCount.ToString();
            AlarmCountBadge.Background = activeCount > 0
                ? new SolidColorBrush(Color.FromRgb(204, 34, 34))
                : new SolidColorBrush(Color.FromRgb(68, 68, 68));
        }
 

        private void LogEvent(string type, string description)
        {
            var ev = new HistoryEvent
            {
                Timestamp = DateTime.Now,
                EventType = type,
                EventDescription = description,
                IsAlarm = type == "ALARM"
            };

            _historyEvents.Insert(0, ev);

            HistTotalEvents.Text = _historyEvents.Count.ToString();
            int alarmCount = 0;
            foreach (var e in _historyEvents) if (e.IsAlarm) alarmCount++;
            HistAlarmCount.Text = alarmCount.ToString();
            HistLastTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void LogAlarm(string name, string description)
        {
            _alarmEvents.Insert(0, new AlarmEvent
            {
                Timestamp = DateTime.Now,
                AlarmName = name,
                Description = description,
                Status = "ACTIVE"
            });

            LogEvent("ALARM", $"{name} — {description}");
        }

        private void ClearHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            _historyEvents.Clear();
            HistTotalEvents.Text = "0";
            HistAlarmCount.Text = "0";
            HistLastTime.Text = "—";
        }

        private void AckAllBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var alarm in _alarmEvents)
                alarm.Status = "ACKNOWLEDGED";

            AlarmGrid.Items.Refresh();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _vm.TriggerReset();
            TapSlider.Value = 0;
            CurrentSetpointInput.Text = "0";
            LogEvent("SYSTEM", "Simulator reset");
        }

        private void SetCurrentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(CurrentSetpointInput.Text, out double val))
            {
                _vm.CurrentSetpoint = Math.Max(0, val);
                LogEvent("INFO", $"Current setpoint set to {val:F0} A");
            }
        }

        private void ShowPage(Grid page)
        {
            ProcessPage.Visibility = Visibility.Collapsed;
            ConnectionPage.Visibility = Visibility.Collapsed;
            HistoryPage.Visibility = Visibility.Collapsed;
            AlarmsPage.Visibility = Visibility.Collapsed;
            page.Visibility = Visibility.Visible;

            var gray = (Brush)new BrushConverter().ConvertFrom("#222222");
            ProcessBtn.Background = gray;
            ConnectionBtn.Background = gray;
            HistoryBtn.Background = gray;
            AlarmsBtn.Background = gray;
        }

        private void ProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(ProcessPage);
            ProcessBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray");
        }

        private void ConnectionBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(ConnectionPage);
            ConnectionBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray");
        }

        private void HistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(HistoryPage);
            HistoryBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray");
        }

        private void AlarmsBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowPage(AlarmsPage);
            AlarmsBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray");
        }

    }
}
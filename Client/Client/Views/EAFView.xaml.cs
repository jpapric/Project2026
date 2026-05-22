using Client.Models;
using Client.ViewModel;
using System;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Client.Views
{
    public partial class EAFView : UserControl
    {
        private readonly EAFViewModel _vm;
        private readonly DispatcherTimer _animTimer;

        private double _sparkPhase = 0;
        private double _electrodeCurrentY = 10;

        private const double ElRestY = -5;
        private const double ElActiveY = 90.0;
        private const double ElHeight = 110.0;
        private const double MaxFillH = 173.0;
        private const double MaxEnergy = 500.0;

        public EAFView()
        {
            InitializeComponent();

            _vm = new EAFViewModel();
            DataContext = _vm;

            _vm.StartPolling();
            Loaded += async (s, e) => await LoadPlcConfig();

            _animTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            _animTimer.Tick += (s, e) => DrawFrame();
            _animTimer.Start();
        }

        private void DrawFrame()
        {
            _sparkPhase += 0.18;
            DrawMetricCards();
            DrawElectrodes();
            DrawFurnaceTilt();
            DrawMaterialFill();
            DrawLeds();
            DrawAlarmBanners();
            CurrentTimeText.Text = DateTime.Now.ToString("HH:mm:ss");
            CurrentDateText.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void DrawMetricCards()
        {
            TiltingValue.Text = _vm.ActualTilting.ToString("F1");
            WeightValue.Text = _vm.MaterialWeight.ToString("F1");
            CurrentValue.Text = ((int)_vm.ActualCurrent).ToString();
            TempValue.Text = ((int)_vm.ActualTemperature).ToString();
        }

        private void DrawElectrodes()
        {
            bool melting = _vm.ActualCurrent > 0;
            double targetY = melting ? ElActiveY : ElRestY;

            _electrodeCurrentY += (targetY - _electrodeCurrentY) * 0.08;
            if (Math.Abs(_electrodeCurrentY - targetY) < 0.1)
                _electrodeCurrentY = targetY;

            double sparkY = _electrodeCurrentY + ElHeight + 2;

            Canvas.SetTop(Electrode1, _electrodeCurrentY);
            Canvas.SetTop(Spark1, sparkY);

            Canvas.SetTop(Electrode2, _electrodeCurrentY);
            Canvas.SetTop(Spark2, sparkY);

            Canvas.SetTop(Electrode3, _electrodeCurrentY);
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

        private void DrawFurnaceTilt()
        {
            FurnaceTilt.Angle = _vm.ActualTilting;
        }

        private void DrawMaterialFill()
        {
            double fraction = Math.Max(0, Math.Min(1, _vm.MaterialWeight / 50.0));
            double fillHeight = fraction * MaxFillH;

            MaterialFill.Opacity = fraction > 0.01 ? 0.92 : 0;

            if (fraction > 0.01)
            {
                double left = 23;
                double right = 398;
                double bottom = 194.0;
                double angleRad = _vm.ActualTilting * Math.PI / 180.0;

                double halfWidth = (right - left) / 2.0;
                double dY = halfWidth * Math.Tan(angleRad);

                double centerY = bottom - fillHeight;

                double topLeft = centerY + dY;
                double topRight = centerY - dY;

                topLeft = Math.Max(topLeft, 22);
                topRight = Math.Max(topRight, 22);

                topLeft = Math.Min(topLeft, bottom);
                topRight = Math.Min(topRight, bottom);

                MaterialFill.Points = new PointCollection
                {
                    new System.Windows.Point(left,  topLeft),
                    new System.Windows.Point(right, topRight),
                    new System.Windows.Point(right, bottom),
                    new System.Windows.Point(left,  bottom)
                };
            }
            else
            {
                MaterialFill.Points = new PointCollection();
            }

            Canvas.SetTop(WeightOverlay, Math.Max(194.0 - fillHeight + 6, 166.0));
            WeightOverlay.Text = $"{_vm.MaterialWeight:F1} T  |  {fraction * 100:F0}%";

            double pct = Math.Min(100, (_vm.EnergyConsumed / MaxEnergy) * 100);
            EnergyBar.Value = pct;
            EnergyLabel.Text = $"{_vm.EnergyConsumed:F0} kWh";
            EnergyPctLabel.Text = $"{pct:F0}%";
        }


        private void DrawLeds()
        {
            var green = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            var red = new SolidColorBrush(Color.FromRgb(244, 67, 54));

            LedPlc.Fill = _vm.IsConnected ? green : red;
            LedBackend.Fill = _vm.BackendConnected ? green : red;
            LedDatabase.Fill = _vm.IsConnected ? green : red;

            ConnStatusLed.Fill = _vm.IsConnected ? green : red;
            ConnStatusText.Text = _vm.IsConnected ? "Connected" : "Not connected";

            LedScrap.Fill = _vm.ScrapLoading
                ? green : new SolidColorBrush(Color.FromRgb(68, 68, 68));

            LedTapping.Fill = _vm.TappingActive
                ? new SolidColorBrush(Color.FromRgb(255, 152, 0))
                : new SolidColorBrush(Color.FromRgb(68, 68, 68));

            LoadScrapBtn.IsEnabled = !_vm.FurnaceOverfill;
            LoadScrapBtn.Opacity = _vm.FurnaceOverfill ? 0.4 : 1.0;
        }

        private void DrawAlarmBanners()
        {
            OverfillBanner.Visibility = _vm.FurnaceOverfill ? Visibility.Visible : Visibility.Collapsed;
            OvertempBanner.Visibility = _vm.FurnaceOvertemperature ? Visibility.Visible : Visibility.Collapsed;
            TapErrorBanner.Visibility = _vm.TappingError ? Visibility.Visible : Visibility.Collapsed;
            EmptyBanner.Visibility = _vm.FurnaceEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LoadScrapBtn_Click(object sender, RoutedEventArgs e)
            => _vm.LoadScrapCommand.Execute(null);

        private void TapBtn_Click(object sender, RoutedEventArgs e)
            => _vm.TapCommand.Execute(null);

        private void SetCurrentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(CurrentSetpointInput.Text, out float val))
                _vm.CurrentSetpoint = Math.Max(0, val);
            _vm.SetCurrentCommand.Execute(null);
        }

        private void TapSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TapAngleLabel == null) return;
            TapAngleLabel.Text = $"{TapSlider.Value:F1}°";
            _vm.TapAngleSetpoint = (float)TapSlider.Value;
            _vm.SetAngleCommand.Execute(null);
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            _vm.ResetCommand.Execute(null);
            TapSlider.Value = 0;
            CurrentSetpointInput.Text = "0";
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            string ip = IpInput.Text.Trim();
            string rack = RackInput.Text.Trim();
            string slot = SlotInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(ip))
            {
                MessageBox.Show("Please enter an IP address.");
                return;
            }

            ConnectBtn.IsEnabled = false;
            ConnStatusText.Text = "Connecting...";
            ConnStatusLed.Fill = new SolidColorBrush(Color.FromRgb(255, 152, 0));

            string cpuString = (CpuTypeInput.SelectedItem as ComboBoxItem)?.Content?.ToString();
            PLCDto.CpuType cpu;
            if (cpuString == "S7-1200") cpu = PLCDto.CpuType.S71200;
            else if (cpuString == "S7-400") cpu = PLCDto.CpuType.S7400;
            else if (cpuString == "S7-300") cpu = PLCDto.CpuType.S7300;
            else cpu = PLCDto.CpuType.S71500;

            var plcDto = new PLCDto
            {
                Ip = ip,
                Rack = int.TryParse(rack, out int r) ? r : 0,
                Slot = int.TryParse(slot, out int s) ? s : 1,
                Cpu = cpu
            };
            try
            {
                await Task.Delay(1000);
                _vm.ManuallyDisconnected = false;
                _vm.UpdatePlcCommand.Execute(plcDto);
                //_vm.StartPolling()

                ConnStatusLed.Fill = _vm.IsConnected
                    ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                    : new SolidColorBrush(Color.FromRgb(244, 67, 54));
                ConnStatusText.Text = _vm.IsConnected ? "Connected" : "PLC not reachable";
                PlcAddressText.Text = $"{ip} | Rack {rack} | Slot {slot}";
                CpuText.Text = $"CPU: {cpuString}";
                ConnectBtn.IsEnabled = false;
                DisconnectBtn.IsEnabled = true;
            }
            catch
            {
                ConnStatusLed.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                ConnStatusText.Text = "Connection failed";
                ConnectBtn.IsEnabled = true;
            }
        }
        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            _vm.ManuallyDisconnected = true;
            _vm.IsConnected = false;
            _vm.BackendConnected = false;

            ConnectBtn.IsEnabled = true;
            DisconnectBtn.IsEnabled = false;
            PlcAddressText.Text = "Not connected";
            CpuText.Text = "CPU: —";
        }

        private void ElectrodeDownBtn_Click(object sender, RoutedEventArgs e)
            => _vm.ElectrodesDownCommand.Execute(null);

        private void ElectrodeUpBtn_Click(object sender, RoutedEventArgs e)
            => _vm.ElectrodesUpCommand.Execute(null);

        private async Task LoadPlcConfig()
        {
            try
            {
                var proxy = new Client.Proxies.EAFProxy();
                PLCDto plc = await proxy.GetPlcAsync();
                IpInput.Text = plc.Ip;
                RackInput.Text = plc.Rack.ToString();
                SlotInput.Text = plc.Slot.ToString();

                string cpuName;
                if (plc.Cpu == PLCDto.CpuType.S71200) cpuName = "S7-1200";
                else if (plc.Cpu == PLCDto.CpuType.S7400) cpuName = "S7-400";
                else if (plc.Cpu == PLCDto.CpuType.S7300) cpuName = "S7-300";
                else cpuName = "S7-1500";

                foreach (ComboBoxItem item in CpuTypeInput.Items)
                    if (item.Content.ToString() == cpuName)
                    { CpuTypeInput.SelectedItem = item; break; }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading PLC configuration: {ex.Message}");
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
        { ShowPage(ProcessPage); ProcessBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray"); }

        private void ConnectionBtn_Click(object sender, RoutedEventArgs e)
        { ShowPage(ConnectionPage); ConnectionBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray"); }

        private void HistoryBtn_Click(object sender, RoutedEventArgs e)
        { ShowPage(HistoryPage); HistoryBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray"); }

        private void AlarmsBtn_Click(object sender, RoutedEventArgs e)
        { ShowPage(AlarmsPage); AlarmsBtn.Background = (Brush)new BrushConverter().ConvertFrom("DimGray"); }

    }
}
using Client.Models;
using Client.ViewModel;
using System;
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

        private const double ElRestY = 10.0;
        private const double ElActiveY = 108.0;
        private const double ElHeight = 110.0;
        private const double ElTipH = 16.0;
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

            double tipY = _electrodeCurrentY + ElHeight;
            double sparkY = tipY + ElTipH + 2;

            Canvas.SetTop(Electrode1, _electrodeCurrentY);
            ElTip1.Points = new PointCollection { new Point(148, tipY), new Point(166, tipY), new Point(157, tipY + ElTipH) };
            Canvas.SetTop(Spark1, sparkY);

            Canvas.SetTop(Electrode2, _electrodeCurrentY);
            ElTip2.Points = new PointCollection { new Point(271, tipY), new Point(289, tipY), new Point(280, tipY + ElTipH) };
            Canvas.SetTop(Spark2, sparkY);

            Canvas.SetTop(Electrode3, _electrodeCurrentY);
            ElTip3.Points = new PointCollection { new Point(394, tipY), new Point(412, tipY), new Point(403, tipY + ElTipH) };
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

            MaterialFill.Height = fillHeight;
            MaterialFill.Opacity = fraction > 0.01 ? 0.92 : 0;
            Canvas.SetTop(MaterialFill, 194.0 - fillHeight);

            Canvas.SetTop(WeightOverlay, Math.Max(194.0 - fillHeight + 6, 166.0));
            WeightOverlay.Text = $"{_vm.MaterialWeight:F1} T  |  {fraction * 100:F0}%";

            double pct = Math.Min(100, (_vm.EnergyConsumed / MaxEnergy) * 100);
            EnergyBar.Value = pct;
            EnergyLabel.Text = $"{_vm.EnergyConsumed:F0} kWh";
            EnergyPctLabel.Text = $"{pct:F0}%";
        }

        private void DrawLeds()
        {
            LedPlc.Fill = _vm.IsConnected
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                : new SolidColorBrush(Color.FromRgb(244, 67, 54));

            LedScrap.Fill = _vm.ScrapLoading
                ? new SolidColorBrush(Color.FromRgb(76, 175, 80))
                : new SolidColorBrush(Color.FromRgb(68, 68, 68));

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
                _vm.UpdatePlcCommand.Execute(plcDto);

                ConnStatusLed.Fill = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                ConnStatusText.Text = "Connected";
                PlcAddressText.Text = $"{ip} | Rack {rack} | Slot {slot}";
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
            ConnStatusLed.Fill = new SolidColorBrush(Color.FromRgb(244, 67, 54));
            ConnStatusText.Text = "Not connected";
            ConnectBtn.IsEnabled = true;
            DisconnectBtn.IsEnabled = false;
            PlcAddressText.Text = "Not connected";
        }

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
            catch(Exception ex) {
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
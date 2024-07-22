using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TagTracker.Models;
using Usb.Events;

namespace TagTracker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly TagReaderModel _tagReaderModel = new();
    private readonly IUsbEventWatcher _usbEventWatcher = new UsbEventWatcher();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTagPresent))]
    [NotifyPropertyChangedFor(nameof(IsConnectedXorTagPresent))]
    private string? _currentTagUid;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsConnectedXorTagPresent))]
    private bool _isConnected;

    [ObservableProperty] private int _selectedBaudRateIndex = 6;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectOrDisconnectCommand))]
    private int _selectedSerialPortIndex;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectOrDisconnectCommand))]
    private string[] _serialPorts = SerialPort.GetPortNames();

    [ObservableProperty] private ObservableCollection<Tag> _tags;

    public MainWindowViewModel()
    {
        _tagReaderModel.TagReceived += OnTagReceived;
        _usbEventWatcher.UsbDeviceAdded += OnUsbDevicesChanged;
        _usbEventWatcher.UsbDeviceRemoved += OnUsbDevicesChanged;

        using var context = new TagContext();
        _tags = new ObservableCollection<Tag>(context.Tags);
    }

    public bool IsTagPresent => CurrentTagUid != null;
    public bool IsConnectedXorTagPresent => IsConnected != IsTagPresent;

    public int[] BaudRates =>
    [
        110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600,
        115200
    ];

    ~MainWindowViewModel()
    {
        _tagReaderModel.Disconnect();
        _usbEventWatcher.Dispose();
    }

    private void OnUsbDevicesChanged(object? sender, UsbDevice e)
    {
        SerialPorts = SerialPort.GetPortNames();
    }

    private void OnTagReceived(string tagUid)
    {
        CurrentTagUid = tagUid;
    }

    [RelayCommand(CanExecute = nameof(CanConnectOrDisconnect))]
    private void ConnectOrDisconnect()
    {
        if (!IsConnected)
            Connect();
        else
            Disconnect();
    }

    private bool CanConnectOrDisconnect()
    {
        return !string.IsNullOrEmpty(
            SerialPorts.ElementAtOrDefault(SelectedSerialPortIndex));
    }

    private void Connect()
    {
        _tagReaderModel.Connect(
            SerialPorts.ElementAtOrDefault(SelectedSerialPortIndex) ??
            string.Empty,
            BaudRates.ElementAtOrDefault(SelectedBaudRateIndex)
        );
        IsConnected = true;
    }

    private void Disconnect()
    {
        _tagReaderModel.Disconnect();
        IsConnected = false;
        CurrentTagUid = null;
    }
}
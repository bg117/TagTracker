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

    [ObservableProperty] private string? _currentTagUid;

    [ObservableProperty] private bool _isTagPresent;

    [ObservableProperty] private int _selectedBaudRateIndex = 6;

    [ObservableProperty] private int _selectedSerialPortIndex;

    [ObservableProperty]
    private string[] _serialPorts = SerialPort.GetPortNames();

    public MainWindowViewModel()
    {
        _tagReaderModel.TagReceived += OnTagReceived;
        Program.UsbEventWatcher.UsbDeviceAdded += OnUsbDevicesChanged;
        Program.UsbEventWatcher.UsbDeviceRemoved += OnUsbDevicesChanged;
    }


    public int[] BaudRates =>
    [
        110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600,
        115200
    ];

    private void OnUsbDevicesChanged(object? sender, UsbDevice e)
    {
        SerialPorts = SerialPort.GetPortNames();
    }


    private void OnTagReceived(string tagUid)
    {
        CurrentTagUid = tagUid;
        IsTagPresent = true;
    }

    [RelayCommand]
    private void Connect()
    {
        _tagReaderModel.Connect(
            SerialPorts.ElementAtOrDefault(SelectedSerialPortIndex) ??
            string.Empty,
            BaudRates.ElementAtOrDefault(SelectedBaudRateIndex)
        );
    }
}
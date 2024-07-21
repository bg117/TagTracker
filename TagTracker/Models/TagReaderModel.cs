using System.Collections.Generic;
using System.IO.Ports;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using Usb.Events;

namespace TagTracker.Models;

public partial class TagReaderModel : ObservableObject
{
    private readonly SerialPort _serialPort = new();
    private int _baudRate;

    private string? _currentTagUid;

    private string _portName = string.Empty;

    public TagReaderModel()
    {
        Program.UsbEventWatcher.UsbDeviceAdded += OnUsbDevicesChanged;
        Program.UsbEventWatcher.UsbDeviceRemoved += OnUsbDevicesChanged;
    }

    public IEnumerable<string> SerialPorts
        => SerialPort.GetPortNames();

    public string PortName
    {
        get => _portName;
        set => SetProperty(ref _portName, value);
    }

    public int BaudRate
    {
        get => _baudRate;
        set => SetProperty(ref _baudRate, value);
    }

    public string? CurrentTagUid
    {
        get => _currentTagUid;
        set => SetProperty(ref _currentTagUid, value);
    }

    private void OnUsbDevicesChanged(object? sender, UsbDevice e)
    {
        OnPropertyChanged(nameof(SerialPorts));
    }

    public void Connect()
    {
        // check if the port is already open
        Disconnect();

        _serialPort.PortName = PortName;
        _serialPort.BaudRate = BaudRate;
        _serialPort.DataReceived += OnDataReceived;
        _serialPort.Open();
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        // search for the pattern [A-Za-z0-9]{8|16|20} in the string
        var input = _serialPort.ReadExisting();
        var regex = UidRegex();
        var match = regex.Match(input ?? string.Empty);

        if (match.Success)
            CurrentTagUid = match.Value;
    }

    public void Disconnect()
    {
        if (_serialPort.IsOpen)
            _serialPort.Close();
    }

    [GeneratedRegex(@"[0-9A-Fa-f]{8}|[0-9A-Fa-f]{14}|[0-9A-Fa-f]{29}")]
    private static partial Regex UidRegex();
}
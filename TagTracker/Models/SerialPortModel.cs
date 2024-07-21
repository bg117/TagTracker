using System.Collections.Generic;
using System.IO.Ports;
using CommunityToolkit.Mvvm.ComponentModel;
using Usb.Events;

namespace TagTracker.Models;

public class SerialPortModel : ObservableObject
{
    private int _baudRate;

    private string _portName = string.Empty;

    private readonly SerialPort _serialPort = new();

    public SerialPortModel()
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

    private void OnUsbDevicesChanged(object? sender, UsbDevice e)
    {
        OnPropertyChanged(nameof(SerialPorts));
    }

    public void Connect()
    {
        // check if the port is already open
        if (_serialPort.IsOpen)
            _serialPort.Close();

        _serialPort.PortName = PortName;
        _serialPort.BaudRate = BaudRate;
        _serialPort.Open();
    }
}
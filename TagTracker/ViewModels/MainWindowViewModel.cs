using System.Collections.Generic;
using System.ComponentModel;
using TagTracker.Models;

namespace TagTracker.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly SerialPortModel _serialPortModel = new();

    private int _selectedBaudRateIndex;

    private int _selectedSerialPortIndex;

    public MainWindowViewModel()
    {
        _serialPortModel.PropertyChanged += OnSerialPortsChanged;
    }

    public IEnumerable<string> SerialPorts => _serialPortModel.SerialPorts;

    public IEnumerable<int> BaudRates =>
    [
        110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600,
        115200
    ];

    public int SelectedSerialPortIndex
    {
        get => _selectedSerialPortIndex;
        set => SetProperty(ref _selectedSerialPortIndex, value);
    }

    public int SelectedBaudRateIndex
    {
        get => _selectedBaudRateIndex;
        set => SetProperty(ref _selectedBaudRateIndex, value);
    }

    private void OnSerialPortsChanged(object? sender,
        PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_serialPortModel.SerialPorts))
            OnPropertyChanged(nameof(SerialPorts));
    }
}
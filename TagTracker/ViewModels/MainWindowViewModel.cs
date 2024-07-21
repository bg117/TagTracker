using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TagTracker.Models;

namespace TagTracker.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly TagReaderModel _tagReaderModel = new();

    private int _selectedBaudRateIndex = 6;

    private int _selectedSerialPortIndex;

    public MainWindowViewModel()
    {
        _tagReaderModel.PropertyChanged += OnTagDataChanged;
    }

    public IEnumerable<string> SerialPorts => _tagReaderModel.SerialPorts;

    public IEnumerable<int> BaudRates =>
    [
        110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600,
        115200
    ];

    public int SelectedSerialPortIndex
    {
        get => _selectedSerialPortIndex;
        set
        {
            SetProperty(ref _selectedSerialPortIndex, value);
            _tagReaderModel.PortName =
                SerialPorts.ElementAt(_selectedSerialPortIndex);
        }
    }

    public int SelectedBaudRateIndex
    {
        get => _selectedBaudRateIndex;
        set
        {
            SetProperty(ref _selectedBaudRateIndex, value);
            _tagReaderModel.BaudRate =
                BaudRates.ElementAt(_selectedBaudRateIndex);
        }
    }

    public bool IsTagPresent => CurrentTagUid != null;
    public string? CurrentTagUid => _tagReaderModel.CurrentTagUid;

    private void OnTagDataChanged(object? sender,
        PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_tagReaderModel.SerialPorts):
                OnPropertyChanged(nameof(SerialPorts));
                break;
            case nameof(_tagReaderModel.CurrentTagUid):
                OnPropertyChanged(nameof(CurrentTagUid));
                OnPropertyChanged(nameof(IsTagPresent));
                break;
        }
    }
}
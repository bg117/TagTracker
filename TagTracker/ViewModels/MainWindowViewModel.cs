using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using TagTracker.Models;
using Usb.Events;

namespace TagTracker.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly TagContext      _tagContext      = new();
    private readonly TagReaderModel  _tagReaderModel  = new();
    private readonly UsbEventWatcher _usbEventWatcher = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTagPresent))]
    [NotifyPropertyChangedFor(nameof(IsConnectedXorTagPresent))]
    private string? _currentTagUid;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpsertTagCommand))]
    private string _fullName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsConnectedXorTagPresent))]
    private bool _isConnected;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(UpsertTagCommand))]
    private string _lrn = string.Empty;

    [ObservableProperty]
    private int _selectedBaudRateIndex = 6;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectOrDisconnectCommand))]
    private int _selectedSerialPortIndex;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConnectOrDisconnectCommand))]
    private string[] _serialPorts = SerialPort.GetPortNames();

    [ObservableProperty]
    private ObservableCollection<Tag> _tags = [];

    public MainWindowViewModel()
    {
        _tagReaderModel.TagReceived       += OnTagReceived;
        _usbEventWatcher.UsbDeviceAdded   += OnUsbDevicesChanged;
        _usbEventWatcher.UsbDeviceRemoved += OnUsbDevicesChanged;

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await _tagContext.Tags.LoadAsync();
            Tags = _tagContext.Tags.Local.ToObservableCollection();
        });
    }

    public bool IsTagPresent             => CurrentTagUid != null;
    public bool IsConnectedXorTagPresent => IsConnected   != IsTagPresent;

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

    private async void OnUsbDevicesChanged(object? sender, UsbDevice e)
    {
        await Dispatcher.UIThread.InvokeAsync(() => SerialPorts =
                                                        SerialPort.GetPortNames());
    }

    private async void OnTagReceived(string tagUid)
    {
        CurrentTagUid = tagUid;

        // check if tag is in database; if yes, set FullName and Lrn
        // if not, string.Empty both
        var tag = await _tagContext.Tags
                                   .FirstOrDefaultAsync(t => t.Uid == tagUid);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            FullName = tag?.FullName ?? string.Empty;
            Lrn      = tag?.Lrn      ?? string.Empty;
        });
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
        return !string.IsNullOrEmpty(SerialPorts
                                        .ElementAtOrDefault(SelectedSerialPortIndex));
    }

    private void Connect()
    {
        _tagReaderModel
           .Connect(SerialPorts.ElementAtOrDefault(SelectedSerialPortIndex) ?? string.Empty,
                    BaudRates.ElementAtOrDefault(SelectedBaudRateIndex));
        IsConnected = true;
    }

    private void Disconnect()
    {
        _tagReaderModel.Disconnect();
        IsConnected   = false;
        CurrentTagUid = null;
    }

    [RelayCommand(CanExecute = nameof(CanUpsertTag))]
    private async Task UpsertTag()
    {
        var tag = new Tag
                  {
                      Uid = CurrentTagUid ?? string.Empty, FullName = FullName, Lrn = Lrn
                  };

        // if tag is already in database, update it; if not, add it
        if (Tags.Any(t => t.Uid == tag.Uid))
        {
            var entry = _tagContext.Tags.Single(t => t.Uid == tag.Uid);
            entry.FullName = tag.FullName;
            entry.Lrn      = tag.Lrn;

            // remove and re-add to update the UI
            Tags.Remove(tag);
            Tags.Add(tag);
        }
        else
        {
            _tagContext.Tags.Add(tag); // automagically updates the ObservableCollection
        }

        await _tagContext.SaveChangesAsync();
    }

    private bool CanUpsertTag()
    {
        return !string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(Lrn);
    }
}

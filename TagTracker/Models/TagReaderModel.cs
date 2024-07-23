using System.IO.Ports;
using System.Text.RegularExpressions;

namespace TagTracker.Models;

public delegate void TagReceivedEventHandler(string tagUid);

public delegate void DataReceivedEventHandler(string data);

public partial class TagReaderModel
{
    private readonly SerialPort _serialPort = new();

    public event TagReceivedEventHandler?  TagReceived;
    public event DataReceivedEventHandler? DataReceived;

    public void Connect(string portName, int baudRate)
    {
        // check if the port is already open
        Disconnect();

        _serialPort.PortName     =  portName;
        _serialPort.BaudRate     =  baudRate;
        _serialPort.DataReceived += OnDataReceived;
        _serialPort.Open();
    }

    private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        // search for the pattern [A-Za-z0-9]{8|16|20} in the string
        var input = _serialPort.ReadExisting();

        // send data to event handler
        DataReceived?.Invoke(input);

        var regex = UidRegex();
        var match = regex.Match(input ?? string.Empty);

        if (match.Success)
            TagReceived?.Invoke(match.Value);
    }

    public void Disconnect()
    {
        if (_serialPort.IsOpen)
            _serialPort.Close();
    }

    [GeneratedRegex(@"[0-9A-Fa-f]{8}|[0-9A-Fa-f]{14}|[0-9A-Fa-f]{29}")]
    private static partial Regex UidRegex();
}

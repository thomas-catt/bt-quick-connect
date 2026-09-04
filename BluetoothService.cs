using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BtQuickConnect;

sealed class BluetoothService
{
    private readonly string _searchString;
    private DeviceInformation? _deviceInfo;

    public string? DeviceName { get; private set; }

    public BluetoothService(string searchString)
    {
        _searchString = searchString;
    }

    /// <summary>
    /// Searches paired Bluetooth devices for one whose name contains the search string.
    /// </summary>
    public async Task<bool> FindDeviceAsync()
    {
        var selector = BluetoothDevice.GetDeviceSelectorFromPairingState(true);
        var devices = await DeviceInformation.FindAllAsync(selector);

        foreach (var device in devices)
        {
            if (device.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            {
                _deviceInfo = device;
                DeviceName = device.Name;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Connects to the previously found Bluetooth device.
    /// Requesting RFCOMM services forces the Bluetooth stack to establish
    /// the underlying ACL link, which in turn triggers profile-level
    /// connections (A2DP, HFP, etc.) managed by Windows.
    /// </summary>
    public async Task<bool> ConnectAsync()
    {
        try
        {
            if (_deviceInfo == null && !await FindDeviceAsync())
                return false;

            using var device = await BluetoothDevice.FromIdAsync(_deviceInfo!.Id);

            if (device == null)
                return false;

            // Requesting services with Uncached mode forces a live connection
            var result = await device.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);

            return device.ConnectionStatus == BluetoothConnectionStatus.Connected
                || result.Services.Count > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

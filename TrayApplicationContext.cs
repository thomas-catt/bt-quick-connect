using System.Drawing.Drawing2D;

namespace BtQuickConnect;

sealed class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly BluetoothService _bluetooth;
    private readonly string _searchString;
    private bool _connecting;

    public TrayApplicationContext(string searchString)
    {
        _searchString = searchString;
        _bluetooth = new BluetoothService(searchString);

        _notifyIcon = new NotifyIcon
        {
            Icon = CreateBluetoothIcon(),
            Text = $"BT Quick Connect: {searchString}",
            Visible = true,
            ContextMenuStrip = CreateContextMenu()
        };

        _notifyIcon.MouseClick += OnTrayIconClick;

        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        var found = await _bluetooth.FindDeviceAsync();

        if (found)
        {
            _notifyIcon.Text = $"BT: {_bluetooth.DeviceName} (click to connect)";
            ShowBalloon($"Found: {_bluetooth.DeviceName}\nClick to connect.", ToolTipIcon.Info);
        }
        else
        {
            ShowBalloon($"No paired device matching '{_searchString}' found.", ToolTipIcon.Warning);
        }
    }

    private async void OnTrayIconClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left || _connecting)
            return;

        await ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        if (_connecting)
            return;

        _connecting = true;

        try
        {
            ShowBalloon("Connecting...", ToolTipIcon.Info);

            var success = await _bluetooth.ConnectAsync();

            if (success)
            {
                ShowBalloon($"Connected to {_bluetooth.DeviceName}.", ToolTipIcon.Info);
            }
            else
            {
                ShowBalloon("Connection failed. Is the device on and in range?", ToolTipIcon.Error);
            }
        }
        finally
        {
            _connecting = false;
        }
    }

    private void ShowBalloon(string text, ToolTipIcon icon)
    {
        _notifyIcon.ShowBalloonTip(3000, "BT Quick Connect", text, icon);
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();

        menu.Items.Add("Connect", null, async (_, _) => await ConnectAsync());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) =>
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        });

        return menu;
    }

    private static Icon CreateBluetoothIcon()
    {
        const int size = 32;
        var bitmap = new Bitmap(size, size);

        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            // Blue circle background
            using var bgBrush = new SolidBrush(Color.FromArgb(0, 120, 215));
            g.FillEllipse(bgBrush, 1, 1, size - 2, size - 2);

            // White Bluetooth symbol
            using var pen = new Pen(Color.White, 2.2f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round,
                LineJoin = LineJoin.Round
            };

            // Vertical center line
            g.DrawLine(pen, 16, 5, 16, 27);

            // Upper chevron: bottom-left → top-right → top-center
            g.DrawLines(pen, [new PointF(10, 21), new PointF(22, 10), new PointF(16, 5)]);

            // Lower chevron: top-left → bottom-right → bottom-center
            g.DrawLines(pen, [new PointF(10, 11), new PointF(22, 22), new PointF(16, 27)]);
        }

        return Icon.FromHandle(bitmap.GetHicon());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
        }

        base.Dispose(disposing);
    }
}

namespace BtQuickConnect;

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            MessageBox.Show(
                "Usage: BtQuickConnect.exe <device-name-search-string>\n\n" +
                "Provide a search string to match against paired Bluetooth device names.\n" +
                "Click the tray icon to connect to the matched device.",
                "BT Quick Connect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var searchString = string.Join(" ", args);

        ApplicationConfiguration.Initialize();
        Application.Run(new TrayApplicationContext(searchString));
    }
}

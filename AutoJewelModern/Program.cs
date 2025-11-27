namespace AutoJewelModern;

static class Program
{
    /// <summary>
    /// The main entry point for the AutoJewel application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
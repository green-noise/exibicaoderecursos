using System; // Provides basic functions and types.
using System.Windows.Forms; // Provides classes for creating Windows-based applications with a graphical user interface.

namespace exibição_de_recursos_CS // Defines a namespace for the project.
{
    static class Program // Static class for the main entry point of the application.
    {
        [STAThread] // Specifies that the COM threading model for the application is single-threaded apartment.
        static void Main() // Main entry point of the application.
        {
            Application.EnableVisualStyles(); // Enables visual styles for the application.
            Application.SetCompatibleTextRenderingDefault(false); // Sets text rendering to be compatible with older versions.
            Application.Run(new MainForm()); // Runs the main form of the application.
        }
    }
}
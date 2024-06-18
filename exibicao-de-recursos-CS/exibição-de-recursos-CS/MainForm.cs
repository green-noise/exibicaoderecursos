using System; // Provides basic functions and types.
using System.Diagnostics; // Provides classes for interacting with system processes and performance counters.
using System.Drawing; // Provides access to GDI+ basic graphics functionality.
using System.Linq; // Provides LINQ functionality for querying collections.
using System.Management; // Provides access to a rich set of management information and management events.
using System.Runtime.InteropServices; // Provides a collection of methods for interacting with unmanaged code.
using System.Windows.Forms; // Provides classes for creating Windows-based applications with a graphical user interface.

namespace exibição_de_recursos_CS // Defines a namespace for the project.
{
    public partial class MainForm : Form // MainForm inherits from the Form class, providing GUI capabilities.
    {
        private Timer updateTimer; // Timer to periodically update resource information.
        private PerformanceCounter cpuCounter; // Performance counter for CPU usage.
        private PerformanceCounter ramCounter; // Performance counter for available RAM.
        private PerformanceCounter diskReadCounter; // Performance counter for disk read operations.
        private PerformanceCounter diskWriteCounter; // Performance counter for disk write operations.
        private PerformanceCounter processCpuCounter; // Performance counter for focused window's CPU usage.
        private PerformanceCounter processDiskReadCounter; // Performance counter for focused window's disk read operations.
        private PerformanceCounter processDiskWriteCounter; // Performance counter for focused window's disk write operations.
        private bool trackSystemResources; // Flag to determine whether to track system resources or focused window resources.
        private string focusedWindowTitle; // Title of the currently focused window.
        private Process focusedWindowProcess; // Process associated with the currently focused window.

        public MainForm() // Constructor for MainForm.
        {
            InitializeComponent(); // Initializes the components on the form.
            InitializePerformanceCounters(); // Initializes the performance counters for system-wide resources.
            InitializeUpdateTimer(); // Initializes the timer to update resource information.
            ShowTrackingOptionDialog(); // Shows a dialog to ask the user if they want to track system resources or focused window resources.
        }

        private void InitializePerformanceCounters() // Initializes the performance counters for system-wide resources.
        {
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"); // CPU usage counter for all processors.
            ramCounter = new PerformanceCounter("Memory", "Available MBytes"); // Available RAM counter.
            diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total"); // Disk read operations counter for all disks.
            diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total"); // Disk write operations counter for all disks.
        }

        private void InitializeUpdateTimer() // Initializes the timer to update resource information.
        {
            updateTimer = new Timer(); // Creates a new Timer object.
            updateTimer.Interval = 1000; // Sets the timer interval to 1000 milliseconds (1 second).
            updateTimer.Tick += new EventHandler(OnUpdateTimerTick); // Attaches the event handler for the Tick event.
            updateTimer.Start(); // Starts the timer.
        }

        private void ShowTrackingOptionDialog() // Shows a dialog to ask the user if they want to track system resources or focused window resources.
        {
            var result = MessageBox.Show("Do you want to track System Resources? Click 'No' to track the currently focused window.", "Tracking Option", MessageBoxButtons.YesNo); // Displays a message box with Yes and No options.
            trackSystemResources = (result == DialogResult.Yes); // Sets the flag based on the user's choice.
        }

        private void OnUpdateTimerTick(object sender, EventArgs e) // Event handler for the timer's Tick event.
        {
            if (trackSystemResources) // Checks if the user chose to track system resources.
            {
                this.Invalidate(); // Invalidates the form, causing it to be repainted.
            }
            else // If the user chose to track the focused window.
            {
                TrackFocusedWindow(); // Tracks the focused window's resources.
                this.Invalidate(); // Invalidates the form, causing it to be repainted.
            }
        }

        private void OnPaint(object sender, PaintEventArgs e) // Event handler for the form's Paint event.
        {
            var graphics = e.Graphics; // Gets the Graphics object for drawing on the form.
            graphics.Clear(Color.Black); // Clears the form with a black background.

            string infoText = trackSystemResources ? GetSystemResourceInfo() : GetFocusedWindowInfo(); // Gets the appropriate resource information based on the user's choice.
            using (Brush textBrush = new SolidBrush(Color.White)) // Creates a white brush for drawing text.
            using (Font textFont = new Font("Arial", 10)) // Creates a font for drawing text.
            {
                graphics.DrawString(infoText, textFont, textBrush, new PointF(10, 10)); // Draws the resource information on the form.
            }
        }

        private string GetSystemResourceInfo() // Retrieves and formats system resource information.
        {
            string ntKernelVersion = GetNTKernelVersion(); // Gets the NT Kernel version.
            float cpuUsage = cpuCounter.NextValue(); // Gets the current CPU usage.
            float availableRam = ramCounter.NextValue(); // Gets the available RAM.
            string gpuInfo = GetGPUInfo(); // Gets the GPU information.
            float diskRead = diskReadCounter.NextValue() / 1024 / 1024; // Gets the disk read speed in MB/s.
            float diskWrite = diskWriteCounter.NextValue() / 1024 / 1024; // Gets the disk write speed in MB/s.

            return $"NT Kernel Version: {ntKernelVersion}\n" +
                   $"CPU Usage: {cpuUsage:F2}%\n" +
                   $"Available RAM: {availableRam:F2} MB\n" +
                   $"{gpuInfo}\n" +
                   $"Disk Read: {diskRead:F2} MB/s\n" +
                   $"Disk Write: {diskWrite:F2} MB/s"; // Formats the resource information as a string.
        }

        private string GetNTKernelVersion() // Retrieves the NT Kernel version.
        {
            return Environment.OSVersion.VersionString; // Returns the OS version string.
        }

        private string GetGPUInfo() // Retrieves and formats GPU information.
        {
            string gpuInfo = "GPU Info:\n"; // Initializes the GPU information string.
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_VideoController"); // Creates a searcher for GPU information.
                foreach (ManagementObject obj in searcher.Get()) // Iterates through the GPU information objects.
                {
                    gpuInfo += $"Name: {obj["Name"]}\n"; // Adds the GPU name to the string.
                    gpuInfo += $"Driver Version: {obj["DriverVersion"]}\n"; // Adds the GPU driver version to the string.
                    gpuInfo += $"Status: {obj["Status"]}\n"; // Adds the GPU status to the string.
                    gpuInfo += $"Current Refresh Rate: {obj["CurrentRefreshRate"]} Hz\n"; // Adds the GPU refresh rate to the string.
                }
            }
            catch (Exception ex) // Catches any exceptions that occur during the search.
            {
                gpuInfo += $"Error: {ex.Message}\n"; // Adds the error message to the string.
            }

            return gpuInfo; // Returns the formatted GPU information string.
        }

        private void TrackFocusedWindow() // Tracks the resources of the currently focused window.
        {
            var handle = GetForegroundWindow(); // Gets the handle of the currently focused window.
            if (handle != IntPtr.Zero) // Checks if the handle is valid.
            {
                var length = GetWindowTextLength(handle); // Gets the length of the window's title text.
                var stringBuilder = new System.Text.StringBuilder(length + 1); // Creates a StringBuilder to hold the title text.
                GetWindowText(handle, stringBuilder, stringBuilder.Capacity); // Gets the window's title text.
                focusedWindowTitle = stringBuilder.ToString(); // Stores the window's title text.

                uint processId; // Variable to hold the process ID.
                GetWindowThreadProcessId(handle, out processId); // Gets the process ID of the window's process.
                focusedWindowProcess = Process.GetProcessById((int)processId); // Gets the process associated with the window.
                InitializeProcessPerformanceCounters(focusedWindowProcess); // Initializes the performance counters for the process.
            }
        }

        private void InitializeProcessPerformanceCounters(Process process) // Initializes the performance counters for the focused window's process.
        {
            processCpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName); // CPU usage counter for the process.
            processDiskReadCounter = new PerformanceCounter("Process", "IO Read Bytes/sec", process.ProcessName); // Disk read operations counter for the process.
            processDiskWriteCounter = new PerformanceCounter("Process", "IO Write Bytes/sec", process.ProcessName); // Disk write operations counter for the process.
        }

        private string GetFocusedWindowInfo() // Retrieves and formats resource information for the focused window.
        {
            if (focusedWindowProcess == null) // Checks if the process is valid.
            {
                return $"Focused Window: {focusedWindowTitle}\n"; // Returns the window title if the process is not valid.
            }

            string windowTitle = focusedWindowTitle; // Gets the window title.
            string ntKernelVersion = GetNTKernelVersion(); // Gets the NT Kernel version.
            float cpuUsage = processCpuCounter.NextValue() / Environment.ProcessorCount; // Gets the CPU usage of the process.
            long memoryUsage = focusedWindowProcess.WorkingSet64 / 1024 / 1024; // Gets the memory usage of the process in MB.
            float diskRead = processDiskReadCounter.NextValue() / 1024 / 1024; // Gets the disk read speed of the process in MB/s.
            float diskWrite = processDiskWriteCounter.NextValue() / 1024 / 1024; // Gets the disk write speed of the process in MB/s.
            string gpuInfo = GetGPUInfo(); // Gets the GPU information.

            return $"NT Kernel Version: {ntKernelVersion}\n" +
                   $"Focused Window: {windowTitle}\n" +
                   $"CPU Usage: {cpuUsage:F2}%\n" +
                   $"Memory Usage: {memoryUsage} MB\n" +
                   $"Disk Read: {diskRead:F2} MB/s\n" +
                   $"Disk Write: {diskWrite:F2} MB/s\n" +
                   $"{gpuInfo}"; // Formats the resource information as a string.
        }

        [DllImport("user32.dll")] // Import the user32.dll for accessing unmanaged functions.
        private static extern IntPtr GetForegroundWindow(); // Declaration for getting the handle of the foreground window.

        [DllImport("user32.dll", SetLastError = true)] // Import the user32.dll for accessing unmanaged functions.
        private static extern int GetWindowTextLength(IntPtr hWnd); // Declaration for getting the length of the window's title text.

        [DllImport("user32.dll", SetLastError = true)] // Import the user32.dll for accessing unmanaged functions.
        private static extern int GetWindowText(IntPtr hWnd, System.Text.StringBuilder lpString, int nMaxCount); // Declaration for getting the window's title text.

        [DllImport("user32.dll")] // Import the user32.dll for accessing unmanaged functions.
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId); // Declaration for getting the process ID of the window's process.
    }
}
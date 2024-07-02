using System;
using System.Threading;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;

// This script keeps the cursor active when you move it slightly
// after x minutes of inactivity, preventing the system from going into idle state.

//TODO Separate class
class Program
{

    // ----CUSTOM COMPLETE---- //
    private static readonly int startHour = 9;
    private static readonly int finishHour = 18;

    private static readonly int startPauseHour = 13;
    private static readonly int finishPauseHour = 14;
    // ---------------------- //

    static async Task Main(string[] args)
    {
        await SendMessageTelegramAsync(GetSystemInfo());

        while (true)
        {
            await IsValidTimePeriodAsync(startHour, finishHour, startPauseHour, finishPauseHour);
            await Task.Delay(1000);
        }
    }

    static async Task IsValidTimePeriodAsync(int startHour, int finishHour, int startPauseHour, int finishPauseHour)
    {
        await WaitUntilWeekdayAsync();

        int currentHour = DateTime.Now.Hour;
        if ((currentHour >= startHour) && (currentHour < finishHour))
        {
            if (!PauseExecutionAsync(currentHour, startPauseHour, finishPauseHour))
            {
                PressScrollLockKey();
            }
        }
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

    static bool PauseExecutionAsync(int currentHour, int startPauseHour, int finishPauseHour)
    {
        return (currentHour >= startPauseHour && currentHour < finishPauseHour);
    }

    static void PressScrollLockKey()
    {
        SendKeys.SendWait("{SCROLLLOCK}");
        Thread.Sleep(1000);
    }

    static async Task SendMessageTelegramAsync(string textToSend)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"https://api.telegram.org/botCOMPLETE:COMPLETE/sendMessage?parse_mode=markdown&chat_id=COMPLETE&text={Uri.EscapeDataString(textToSend)}";
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

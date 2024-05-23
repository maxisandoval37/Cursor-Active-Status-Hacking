using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

//This script keeps the cursor active when you move it slightly
//after x minutes of inactivity, preventing the system from going into idle state.

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
    static private readonly int timer = 1;
    static private readonly int startHour = 9;
    static private readonly int finishHour = 18;
    // ---------------------- //

    static void Main()
    {
        POINT lastPos;
        GetCursorPos(out lastPos);
        DateTime lastMoved = DateTime.Now;

        while (true)
        {
            int currentHour = DateTime.Now.Hour;
            if (currentHour < startHour || currentHour >= finishHour)
            {
                // Case: Outside of permitted hours. The program will stop.
                break; // Exit the loop and finish execution.
            }

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
                    MoveMouseSlightly();
                    lastMoved = DateTime.Now;
                    GetCursorPos(out lastPos); // Update last position after move
                }
            }

            Thread.Sleep(1000);
        }
    }

    static void MoveMouseSlightly()
    {
        POINT currentPos;
        GetCursorPos(out currentPos);

        Cursor.Position = new System.Drawing.Point(currentPos.X + 1, currentPos.Y);
        Thread.Sleep(50); // Short delay
        Cursor.Position = new System.Drawing.Point(currentPos.X - 1, currentPos.Y);
    }
}
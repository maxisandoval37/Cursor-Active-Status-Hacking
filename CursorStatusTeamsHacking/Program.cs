using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

//Este script mantiene el cursor activo al moverlo ligeramente
//después de x minutos de inactividad, evitando que el sistema entre en estado inactivo.
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

    static private readonly int timer = 1;

    static void Main()
    {
        POINT lastPos;
        GetCursorPos(out lastPos);
        DateTime lastMoved = DateTime.Now; //records the last time the cursor moved

        while (true)
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
                    MoveMouseSlightly();
                    lastMoved = DateTime.Now;
                    GetCursorPos(out lastPos); // Update last position after move
                }
            }

            Thread.Sleep(1000); // Check every second
        }
    }

    static void MoveMouseSlightly()
    {
        POINT currentPos;
        GetCursorPos(out currentPos);

        // Move cursor slightly to the right
        Cursor.Position = new System.Drawing.Point(currentPos.X + 1, currentPos.Y);
        Thread.Sleep(50); // Short delay
        // Move cursor slightly to the left
        Cursor.Position = new System.Drawing.Point(currentPos.X - 1, currentPos.Y);
    }
}
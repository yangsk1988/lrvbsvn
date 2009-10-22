using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VirtualBicycle
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Game game = new Game();

            game.Run();

            game.Dispose();
        }
    }
}

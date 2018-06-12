using System;
using Microsoft.Owin.Hosting;
using DetectionAPI;

namespace DetectionAPIConsoleHost
{
    class Program
    {
        /// <summary>
        /// Главная функция программы
        /// </summary>
        /// <param name="args">аргументы командной строки</param>
        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>("http://localhost:8002"))
            {
                Console.WriteLine("API started and ready to process requests...");
                Console.ReadLine();
            }
        }
    }
}

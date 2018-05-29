﻿using System;
using Microsoft.Owin.Hosting;
using DetectionAPI;

namespace DetectionAPIConsoleHost
{
    class Program
    {
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

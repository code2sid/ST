using OpenQA.Selenium;
using System;
using System.Net;

namespace STCrawler
{
    class Program
    {
        public static event ConsoleCancelEventHandler CancelKeyPress;
        static void Main(string[] args)
        {
            ConsoleKeyInfo cki;

            Console.Clear();

            // Establish an event handler to process key press events.
            //Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);

            BrowserDriver m = new BrowserDriver();
            m.setup();
            m.ClickController();
        }

      

    }



}

using System;
using System.Linq;
using STLibs;

namespace STCrawler
{
    class Program
    {
        static ICrawler crawler = (ICrawler)new ST();
        public static bool isScheduled = STConfigurations.Default.ScheduledHour.Split(',').Where(s => DateTime.Now.TimeOfDay.Hours.ToString().Equals(s)).Count() > 0;
        public static string theme = STConfigurations.Default.theme;

        static void Main(string[] args)
        {

            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
            Console.Clear();
            crawler.Setup();
            crawler.ClickController();
        }

        static void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            crawler.CloseAll();
        }



    }



}

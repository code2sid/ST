using STLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdCashCrawler
{
    class Program
    {
        static ICrawler crawler = (ICrawler)new AdCash();
        static void Main(string[] args)
        {
<<<<<<< HEAD
=======
            crawler.Setup();
            crawler.ClickController();
>>>>>>> ec6abb2091b7bd62fd79aa908b263991b54df311

            crawler.Setup();
            crawler.ClickController();
        }
    }
}

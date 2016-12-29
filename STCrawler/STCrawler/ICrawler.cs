using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STCrawler
{
    interface ICrawler
    {
        void Setup();
        void ClickController(bool firstTime = true, string range = "");
        void CloseAll();

    }
}

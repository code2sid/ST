using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NUnit.Framework;
using System.Collections;
using System.Net;
using System.Threading;

namespace STCrawler
{
    
    class BrowserDriver
    {
        public static event ConsoleCancelEventHandler CancelKeyPress;
        private IWebDriver driver;
        public IList<string> allWindowHandles = null;
        string sitePath = "https://www.socialtrade.biz/user/TodayTask179.aspx", input = string.Empty;
        int weLeftOn = 1, iterator = 1;
        int reAttempts = 10;
        //public readonly ProgressBar pb = new ProgressBar();

        public void setup()
        {

            Console.Clear();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);
            ChromeDriverService service = null;

            Console.WriteLine("Which driver (c/m/i)");
            var opt = Console.ReadLine();
            if (opt.ToLower().Contains("m"))
                driver = new FirefoxDriver();
            else if (opt.ToLower().Contains("c"))
            {
                service = ChromeDriverService.CreateDefaultService(STConfigurations.Default.ChromePath);
                service.Port = 90;
                driver = new ChromeDriver(STConfigurations.Default.ChromePath);
            }

            else
                driver = new FirefoxDriver();

            Console.Clear();
        }

        public void ClickController()
        {
            testConnection();

            if (driver.Title.ToLower().Contains("maintenance"))
            {
                Console.WriteLine("its is in under maintenance !!!!! try out after some time... Bbye !!!!");
                Console.ReadLine();
                return;
            }

            login_GetWork();
            Console.Clear();

            var rows = AskOptions();
            Console.Clear();
            try
            {
                if (rows.Count() > 1)
                    ExecuteClicks(rows);
                else
                    ExecuteClicks(strt: weLeftOn = int.Parse(rows[0]), stp: 1, iterator: iterator);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Re-run attempts = {0}", reAttempts = int.Parse(STConfigurations.Default.ReAttempts)));
                testConnection();
                for (int reAttemCntr = 0; reAttemCntr < reAttempts; reAttemCntr++)
                    //ExecuteClicks(strt: weLeftOn, singleTab: true);
                    ExecuteClicks(strt: weLeftOn, stp: 1, iterator: iterator);

            }



        }

        public void testConnection()
        {
            Console.Write("testing Connection...");
            try
            {
                driver.Navigate().GoToUrl(sitePath);
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Not in mood.... Go fuck around somewhere else !!!!");
                Console.ReadLine();
                Environment.Exit(0);
            }



        }

        public void login_GetWork()
        {
            Console.Clear();
            Console.WriteLine("Enter username: ");
            var username = Console.ReadLine();
            var password = string.Empty;

            switch (username.ToLower())
            {
                case "ruchi": { username = "61053682"; password = "smile1510"; input = "784456146"; break; }
                case "rmum": { username = "61081007"; password = "qwert123"; input = "784604577"; break; }
                default:
                    {
                        Console.WriteLine("Enter password: ");
                        password = Utilitiy.ReadLineMasked(); break;
                    }
            }

            Console.Clear();
            Console.Write("Logging-in Please be patient...");

            driver.FindElement(By.XPath("//*[@id='ctl00_ContentPlaceHolder1_txtEmailID']")).SendKeys(username);
            driver.FindElement(By.XPath("//*[@id='ctl00_ContentPlaceHolder1_txtPassword']")).SendKeys(password);
            driver.FindElement(By.Name("ctl00$ContentPlaceHolder1$CndSignIn")).Click();

            Console.Clear();
            Console.Write("Getting todays work...");

            if (Convert.ToInt16(STConfigurations.Default.SkipRequestWork) == 0)
            {
                if (driver.FindElements(By.XPath("//*[@id='w2ui-popup']/div[1]/div")).Count > 0)
                    driver.FindElement(By.XPath("//*[@id='w2ui-popup']/div[1]/div")).Click();

                if (driver.FindElements(By.XPath("//*[@id='w2ui-popup']/div[3]/div")).Count > 0)
                    driver.FindElement(By.XPath("//*[@id='w2ui-popup']/div[3]/div")).Click();
            }

        }

        public void ExecuteClicks(int strt = 1, int stp = 250, bool singleTab = true)
        {
            var linkNo = "0";
            if (!driver.PageSource.Contains("TodayTask"))
                driver.Navigate().GoToUrl(string.Concat(sitePath, "/user/todayTask.aspx"));


            for (int myCntr = strt; myCntr <= stp; myCntr++)
            {
                linkNo = (myCntr + 1).ToString().Length < 2 ? string.Concat("0", myCntr + 1) : (myCntr + 1).ToString();
                Console.WriteLine(string.Format("clicking row:{0} link", myCntr));
                try
                {
                    {
                        //*[@id="hand_745413613"]
                        if (driver.FindElements(By.XPath(string.Format("//*[@id='ctl00_ContentPlaceHolder1_gvAssignment_ctl{0}_Panel4']/a", linkNo))).Count > 0)
                        {
                            driver.FindElement(By.XPath(string.Format("//*[@id='ctl00_ContentPlaceHolder1_gvAssignment_ctl{0}_Panel4']/a", linkNo))).Click();
                            Thread.Sleep(int.Parse(STConfigurations.Default.SleepBy));
                        }
                        else
                            Console.Write("\r Ohh!!! Its already clicked");
                    }

                    CloseAll();

                }
                catch (Exception e)
                {
                    weLeftOn = myCntr--;
                }
            }
        }

        public void ExecuteClicks(int strt, int stp, int iterator)
        {
            var linkNo = "0";
            var tabCnt = 0;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            while (strt != stp)
            {
                linkNo = (strt + (1 * iterator)).ToString().Length < 2 ? string.Concat("0", strt + (1 * iterator)) : (strt + (1 * iterator)).ToString();
                Console.WriteLine(string.Format("clicking row:{0} link", strt));
                try
                {
                    allWindowHandles = driver.WindowHandles;
                    foreach (var item in allWindowHandles)
                    {
                        if (item == driver.CurrentWindowHandle)
                            tabCnt++;
                    }
                    if (tabCnt <= 20)
                    {
                        driver.FindElement(By.TagName("body")).SendKeys(Keys.Control + "t");
                        driver.Navigate().GoToUrl(sitePath);
                        //*[@id="hand_745413454"]/i
                        //*[@id="hand_745413455"]
                        //*[@id="hand_765186898"]
                        //hand_765187218
                        if (driver.FindElements(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Count > 0)
                        {
                            js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                            js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                        }
                    }

                    else
                    {
                        driver.FindElement(By.TagName("body")).SendKeys(Keys.Control + "\t");
                        driver.Navigate().Refresh();
                        if (driver.FindElements(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Count > 0)
                        {
                            js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                            js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                        }
                    }
                }
                catch (Exception e)
                {
                    weLeftOn = strt--;
                    throw;

                }

                strt += (1 * iterator);
            }
        }

        public void ExecuteClicks(string[] rows)
        {
            var linkNo = "0";
            var tabCnt = 0;
            int myCntr = 0;
            foreach (var row in rows)
            {
                myCntr = int.Parse(row);

                linkNo = (myCntr + 1).ToString().Length < 2 ? string.Concat("0", myCntr + 1) : (myCntr + 1).ToString();
                Console.WriteLine(string.Format("clicking row:{0} link", myCntr));
                try
                {
                    allWindowHandles = driver.WindowHandles;
                    foreach (var item in allWindowHandles)
                    {
                        if (item == driver.CurrentWindowHandle)
                            tabCnt++;
                    }
                    if (tabCnt <= 20)
                    {
                        driver.FindElement(By.TagName("body")).SendKeys(Keys.Control + "t");
                        driver.Navigate().GoToUrl(sitePath);

                        if (driver.FindElements(By.XPath(string.Format("//*[@id='ctl00_ContentPlaceHolder1_gvAssignment_ctl{0}_Panel4']/a", linkNo))).Count > 0)
                            driver.FindElement(By.XPath(string.Format("//*[@id='ctl00_ContentPlaceHolder1_gvAssignment_ctl{0}_Panel4']/a", linkNo))).Click();
                    }

                    else
                    {
                        driver.FindElement(By.TagName("body")).SendKeys(Keys.Control + "\t");
                        driver.Navigate().Refresh();
                        if (driver.FindElements(By.XPath(string.Format("//*[@id='ctl00_ContentPlaceHolder1_gvAssignment_ctl{0}_Panel4']/a", linkNo))).Count > 0)
                            driver.FindElement(By.XPath(string.Format("//*[@id='ctl00_ContentPlaceHolder1_gvAssignment_ctl{0}_Panel4']/a", linkNo))).Click();
                    }
                }
                catch (Exception e)
                {
                    weLeftOn = myCntr--;
                    throw;

                }



            }
        }

        public void CloseAll()
        {
            Console.Clear();
            Console.WriteLine("Have a break !!!! have a Kit-kat ;)");
            if (driver.CurrentWindowHandle != null)
                driver.Close();
            Console.ReadLine();

        }

        public string[] AskOptions()
        {
            Console.Write("Pick you option:\n\r 1)Range \n\r 2)Specific Rows ");
            var opt = Console.Read();
            Console.Clear();

            if (!string.IsNullOrEmpty(input) && opt == 1)
            {
                Console.Write(string.Format("The start and iterator Values are: {0} and {1}\n Press 1\0 to continue !!!", input, iterator));
                if (Console.Read() == 0)
                {
                    Console.WriteLine("Enter start point: ");
                    input = Console.ReadLine();
                    Console.WriteLine("Enter iterator value: ");
                    iterator = Console.Read();




                }
            }

            else if (opt == 2)
            {
                Console.WriteLine("Enter ur Row Numbers(separated by space): ");
                input = Console.ReadLine();
            }
            return input.Split(' ');
        }

        protected void myHandler(object sender, ConsoleCancelEventArgs args)
        {
            //Console.WriteLine("Enter option \n\r1) Close \n\r2) New Run");
            //if (Console.ReadLine() == "1")
            //    CloseAll();
            //else
            //    AskOptions();
        }
    }
}

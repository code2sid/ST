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
        private IJavaScriptExecutor js;
        public IList<string> allWindowHandles = null;
        string sitePath = "https://www.socialtrade.biz/user/TodayTask179.aspx", linkNo = string.Empty;
        int weLeftOn = 1, iterator = 1, rownum = 1, reAttempts = 10;

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

            js = (IJavaScriptExecutor)driver;


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
                Console.WriteLine("Re-run attempts = {0}", reAttempts = int.Parse(STConfigurations.Default.ReAttempts));
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
                case "ruchi": { username = "61053682"; password = "smile1510"; rownum = 250; break; }
                case "rmum": { username = "61081007"; password = "qwert123"; rownum = 250; break; }
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

        public string[] AskOptions()
        {
            Console.Write("Pick you option:\n\r 1)Range \n\r 2)Specific Rows ");
            var opt = Console.ReadLine();
            Console.Clear();
            
            driver.Navigate().GoToUrl(sitePath);

            var e = driver.FindElements(By.XPath("//*[@id='dvCustomers']/tr[250]/td[4]/span"));
            if (e.Count > 1)
                linkNo = e[0].GetAttribute("id").Replace("hand_", "");
            if (rownum == 250  && opt.Equals("1"))
            {
                Console.Write("The start and iterator Values are: {0} and {1}\n Press 1\0 to continue !!!", rownum, iterator);
                if (Console.ReadLine().Equals("0"))
                {
                    Console.WriteLine("Enter start point: ");
                    linkNo = Console.ReadLine();
                    Console.WriteLine("Enter iterator value: ");
                    iterator = int.Parse(Console.ReadLine());
                }
            }

            else if (opt.Equals("2"))
            {
                Console.WriteLine("Enter ur Row Numbers(separated by space): ");
                linkNo = Console.ReadLine();
            }
            return linkNo.Split(' ');
        }

        public void ExecuteClicks(int strt = 1, int stp = 250, bool singleTab = true)
        {
            var linkNo = "0";
            if (!driver.PageSource.Contains("TodayTask"))
                driver.Navigate().GoToUrl(string.Concat(sitePath, "/user/todayTask.aspx"));


            for (int myCntr = strt; myCntr <= stp; myCntr++)
            {
                linkNo = (myCntr + 1).ToString().Length < 2 ? string.Concat("0", myCntr + 1) : (myCntr + 1).ToString();
                Console.WriteLine("clicking row:{0} link", myCntr);
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
            while (strt != stp)
            {
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
                        linkNo = driver.FindElements(By.XPath("//*[@id='dvCustomers']/tr[" + strt + "]/td[4]/span"))[0].GetAttribute("id").Replace("hand_", "");
                        if (!string.IsNullOrEmpty(linkNo))
                        {
                            js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                            js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                            Console.WriteLine("clicked row:{0} link", strt);
                        }
                        else
                            Console.WriteLine("row:{0} link is already clicked/have some error ", strt);
                    }

                    else
                    {
                        driver.FindElement(By.TagName("body")).SendKeys(Keys.Control + "\t");
                        driver.Navigate().Refresh();
                        linkNo = driver.FindElements(By.XPath("//*[@id='dvCustomers']/tr[" + strt + "]/td[4]/span"))[0].GetAttribute("id").Replace("hand_", "");
                        if (!string.IsNullOrEmpty(linkNo))
                        {
                            js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                            js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                            Console.WriteLine("clicked row:{0} link", strt);
                        }
                        else
                            Console.WriteLine("row:{0} link is already clicked/have some error ", strt);
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

                linkNo = myCntr.ToString().Length < 2 ? string.Concat("0", myCntr) : myCntr.ToString();
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

                        if (driver.FindElements(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Count > 0)
                        {
                            js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                            js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                            Console.WriteLine("clicked row:{0} link", myCntr);
                        }
                        else
                            Console.WriteLine("row:{0} link is already clicked/have some error ", myCntr);
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
                            Console.WriteLine("clicked row:{0} link", myCntr);
                        }
                        else
                            Console.WriteLine("row:{0} link is already clicked/have some error ", myCntr);
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

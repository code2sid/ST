using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using STLibs;
using System.Threading;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium.Chrome;

namespace AdCashCrawler
{
    public class AdCash : ICrawler
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js;
        public IList<string> allWindowHandles = null;
        string sitePath = ACConfigurations.Default.AC_URL, linkNo = string.Empty, opt = string.Empty;
        int weLeftOn = 1, iterator = 1, reAttempts = 10;
        string hrOfTime = DateTime.Now.TimeOfDay.Hours.ToString();
        bool isScheduled = ACConfigurations.Default.ScheduledHour.Split(',').Where(s => DateTime.Now.TimeOfDay.Hours.ToString().Equals(s)).Count() > 0;

        public void Setup()
        {
            Utilitiy.DeleteTempFiles();
            driver = new FirefoxDriver();
            js = (IJavaScriptExecutor)driver;
            /*opt = "c";
            ChromeDriverService service = null;
            Process chromeProcess = Process.GetProcessesByName("chrome")[0];

            if (chromeProcess != null)
            {

                if (!System.IO.File.Exists(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\chrome.exe"))
                {
                    string chrmFolderSrc = chromeProcess.Modules[0].FileName.Replace("\\chrome.exe", "");
                    string chrmFolderDest = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

                    Utilitiy.DirectoryCopy(chrmFolderSrc, ".", true);
                }

                service = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location));
                service.Port = 90;
                driver = new ChromeDriver(service);
            }*/
        }

        public void TestConnection()
        {
            Console.WriteLine("Testing Connection...");
            try
            {
                driver.Navigate().GoToUrl(sitePath);
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(90));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connection break down!!!!");
                ContiClose();
            }



        }

        public void login_GetWork()
        {
            string username, password;
            username = password = string.Empty;
            linkNo = "250";

            username = ACConfigurations.Default.AC_UsernamePassword.Split('~')[0];
            Console.WriteLine("\n\nIts running for user: {0}", username);
            password = ACConfigurations.Default.AC_UsernamePassword.Split('~')[1];

            List<UserCredentials> admingrp = new List<UserCredentials>();
            admingrp.Add(new UserCredentials { UserId = 61099880, Name = "Nidhi", Password = "nids@1234" });
            admingrp.Add(new UserCredentials { UserId = 61081007, Name = "Mum", Password = "Rbt@1234" });
            admingrp.Add(new UserCredentials { UserId = 61049490, Name = "Anjali", Password = "qwerty@27" });
            admingrp.Add(new UserCredentials { UserId = 61099902, Name = "Disha", Password = "disha@123" });

            if (username.Equals("34119123"))
            {
                Console.WriteLine("Hi Ruchi.. u r admin !!! no need of code");
                string proxyDetails = proxy(admingrp);
                if (!string.IsNullOrWhiteSpace(proxyDetails))
                {
                    username = proxyDetails.Split('~')[0].ToString();
                    password = proxyDetails.Split('~')[1].ToString();
                }
            }
            else
            {
                var user = admingrp.Where(grp => grp.UserId.ToString().Equals(username)).FirstOrDefault();
                if (user != null && !string.IsNullOrEmpty(user.Name))
                    Console.WriteLine("Hi {0}.. u r admin grp!!! no need of code", user.Name);
                else
                    Authentication(username);
            }

            Console.WriteLine("\n\nLogging-in Please be patient...");

            driver.FindElement(By.XPath("//*[@id='contact-form']/div[1]/div[1]/input")).SendKeys(username);
            driver.FindElement(By.XPath("//*[@id='contact-form']/div[1]/div[2]/input")).SendKeys(password);
            driver.FindElement(By.XPath("//*[@id='contact-form']/div[2]/button")).Click();

            Console.WriteLine("\n\nGetting todays work...");

        }

        public string proxy(List<UserCredentials> admingrp)
        {
            Console.WriteLine("Do u want to make a proxy ? y/n");
            var opt = isScheduled ? "n" : Console.ReadLine();
            if (opt.Equals("y"))
            {
                Console.Clear();
                Console.WriteLine("Enter name from admin grp: ");
                var member = Console.ReadLine();
                var adminMember = admingrp.Where(grp => grp.Name.ToLower().Equals(member.ToLower())).FirstOrDefault();

                member = adminMember != null ? string.Join("~", adminMember.UserId.ToString(), adminMember.Password) : string.Empty;
                if (member.Contains("~"))
                    return member;
                else
                {
                    Console.WriteLine("Enter username and password in format: usrnm-pwd:");
                    return Console.ReadLine();
                }
            }
            else
                return string.Empty;
        }

        private void Authentication(string username)
        {
            string sysCode = string.Empty, purchasedCode = string.Empty;
            var dt = Utilitiy.GetNistTime();
            sysCode = string.Format("{0}~01-{1}-{2}~{3}-{1}-{2}", username, dt.ToString("MMM"), dt.Year, DateTime.DaysInMonth(dt.Year, dt.Month));
            purchasedCode = ACConfigurations.Default.Code;
            purchasedCode = Security.Decrypt(purchasedCode, Utilitiy.passKey);

            DateTime codeValidity = Convert.ToDateTime(purchasedCode.Split('~')[2]);

            if (!purchasedCode.Split('~')[0].ToString().Equals(username) || DateTime.Compare(dt, codeValidity) > 0)
            {
                Console.Write("******Please purchase code to run******");
                Console.ReadLine();
                Environment.Exit(0);
            }

        }

        public string[] AskOptions(bool firstTime = true, string range = "")
        {
            Console.Clear();
            return linkNo.Split(' ');
        }

        public void ClickController(bool firstTime = true, string range = "")
        {
            TestConnection();

            if (driver.Title.ToLower().Contains("maintenance"))
            {
                Console.WriteLine("its is in under maintenance !!!!! try out after some time... Bbye !!!!");
                Console.ReadLine();
                return;
            }

            login_GetWork();
            Console.Clear();

            var rows = AskOptions();
            int strt = 1, stp = 60;
            Console.Clear();
            try
            {

                ExecuteClicks(strt: weLeftOn = strt, stp: stp, iterator: iterator);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Re-run attempts = {0}", reAttempts = 10);
                TestConnection();
                for (int reAttemCntr = 0; reAttemCntr < reAttempts; reAttemCntr++)
                    ExecuteClicks(strt: weLeftOn, stp: stp, iterator: iterator);
            }
        }

        public void ExecuteClicks(int strt, int stp, int iterator)
        {
            Console.WriteLine("Started @{0} for {1} to {2}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"), strt, stp);
            stp += (1 * iterator);
            while (strt != stp)
            {
                try
                {
                    allWindowHandles = driver.WindowHandles;
                    if (allWindowHandles.Count > int.Parse(ACConfigurations.Default.ClosePopupsAfter))
                    {
                        Thread.Sleep(5000);
                        allWindowHandles = driver.WindowHandles;
                        for (int i = 1; i < allWindowHandles.Count; i++)
                        {
                            driver.SwitchTo().Window(allWindowHandles[i]);
                            driver.Close();
                        }

                        driver.SwitchTo().Window(allWindowHandles[0]);
                    }
                    if (opt.Equals("c"))
                    {
                        Thread.Sleep(5000);
                        //js.ExecuteScript(string.Format("window.open('{0}', '_blank');", string.Format("{0}?act={1}", sitePath, strt)));
                    }
                    else
                    {
                        driver.FindElement(By.TagName("body")).SendKeys(Keys.Control + "t");
                        driver.Navigate().GoToUrl(string.Format("{0}?act={1}", sitePath, strt));
                    }

                    js.ExecuteScript("$('#submit_box').show()", null);
                    driver.FindElement(By.XPath("//*[@id='submit_box']/input")).Click();
                    Console.WriteLine("clicked link:{0} ", strt);




                }
                catch (Exception e)
                {
                    weLeftOn = strt--;
                    break;
                }

                strt += (1 * iterator);
            }
            Console.WriteLine(strt == stp ? "Task Completed" : "Task breaked at: {0}", strt);
            Console.WriteLine("Completed @{0}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"));
            ContiClose(string.Join(",", strt, stp));
        }

        public void ContiClose(string range = null)
        {
            Console.WriteLine("Do you want to Continue?(y/n)");
            var moreOption = Console.ReadLine();
            if (moreOption.Equals("y"))
                ClickController(false, range);
            else
                CloseAll();
        }


        public void CloseAll()
        {
            Console.Clear();
            Console.WriteLine("Have a break !!!! have a Kit-kat ;)");
            if (driver.CurrentWindowHandle != null)
                driver.Close();
            Console.ReadLine();

        }

    }
}

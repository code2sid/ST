using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using STLibs;
using System.Threading;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Diagnostics;

namespace STCrawler
{

    public class ST : ICrawler
    {
        private IWebDriver driver;
        private IJavaScriptExecutor js;
        public IList<string> allWindowHandles = null;
        string sitePath = STConfigurations.Default.ST_URL, linkNo = string.Empty, opt = string.Empty;
        int weLeftOn = 1, iterator = -1, reAttempts = 10;
        string hrOfTime = DateTime.Now.TimeOfDay.Hours.ToString();


        bool isScheduled = STConfigurations.Default.ScheduledHour.Split(',').Where(s => DateTime.Now.TimeOfDay.Hours.ToString().Equals(s)).Count() > 0;

        public void Setup()
        {
            Utilitiy.DeleteTempFiles();

            ChromeDriverService service = null;

            /*Console.WriteLine("Which driver (c/m)");
            opt = isScheduled ? "m" : Console.ReadLine();
            if (opt.ToLower().Contains("m"))
                driver = new FirefoxDriver();
            else if (opt.ToLower().Contains("c"))
            {
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
                }
            }

            else*/
            driver = new FirefoxDriver();
            js = (IJavaScriptExecutor)driver;


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
                Console.WriteLine("Connection break down!!!! Please try after some time !!!");
            }



        }

        public void login_GetWork()
        {
            string username, password;
            username = password = string.Empty;

            username = STConfigurations.Default.ST_UsernamePassword.Split('~')[0];
            Console.WriteLine("\n\nIts running for user: {0}", username);
            password = STConfigurations.Default.ST_UsernamePassword.Split('~')[1];

            List<UserCredentials> admingrp = new List<UserCredentials>();
            admingrp.Add(new UserCredentials { UserId = 61099880, Name = "Nidhi", Password = "nids@1234" });
            admingrp.Add(new UserCredentials { UserId = 61081007, Name = "Mum", Password = "Rbt@1234" });
            admingrp.Add(new UserCredentials { UserId = 61049490, Name = "Anjali", Password = "qwerty@27" });
            admingrp.Add(new UserCredentials { UserId = 61099902, Name = "Disha", Password = "disha@123" });

            if (username.Equals("61053682") && Program.theme.Equals("sid"))
            {
                Console.WriteLine("Hi Ruchi.. u r admin !!! no need of code");
                string proxyDetails = proxy(admingrp);
                if (!string.IsNullOrWhiteSpace(proxyDetails) && proxyDetails.Split('~')[2].Equals("010786"))
                {
                    username = proxyDetails.Split('~')[0].ToString();
                    password = proxyDetails.Split('~')[1].ToString();
                }
            }
            else if (!username.Equals("61053682"))
            {
                var user = admingrp.Where(grp => grp.UserId.ToString().Equals(username)).FirstOrDefault();
                if (user != null && !string.IsNullOrEmpty(user.Name))
                    Console.WriteLine("Hi {0}.. u r admin grp!!! no need of code", user.Name);
                else
                    Authentication(username);
            }

            Console.WriteLine("\n\nLogging-in Please be patient...");


            driver.FindElement(By.XPath("//*[@id='txtEmailID']")).SendKeys(username);
            driver.FindElement(By.XPath("//*[@id='txtPassword']")).SendKeys(password);
            driver.FindElement(By.Name("CndSignIn")).Click();

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
                    return member + "~010786";
                else
                {
                    string adminpwd = "";


                    while (!adminpwd.Contains("010786") && adminpwd.Length - adminpwd.Replace("~", "").Length != 2)
                    {
                        Console.WriteLine("Enter username and password in format: usrnm-pwd-AdminPwd:");
                        adminpwd = Console.ReadLine();
                        if(adminpwd.Length - adminpwd.Replace("~", "").Length != 2)
                            Console.WriteLine("Please enter in correct format");
                    }

                    return adminpwd;
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
            purchasedCode = STConfigurations.Default.Code;
            try
            {
                purchasedCode = Security.Decrypt(purchasedCode, Utilitiy.passKey);
            }
            catch (Exception e)
            {
                Console.Write("Please purchase code to run");
                Console.ReadLine();
                Environment.Exit(0);
            }

            DateTime codeValidity = Convert.ToDateTime(purchasedCode.Split('~')[2]);

            if (!purchasedCode.Split('~')[0].ToString().Equals(username) || DateTime.Compare(dt, codeValidity) > 0)
            {
                Console.Write("Please purchase code to run");
                Console.ReadLine();
                Environment.Exit(0);
            }

        }

        public string[] AskOptions()
        {
            Console.Clear();
            if (driver.Url.Contains("dashboard.aspx"))
                driver.FindElement(By.XPath("//*[@id='ctl00_ContentPlaceHolder1_UpPanel1']/div/div/div/div/div/div[2]/b/a")).Click();
            //driver.Navigate().GoToUrl(sitePath);

            Console.WriteLine("Please choose following options:1,2,3,4");
            Console.WriteLine("1) Run Crawler from 1 to 250.");
            Console.WriteLine("2) Run Crawler from 250 to 1.");
            Console.WriteLine("3) Run Crawler for Customized Range. ");
            Console.WriteLine("4) Run Crawler for Pendings. ");
            Console.Write("5) Run Crawler for Specific Rows.\n\n Enter your Option (1,2,3,4,5): ");

            var choice = isScheduled ? "1" : Console.ReadLine();
            switch (choice)
            {
                case "1": { linkNo = "1,250"; iterator = 1; break; }
                case "2": { linkNo = "250,1"; iterator = -1; break; }
                case "3":
                    {
                        Console.Write("Enter start stop separated by comma(,) (eg-1,100):  ");
                        linkNo = Console.ReadLine();
                        iterator = 1; break;
                    }
                case "4":
                    {
                        Console.Write("Enter start stop separated by comma(,) (eg-1,100):  ");
                        linkNo = Console.ReadLine();
                        linkNo += ",pendings";
                        break;
                    }

                case "5":
                    {
                        Console.Write("Enter row numbers separated by space( ) (eg-5 10 50 104):  ");
                        linkNo = Console.ReadLine();
                        iterator = 1; break;
                    }
                default:
                    break;
            }
            return linkNo.Split(' ');
        }

        public void ClickController(bool firstTime = true, string range = "")
        {
            if (firstTime)
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
            }
            var rows = string.IsNullOrEmpty(range) ? AskOptions() : new[] { range };
            int strt = 1, stp = 250;
            Console.Clear();
            try
            {
                if (rows.Count() > 1)
                    ExecuteClicks(rows);
                else
                {
                    if (rows[0].Contains(',') && !linkNo.Contains("pendings"))
                    {
                        strt = int.Parse(rows[0].Split(',')[0]);
                        stp = int.Parse(rows[0].Split(',')[1]);
                        ExecuteClicks(strt: weLeftOn = strt, stp: stp, iterator: iterator);
                    }
                    else if (linkNo.Contains("pendings"))
                    {
                        strt = int.Parse(rows[0].Split(',')[0]);
                        stp = int.Parse(rows[0].Split(',')[1]);
                        ExecuteClicks(strt: strt, stp: stp, iterator: 1, spl: "pendings");

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Re-run attempts = {0}", reAttempts = int.Parse(STConfigurations.Default.ReAttempts));
                TestConnection();
                for (int reAttemCntr = 0; reAttemCntr < reAttempts; reAttemCntr++)
                    ExecuteClicks(strt: weLeftOn, stp: stp, iterator: iterator);
            }
        }

        public void ExecuteClicks(int strt, int stp, int iterator, string spl = "")
        {
            Console.WriteLine("Started @{0} for {1} to {2}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"), strt, stp);
            var linkNo = "0";
            stp += (1 * iterator);
            while (strt != stp)
            {
                try
                {
                    linkNo = driver.FindElements(By.XPath(string.Format(STConfigurations.Default.placeholder, strt)))[1].GetAttribute("id").Replace("hand_", "");
                    if (!string.IsNullOrEmpty(linkNo) && !linkNo.Contains("facebook"))
                    {
                        js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                        js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                        if (spl.Equals("pendings")
                            && driver.FindElements(By.XPath(string.Format(STConfigurations.Default.placeholder.Replace("td[4]/span", "td[3]/span"), strt)))[0].GetAttribute("id").Contains("pending"))
                        {
                            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                            Console.WriteLine("clicked row:{0} link", strt);
                        }
                        else if (!spl.Equals("pendings"))
                        {
                            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                            Console.WriteLine("clicked row:{0} link", strt);
                        }
                    }
                    else
                    {
                        ExecuteFlikes(linkNo.Replace("facebook_", ""), driver.FindElements(By.XPath(string.Format(STConfigurations.Default.placeholder, strt)))[1].GetAttribute("link"));
                        Console.WriteLine("clicked row:{0} facebook link", strt);
                    }

                    if (opt.Equals("c")) Thread.Sleep(5000);

                    if (driver.WindowHandles.Count > int.Parse(STConfigurations.Default.ClosePopupsAfter))
                    {
                        Console.WriteLine("Please wait for ({0}) seconds closing popups...", STConfigurations.Default.PopupWaitTiming);
                        Thread.Sleep(1000 * int.Parse(STConfigurations.Default.PopupWaitTiming));
                        allWindowHandles = driver.WindowHandles;
                        for (int i = 1; i < allWindowHandles.Count; i++)
                        {
                            driver.SwitchTo().Window(allWindowHandles[i]);
                            try
                            {
                                driver.Close();
                                Thread.Sleep(1000);
                            }
                            catch (Exception ex)
                            {
                                //nothing to perform and close other....
                            }
                        }
                        strt -= (1 * iterator);
                        driver.SwitchTo().Window(allWindowHandles[0]);
                    }

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
            var r = strt == stp ? null : string.Join(",", strt, stp);
            ContiClose(r);
        }

        public void ExecuteClicks(string[] rows)
        {
            Console.WriteLine("Started @{0}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"));
            var linkNo = "0";
            int myCntr = 0;
            foreach (var row in rows)
            {
                myCntr = int.Parse(row);

                linkNo = myCntr.ToString().Length < 2 ? string.Concat("0", myCntr) : myCntr.ToString();
                try
                {
                    allWindowHandles = driver.WindowHandles;


                    linkNo = driver.FindElements(By.XPath(string.Format(STConfigurations.Default.placeholder, row)))[1].GetAttribute("id").Replace("hand_", "");
                    if (!string.IsNullOrEmpty(linkNo) && !linkNo.Contains("facebook"))
                    {
                        js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                        js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                        driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                        Console.WriteLine("clicked row:{0} link", row);
                    }
                    else
                    {
                        ExecuteFlikes(linkNo.Replace("facebook_", ""), driver.FindElements(By.XPath(string.Format(STConfigurations.Default.placeholder, row)))[1].GetAttribute("link"));
                        Console.WriteLine("clicked row:{0} facebook link", row);
                    }

                    if (opt.Equals("c")) Thread.Sleep(5000);

                    if (allWindowHandles.Count > int.Parse(STConfigurations.Default.ClosePopupsAfter))
                    {
                        Console.WriteLine("Please wait for ({0}) seconds closing popups...", STConfigurations.Default.PopupWaitTiming);
                        Thread.Sleep(1000 * int.Parse(STConfigurations.Default.PopupWaitTiming));
                        allWindowHandles = driver.WindowHandles;
                        for (int i = 1; i < allWindowHandles.Count; i++)
                        {
                            driver.SwitchTo().Window(allWindowHandles[i]);
                            try
                            {
                                driver.Close();
                            }
                            catch (Exception ex)
                            {
                                //nothing to perform and close other....
                            }
                        }

                        driver.SwitchTo().Window(allWindowHandles[0]);
                    }
                }
                catch (Exception e)
                {
                    weLeftOn = myCntr--;
                    break;
                }
            }
            Console.WriteLine(rows[rows.Count() - 1].Equals(myCntr) ? "Task Completed" : "Task breaked at: {0}", myCntr);
            Console.WriteLine("Completed @{0}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"));
            ContiClose(null);
        }

        public void ExecuteFlikes(string id, string link)
        {
            string refresh = string.Format(@"<span class='handIcon' title='Refresh' id='refresh_{0}' 
                               link='{1}' onclick='refreshTask({0}, this)'><i class='fa fa-refresh' aria-hidden='true'></i></span>", id, link);
            js.ExecuteScript(string.Format("$('#action_{0}').html(\"{1}\");", id, refresh), null);
            driver.FindElement(By.XPath(string.Format("//*[@id='refresh_{0}']", id))).Click();
            driver.Navigate().GoToUrl(sitePath);
            driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", id))).Click();
        }

        public void ContiClose(string range = null)
        {
            Console.WriteLine("Do you want to Continue?(y/n)");
            var moreOption = Console.ReadLine();

            var originialHandle = driver.CurrentWindowHandle;
            if (moreOption.Equals("y"))
            {
                ClickController(false, range);
            }
            else
                CloseAll();
        }

        public void CloseAll()
        {
            Console.Clear();
            Console.WriteLine("Closing everything !!!");
            Console.WriteLine("Please wait for ({0}) seconds closing popups...", STConfigurations.Default.PopupWaitTiming);
            Thread.Sleep(1000 * int.Parse(STConfigurations.Default.PopupWaitTiming));
            allWindowHandles = driver.WindowHandles;
            for (int i = 1; i < allWindowHandles.Count; i++)
            {
                driver.SwitchTo().Window(allWindowHandles[i]);
                try
                {
                    driver.Close();
                }
                catch (Exception ex)
                {
                    //nothing to perform and close other....
                }
            }

            Console.WriteLine("Please wait for ({0}) seconds closing popups...", STConfigurations.Default.PopupWaitTiming);
            Thread.Sleep(1000 * int.Parse(STConfigurations.Default.PopupWaitTiming));

            driver.SwitchTo().Window(allWindowHandles[0]);
            driver.Close();
            Console.ReadLine();

        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }



}
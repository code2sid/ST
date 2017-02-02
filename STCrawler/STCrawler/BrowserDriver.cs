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
        bool isManualLogin = false;
        List<UserCredentials> admingrp = new List<UserCredentials>();



        string clickButtonplaceholder = "//*[@id='stpostdiv']/table[2]/tbody/tr[{0}]/td[4]/span[2]/span";

        public void Setup()
        {
            //Utilitiy.DeleteTempFiles();

            ChromeDriverService service = null;

           
            driver = new FirefoxDriver();
            js = (IJavaScriptExecutor)driver;

            admingrp.Add(new UserCredentials { UserId = 61334986, Name = "Ruchi20", Password = "smile@15" });
            admingrp.Add(new UserCredentials { UserId = 61274966, Name = "Anjali20", Password = "qwerty@27" });
            admingrp.Add(new UserCredentials { UserId = 61099880, Name = "Nidhi", Password = "nids@1234" });
            admingrp.Add(new UserCredentials { UserId = 61081007, Name = "Mum", Password = "Rbt@1234" });
            admingrp.Add(new UserCredentials { UserId = 61049490, Name = "Anjali", Password = "qwerty@27" });
            admingrp.Add(new UserCredentials { UserId = 61099902, Name = "Disha", Password = "disha@123" });
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

        public UserCredentials Authorization(string username)
        {
           

            var user = admingrp.Where(grp => grp.UserId.ToString().Equals(username)).FirstOrDefault();
            if (user != null && !string.IsNullOrEmpty(user.Name))
                Console.WriteLine("Hi {0}.. u r admin grp!!! no need of code", user.Name);
            else
                Authentication(username);

            return user;
        }

        public void login_GetWork()
        {
            string username, password;
            username = password = string.Empty;

            username = STConfigurations.Default.ST_UsernamePassword.Split('~')[0];
            Console.WriteLine("\n\nIts running for user: {0}", username);
            password = STConfigurations.Default.ST_UsernamePassword.Split('~')[1];


            if (IsElementPresent(By.XPath(" //*[@id='agreeconditions']")))
            {
                driver.FindElement(By.XPath(" //*[@id='agreeconditions']")).Click();
                Thread.Sleep(2000);
                driver.FindElement(By.XPath(" //*[@id='linkweb']")).Click();
            }

            //*[@id="linkweb"]
            if (IsElementPresent(By.XPath("//*[@id='form1']/div[3]/ul/li[2]/a")))
                driver.FindElement(By.XPath("//*[@id='form1']/div[3]/ul/li[2]/a")).Click();



            if (username.Equals("61053682"))
            {
                Console.WriteLine("Hi Ruchi.. u r admin !!! no need of code");
                string proxyDetails = proxy(admingrp);
                if (!string.IsNullOrWhiteSpace(proxyDetails) && proxyDetails.Split('~')[2].Equals("010786"))
                {
                    username = proxyDetails.Split('~')[0].ToString();
                    password = proxyDetails.Split('~')[1].ToString();
                }
            }

            else
            {
                UserCredentials user = Authorization(username);
                if (user != null)
                {
                    username = user.UserId.ToString();
                    password = user.Password;
                }
            }

            Console.WriteLine("\n\nLogging-in Please be patient...");
            Thread.Sleep(2000);

            try
            {
                driver.FindElement(By.XPath("//*[@id='txtEmailID']")).SendKeys(username);
                Thread.Sleep(1000);
                driver.FindElement(By.XPath("//*[@id='txtPassword']")).SendKeys(password);
                Thread.Sleep(1000);
                driver.FindElement(By.Name("CndSignIn")).Click();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Due to some error please login manually and hit enter");
                Console.ReadLine();
                isManualLogin = true;
            }
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
                    var adminpwd = string.Empty;
                    bool proxyCondition = true;
                    while (proxyCondition)
                    {
                        Console.WriteLine("Enter username and password in format: usrnm-pwd-AdminPwd:");
                        adminpwd = Console.ReadLine();

                        if (adminpwd.Length - adminpwd.Replace("~", "").Length != 2 || !adminpwd.Contains("010786"))
                            Console.WriteLine("Please enter correct password !!!");
                        else
                            proxyCondition = false;

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
            try
            {
                Console.Clear();
                Thread.Sleep(10000);
                driver.Navigate().GoToUrl(sitePath);

                if (isManualLogin)
                {
                    var uname = IsElementPresent(By.XPath("//*[@id='ctl00_lblUserID']")) ? driver.FindElement(By.XPath("//*[@id='ctl00_lblUserID']")).Text : "0000";
                    if (!uname.Equals("61053682")) Authorization(uname);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem in clicking popup or View Advertisements.. please do manually");
                Console.ReadLine();
                Console.Clear();
            }
            Console.WriteLine("Please choose following options:1,2,3,4");
            Console.WriteLine("1) Run Crawler from 1 to 200.");
            Console.WriteLine("2) Run Crawler from 1 to 100.");
            Console.WriteLine("3) Run Crawler for Customized Range. ");
            Console.WriteLine("Enter your Option (1,2,3,4,5): ");

            var choice = isScheduled ? "1" : Console.ReadLine();
            switch (choice)
            {
                case "1": { linkNo = "1,200"; iterator = 1; break; }
                case "2": { linkNo = "1,100"; iterator = 1; break; }
                case "3":
                    {
                        Console.Write("Enter start stop separated by comma(,) (eg-1,100):  ");
                        linkNo = Console.ReadLine();
                        iterator = 1; break;
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
                Console.WriteLine("Re-run attempts = {0}", reAttempts = 10);
                TestConnection();
                for (int reAttemCntr = 0; reAttemCntr < reAttempts; reAttemCntr++)
                    ExecuteClicks(strt: weLeftOn, stp: stp, iterator: iterator);
            }
        }

        public void ExecuteClicks(int strt, int stp, int iterator, string spl = "", bool isMore = false)
        {
            if (IsElementPresent(By.XPath("//*[@id='Ul2']/li[3]/a")))
                driver.FindElement(By.XPath("//*[@id='Ul2']/li[3]/a")).Click();
            else
            {
                Console.WriteLine("Please click Connect wid pages and hit enter");
                Console.ReadLine();
            }
            var uName = IsElementPresent(By.XPath("//*[@id='iduser_profileName']")) ? driver.FindElement(By.XPath("//*[@id='iduser_profileName']")).Text : "NA";
            js.ExecuteScript("$('.like_button').addClass('handIcon');", null);
            int waitCntr = 0;
            int tabCnt = 0;

            Console.WriteLine("Started for Name: {3}  @{0} Range: {1} to {2}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"), strt, stp, uName);
            var linkNo = "0";
            stp += (1 * iterator);

            try
            {
                linkNo = driver.FindElements(By.XPath(string.Format(clickButtonplaceholder, strt)))[0].GetAttribute("id").Replace("hand_", "");
            }
            catch (Exception)
            {
                Console.WriteLine("page is not loaded.. Please manually load it and then hit enter !!!");
                Console.ReadLine();
            }


            while (strt != stp)
            {
                try
                {
                    linkNo = driver.FindElements(By.XPath(string.Format(clickButtonplaceholder, strt)))[0].GetAttribute("id").Replace("hand_", "");
                    if (!string.IsNullOrEmpty(linkNo) && !linkNo.Contains("facebook"))
                    {
                        //js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                        js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                        driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                        Console.WriteLine("clicked row:{0} link", strt);
                        if (waitCntr > 10)
                        {
                            Thread.Sleep(1000 * int.Parse(STConfigurations.Default.WaitAfterClicks));
                            waitCntr = 0;
                        }
                        waitCntr++;

                    }
                    else
                        Console.WriteLine("clicked row:{0} Already Clicked", strt);


                }
                catch (Exception e)
                {
                    weLeftOn = strt--;
                }

                strt += (1 * iterator);
            }
            Console.WriteLine(strt == stp ? "Task Completed" : "Task breaked at: {0}", strt);
            Console.WriteLine("Completed for Name: {1} @{0}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"), uName);

            var r = strt == stp ? null : string.Join(",", strt, stp);
            if (!isMore)
                ContiClose(r);
        }

        public void ExecuteClicks(string[] rows)
        {
            Console.WriteLine("Started @{0}", DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt"));
            var linkNo = "0";
            int myCntr = 0;
            var mainWindow = driver.CurrentWindowHandle;
            foreach (var row in rows)
            {
                myCntr = int.Parse(row);

                linkNo = myCntr.ToString().Length < 2 ? string.Concat("0", myCntr) : myCntr.ToString();
                try
                {

                    linkNo = driver.FindElements(By.XPath(string.Format(clickButtonplaceholder, row)))[1].GetAttribute("id").Replace("hand_", "");
                    if (!string.IsNullOrEmpty(linkNo) && !linkNo.Contains("facebook"))
                    {
                        js.ExecuteScript(string.Format("$('#hand_{0}').addClass('handIcon');", linkNo), null);
                        js.ExecuteScript(string.Format("$('#hand_{0}').attr('onclick','updateTask({0},this)');", linkNo), null);
                        driver.FindElement(By.XPath(string.Format("//*[@id='hand_{0}']", linkNo))).Click();
                        Console.WriteLine("clicked row:{0} link", row);
                    }
                    else
                    {
                        ExecuteFlikes(linkNo.Replace("facebook_", ""), driver.FindElements(By.XPath(string.Format(clickButtonplaceholder, row)))[1].GetAttribute("link"));
                        Console.WriteLine("clicked row:{0} facebook link", row);
                    }

                    if (opt.Equals("c")) Thread.Sleep(5000);

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
            var moreOption = isScheduled ? "n" : Console.ReadLine();

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
            Console.WriteLine("Have a break !!!! have a Kit-kat ;)");
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

                    //do nothing
                }

            }


            driver.SwitchTo().Window(allWindowHandles[0]);
            driver.Close();
            Console.ReadLine();

        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(3));
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }



}

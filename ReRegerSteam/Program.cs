using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V105.IndexedDB;
using SteamAuth;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection.Metadata;

namespace ReRegerSteam
{
    internal class Program
    {
        static IWebDriver browser;
        static bool isCaptchaCreated = true;

        private static string GetGuardCode(string secretKey)
        {
            SteamGuardAccount acc = new SteamGuardAccount();
            acc.SharedSecret = secretKey;
            string codeGuard = acc.GenerateSteamGuardCode();
            return codeGuard;
        }

        static void AcceptAllCookie()
        {
            try
            {
                browser.FindElement(By.XPath("//*[@id=\"acceptAllButton\"]/span")).Click();
            }
            catch { }
        }

        static string GetNickName()
        {
            try
            {
                Thread.Sleep(1000);
                ((IJavaScriptExecutor)browser).ExecuteScript("window.open();");
                browser.SwitchTo().Window(browser.WindowHandles.Last());
                browser.Navigate().GoToUrl("https://names.drycodes.com/1?format=text");
                Thread.Sleep(1000);
                string result = browser.FindElement(By.XPath("/html/body/pre")).Text.Replace('_', ' ');
                browser.Close();
                browser.SwitchTo().Window(browser.WindowHandles.ToArray()[0]);
                Thread.Sleep(1000);
                return result;
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad get nickname process. Type it yourself");
                Console.ResetColor();
                return Console.ReadLine();
            }
           
        }

        static string GetCodeRambler(int letterNum,string windowRambler)
        {
            Thread.Sleep(20000);
            browser.SwitchTo().Window(windowRambler);
            Thread.Sleep(1000);
            browser.Navigate().GoToUrl("https://mail.rambler.ru/folder/INBOX?category=lists");
            Thread.Sleep(2000);

            try
            {
                letterNum = letterNum + 2; //3 это самое первое, 4 второе и тд
                if (browser.FindElement(By.XPath($"//*[@id=\"app\"]/div[2]/div[4]/div[2]/div[2]/div[1]/div/div/div[3]/div/div/div[{letterNum}]/div/a/div[3]/span")).Text.Contains("Steam"))
                {
                    string className = browser.FindElement(By.XPath($"//*[@id=\"app\"]/div[2]/div[4]/div[2]/div[2]/div[1]/div/div/div[3]/div/div/div[{letterNum}]/div/a")).GetAttribute("class");
                    if (className.Contains("unseen"))
                    {
                        string hrefMessage = browser.FindElement(By.XPath($"//*[@id=\"app\"]/div[2]/div[4]/div[2]/div[2]/div[1]/div/div/div[3]/div/div/div[{letterNum}]/div/a")).GetAttribute("href");

                        Thread.Sleep(1000);
                        string lastWindow = browser.WindowHandles.Last();
                        ((IJavaScriptExecutor)browser).ExecuteScript("window.open();");
                        browser.SwitchTo().Window(browser.WindowHandles.Last());
                        browser.Navigate().GoToUrl(hrefMessage);
                        Thread.Sleep(2000);
                        string result = null;
                        // либо ошибка в хпасе для получения кода либо последнее сообщение не с кодом
                        try
                        {
                           result = browser.FindElement(By.XPath("//*[@id=\"part2\"]/div/div/div/div/center[1]/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[1]/td/table/tbody/tr[2]/td/table[3]/tbody/tr/td/table/tbody/tr/td/table/tbody/tr/td")).Text;
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unable to get guard code rambler. Check XPas or another letter");
                            Console.ResetColor();
                            Console.ReadLine();
                        }
                        
                        browser.Close();
                        browser.SwitchTo().Window(lastWindow);
                        return result;
                    }
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad get code rambler process");
                Console.ResetColor();
                Console.ReadLine();
            }            
            return null;
        }

        static string GetCodeProton(string windowProton,int letterNum)
        {
            Thread.Sleep(20000);
            browser.SwitchTo().Window(windowProton);
            Thread.Sleep(1000);
            browser.Navigate().GoToUrl("https://mail.proton.me/u/1/inbox#filter=unread");
            Thread.Sleep(8000);

            try
            {
                int nomerMessage = letterNum; //1 это самое первое, 1 второе и тд
                if (browser.FindElement(By.XPath($"/html/body/div[1]/div[3]/div/div[2]/div/div[2]/div/main/div/div[1]/div/div[2]/div[{nomerMessage}]/div/div/div/div[1]/div[1]/div/span[2]")).Text.Contains("Steam"))
                {
                    browser.FindElement(By.XPath($"/html/body/div[1]/div[3]/div/div[2]/div/div[2]/div/main/div/div/div/div[2]/div[{nomerMessage}]")).Click();

                    Thread.Sleep(3000);
                    string result = null;
                    // либо ошибка в хпасе для получения кода либо последнее сообщение не с кодом
                    try
                    {
                        browser.SwitchTo().ParentFrame();
                        browser.SwitchTo().Frame(browser.FindElement(By.CssSelector("iframe")));

                        result = browser.FindElement(By.XPath("//*[@id=\"proton-root\"]/div[2]/div/div/center[1]/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[1]/td/table/tbody/tr[2]/td/table[3]/tbody/tr/td/table/tbody/tr/td/table/tbody/tr/td")).Text;                                                              
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unable to get guard code proton. Check XPas or another letter");
                        Console.ResetColor();
                        Console.ReadLine();
                    }

                    return result;
                }
                
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad get code RU mail process");
                Console.ResetColor();
                Console.ReadLine();
            }
            return null;
        }

        static string GetCodeMailRu(string windowMail, int letterNum, string url = null)
        {
            Thread.Sleep(15000);
            browser.SwitchTo().Window(windowMail);
            Thread.Sleep(1000);
            browser.Navigate().GoToUrl("https://e.mail.ru/inbox/");
            Thread.Sleep(4000);

            try
            {
                int divNum = 1; //иногда на почтах есть разделение писем на сегодня завтра, а иногда нет, для этого доп дип юзается и 1 меняется на 2
                try
                {
                    browser.FindElement(By.XPath($"//*[@id=\"app-canvas\"]/div/div[1]/div[1]/div/div[2]/span/div[2]/div/div/div/div/div[2]/div/div/div/div[1]/div/div/a[{letterNum}]/div[4]/div/div[1]/span"));
                    divNum = 2;
                }
                catch
                {

                }

                try
                {
                    browser.FindElement(By.XPath("/html/body/div[17]/div[2]/div/div/div/div[1]/svg")).Click();                    
                }
                catch
                {

                }

                if (browser.FindElement(By.XPath($"//*[@id=\"app-canvas\"]/div/div[1]/div[1]/div/div[2]/span/div[2]/div/div/div/div/div[{divNum}]/div/div/div/div[1]/div/div/a[{letterNum}]/div[4]/div/div[1]/span")).Text.Contains("Steam"))
                {
                    browser.FindElement(By.XPath($"//*[@id=\"app-canvas\"]/div/div[1]/div[1]/div/div[2]/span/div[2]/div/div/div/div/div[{divNum}]/div/div/div/div[1]/div/div/a[{letterNum}]")).Click();

                    Thread.Sleep(3000);
                    string result = null;
                    // либо ошибка в хпасе для получения кода либо последнее сообщение не с кодом
                    try
                    {                        
                        result = browser.FindElement(By.XPath("//td[contains(@class,'title-48_mr_css_attr')][contains(@class,'c-blue1_mr_css_attr')][contains(@class,'fw-b_mr_css_attr')][contains(@class,'a-center_mr_css_attr')]")).Text;
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unable to get guard code mail.ru. Check XPas or another letter");
                        Console.ResetColor();
                        Console.ReadLine();
                    }

                    return result;
                }

            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad get code mail.ru process");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
            return null;
        }

        static string GetCodeChina(string windowChinaMail, int letterNum, string url)
        {
            Thread.Sleep(15000);
            browser.SwitchTo().Window(windowChinaMail);
            Thread.Sleep(1000);
            browser.Navigate().GoToUrl($"{url}?_task=mail&_mbox=INBOX");
            Thread.Sleep(4000);

            try
            {
                if (browser.FindElement(By.XPath($"/html/body/div[1]/div[3]/div[4]/table[2]/tbody/tr[{letterNum}]/td[2]/span[1]/span/span")).Text.Contains("Steam"))
                {
                    string className = browser.FindElement(By.XPath($"/html/body/div[1]/div[3]/div[4]/table[2]/tbody/tr[{letterNum}]")).GetAttribute("class");
                    if (className.Contains("unread"))
                    {
                        string hrefMessage = browser.FindElement(By.XPath($"/html/body/div[1]/div[3]/div[4]/table[2]/tbody/tr[{letterNum}]/td[2]/span[3]/a")).GetAttribute("href");
                        Thread.Sleep(1000);
                        string lastWindow = browser.WindowHandles.Last();
                        ((IJavaScriptExecutor)browser).ExecuteScript("window.open();");
                        browser.SwitchTo().Window(browser.WindowHandles.Last());
                        browser.Navigate().GoToUrl(hrefMessage);
                        Thread.Sleep(2000);
                        string result = null;
                        // либо ошибка в хпасе для получения кода либо последнее сообщение не с кодом
                        try
                        {
                            result = browser.FindElement(By.XPath("//*[@id=\"message-htmlpart1\"]/div/center[1]/table/tbody/tr/td/table/tbody/tr/td/table/tbody/tr[1]/td/table/tbody/tr[2]/td/table[3]/tbody/tr/td/table/tbody/tr/td/table/tbody/tr/td")).Text;
                        }
                        catch
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unable to get guard code china. Check XPas or another letter");
                            Console.ResetColor();
                            Console.ReadLine();
                        }

                        browser.Close();
                        browser.SwitchTo().Window(lastWindow);
                        return result;
                    }
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad get code chinese process");
                Console.ResetColor();
                Console.ReadLine();
            }           
            return null;
        }

         static void LoginMailRu(string login, string password, string windowMail)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(windowMail);
            Thread.Sleep(2000);

            try
            {
                browser.FindElement(By.XPath("//*[@id=\"root\"]/div[2]/div/div/div/div/div/form/div[2]/div/div[1]/div/div/div/div/div/div[1]/div/input")).SendKeys(login);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"root\"]/div[2]/div/div/div/div/div/form/div[2]/div/div[3]/div/div/div[1]/button/span")).Click();
                Thread.Sleep(2000);
                browser.FindElement(By.XPath("//*[@id=\"root\"]/div[2]/div/div/div/div/div/form/div[2]/div/div[2]/div/div/div/div/div/input")).SendKeys(password);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"root\"]/div[2]/div/div/div/div/div/form/div[2]/div/div[3]/div/div/div[1]/div/button/span")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad mail.ru login process");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void LoginProtonMail(string login, string password, string windowProtonMail)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(windowProtonMail);
            Thread.Sleep(2000);

            try
            {
                browser.FindElement(By.XPath("//*[@id=\"username\"]")).SendKeys(login);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"password\"]")).SendKeys(password);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("/html/body/div[1]/div[3]/div[1]/div/main/div[2]/form/button")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad proton login process");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void LoginRambler(string login, string password, string windowRambler)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(windowRambler);
            Thread.Sleep(2000);
            try
            {
                browser.FindElement(By.XPath("//*[@id=\"login\"]")).SendKeys(login);
                browser.FindElement(By.XPath("//*[@id=\"password\"]")).SendKeys(password);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"__next\"]/div/div/div[2]/div/div/div/div[1]/form/button/span")).Click();

                Thread.Sleep(3000);
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"__next\"]/div/div/div[2]/div/div/div/div[1]/form/section[2]/div/div/div[2]"));
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bad rambler login or password");
                    Console.ResetColor();
                    Console.ReadLine();
                }
                catch
                {

                }
                IWebElement captcha2 = default;

                //если нет поля, то пропустило без капчи
                try
                {
                    captcha2 = browser.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div/div/div/div[1]/form/section[3]/div/div/div[1]/div/div/div/div[2]"));
                }
                catch
                {
                    isCaptchaCreated = false;
                    return;
                }

                if (captcha2.Text == "Решить с 2Captcha")
                {
                    browser.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div/div/div/div[1]/form/section[3]/div/div/div[1]/div/div/div")).Click();
                }
            }
            catch
            {
                browser.Navigate().Refresh();
                Thread.Sleep(1000);
                LoginRambler(login, password,windowRambler);
                return;
            }

            try
            {
                if (isCaptchaCreated == true)
                {
                    while ("Капча решена!" != browser.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div/div/div/div[1]/form/section[3]/div/div/div[1]/div/div/div/div[2]")).Text)
                    {
                        Thread.Sleep(1000);

                        if ("ERROR_SITEKEY" == browser.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div/div/div/div[1]/form/section[3]/div/div/div[1]/div/div/div/div[2]")).Text)
                        {
                            browser.Navigate().Refresh();
                            Thread.Sleep(1000);
                            LoginRambler(login, password, windowRambler);
                            return;
                        }
                    }
                    Thread.Sleep(1000);
                    browser.FindElement(By.XPath("//*[@id=\"__next\"]/div/div/div[2]/div/div/div/div[1]/form/button/span")).Click();
                }

                Thread.Sleep(2000);
                browser.Navigate().GoToUrl("https://mail.rambler.ru/folder/INBOX?category=lists");
                Thread.Sleep(2000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad captha solve process");
                Console.ResetColor();
                Console.ReadLine();
            }           
        }

        static void LoginChiniseMail(string login, string password,string windowChinaMail)
        {
            try
            {
                Thread.Sleep(1000);
                browser.SwitchTo().Window(windowChinaMail);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"rcmloginuser\"]")).SendKeys(login);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"rcmloginpwd\"]")).SendKeys(password);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"rcmloginsubmit\"]")).Click();
                Thread.Sleep(2000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad china mail login process");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void LoginSteam(string loginSteam, string passwordSteam, string windowSteam, string windowActualMail, GetCode getCodeActualMail, string urlActualMail = null)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(windowSteam);
            Thread.Sleep(1000);
            browser.Navigate().GoToUrl("https://store.steampowered.com/login/?redir=&redir_ssl=1&snr=1_4_660__global-header");
            Thread.Sleep(2000);
            AcceptAllCookie();

            string codeActualMail = null;

            try
            {
                //ввод логина и пароля стим
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/div/form/div[1]/input")).SendKeys(loginSteam);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/div/form/div[2]/input")).SendKeys(passwordSteam);
                Thread.Sleep(1000);

                //клик по кнопке входа
                IWebElement btnSend = browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/div/form/div[4]/button"));
                btnSend.Click();
                Thread.Sleep(7000);

                bool needMailConfirm = true;
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/form/div/div[2]/div/input[1]"));
                }
                catch
                {
                    needMailConfirm = false;
                }

                if (needMailConfirm == false)
                {
                    return;
                }
                codeActualMail = getCodeActualMail(windowActualMail, 1, urlActualMail);
                if (codeActualMail == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[LoginSteam] codeActualMail == null");
                    Console.ResetColor();
                    Console.ReadLine();
                }

                Thread.Sleep(1000);
                browser.SwitchTo().Window(windowSteam);
                Thread.Sleep(2000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Cant enter login and password");
                Console.ResetColor();
                Console.ReadLine();
            }

            try
            {
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/form/div/div[2]/div/input[1]")).SendKeys(codeActualMail);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad steam login or password");
                Console.ResetColor();
                Console.ReadLine();
            }

            Thread.Sleep(3000);
        }

        static void ClearFriends(string steamWindow)
        {
            try
            {
                Thread.Sleep(1000);
                browser.SwitchTo().Window(steamWindow);
                Thread.Sleep(1000);
                browser.Navigate().GoToUrl("https://steamcommunity.com/friends");
                Thread.Sleep(2000);
                AcceptAllCookie();

                browser.FindElement(By.XPath("//*[@id=\"manage_friends_control\"]/span")).Click();
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"manage_friends\"]/div[1]/span/span[2]/a")).Click();
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"manage_friends\"]/div[2]/div/span[1]/span")).Click();
                Thread.Sleep(1000);

                browser.FindElement(By.XPath("//*[@id=\"pagecontent\"]/div[2]/div[1]/a[3]")).Click();
                Thread.Sleep(1000);
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"manage_friends_control\"]/span")).Click();
                }
                catch { }
                Thread.Sleep(1000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad clear friends process");
                Console.ResetColor();
                Console.ReadLine();
            }           
        }

        static void SteamProfileRedact(string nickName,string steamWindow)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(steamWindow);
            Thread.Sleep(2000);
            AcceptAllCookie();

            try
            {
                //захождение в профиль 
                browser.FindElement(By.XPath("//*[@id=\"global_actions\"]/a/img")).Click();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No profile button");
                Console.ResetColor();
                Console.ReadLine();
            }
            
            try
            { 
                //очитска истории ников 
                Thread.Sleep(2000);
                AcceptAllCookie();
                browser.FindElement(By.XPath("//*[@id=\"getnamehistory_arrow\"]")).Click();
            }
            catch //либо ошибка отсутствия хпаса либо акк совсем не настроенный и надо нажать кнопку первичной настройки
            {
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"btn\"]/a/span")).Click();
                    //если нажалось то акк реально не активирован и пойдёт выполнятся код дальше снизу
                    Thread.Sleep(10000);
                    browser.Navigate().Refresh();
                    Thread.Sleep(1000);
                    SteamProfileRedact(nickName,steamWindow);
                    return;
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No profile page");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }

            Thread.Sleep(1000);
            try
            {
                AcceptAllCookie();
                browser.FindElement(By.XPath("//*[@id=\"NamePopupClearAliases\"]/a")).Click();
                Thread.Sleep(2000);
                AcceptAllCookie();
                browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[1]/span")).Click();
            }
            catch (ElementNotInteractableException ex)
            {
                //значит кнопки очистики нет(в штмл коде есть) и старых ников нет
            }
            catch (NoSuchElementException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No clear nickname button"); // в штмл коде нет
                Console.ResetColor();
                Console.ReadLine();
            }

            try
            {
                //кнопка редактирования профиля
                Thread.Sleep(2000);
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div[1]/div[1]/div/div/div/div[3]/div[2]/a/span")).Click();

                //очистка полей на странице и ввод своих данных
                Thread.Sleep(2000);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[3]/div[2]/div[1]/label/div[2]/input")).Clear();
                Thread.Sleep(100);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[3]/div[2]/div[1]/label/div[2]/input")).SendKeys(nickName);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[3]/div[2]/div[2]/label/div[2]/input")).Clear();
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[3]/div[2]/div[3]/label/div[2]/input")).SendKeys(Keys.Control + "a");
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[3]/div[2]/div[3]/label/div[2]/input")).SendKeys(Keys.Delete);
                Thread.Sleep(1000);

                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[4]/div[2]/div/div[2]/div[1]")).Click();
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("/html/body/div[4]/div/div[1]")).Click();

                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[5]/div[2]/textarea")).Clear();
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/form/div[7]/button[1]")).Click();

                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[1]/a[2]")).Click();
                Thread.Sleep(1000);

                // 1-3-5-7 картинки авы на выбор
                Random rnd = new Random();
                int num = rnd.Next(1,5);
                num = (num + num) - 1;

                browser.FindElement(By.XPath($"//*[@id=\"application_root\"]/div[3]/div[2]/div/div[1]/div[4]/div[2]/div/div[{num}]/img")).Click();
                Thread.Sleep(2000);
                browser.FindElement(By.XPath("//*[@id=\"application_root\"]/div[3]/div[2]/div/div[2]/button[1]")).Click();

                Thread.Sleep(1000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad profile reduct procerss");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

         static void SteamChangeMail(string windowSteam,string windowNewMail,string windowOldMail, string loginNewMail, string urlOldMail, GetCode getCodeNewMail, GetCode getCodeOldMail)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(windowSteam);
            Thread.Sleep(1000);

            browser.Navigate().GoToUrl("https://store.steampowered.com/account/");
            Thread.Sleep(2000);
            AcceptAllCookie();

            try
            {
                browser.FindElement(By.XPath("//*[@id=\"main_content\"]/div[2]/div[4]/div[1]/div[3]/a")).Click(); // кнопка смены почты
                Thread.Sleep(2000);
                browser.FindElement(By.XPath("//*[@id=\"wizard_contents\"]/div/a[2]/span")).Click();

                string codeOldMail = getCodeOldMail(windowOldMail, 1, urlOldMail);
                if (codeOldMail == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[SteamChangeMail] codeOldMail == null");
                    Console.ResetColor();
                    Console.ReadLine();
                }

                Thread.Sleep(1000);
                browser.SwitchTo().Window(windowSteam);
                Thread.Sleep(1000);

                browser.FindElement(By.XPath("//*[@id=\"forgot_login_code\"]")).SendKeys(codeOldMail);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"forgot_login_code_form\"]/div[3]/input")).Click();
                Thread.Sleep(2000);

                browser.FindElement(By.XPath("//*[@id=\"email_reset\"]")).SendKeys(loginNewMail);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"change_email_area\"]/input")).Click();

                string codeNewMail = getCodeNewMail(windowNewMail, 1, null);
                if (codeNewMail == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[SteamChangeMail] codeNewMail == null");
                    Console.ResetColor();
                    Console.ReadLine();
                }

                Thread.Sleep(1000);
                browser.SwitchTo().Window(windowSteam);
                Thread.Sleep(1000);

                browser.FindElement(By.XPath("//*[@id=\"email_change_code\"]")).SendKeys(codeNewMail);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"confirm_email_form\"]/div[2]/input")).Click();
                Thread.Sleep(1000);

            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Change steam mail exceprion");
                Console.ResetColor();
                Console.ReadLine();
            }

        }

        static void CheckPrime(string windowSteam)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(windowSteam);
            Thread.Sleep(1000);

            browser.Navigate().GoToUrl("https://store.steampowered.com/app/730/CounterStrike_Global_Offensive/");
            Thread.Sleep(4000);
            AcceptAllCookie();

            try
            {
                //тут может быть плашка увидеть контент 18+. Жать да если она есть, если нет забить
            }
            catch { }

            try
            {
                browser.FindElement(By.XPath("//*[@id=\"btn_add_to_cart_54029\"]/span")).Click();
                Thread.Sleep(4000);                
                //кнопка клика ниже есть только уже у праймов, у нонпраймов другой хпас
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div[1]/div[2]/div[4]/div[1]/div[3]/div[3]/div/span")).Click();
            }
            catch (OpenQA.Selenium.ElementClickInterceptedException)
            {
                //кнопка существует, но нельзя кликнуть, тк уже куплен прайм
            }
            //если кнопка купить для себя есть, значит no such element
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No Prime Status");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }

        }

        static void SteanChangePassword(string windowSteam, string windowNewMail, string passwordNew, GetCode getCodeNewMail)
        {
            Thread.Sleep(1000);
            browser.SwitchTo().Window(windowSteam);
            Thread.Sleep(1000);

            browser.Navigate().GoToUrl("https://store.steampowered.com/account/");
            Thread.Sleep(2000);
            AcceptAllCookie();

            try
            {
                browser.FindElement(By.XPath("//*[@id=\"main_content\"]/div[2]/div[6]/div[1]/div[2]/div[2]/a")).Click();
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"wizard_contents\"]/div/a[2]/span")).Click();
                Thread.Sleep(1000);

                string codeNewMail = getCodeNewMail(windowNewMail, 1, null);
                if (codeNewMail == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[SteanChangePassword] codeNewMail == null");
                    Console.ResetColor();
                    Console.ReadLine();
                }

                Thread.Sleep(1000);
                browser.SwitchTo().Window(windowSteam);
                Thread.Sleep(1000);

                browser.FindElement(By.XPath("//*[@id=\"forgot_login_code\"]")).SendKeys(codeNewMail);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"forgot_login_code_form\"]/div[3]/input")).Click();
                Thread.Sleep(2000);

                browser.FindElement(By.XPath("//*[@id=\"password_reset\"]")).SendKeys(passwordNew);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"password_reset_confirm\"]")).SendKeys(passwordNew);
                Thread.Sleep(1000);
                browser.FindElement(By.XPath("//*[@id=\"change_password_form\"]/div[3]/input")).Click();
                Thread.Sleep(1000);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Change steam password exceprion");
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void SetVariables(string input,ref string loginSteam, ref string passwordSteam, ref string urlOldMail, ref string loginOldMail, ref string passwordOldMail, ref string loginNewMail, ref string passwordNewMail)
        {
            try
            {
                string[] subs = default;
                subs = input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < subs.Length; i++)
                {
                    string[] subs2 = subs[i].Split(';');

                    loginSteam = subs2[0];
                    passwordSteam = subs2[1];
                    urlOldMail = subs2[2];
                    loginOldMail = subs2[3];
                    passwordOldMail = subs2[4];
                    loginNewMail = subs2[5];
                    passwordNewMail = subs2[6];

                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SetVariables] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        delegate string GetCode(string window,int letter, string mailUrl = null);

        static void Main(string[] args)
        {
            if (File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\inputData.txt"))
            {
                string path = $@"{AppDomain.CurrentDomain.BaseDirectory}\inputData.txt";
                string input = "";
                try
                {
                    int count = File.ReadAllLines(path).Length;
                    StreamReader sr = new StreamReader(path);                    
                    while (input != null)
                    {
                        input = sr.ReadLine();

                        string loginSteam = null;
                        string passwordSteam = null;

                        string loginOldMail = null;
                        string passwordOldMail = null;
                        string urlOldMail = null;

                        string loginNewMail = null;
                        string passwordNewMail = null;
                        string urlNewMail = "https://auth.mail.ru/cgi-bin/auth?from=portal";
                        //https://e.mail.ru/login?from=portal
                        //https://id.rambler.ru/login-20/login
                        //https://account.proton.me/login

                        GetCode getCodeOldMail = GetCodeChina;
                        GetCode getCodeNewMail = GetCodeMailRu;
                        SetVariables(input,ref loginSteam, ref passwordSteam, ref urlOldMail, ref loginOldMail, ref passwordOldMail, ref loginNewMail, ref passwordNewMail);

                        // для расширения с капчей
                        //ChromeOptions options = new ChromeOptions();
                        //options.AddExtension("C:\\Users\\gvozd\\Desktop\\extension_3_2_1_0.crx");
                        //options.AddArgument("user-data-dir=C:\\Users\\gvozd\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1");

                        browser = new ChromeDriver(@"C:\Users\gvozd\Desktop"); //2 параметр options если расширение
                        browser.Navigate().GoToUrl("https://store.steampowered.com/login/?redir=&redir_ssl=1&snr=1_4_660__global-header");
                        string windowSteam = browser.WindowHandles.Last();
                        browser.Manage().Window.Maximize();

                        Thread.Sleep(1000);
                        ((IJavaScriptExecutor)browser).ExecuteScript("window.open();");
                        browser.SwitchTo().Window(browser.WindowHandles.Last());
                        browser.Navigate().GoToUrl(urlNewMail);
                        string windowNewMail = browser.WindowHandles.Last();

                        Thread.Sleep(1000);
                        ((IJavaScriptExecutor)browser).ExecuteScript("window.open();");
                        browser.SwitchTo().Window(browser.WindowHandles.Last());
                        browser.Navigate().GoToUrl(urlOldMail);
                        string windowOldMail = browser.WindowHandles.Last();

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Steam login is {loginSteam}");
                        Console.ResetColor();

                        LoginMailRu(loginNewMail, passwordNewMail, windowNewMail);
                        LoginChiniseMail(loginOldMail, passwordOldMail, windowOldMail);
                        LoginSteam(loginSteam, passwordSteam, windowSteam, windowOldMail, getCodeOldMail, urlOldMail);

                        CheckPrime(windowSteam);

                        ClearFriends(windowSteam);
                        SteamProfileRedact(GetNickName(), windowSteam);
                        SteamChangeMail(windowSteam, windowNewMail, windowOldMail, loginNewMail, urlOldMail, getCodeNewMail, getCodeOldMail);
                        SteanChangePassword(windowSteam, windowNewMail, passwordNewMail, getCodeNewMail);

                        Thread.Sleep(1000);
                        LoginSteam(loginSteam, passwordNewMail, windowSteam, windowNewMail, getCodeNewMail);

                        Thread.Sleep(1000);
                        browser.Quit();
                        Thread.Sleep(2000);
                    }
                    sr.Close();
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[SYSTEM] Accounts.txt not found");
                Console.ResetColor();
                Console.ReadLine();
            }

            Console.ReadLine();
        }
    }
}
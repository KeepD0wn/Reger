using MySql.Data.MySqlClient;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V106.Debugger;
using SteamAuth;
using System.Data.Common;

namespace SteamSaleFarm
{
    internal class Program
    {
        static MySqlConnection conn;
        static IWebDriver browser;
        static bool steamEx = false;

        static string GetGuardCode(string secretKey)
        {
            SteamGuardAccount acc = new SteamGuardAccount();
            acc.SharedSecret = secretKey;
            string codeGuard = acc.GenerateSteamGuardCode();
            return codeGuard;
        }

        static void LoginSteam(string login, string password, string guardCode)
        {
            try
            {
                Thread.Sleep(1000);
                //ввод логина и пароля стим
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/div/form/div[1]/input")).SendKeys(login);
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/div/form/div[2]/input")).SendKeys(password);

                //клик по кнопке входа
                IWebElement btnSend = browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/div/form/div[4]/button"));
                btnSend.Click();

                //генерация гвард кода и его ввод
                string code = GetGuardCode(guardCode);
                Thread.Sleep(2000);
                browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div/div[1]/div/div/div/div[2]/form/div/div[2]/div/input[1]")).SendKeys(code);
                Thread.Sleep(2000);
            }
            catch
            {
                if (steamEx == true)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Аккаунт {login} ОШИБКА ВХОДА");
                    Console.ResetColor();
                    return;
                }
                steamEx = true;
                browser.Navigate().Refresh();
                Thread.Sleep(1000);
                LoginSteam(login,password,guardCode);
            }            
        }

        static void Farm(string login, string password, string secretCode)
        {
            try
            {
                Thread.Sleep(1000);
                browser.Navigate().GoToUrl("https://store.steampowered.com/steamawards");
                Thread.Sleep(2000);               

                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_72_534380\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }

                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_73_1592190\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }

                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_74_570\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
                
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_75_648800\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
                
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_76_698670\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
               
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_77_261550\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
                
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_78_493520\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
                
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_79_1061910\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
               
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_80_1182900\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
                
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_81_920210\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
                
                try
                {
                    browser.FindElement(By.XPath("//*[@id=\"option_82_1449850\"]/button/span")).Click();
                    Thread.Sleep(2000);
                    browser.FindElement(By.XPath("/html/body/div[3]/div[3]/div/div[2]/div[2]/span")).Click();
                    Thread.Sleep(2000);
                }
                catch
                {
                    browser.Navigate().Refresh();
                    Thread.Sleep(2000);
                }
                           }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Аккаунт {login} ОШИБКА ГОЛОСОВАНИЯ");
                Console.ResetColor();
                return;
            }
        }

        static void Main(string[] args)
        {
            conn = new MySqlConnection("Server=localhost;port=3306;User Id=root;password=root");
            //ChromeOptions options = new ChromeOptions();
           // options.AddArgument("user-data-dir=C:\\Users\\gvozd\\AppData\\Local\\Google\\Chrome\\User Data\\Profile 1");

            //browser = new ChromeDriver(@"C:\Users\gvozd\Desktop", options);
            //browser.Navigate().GoToUrl("https://store.steampowered.com/login/?redir=&redir_ssl=1&snr=1_4_660__global-header");
            //browser.Manage().Window.Maximize();

            for (int i=1;i<1000;i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Аккаунт номер {i}");
                Console.ResetColor();

                browser = new ChromeDriver($@"C:\Users\{Environment.UserName}\Desktop");
                browser.Navigate().GoToUrl("https://store.steampowered.com/login/?redir=&redir_ssl=1&snr=1_4_660__global-header");
                browser.Manage().Window.Minimize();

                conn.Open();
                var com = new MySqlCommand("USE csgo; " +
                    "select * from accounts where id = @id", conn);
                com.Parameters.AddWithValue("@id", i);

                string login=null;
                string password = null;
                string secretKey = null;

                using (DbDataReader reader = com.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            login = reader.GetString(1);
                            password = reader.GetString(2);
                            secretKey = reader.GetString(3);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                conn.Close();

                LoginSteam(login, password, secretKey);
                Farm(login, password, secretKey);
                steamEx = false;
                browser.Quit();

                Thread.Sleep(1000);
            }

            Console.WriteLine("Done");
        }
    }
}
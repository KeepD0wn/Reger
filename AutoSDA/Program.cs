using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Net;
using System;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Runtime.CompilerServices;
using OpenQA.Selenium.DevTools;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenQA.Selenium.DevTools.V105.IndexedDB;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Tesseract;
using System.Windows.Forms;
using System.Reflection.Metadata;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Reflection;
using System.Linq;
using System.Windows.Documents;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using static System.Windows.Forms.LinkLabel;
using System.Media;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Net.Http;
using TwoCaptcha.Captcha;
using static System.Windows.Forms.DataFormats;
using System.Security.Cryptography;
using AutoSDA.Deserialize_Classes;
using System.Configuration.Provider;
using OpenQA.Selenium.Interactions;
using System.Xml.Linq;

namespace AutoSDA
{
    internal class Program
    {
       // static IWebDriver browser = new ChromeDriver(@"C:\Users\gvozd\Desktop");

        const int WM_KEYDOWN = 0x100;

        const int VK_ENTER = 0x0D;

        const int VK_TAB = 0x09;

        const uint SWP_NOSIZE = 0x0001;

        const uint SWP_NOZORDER = 0x0004;

        const int wmChar = 0x0102;

        private const int KEYEVENTF_EXTENDEDKEY = 1;

        private const int KEYEVENTF_KEYUP = 2;

        public const int KEYEVENTF_KEYDOWN = 0x0000;

        [DllImport("user32.dll")]
        static extern int LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 pid);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern short VkKeyScan(char ch);



        static void LoginMailRu(string login, string password, string windowMail, IWebDriver browser)
        {
            Thread.Sleep(1000);
           // browser.SwitchTo().Window(windowMail);
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
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad mail.ru login process");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static string GetCodeMailRu(string windowMail, int letterNum, IWebDriver browser, string url = null)
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
                    catch(Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unable to get guard code mail.ru. Check XPas or another letter");
                        Console.WriteLine(ex.Message);
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

        static void ConfirmMobileMail(string windowMail, int letterNum, IWebDriver browser, string url = null)
        {
            Thread.Sleep(15000);
            browser.SwitchTo().Window(windowMail);
            Thread.Sleep(1000);
            browser.Navigate().GoToUrl("https://e.mail.ru/inbox/");
            Thread.Sleep(5000);

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

                if (browser.FindElement(By.XPath($"//*[@id=\"app-canvas\"]/div/div[1]/div[1]/div/div[2]/span/div[2]/div/div/div/div/div[{divNum}]/div/div/div/div[1]/div/div/a[{letterNum}]/div[4]/div/div[1]/span")).Text.Contains("Steam"))
                {
                    browser.FindElement(By.XPath($"//*[@id=\"app-canvas\"]/div/div[1]/div[1]/div/div[2]/span/div[2]/div/div/div/div/div[{divNum}]/div/div/div/div[1]/div/div/a[{letterNum}]")).Click();

                    Thread.Sleep(3000);
                    string result = null;
                    // либо ошибка в хпасе для получения кода либо последнее сообщение не с кодом
                    try
                    {
                        // на 1080 элемент почты перекрывает клик кнопки подтверждения номера. Поэтому через js
                        IJavaScriptExecutor js = (IJavaScriptExecutor)browser;
                        js.ExecuteScript("arguments[0].click()", browser.FindElement(By.XPath("//a[contains(@class,'link_mr_css_attr')][contains(@class,'c-grey4_mr_css_attr')]")));
                                             
                        Thread.Sleep(10000);
                        browser.SwitchTo().Window(browser.WindowHandles.Last());
                        try
                        {
                           if(browser.FindElement(By.XPath("//*[@id=\"responsive_page_template_content\"]/div[1]/div/h2")).Text != "ЭЛ. ПОЧТА ПОДТВЕРЖДЕНА")
                           {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Email not confirmed");
                                Console.ResetColor();
                                Console.ReadLine();
                           }
                            
                        }
                        catch
                        {

                        }

                        browser.Close();
                    }
                    catch(Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Unable to confirm mobile mail.ru. Check XPas or another letter");
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                        Console.ReadLine();
                    }
                }

            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Bad confirm phone by mail.ru process");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        private static void LangToEn()
        {
            string lang = "00000409";
            int ret = LoadKeyboardLayout(lang, 1);
            PostMessage(GetForegroundWindow(), 0x50, 1, ret);
        }

        static string GetRequest(string url)
        {
            try
            {
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                httpRequest.Headers["Authorization"] = "Bearer eyJhbGciOiJSUzUxMiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MDE0MzE0NDAsImlhdCI6MTY2OTg5NTQ0MCwicmF5IjoiZjI1OGEzOWUxMjc2ODg5YmY3MDU0ZTNlYmYwNzJhMjIiLCJzdWIiOjEyNjAwODR9.uLRHAzYUqy7ZnhTv32C49TuTT1o1ZAjhPPUiCIRhIKRm-ZNGmgOZNcFm-AYAQJAfp0WENKtQ03zvxkIVEVsC_JZIleb9a6SM_8_LCZ0EQUOge8VwyoeUbOhlFKXjK2AGH6a1cHzsn_iUiaf9p7OCQreB5VuWjBxMFhzPUxHQPYHyP5nIrICz-uXKrIkxZE8k3d2TyD5vk8l3fCwnzgcnXxGpawSopcJS2ikZ41xQpGjfyXw6p5KZqq9E_6RqN7DtIkqJdPuac-73d0XSkGVQRMChNd2F0QJHYjc5CjJwz5myrcw2Lh0a6_hSxZ-7d-pnIFuxopDqIgmMhEJ391a4jw";

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                if (httpResponse.StatusCode.ToString() == "OK")
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return result;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [GetRequest] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
            return null;
        }

        static void TypeText2(IntPtr targer, string text)
        {
            LangToEn();
            Thread.Sleep(100);
            SetForegroundWindow(targer);
            Thread.Sleep(500);
            foreach (char ch in text)
            {
                SendKeys.SendWait(Convert.ToString(ch));
                Thread.Sleep(100);
            }
            Thread.Sleep(500);
            PostMessage(targer, WM_KEYDOWN, VK_TAB, 1);
            Thread.Sleep(500);
            PostMessage(targer, WM_KEYDOWN, VK_TAB, 1);
            Thread.Sleep(500);
            PostMessage(targer, WM_KEYDOWN, VK_ENTER, 1);
        }

        static void TypeText(IntPtr targer, string text)
        {
            LangToEn();
            Thread.Sleep(100);
            SetForegroundWindow(targer);
            Thread.Sleep(500);
            foreach (char ch in text)
            {
                SendKeys.SendWait(Convert.ToString(ch));
                Thread.Sleep(100);
            }
            Thread.Sleep(500);
            PostMessage(targer, WM_KEYDOWN, VK_TAB, 1);
            Thread.Sleep(500);           
            PostMessage(targer, WM_KEYDOWN, VK_ENTER, 1);
        }

        static void StartSDA(ref Process sdaProc,ref int sdaProcId)
        {
            try
            {
                IntPtr windowSDA = FindWindow(null, "Steam Desktop Authenticator");
                if (windowSDA.ToString() != "0" && !listWindowsSDA.Contains(windowSDA.ToString()))
                {
                    GetWindowThreadProcessId(windowSDA, ref sdaProcId);
                    sdaProc = Process.GetProcessById(sdaProcId);
                    listWindowsSDA.Add(windowSDA.ToString());

                    SetForegroundWindow(windowSDA);
                    Thread.Sleep(500);
                    for (int i = 0; i < 5; i++)
                    {
                        PostMessage(windowSDA, WM_KEYDOWN, VK_TAB, 1);
                        Thread.Sleep(500);
                    }
                    PostMessage(windowSDA, WM_KEYDOWN, VK_ENTER, 1);
                    Thread.Sleep(7000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [StartSDA] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }            
        }

        static void LoginSDA(string loginSteam,string password)
        {
            try
            {
                IntPtr windowSteamLoginSDA = FindWindow(null, "Steam Login");
                if (windowSteamLoginSDA.ToString() != "0" && !listWindowsSDA.Contains(windowSteamLoginSDA.ToString()))
                {
                    listWindowsSDA.Add(windowSteamLoginSDA.ToString());
                    SetForegroundWindow(windowSteamLoginSDA);
                    TypeLoginPas(windowSteamLoginSDA, loginSteam, password);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SteamLoginSDA] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }            
        }

        static void TypeLoginPas(IntPtr targer, string login, string password)
        {
            LangToEn();
            Thread.Sleep(100);
            SetForegroundWindow(targer);
            Thread.Sleep(500);
            foreach (char ch in login)
            {
                SendKeys.SendWait(Convert.ToString(ch));
                Thread.Sleep(100);
            }
            Thread.Sleep(500);
            PostMessage(targer, WM_KEYDOWN, VK_TAB, 1);
            Thread.Sleep(500);
            foreach (char ch in password)
            {
                SendKeys.SendWait(Convert.ToString(ch));
                Thread.Sleep(100);
            }
            Thread.Sleep(500);
            PostMessage(targer, WM_KEYDOWN, VK_TAB, 1);
            Thread.Sleep(500);
            PostMessage(targer, WM_KEYDOWN, VK_ENTER, 1);
        }
        
        static void EnterMailCodeSDA(string windowMail, IWebDriver browser)
        {
            try
            {
                IntPtr EnterMailCodeWindow = GetForegroundWindow();
                if (EnterMailCodeWindow.ToString() != "0" && !listWindowsSDA.Contains(EnterMailCodeWindow.ToString()))
                {
                    listWindowsSDA.Add(EnterMailCodeWindow.ToString());
                    string codeMail = GetCodeMailRu(windowMail, 1, browser);
                    TypeText(EnterMailCodeWindow, codeMail);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [EnterMailCodeSDA] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }           

        }

        static void EnterPhoneSDA(string phoneNum)
        {
            try
            {
                IntPtr EnterPhoneWindow = GetForegroundWindow();
                if (EnterPhoneWindow.ToString() != "0" && !listWindowsSDA.Contains(EnterPhoneWindow.ToString()))
                {
                    SendKeys.SendWait("{+}");
                    TypeText(EnterPhoneWindow, phoneNum);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [EnterPhoneSDA] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }            
        }

        static void ConfirmMobileSDA(string windowMail, IWebDriver browser)
        {
            try
            {
                IntPtr OkWindow = GetForegroundWindow();
                if (OkWindow.ToString() != "0" && !listWindowsSDA.Contains(OkWindow.ToString()))
                {
                    ConfirmMobileMail(windowMail, 1, browser);
                    SetForegroundWindow(OkWindow);
                    Thread.Sleep(1000);
                    PostMessage(OkWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(OkWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(OkWindow, WM_KEYDOWN, VK_ENTER, 1);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }                
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [ConfirmMobileSDA] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }            
        }

        static void SkipEncryptSDA()
        {
            try
            {
                IntPtr EncryptWindow = GetForegroundWindow();
                if (EncryptWindow.ToString() != "0" && !listWindowsSDA.Contains(EncryptWindow.ToString()))
                {
                    SetForegroundWindow(EncryptWindow);
                    Thread.Sleep(500);
                    PostMessage(EncryptWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(EncryptWindow, WM_KEYDOWN, VK_ENTER, 1);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }                
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SkipEncryptSDA] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }            
        }

        static void SkipWarningWindow()
        {
            try
            {
                IntPtr WarningWindow = GetForegroundWindow();
                if (WarningWindow.ToString() != "0" && !listWindowsSDA.Contains(WarningWindow.ToString()))
                {
                    SetForegroundWindow(WarningWindow);
                    Thread.Sleep(500);
                    PostMessage(WarningWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(WarningWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(WarningWindow, WM_KEYDOWN, VK_ENTER, 1);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }

            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SkipWarningWindow] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }       
        }

        static async Task<string> SaveRCodeFilteredWindow(string currentDirectory, double screenScalingFactor, int imageCount)
        {
            try
            {
                IntPtr RCodeWindow = GetForegroundWindow();
                if (RCodeWindow.ToString() != "0" && !listWindowsSDA.Contains(RCodeWindow.ToString()))
                {                    
                    SetWindowPos(RCodeWindow, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                    SetForegroundWindow(RCodeWindow);
                    Thread.Sleep(500);
                    Bitmap printscreen = new Bitmap(50, 20);
                    Graphics g;
                    g = Graphics.FromImage(printscreen as System.Drawing.Image);
                    g.CopyFromScreen(Convert.ToInt32(305 * screenScalingFactor), Convert.ToInt32(65 * screenScalingFactor), 0, 0, printscreen.Size);
                    printscreen.Save(@$"{currentDirectory}\stuff\RCode{imageCount}.png", System.Drawing.Imaging.ImageFormat.Png);                    
                    Thread.Sleep(500);

                    Bitmap newImage = new Bitmap(250, 100);
                    Graphics gr = Graphics.FromImage(newImage);
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    System.Drawing.Image img1 = System.Drawing.Image.FromFile(@$"{currentDirectory}\stuff\RCode{imageCount}.png");
                    gr.DrawImage(img1, new System.Drawing.Rectangle(0, 0, 250, 100));
                    newImage.Save(@$"{currentDirectory}\stuff\RCodeHightQual{imageCount}.png", System.Drawing.Imaging.ImageFormat.Png);
                    Thread.Sleep(500);


                    string rCode = await Captcha(imageCount);
                    Console.WriteLine(rCode);
                    rCode = rCode.ToUpper();
                    if (rCode[0] != 'R')
                    {
                        rCode = "R" + rCode;
                    }
                    rCode = rCode.Replace("o", "0");
                    rCode = rCode.Replace("O", "0");
                    Console.WriteLine(rCode);
                    //var ocrengine = new TesseractEngine(@$"{currentDirectory}\tessdata", "eng", EngineMode.Default);
                    //var img = Pix.LoadFromFile(@$"{currentDirectory}\stuff\RCodeHightQual.png");
                    //var res = ocrengine.Process(img);
                    //string rCode = res.GetText();
                    //Console.WriteLine($"before replacement {rCode}");
                    //rCode = rCode.Replace("\n", string.Empty);
                    //rCode = rCode.Replace("O", "0");
                    //rCode = rCode.Replace("o", "0");
                    //rCode = rCode.Replace("T", "7");
                    //rCode = rCode.Replace("D", "0");
                    //rCode = rCode.Replace("I", "1");
                    //rCode = rCode.Replace("i", "1");
                    //rCode = rCode.Replace("v", "7");
                    //rCode = rCode.Replace("V", "7");
                    //Console.WriteLine($"after replacement {rCode}");

                    SetForegroundWindow(RCodeWindow);
                    Thread.Sleep(500);
                    PostMessage(RCodeWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(RCodeWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(RCodeWindow, WM_KEYDOWN, VK_ENTER, 1);
                    Thread.Sleep(5000);
                    return rCode;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();                    
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SaveRCodeWindow] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
            return null;
        }

        static void EnterPhoneCodeSDA(string phoneCode)
        {
            try
            {
                IntPtr EnterPhoneCodeWindow = GetForegroundWindow();
                if (EnterPhoneCodeWindow.ToString() != "0" && !listWindowsSDA.Contains(EnterPhoneCodeWindow.ToString()))
                {
                    TypeText(EnterPhoneCodeWindow, phoneCode);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SaveRCodeWindow] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }            
        }

        static void ReEnterRCode(string rCode)
        {
            try
            {
                IntPtr reEnterRCodeWindow = GetForegroundWindow();
                if (reEnterRCodeWindow.ToString() != "0" && !listWindowsSDA.Contains(reEnterRCodeWindow.ToString()))
                {
                    TypeText(reEnterRCodeWindow, rCode);
                    Thread.Sleep(10000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }
               
               
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SaveRCodeWindow] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void SkipLinkedWindow()
        {
            try
            {
                IntPtr linkedWindow = GetForegroundWindow();
                if (linkedWindow.ToString() != "0" && !listWindowsSDA.Contains(linkedWindow.ToString()))
                {
                    Thread.Sleep(500);
                    PostMessage(linkedWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(linkedWindow, WM_KEYDOWN, VK_TAB, 1);
                    Thread.Sleep(500);
                    PostMessage(linkedWindow, WM_KEYDOWN, VK_ENTER, 1);
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Window has not changed");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [SaveRCodeWindow] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }       
        }

        static void CheckSteamNumberAvailable(ref double balance, ref int countProvider, ref double costProvider, string provider)
        {
            try
            {
                string providersResult = GetRequest("https://5sim.net/v1/guest/prices?country=russia&product=steam");
                RootOperator providers = JsonConvert.DeserializeObject<RootOperator>(providersResult);

                string accountResult = GetRequest("https://5sim.net/v1/user/profile");
                RootAccount account = JsonConvert.DeserializeObject<RootAccount>(accountResult);

                balance = account.balance;
                if (provider == "megafon")
                {
                    countProvider = providers.russia.steam.megafon.count;
                    costProvider = providers.russia.steam.megafon.cost;
                }
                else if (provider == "mts")
                {
                    countProvider = providers.russia.steam.mts.count;
                    costProvider = providers.russia.steam.mts.cost;
                }
                else if (provider == "virtual15")
                {
                    countProvider = providers.russia.steam.virtual15.count;
                    costProvider = providers.russia.steam.virtual15.cost;
                }

                if (balance < costProvider)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("5sim no balance");
                    Console.ResetColor();
                    Console.ReadLine();
                }

                if (countProvider < 10)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("5sim less then 10 numbers");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [CheckSteamNumberAvailable] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void OrderPhone(ref string phoneNumber, ref int phoneId, string provider)
        {
            try
            {
                string buyResult = GetRequest($"https://5sim.net/v1/user/buy/activation/russia/{provider}/steam"); //TODO no free phones бывает ответ и ошибка вылезает
                RootPhone phone = JsonConvert.DeserializeObject<RootPhone>(buyResult);

                phoneNumber = phone.phone;
                phoneId = phone.id;
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [OrederPhone] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void OrderPhoneOnlineSim(ref string phoneNumber, ref int phoneId, string provider)
        {
            try
            {                
                string buyResult = GetRequest($"https://onlinesim.ru/api/getNum.php?PARAMETERS&apikey=FFTsQxu2J14afLT-K2xN7H1b-wHr55zRR-q59tdKvF-6xy98m9LYSFevT3&lang=ru&service=steam&number=true"); //TODO no free phones бывает ответ и ошибка вылезает
                OnlimeSimNumber phone = JsonConvert.DeserializeObject<OnlimeSimNumber>(buyResult);

                phoneNumber = phone.number;
                phoneId = phone.tzid;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [OrederPhone] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static string CheckSms(int phoneId)
        {
            try
            {
                bool isCodeCame = false;

                DateTime end = DateTime.Now.AddSeconds(40);
                while (DateTime.Now < end)
                {
                    Console.WriteLine("Waiting sms code");
                    string smsResult = GetRequest($"https://5sim.net/v1/user/check/{phoneId}");
                    RootSms smsRoot = JsonConvert.DeserializeObject<RootSms>(smsResult);
                    if (smsRoot.sms != null && smsRoot.sms.Any())
                    {
                        isCodeCame = true;
                        return smsRoot.sms.Last<Sms>().code;
                    }
                    Thread.Sleep(5000);
                }

            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [CheckSms] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
            return null;
        }

        static string ReturnSmsOnlineSim(int phoneId)
        {
            try
            {
                bool isCodeCame = false;

                DateTime end = DateTime.Now.AddSeconds(40);
                while (DateTime.Now < end)
                {
                    Console.WriteLine("Waiting sms code");
                    string smsResult = GetRequest($"https://onlinesim.ru/api/getState.php?PARAMETERS&apikey=FFTsQxu2J14afLT-K2xN7H1b-wHr55zRR-q59tdKvF-6xy98m9LYSFevT3&tzid={phoneId}");
                    smsResult = smsResult.Substring(1, smsResult.Length - 2);                    
                    TzIDOnlineSim smsRoot = JsonConvert.DeserializeObject<TzIDOnlineSim>(smsResult);
                    if (smsRoot.msg != null)
                    {
                        isCodeCame = true;                       
                        return smsRoot.msg;
                    }
                    Thread.Sleep(5000);
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [CheckSms] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
            return null;
        }

        static void MoveMaFile(string currentDirectory, string loginSteam)
        {
            try
            {
                Directory.CreateDirectory(@$"{currentDirectory}\mafiles");
                Directory.CreateDirectory(@$"{currentDirectory}\jsons");
                Directory.CreateDirectory(@$"{currentDirectory}\data");
                string[] allfiles = Directory.GetFiles(@$"{currentDirectory}\SDA\maFiles");
                if (allfiles.Count() == 2) //1 это манифест, второй это новосозданный мафайл
                {
                    foreach (string fileAllPath in allfiles)
                    {
                        string fileName = fileAllPath.ToString().Split('\\').Last();
                        if (fileName.Contains("maFile"))
                        {
                            File.Move(fileAllPath, @$"{currentDirectory}\mafiles\{loginSteam}.maFile");
                            File.Copy($@"{currentDirectory}\stuff\ReferenceManifest.json", $@"{currentDirectory}\SDA\maFiles\manifest.json", true); //сброс манифеста
                        }
                    }
                    Thread.Sleep(1000);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(@"MaFile not created");
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [ReplaceMaFile] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }            
        }

        static void CreateJSON(string currentDirectory, string loginSteam, string password)
        {
            try
            {
                File.Copy(@$"{currentDirectory}\stuff\referenceJSON.json", @$"{currentDirectory}\jsons\{loginSteam}.json");

                StreamReader f = new StreamReader(@$"{currentDirectory}\jsons\{loginSteam}.json"); 
                string resultString = null;
                while (!f.EndOfStream)
                {
                    resultString += f.ReadLine();
                }
                f.Close();

                resultString = resultString.Replace("{login}", loginSteam); 
                resultString = resultString.Replace("{password}", password);

                using (FileStream fileStream = File.Open(@$"{currentDirectory}\jsons\{loginSteam}.json", FileMode.Create))
                {
                    using (StreamWriter output = new StreamWriter(fileStream))
                    {
                        output.Write(resultString);
                    }
                }
                Thread.Sleep(1000);
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [CreateJSON] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static void WriteAccData(string currentDirectory, string login, string password, string sharedSecret, string mail, string rCode)
        {
            try
            {
                using (StreamWriter w = new StreamWriter(@$"{currentDirectory}\data\accountData.txt", true))
                {
                    w.WriteLine($"{login}:{password}:{sharedSecret}");
                }

                using (StreamWriter w = new StreamWriter(@$"{currentDirectory}\data\allData.txt", true))
                {
                    w.WriteLine($"{login}:{password}:{mail}:{rCode}");
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [CreateJSON] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static string GetMaFileCode(string currentDirectory, string loginSteam)
        {
            try
            {
                StreamReader f = new StreamReader(@$"{currentDirectory}\mafiles\{loginSteam}.maFile");
                string resultString = null;
                while (!f.EndOfStream)
                {
                    resultString += f.ReadLine();
                }
                f.Close();
                Thread.Sleep(1000);
                return resultString.Split('"')[3];
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [GetMaFileCode] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
            return null;
        }

        static void SetVariables(string input,ref string loginSteam, ref string password, ref string loginMailRu)
        {
            try
            {
                string[] subs = default;
                subs = input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < subs.Length; i++)
                {
                    string[] subs2 = subs[i].Split(';');

                    loginSteam = subs2[0];
                    loginMailRu = subs2[5];
                    password = subs2[6];

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

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117
        }

        static double GetWindowsScreenScalingFactor(bool percentage = true)
        {
            //Create Graphics object from the current windows handle
            Graphics GraphicsObject = Graphics.FromHwnd(IntPtr.Zero);
            //Get Handle to the device context associated with this Graphics object
            IntPtr DeviceContextHandle = GraphicsObject.GetHdc();
            //Call GetDeviceCaps with the Handle to retrieve the Screen Height
            int LogicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
            //Divide the Screen Heights to get the scaling factor and round it to two decimals
            double ScreenScalingFactor = Math.Round((double)PhysicalScreenHeight / (double)LogicalScreenHeight, 2);
            //If requested as percentage - convert it
            if (percentage)
            {
                ScreenScalingFactor *= 100.0;
            }
            //Release the Handle and Dispose of the GraphicsObject object
            GraphicsObject.ReleaseHdc(DeviceContextHandle);
            GraphicsObject.Dispose();
            //Return the Scaling Factor
            return ScreenScalingFactor/100;
        }

        static bool MailCodeNeeded(string currentDirectory, double screenScalingFactor, int imageCount)
        {
            try
            {
                IntPtr needCode = GetForegroundWindow();
                SetWindowPos(needCode, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                SetForegroundWindow(needCode);
                Thread.Sleep(500);

                using (Bitmap printscreen = new Bitmap(330, 55))
                using (Graphics g = Graphics.FromImage(printscreen as System.Drawing.Image))
                {
                    g.CopyFromScreen(Convert.ToInt32(15 * screenScalingFactor), Convert.ToInt32(35 * screenScalingFactor), 0, 0, printscreen.Size);
                    printscreen.Save(@$"{currentDirectory}\stuff\NeedCode{imageCount}.png", System.Drawing.Imaging.ImageFormat.Png);
                }    

                Bitmap newImage = new Bitmap(1650, 275);
                Graphics gr = Graphics.FromImage(newImage);
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                System.Drawing.Image img1 = System.Drawing.Image.FromFile(@$"{currentDirectory}\stuff\NeedCode{imageCount}.png");
                gr.DrawImage(img1, new System.Drawing.Rectangle(0, 0, 1650, 275));
                newImage.Save(@$"{currentDirectory}\stuff\NeedCodeHightRes{imageCount}.png", System.Drawing.Imaging.ImageFormat.Png);
                gr.Dispose();
                newImage.Dispose();
                Thread.Sleep(500);

                var ocrengine = new TesseractEngine(@$"{currentDirectory}\tessdata", "eng", EngineMode.Default);
                var img = Pix.LoadFromFile(@$"{currentDirectory}\stuff\NeedCodeHightRes{imageCount}.png");
                var res = ocrengine.Process(img);
                string message = res.GetText();
                message = message.Replace("\n", string.Empty);

                SetForegroundWindow(needCode);
                Thread.Sleep(1000);

                if (message.Contains("phone number"))
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [CheckGeneralFailure] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }

            return true;
        }

        static void CheckGeneralFailure(string currentDirectory, double screenScalingFactor, Process sdaProc, int imageCount)
        {
            try
            {
                IntPtr GeneralFailure = GetForegroundWindow();
                SetWindowPos(GeneralFailure, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
                SetForegroundWindow(GeneralFailure);
                Thread.Sleep(500);
                Bitmap printscreen = new Bitmap(365, 25);
                using (Graphics g = Graphics.FromImage(printscreen as System.Drawing.Image))
                {
                    g.CopyFromScreen(Convert.ToInt32(10 * screenScalingFactor), Convert.ToInt32(50 * screenScalingFactor), 0, 0, printscreen.Size);
                    printscreen.Save(@$"{currentDirectory}\stuff\GenFailure{imageCount}.png", System.Drawing.Imaging.ImageFormat.Png);
                    Thread.Sleep(500);
                }

                Bitmap newImage = new Bitmap(1825, 125);
                using (Graphics gr = Graphics.FromImage(newImage))
                {
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    System.Drawing.Image img1 = System.Drawing.Image.FromFile(@$"{currentDirectory}\stuff\GenFailure{imageCount}.png");
                    gr.DrawImage(img1, new System.Drawing.Rectangle(0, 0, 1825, 125));
                    newImage.Save(@$"{currentDirectory}\stuff\GenFailureHightRes{imageCount}.png", System.Drawing.Imaging.ImageFormat.Png);
                    Thread.Sleep(500);
                }

                var ocrengine = new TesseractEngine(@$"{currentDirectory}\tessdata", "eng", EngineMode.Default);
                var img = Pix.LoadFromFile(@$"{currentDirectory}\stuff\GenFailureHightRes{imageCount}.png");
                var res = ocrengine.Process(img);
                string message = res.GetText();
                message = message.Replace("\n", string.Empty);                

                SetForegroundWindow(GeneralFailure);

                if (message.Contains("GeneralFailure") || message.Contains("Error adding"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("SDA exception Genereal Failure");
                    Console.ResetColor();
                    //try
                    //{
                    //    sdaProc.Kill();
                    //    Thread.Sleep(1000);
                    //}
                    //catch (Exception ex) { Console.WriteLine(ex.Message); }
                    Console.ReadLine();
                }
               
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception in [CheckGeneralFailure] method");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.ReadLine();
            }
        }

        static List<string> listWindowsSDA = new List<string>();

        static void TryAgainAfterNoSms(string currentDirectory, string loginSteam, string loginMailRu, string password, Process sdaProc, int sdaProcId, double screenScalingFactor, int imageCount, string phoneNumber, int phoneOrderId,string providerName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Steam login is {loginSteam}");
            Console.ResetColor();

            //CheckSteamNumberAvailable(ref balance, ref countProviderPhones, ref costProvider, providerName);

            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Arguments = string.Format("/C \"{0}\" ", new object[]
            {
                       @$"{currentDirectory}\SDA\Steam Desktop Authenticator.exe",
            });
            process.StartInfo = processStartInfo;
            process.Start();
            Thread.Sleep(3000);

            IWebDriver browser = new ChromeDriver(@"C:\Users\gvozd\Desktop"); //2 параметр options если расширение
            browser.Navigate().GoToUrl("https://auth.mail.ru/cgi-bin/auth?from=portal");
            Thread.Sleep(500);
            string windowMail = browser.WindowHandles.Last();
            Thread.Sleep(500);

            LoginMailRu(loginMailRu, password, windowMail, browser);

            StartSDA(ref sdaProc, ref sdaProcId);

            LoginSDA(loginSteam, password);

            bool isCodeNeeded = MailCodeNeeded(currentDirectory, screenScalingFactor, imageCount);

            if (isCodeNeeded == true)
            {
                //иногда не нужен, есть акки которые пускают без кода
                EnterMailCodeSDA(windowMail, browser);
            }

            //OrderPhone(ref phoneNumber, ref phoneOrderId, providerName);
            OrderPhoneOnlineSim(ref phoneNumber, ref phoneOrderId, providerName);

            EnterPhoneSDA(phoneNumber);

            ConfirmMobileSDA(windowMail, browser);

            //TODO часто ни с чего вылезает генерал фаилюр, перезапускать наверное хз
            CheckGeneralFailure(currentDirectory, screenScalingFactor, sdaProc, imageCount);

            SkipEncryptSDA();

            SkipWarningWindow();

            string rCode = SaveRCodeFilteredWindow(currentDirectory, screenScalingFactor, imageCount).Result;
            if (rCode == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("OCR can't recognize code");
                Console.ResetColor();
                Console.ReadLine();
            }

            string smsCode = ReturnSmsOnlineSim(phoneOrderId);
        }

        static async Task<string> Captcha(int imageCount)
        {
            TwoCaptcha.TwoCaptcha solver = new TwoCaptcha.TwoCaptcha("d892c7eb922e9223f47e4575981752db");
            Normal captcha = new Normal();
            captcha.SetFile(@$"{Directory.GetCurrentDirectory()}\stuff\RCodeHightQual{imageCount}.png");
            captcha.SetNumeric(4);
            captcha.SetMinLen(6);
            captcha.SetMaxLen(6);
            captcha.SetPhrase(false);
            captcha.SetCaseSensitive(true);
            captcha.SetCalc(false);
            captcha.SetLang("en");
            captcha.SetHintText("Letter R and five numbers");

            try
            {
                await solver.Solve(captcha);
                return captcha.Code;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred: " + e.Message);
            }
            return null;
        }

        static void Main(string[] args)
        {
            if (File.Exists($@"{AppDomain.CurrentDomain.BaseDirectory}\inputData.txt"))
            {
                string pathInputData = $@"{AppDomain.CurrentDomain.BaseDirectory}\inputData.txt";
                string input = "";
                int imageCount = 1;

                int count = File.ReadAllLines(pathInputData).Length;
                StreamReader sr = new StreamReader(pathInputData);
                while (input != null)
                {
                    input = sr.ReadLine();

                    string currentDirectory = Directory.GetCurrentDirectory();
                    double screenScalingFactor = GetWindowsScreenScalingFactor();
                    int sdaProcId = 0;
                    Process sdaProc = default;
                    string loginMailRu = default;
                    string loginSteam = default;
                    string password = default;

                    SetVariables(input,ref loginSteam, ref password, ref loginMailRu);

                    // megafon
                    // mts
                    string providerName = "mts";
                    string phoneNumber = default;
                    int phoneOrderId = default;
                    double balance = default;
                    int countProviderPhones = default;
                    double costProvider = default;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Steam login is {loginSteam}");
                    Console.ResetColor();

                    //CheckSteamNumberAvailable(ref balance, ref countProviderPhones, ref costProvider, providerName);

                    Process process = new Process();
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processStartInfo.FileName = "cmd.exe";
                    processStartInfo.Arguments = string.Format("/C \"{0}\" ", new object[]
                    {
                       @$"{currentDirectory}\SDA\Steam Desktop Authenticator.exe",
                    });
                    process.StartInfo = processStartInfo;
                    process.Start();
                    Thread.Sleep(3000);

                    IWebDriver browser = new ChromeDriver(@"C:\Users\gvozd\Desktop"); //2 параметр options если расширение
                    browser.Navigate().GoToUrl("https://auth.mail.ru/cgi-bin/auth?from=portal");
                    Thread.Sleep(500);
                    string windowMail = browser.WindowHandles.Last();
                    Thread.Sleep(500);

                    LoginMailRu(loginMailRu, password, windowMail, browser);

                    StartSDA(ref sdaProc, ref sdaProcId);

                    LoginSDA(loginSteam, password);

                    bool isCodeNeeded = MailCodeNeeded(currentDirectory, screenScalingFactor, imageCount);

                    if (isCodeNeeded == true)
                    {
                        //иногда не нужен, есть акки которые пускают без кода
                        EnterMailCodeSDA(windowMail, browser);
                    }

                    //OrderPhone(ref phoneNumber, ref phoneOrderId, providerName);
                    OrderPhoneOnlineSim(ref phoneNumber, ref phoneOrderId, providerName);

                    EnterPhoneSDA(phoneNumber);

                    ConfirmMobileSDA(windowMail, browser);

                    //TODO часто ни с чего вылезает генерал фаилюр, перезапускать наверное хз
                    CheckGeneralFailure(currentDirectory, screenScalingFactor, sdaProc, imageCount);

                    SkipEncryptSDA();

                    SkipWarningWindow();

                    string rCode = SaveRCodeFilteredWindow(currentDirectory, screenScalingFactor, imageCount).Result;
                    if (rCode == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("OCR can't recognize code");
                        Console.ResetColor();
                        Console.ReadLine();
                    }

                    string smsCode = ReturnSmsOnlineSim(phoneOrderId);
                    //string smsCode = CheckSms(phoneOrderId);
                    if (smsCode == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("SMS code not received");
                        Console.ResetColor();
                        Console.ReadLine();
                    }

                    EnterPhoneCodeSDA(smsCode);

                    ReEnterRCode(rCode);

                    SkipLinkedWindow();

                    try
                    {
                        sdaProc.Kill();
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }

                    MoveMaFile(currentDirectory, loginSteam);

                    CreateJSON(currentDirectory, loginSteam, password);

                    string secretCode = GetMaFileCode(currentDirectory, loginSteam);

                    WriteAccData(currentDirectory, loginSteam, password, secretCode, loginMailRu, rCode);

                    browser.Quit();
                    imageCount += 1;
                    Thread.Sleep(10000);

                }
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
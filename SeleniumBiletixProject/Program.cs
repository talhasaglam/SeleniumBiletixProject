using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace SeleniumBiletixProject
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            //Biletix sayfa url'ine gidiş.

            IWebDriver webDriver = new ChromeDriver();
            webDriver.Navigate().GoToUrl("https://www.biletix.com");

            try
            {
                new WebDriverWait(webDriver, TimeSpan.FromSeconds(5)).Until(ExpectedConditions.ElementExists((By.Id("_evidon-accept-button"))));
                IWebElement acceptCookies = webDriver.FindElement(By.Id("_evidon-accept-button"));
                acceptCookies.Click();
            }
            catch
            {
                Console.WriteLine("5 saniye boyunca denendi, kabul edilecek Cookie bulunamadı.");
                //Cookie olmadığı loglanabilir.
            }

            try
            {
                new WebDriverWait(webDriver, TimeSpan.FromSeconds(2)).Until(ExpectedConditions.ElementExists((By.Id("dialog_close"))));
                IWebElement closePopUpAd = webDriver.FindElement(By.ClassName("dialog_close"));
                closePopUpAd.Click();
            }
            catch
            {
                Console.WriteLine("2 saniye boyunca denendi, kapatılacak reklam bulunamadı.");
                //Popup reklam olmadığı loglanabilir.
            }


            IWebElement clickCategory = webDriver.FindElement(By.Name("category_sb"));
            SelectElement chooseCategory = new SelectElement(clickCategory);

            IWebElement clickDate = webDriver.FindElement(By.Name("date_sb"));
            SelectElement chooseDate = new SelectElement(clickDate);

            IWebElement clickLocation = webDriver.FindElement(By.Name("city_sb"));
            SelectElement chooseLocation = new SelectElement(clickLocation);

            IWebElement search = webDriver.FindElement(By.ClassName("discoverbar__button"));

            clickCategory.Click();
            chooseCategory.SelectByValue("MUSIC");
            clickDate.Click();
            chooseDate.SelectByValue("next30days");
            clickLocation.Click();
            chooseLocation.SelectByValue("-1");

            //WebDriverWait ve Thread.Sleep ile 2 farklı bekleme yapılmıştır.
            search.Click();
            new WebDriverWait(webDriver, TimeSpan.FromSeconds(5)).Until(ExpectedConditions.ElementExists((By.CssSelector(".ln1.searchResultEventName"))));
            Thread.Sleep(2000);

            //Eventların adı, durumu ve tarihili listeye atılır.

            List<String> allOfNames = new List<string>();
            List<String> allOfStatuses = new List<string>();
            List<String> allOfDates = new List<string>();

            while (true)
            {

                try
                {

                    IReadOnlyCollection<IWebElement> names = webDriver.FindElements(By.CssSelector(".ln1.searchResultEventName"));

                    IReadOnlyCollection<IWebElement> statuses = webDriver.FindElements(By.XPath(".//*[@class='grid_5 omega searchResultInfo1b']/span[1]"));

                    IReadOnlyCollection<IWebElement> dates = webDriver.FindElements(By.XPath(".//*[@class='grid_3 alpha fld3 col-xs-12 searchResultInfo3 hiddenOnMobile']/div[1]/span[1]"));

                    foreach (IWebElement name in names)
                    {
                        if (name.Text.Length > 0)
                        {
                            allOfNames.Add(name.Text);
                        }

                    }

                    foreach (IWebElement status in statuses)
                    {
                        if (status.Text.Length > 0)
                        {
                            allOfStatuses.Add(status.Text);
                        }
                    }

                    foreach (IWebElement date in dates)
                    {
                        if (date.Text.Length > 0)
                        {
                            if (date.Text.Length > 13)
                            {
                                allOfDates.Add(date.Text.Substring(0, 13));
                            }
                            else
                            {
                                allOfDates.Add(date.Text);
                            }
                        }
                    }

                    IWebElement nextPage = webDriver.FindElement(By.XPath("//*[@class='nextbut']"));
                    nextPage.Click();
                    new WebDriverWait(webDriver, TimeSpan.FromSeconds(5)).Until(ExpectedConditions.ElementExists((By.CssSelector(".ln1.searchResultEventName"))));
                }
                catch
                {
                    break;
                }

            }

            // Date List kontrol edilir istenmeyen even çıkarılır.

            for (int i = 0; i < allOfStatuses.Count; i++)
            {
                if (allOfDates[i] == "Paz, 10/01/21")
                {
                    allOfNames.RemoveAt(i);
                    allOfStatuses.RemoveAt(i);
                    allOfDates.RemoveAt(i);
                }

            }

            //İşlem yapacağımız dosyanın yolunu belirtiyoruz.

            string dosya_yolu = @"Aktiviteler.txt";

            //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
            //2.parametre dosya varsa açılacağını yoksa oluşturulacağını belirtir,
            //3.parametre dosyaya erişimin veri yazmak için olacağını gösterir.

            FileStream fs = new FileStream(dosya_yolu, FileMode.OpenOrCreate, FileAccess.Write);

            //Yazma işlemi için bir StreamWriter nesnesi oluşturduk.

            StreamWriter sw = new StreamWriter(fs);

            //Dosyaya ekleyeceğimiz listeleri WriteLine() metodu ile yazacağız.

            for (int i = 0; i < allOfStatuses.Count; i++)
            {
                sw.WriteLine("Event Name:  " + allOfNames[i] + "   Status: " + allOfStatuses[i] + "  Date: " + allOfDates[i]);

            }

            //Veriyi tampon bölgeden dosyaya aktardık.

            sw.Flush();
            sw.Close();
            fs.Close();

            webDriver.Close();

        }
    }
}
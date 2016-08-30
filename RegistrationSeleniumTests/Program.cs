using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RegistrationSeleniumTests
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            using (var driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("http://register.training.business.gov.au");

                var dismiss = driver.FindElement(By.XPath("/html/body/div[2]/button"));
                dismiss.Click();

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }
    }
}

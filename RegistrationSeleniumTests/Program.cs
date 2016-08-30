using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace RegistrationSeleniumTests
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            var step1 = new Step { Action = StepAction.NavigateToUrl, Url = "http://register.training.business.gov.au" };
            var step2 = new Step { Action = StepAction.Click, XPath = "/html/body/div[2]/button" };

            using (var executor = new ScenarioExecutor(
                new Scenario
                {
                    Steps = new[]
                    {
                        step1,
                        step2
                    }.ToList()
                }, DriverType.Chrome))
            {
                while (executor.HasMoreSteps)
                {
                    executor.Step();
                }

                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }
    }
}

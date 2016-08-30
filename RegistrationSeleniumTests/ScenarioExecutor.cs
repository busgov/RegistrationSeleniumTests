using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace RegistrationSeleniumTests
{
    internal class ScenarioExecutor : IDisposable
    {
        private readonly Scenario _scenario;
        private readonly IWebDriver _driver;

        public ScenarioExecutor(Scenario scenario, DriverType driverType)
        {
            _scenario = scenario;
            switch (driverType)
            {
                case DriverType.Chrome:
                    _driver = new ChromeDriver();
                    break;

                case DriverType.Ie:
                    _driver = new InternetExplorerDriver();
                    break;

                case DriverType.Edge:
                    _driver = new EdgeDriver();
                    break;

                case DriverType.Firefox:
                    _driver = new FirefoxDriver();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(driverType), driverType, null);
            }
        }

        public int StepNumber { get; private set; } = 1;

        public bool HasMoreSteps => StepNumber <= _scenario.Steps.Count;

        public void Reset()
        {
            StepNumber = 1;
        }

        public void Step()
        {
            if (!HasMoreSteps) { throw new InvalidOperationException("No more steps"); }

            var stepNumberText = StepNumber.ToString();
            var len = stepNumberText.Length;
            var pad = "".PadLeft(4 - len);

            Console.Write($"Processing step {pad}{StepNumber}: ");

            var step = _scenario.Steps[StepNumber - 1];           

            switch (step.Action)
            {
                case StepAction.None:
                    break;

                case StepAction.Click:
                    Console.WriteLine($"Clicking '{step.XPath}'.");

                    // TODO: invalid XPath?
                    var clickable = _driver.FindElement(By.XPath(step.XPath));
                    clickable?.Click();
                    break;

                case StepAction.SetValue:
                    break;

                case StepAction.NavigateToUrl:
                    Console.WriteLine($"Navigating to URL '{step.Url}'.");
                    _driver.Navigate().GoToUrl(step.Url);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            StepNumber++;
        }

        public void Steps(int count)
        {
            while (count-- > 0)
            {
                Step();
            }
        }

        public void Run(bool reset)
        {
            if (reset)
            {
                Reset();
            }

            while (HasMoreSteps)
            {
                Step();
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}

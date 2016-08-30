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

        public int ActionNumber { get; private set; } = 1;

        public bool HasMore => ActionNumber <= _scenario.Actions.Count;

        public void Reset()
        {
            ActionNumber = 1;
        }

        public bool Next()
        {
            try
            {
                if (!HasMore) { throw new InvalidOperationException("No more actions"); }

                var action = _scenario.Actions[ActionNumber++ - 1];

                var maxLen = _scenario.Actions.Count.ToString().Length;
                var text = ActionNumber.ToString();
                var len = text.Length;
                var pad = $"[{text.PadLeft(maxLen - len)}]=> ";

                Console.WriteLine();
                Console.WriteLine($"{pad}{action.Title}:");
                Console.Write("".PadLeft(pad.Length));

                if (action.Disabled)
                {
                    Console.WriteLine($"Action {action.ActionType} disabled, not executing.");
                    return true;
                }

                System.Threading.Thread.Sleep(action.PreDelay);

                switch (action.ActionType)
                {
                    case ActionType.None:
                        break;

                    case ActionType.Click:
                        Console.WriteLine($"Clicking '{action.XPath}'.");
                        Click(action);
                        break;

                    case ActionType.SetValue:
                        break;

                    case ActionType.NavigateToUrl:
                        Console.WriteLine($"Navigating to URL '{action.Url}'.");
                        _driver.Navigate().GoToUrl(action.Url);
                        break;

                    case ActionType.WaitForKey:
                        Console.WriteLine("Waiting for key to be pressed.");
                        Console.ReadKey();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                System.Threading.Thread.Sleep(action.PostDelay);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                return false;
            }
        }

        public void Run(bool reset, bool stopOnError)
        {
            if (reset)
            {
                Reset();
            }

            while (HasMore)
            {
                if (!Next() && stopOnError)
                {
                    return;
                }
            }
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }

        private IWebElement GetElement(Action action)
        {
            IWebElement element = null;

            if (!string.IsNullOrWhiteSpace(action.XPath))
            {
                element = _driver.FindElement(By.XPath(action.XPath));
            }

            if (!string.IsNullOrWhiteSpace(action.Id))
            {
                element = _driver.FindElement(By.Id(action.Id));
            }

            if (element == null)
            {
                throw new ArgumentException("Did not find element.");
            }

            return element;
        }

        private void Click(Action action)
        {
            GetElement(action).Click();
        }
    }
}

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

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
            var tries = 5;
            while (tries-- > 0)
            {
                try
                {
                    if (!HasMore)
                    {
                        throw new InvalidOperationException("No more actions");
                    }

                    var action = _scenario.Actions[ActionNumber - 1];

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

                        case ActionType.ClickJs:
                            Console.WriteLine($"Clicking '{action.XPath}'.");
                            ClickJs(action);
                            break;

                        case ActionType.ClickAt:
                            Console.WriteLine($"Clicking at '{action.XPath}'.");
                            ClickAt(action);
                            break;

                        case ActionType.SetValue:
                            Console.WriteLine($"Setting value '{action.Value}' for '{action.XPath}'.");
                            SetValue(action);
                            break;

                        case ActionType.SelectByValue:
                            Console.WriteLine($"Selecting value '{action.Value}' for '{action.XPath}'.");
                            SelectByValue(action);
                            break;

                        case ActionType.SelectByText:
                            Console.WriteLine($"Selecting text '{action.Value}' for '{action.XPath}'.");
                            SelectByText(action);
                            break;

                        case ActionType.SelectByIndex:
                            Console.WriteLine($"Selecting index '{action.Value}' for '{action.XPath}'.");
                            SelectByIndex(action);
                            break;

                        case ActionType.NavigateToUrl:
                            Console.WriteLine($"Navigating to URL '{action.Url}'.");
                            _driver.Navigate().GoToUrl(action.Url);
                            break;

                        case ActionType.WaitForKey:
                            Console.WriteLine("Waiting for key to be pressed.");
                            Console.ReadKey();
                            break;

                        case ActionType.MoveTo:
                            MoveTo(action);
                            break;

                        case ActionType.Stop:
                            return false;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    System.Threading.Thread.Sleep(action.PostDelay);
                    ActionNumber++;
                    return true;
                }
                catch (Exception ex)
                {
                    if (tries > 0)
                    {
                        System.Threading.Thread.Sleep(1000);
                        continue;
                    }

                    Console.WriteLine($"{ex}");
                    return false;
                }
            }

            return true;
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

        private void MoveTo(Action action)
        {
            new Actions(_driver).MoveToElement(GetElement(action)).Perform();
        }

        private void SelectByValue(Action action)
        {
            new SelectElement(GetElement(action)).SelectByValue(action.Value);
        }

        private void SelectByIndex(Action action)
        {
            new SelectElement(GetElement(action)).SelectByIndex(int.Parse(action.Value));
        }

        private void SelectByText(Action action)
        {
            new SelectElement(GetElement(action)).SelectByText(action.Value);
        }

        private void SetValue(Action action)
        {
            GetElement(action).SendKeys(action.Value);
        }

        private void Click(Action action)
        {
            GetElement(action).Click();
        }

        private void ClickJs(Action action)
        {
            var js = (IJavaScriptExecutor) _driver;
            var e = GetElement(action);
            js.ExecuteScript("arguments[0].click()", e);
        }

        private void ClickAt(Action action)
        {
            // Move to X, Y to avoid any <a> tags that may be in the element.
            new Actions(_driver)
                .MoveToElement(
                    GetElement(action), 
                    action.ClickAtX, 
                    action.ClickAtY)
                .Click()
                .Perform();
        }
    }
}

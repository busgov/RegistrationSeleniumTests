using System;
using System.Linq;
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
            var tries = 3;
            while (tries-- >= 0)
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
                            Console.WriteLine($"Clicking '{action.ByValue}'.");
                            if (!Click(action)) { return false; }
                            break;

                        case ActionType.ClickJs:
                            Console.WriteLine($"Clicking '{action.ByValue}'.");
                            if (!ClickJs(action)) { return false; }
                            break;

                        case ActionType.ClickAt:
                            Console.WriteLine($"Clicking at '{action.ByValue}'.");
                            if (!ClickAt(action)) { return false; }
                            break;

                        case ActionType.SetValue:
                            Console.WriteLine($"Setting value '{action.Value}' for '{action.ByValue}'.");
                            if (!SetValue(action)) { return false; }
                            break;

                        case ActionType.SelectByValue:
                            Console.WriteLine($"Selecting value '{action.Value}' for '{action.ByValue}'.");
                            if (!SelectByValue(action)) { return false; }
                            break;

                        case ActionType.SelectByText:
                            Console.WriteLine($"Selecting text '{action.Value}' for '{action.ByValue}'.");
                            if (!SelectByText(action)) { return false; }
                            break;

                        case ActionType.SelectByIndex:
                            Console.WriteLine($"Selecting index '{action.Value}' for '{action.ByValue}'.");
                            if (!SelectByIndex(action)) { return false; }
                            break;

                        case ActionType.NavigateToUrl:
                            Console.WriteLine($"Navigating to URL '{action.Value}'.");
                            _driver.Navigate().GoToUrl(action.Value);
                            break;

                        case ActionType.WaitForKey:
                            Console.WriteLine("Waiting for key to be pressed.");
                            Console.ReadKey();
                            break;

                        case ActionType.MoveTo:
                            if (!MoveTo(action)) { return false; }
                            break;

                        case ActionType.Stop:
                            return false;

                        case ActionType.SwitchToFrame:
                            if (!SwitchToFrame(action)) { return false; }
                            break;

                        case ActionType.SwitchToDefault:
                            SwitchToDefault();
                            break;

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

            return false;
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
            if (string.IsNullOrWhiteSpace(action.ByValue))
            {
                throw new InvalidOperationException($"Action {nameof(action.ByValue)} not set to a value.");
            }

            var tries = 5;
            while (true)
            {
                try
                {
                    By by;
                    switch (action.ByType)
                    {
                        case ByType.XPath:
                            by = By.XPath(action.ByValue);
                            break;

                        case ByType.Id:
                            by = By.Id(action.ByValue);
                            break;

                        case ByType.ClassName:
                            by = By.ClassName(action.ByValue);
                            break;

                        case ByType.CssSelector:
                            by = By.CssSelector(action.ByValue);
                            break;

                        case ByType.Name:
                            by = By.Name(action.ByValue);
                            break;

                        case ByType.TagName:
                            by = By.TagName(action.ByValue);
                            break;

                        case ByType.LinkText:
                            by = By.LinkText(action.ByValue);
                            break;

                        case ByType.PartialLinkText:
                            by = By.PartialLinkText(action.ByValue);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(action.ByType));
                    }

                    var element = _driver.FindElement(by);

                    // Visible and enabled?
                    if (element.Displayed && element.Enabled)
                    {
                        return element;
                    }

                    throw new Exception("Element not visible or not enabled.");
                }
                catch (NoSuchElementException nsex)
                {
                    Console.WriteLine($"Failed to get element, tries remaining {tries}.");
                    if (tries-- <= 0)
                    {
                        Console.WriteLine("Try again");
                        Console.WriteLine("  x   - By xpath");
                        Console.WriteLine("  i   - By id");
                        Console.WriteLine("  cn  - By class name");
                        Console.WriteLine("  css - By css selector");
                        Console.WriteLine("  n   - By name");
                        Console.WriteLine("  tn  - By tag name");
                        Console.WriteLine("  lt  - By link text");
                        Console.WriteLine("  plt - By name");
                        Console.WriteLine("  Any other key - Cancel...");
                        var tryAgain = (Console.ReadLine() ?? string.Empty).ToLower().Trim();

                        ByType byType;

                        switch (tryAgain)
                        {
                            case "x":
                                byType = ByType.XPath;
                                Console.WriteLine("Enter XPath:");
                                break;

                            case "i":
                                byType = ByType.Id;
                                Console.WriteLine("Enter Id:");
                                break;

                            case "cn":
                                byType = ByType.ClassName;
                                Console.WriteLine("Enter Class Name:");
                                break;

                            case "css":
                                byType = ByType.CssSelector;
                                Console.WriteLine("Enter CSS Selector:");
                                break;

                            case "n":
                                byType = ByType.Name;
                                Console.WriteLine("Enter Name:");
                                break;

                            case "tn":
                                byType = ByType.TagName;
                                Console.WriteLine("Enter Tag Name:");
                                break;

                            case "lt":
                                byType = ByType.LinkText;
                                Console.WriteLine("Enter Link Text:");
                                break;

                            case "plt":
                                byType = ByType.PartialLinkText;
                                Console.WriteLine("Enter Partial Link Text:");
                                break;

                            default:
                                return null;
                        }

                        var byValue = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(byValue))
                        {
                            Console.WriteLine($"Element not found: {nsex.Message}");
                            return null;
                        }

                        var title = $"[updated: {byType}]";
                        if (!string.IsNullOrWhiteSpace(action.Title))
                        {
                            title = $"{action.Title} {title}";
                        }

                        action.Title = title;
                        action.ByType = byType;
                        action.ByValue = byValue;
                        tries = 5;
                        continue;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private bool MoveTo(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            new Actions(_driver).MoveToElement(element).Perform();
            return true;
        }

        private bool SelectByValue(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            new SelectElement(element).SelectByValue(action.Value);
            return true;
        }

        private bool SelectByIndex(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            new SelectElement(element).SelectByIndex(int.Parse(action.Value));
            return true;
        }

        private bool SelectByText(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            new SelectElement(element).SelectByText(action.Value);
            return true;
        }

        private bool SetValue(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            element.SendKeys(action.Value);

            return true;
        }

        private bool Click(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            element.Click();
            return true;
        }

        private bool ClickJs(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            var js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript("arguments[0].click()", element);

            return true;
        }

        private bool ClickAt(Action action)
        {
            int x;
            int y;

            try
            {
                var xyValues = action
                    .Value
                    .Split(new[] { ',' }, 2)
                    .Select(v => v?.Trim() ?? string.Empty)
                    .ToArray();

                x = int.Parse(xyValues[0]);
                y = int.Parse(xyValues[1]);
            }
            catch (Exception)
            {
                throw new Exception(
                    "There must be two integer values in the for 'x, y' defining the click at action.");
            }

            var element = GetElement(action);
            if (element == null) { return false; }

            // Move to X, Y to avoid any <a> tags that may be in the element.
            new Actions(_driver)
                .MoveToElement(
                    element,
                    x,
                    y)
                .Click()
                .Perform();

            return true;
        }

        private bool SwitchToFrame(Action action)
        {
            var element = GetElement(action);
            if (element == null) { return false; }

            _driver.SwitchTo().Frame(element);
            return true;
        }

        private void SwitchToDefault()
        {
            _driver.SwitchTo().DefaultContent();
        }
    }
}

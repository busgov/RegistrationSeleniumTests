using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OpenQA.Selenium;

namespace RegistrationSeleniumTests
{
    internal class SeleniumExportParser
    {
        private Scenario _scenario;
        private string _href;

        public static Scenario ParseFile(string fileName)
        {
            var text = System.IO.File.ReadAllText(fileName);
            text = text.Replace("&nbsp;", "&amp;nbsp;");
            return new SeleniumExportParser().Parse(text);
        }

        public Scenario Parse(string text)
        {
            _scenario = new Scenario();

            var doc = XDocument.Parse(text);

            const string ns = "http://www.w3.org/1999/xhtml";

            var linkElement = doc
                .Descendants(XName.Get("link", ns))
                .Single(e => e.Attribute("rel").Value == "selenium.base");

            _href = linkElement.Attribute("href").Value;

            var actions = doc
                .Descendants(XName.Get("body", ns)).FirstOrDefault()?
                .Descendants(XName.Get("table", ns)).FirstOrDefault()?
                .Descendants(XName.Get("tbody", ns)).FirstOrDefault()?
                .Descendants(XName.Get("tr", ns))
                .ToArray();

            if (actions == null || actions.Length <= 0) { return _scenario; }

            foreach (var actionElement in actions)
            {
                var actionParts = actionElement
                    .Descendants(XName.Get("td", ns))
                    .Select(tr => tr.Value)
                    .ToList();

                System.Diagnostics.Debug.Assert(actionParts.Count == 3);

                BuildAction(actionParts);
            }

            return _scenario;
        }

        private void BuildAction(IReadOnlyList<string> actionParts)
        {
            By by;   
            switch (actionParts[0].Trim().ToLower())
            {
                case "open":
                    _scenario.Actions.Add(new Action
                    {
                        ActionType = ActionType.NavigateToUrl,
                        Value = _href
                    });
                    break;

                case "click":
                    by = GetElementLocator(actionParts[1]);
                    break;

                case "clickandwait":
                    by = GetElementLocator(actionParts[1]);
                    break;

                case "select":
                    by = GetElementLocator(actionParts[1]);
                    break;

                case "type":
                    by = GetElementLocator(actionParts[1]);
                    break;

                default:
                    break;
            }
        }

        private static By GetElementLocator(string actionPart)
        {
            actionPart = actionPart.Trim();

            if (actionPart.StartsWith("//"))
            {
                return By.XPath(actionPart);
            }

            var subParts = actionPart.Split(new[] { '=' }, 2);

            switch (subParts[0].Trim().ToLower())
            {
                case "css":
                    return By.CssSelector(subParts[1]);

                case "name":
                    return By.Name(subParts[1]);

                case "id":
                    return By.Id(subParts[1]);

                case "link":
                    return By.LinkText(subParts[1]);

                case "class":
                    return By.ClassName(subParts[1]);

                case "tag":
                    return By.TagName(subParts[1]);

                default:
                    throw new Exception("Unknown element....");
            }
        }
    }
}

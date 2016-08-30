using System;
using System.Linq;

namespace RegistrationSeleniumTests
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            const int postDelay = 300;

            var actions = new[]
            {
                new Action { Title = "Opening select registrations page",  ActionType = ActionType.NavigateToUrl, Url = "http://register.training.business.gov.au", PostDelay = postDelay },
                new Action { Title = "Dismissing test system message", ActionType = ActionType.Click, XPath = "/html/body/div[2]/button", PostDelay = postDelay },
                new Action { Title = "Selecting ABN", ActionType = ActionType.Click, XPath = "//*[@id=\"select-registrations\"]/div/div[1]/p[1]/label", PostDelay = postDelay },
                new Action { Title = "Selecting business name", ActionType = ActionType.Click, XPath = "//*[@id=\"select-registrations\"]/div/div[1]/p[2]/label", PostDelay = postDelay },
                new Action { Title = "Selecting GST", ActionType = ActionType.Click, XPath = "//*[@id=\"select-registrations\"]/div/div[1]/p[3]/label", PostDelay = postDelay },
                new Action { Title = "Selecting company", ActionType = ActionType.Click, XPath = "//*[@id=\"select-registrations\"]/div/div[1]/p[4]/label", PostDelay = postDelay },
                new Action { Title = "Moving to next section", ActionType = ActionType.Click, XPath = "//*[@id=\"ajax-container-for-businessdetails\"]/div[2]/div[3]/div/button", PostDelay = postDelay },
                new Action { Title = "Start applying...", ActionType = ActionType.Click, XPath = "//*[@id=\"ajax-container-for-businessdetails\"]/div[3]/div/div/button[2]", PostDelay = postDelay * 3 },
                new Action { Title = "Skipping logon section", ActionType = ActionType.Click, XPath = "//*[@id=\"ajax-container-for-entitlement\"]/div[2]/div[1]/div/div/button[2]", PostDelay = postDelay },
                new Action { Title = "Selecting private company", ActionType = ActionType.Click, XPath = "//*[@id=\"ajax-container-for-entitlement\"]/div[3]/div[2]/div[1]/fieldset/div/div[2]/p/label", PostDelay = postDelay }
            }.ToList();

            var scenario = new Scenario
            {
                Actions = actions
            };

            var json = SerializationHelper.Serialize(scenario);

            using (var executor = new ScenarioExecutor(scenario, DriverType.Chrome))
            {
                executor.Run(true, true);

                Console.WriteLine();
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }
    }
}

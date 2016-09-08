using System;

namespace RegistrationSeleniumTests
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            //var scenarios = SeleniumExportParser.ParseFile(@"Scenarios\Test.xml");
            //return;

            if (args.Length > 0)
            {
                CreateJson();
                return;
            }

            const string scenarioName = "Scenarios\\AbnBnGstCo{0}.json";

            var json = System.IO.File.ReadAllText(string.Format(scenarioName, ""));
            var scenario = SerializationHelper.Deserialize<Scenario>(json);            
            //json = SerializationHelper.Serialize(scenario);            

            //for (var i = 0; i < 1000; i++)
            //{
            //    using (var executor = new ScenarioExecutor(scenario, DriverType.Chrome))
            //    {
            //        executor.Run(true, true);
            //    }
            //}

            using (var executor = new ScenarioExecutor(scenario, DriverType.Chrome))
            {
                executor.Run(true, true);

                Console.WriteLine();
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }

            // Save scenario in case By values were updated.
            System.IO.File.WriteAllText(string.Format(scenarioName, ".updated"), SerializationHelper.Serialize(scenario));
        }

        private static void CreateJson()
        {
            var scenario = new CsvToScenarioParser().Parse(@"D:\Source\scenarios.txt");
            System.IO.File.WriteAllText(@"D:\Source\scenarios.json", SerializationHelper.Serialize(scenario));
        }
    }
}

using System;
using System.Linq;

namespace RegistrationSeleniumTests
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                CreateJson();
                return;
            }

            var json = System.IO.File.ReadAllText("Scenarios\\AbnBnGstCo.json");
            var scenario = SerializationHelper.Deserialize<Scenario>(json);

            using (var executor = new ScenarioExecutor(scenario, DriverType.Chrome))
            {
                executor.Run(true, true);

                Console.WriteLine();
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
        }

        private static void CreateJson()
        {
            var scenario = new CsvToScenarioParser().Parse(@"D:\Source\scenarios.txt");
            System.IO.File.WriteAllText(@"D:\Source\scenarios.json", SerializationHelper.Serialize(scenario));
        }
    }
}

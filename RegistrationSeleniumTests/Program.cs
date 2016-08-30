using System;
using System.Linq;

namespace RegistrationSeleniumTests
{
    internal class Program
    {
        internal static void Main(string[] args)
        {            
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
    }
}

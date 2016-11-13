using System;
using TechTalk.SpecFlow;

namespace BehaviourSpecs
{
    [Binding]
    public class SpecFlowOutputPrettifier
    {
        [BeforeStep]
        public void BeforeStep()
        {
            Console.WriteLine("___________________________________________________________________________________________________________________________");
            Console.WriteLine();
        }

        [AfterStep]
        public void AfterStep()
        {
            Console.WriteLine("============================================ Step Done ============================================");
            Console.WriteLine();
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Console.WriteLine();
            Console.WriteLine("######################################## Scenario Completed ########################################");
        }
    }
}

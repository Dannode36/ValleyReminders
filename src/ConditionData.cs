using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    [Serializable]
    class ConditionData
    {
        public string MethodName { get; set; } = string.Empty;
        public List<string> Parameters { get; set; } = new();

        public ConditionData() { }

        public ConditionData(string methodName, string parameter)
        {
            MethodName = methodName;
            Parameters = new() { parameter };
        }

        public ConditionData(string methodName, List<string> parameters)
        {
            MethodName = methodName;
            Parameters = parameters;
        }
    }
}

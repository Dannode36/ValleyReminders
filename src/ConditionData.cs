using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    [Serializable]
    class ConditionData
    {
        public string MethodName { get; set; } = string.Empty;
        public List<string> ParameterValues { get; set; } = new();

        public ConditionData() { }
        public ConditionData(string methodName) 
        {
            MethodName = methodName;
            ParameterValues = new(Conditions.GetParameterList(methodName).Count);
        }

        public ConditionData(string methodName, List<string> parameterValues)
        {
            MethodName = methodName;
            ParameterValues = parameterValues;
        }

        //Only for development purposes
        [Obsolete]
        public ConditionData(string methodName, string parameterValues)
        {
            MethodName = methodName;
            ParameterValues = new() { parameterValues };
        }
    }
}

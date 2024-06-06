using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    /// <summary>
    /// Stores the users reminder condition data (e.g. parameter values for IsTimeOfDay(string time))
    /// </summary>
    [Serializable]
    class ConditionUserData
    {
        public string MethodName { get; set; } = string.Empty;
        public List<string> ParameterValues { get; set; } = new();

        public ConditionUserData() { }
        public ConditionUserData(string methodName) 
        {
            MethodName = methodName;
            ParameterValues = new(Conditions.GetParameterCount(methodName));
        }

        public ConditionUserData(string methodName, List<string> parameterValues)
        {
            MethodName = methodName;
            ParameterValues = parameterValues;
        }

        //Only for development purposes
        /*[Obsolete]
        public ConditionUserData(string methodName, string parameterValues)
        {
            MethodName = methodName;
            ParameterValues = new() { parameterValues };
        }*/
    }
}

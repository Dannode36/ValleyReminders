using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders.src
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class ParameterNamesAttribute : Attribute
    {
        readonly string[] paramNames;

        public ParameterNamesAttribute(params string[] paramNames)
        {
            this.paramNames = paramNames;
        }

        public string[] ParamNames
        {
            get { return paramNames; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    sealed class ParameterTypeAttribute : Attribute
    {
        readonly ParameterType[] paramTypes;

        public ParameterTypeAttribute(params ParameterType[] paramTypes)
        {
            this.paramTypes = paramTypes;
        }

        public ParameterType[] ParamTypes
        {
            get { return paramTypes; }
        }
    }
}

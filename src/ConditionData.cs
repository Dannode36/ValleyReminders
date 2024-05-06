using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    class ConditionData
    {
        public string MethodName { get; set; } = string.Empty;
        public List<string> Parameters { get; set; } = new();
    }
}

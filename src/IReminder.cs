using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    interface IReminder
    {
        bool IsReadyToNotify();
        bool IsRecurring();
        void Notify();
    }
}

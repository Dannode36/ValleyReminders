using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    class ModConfig
    {
        public KeybindList ToggleKey { get; set; } = KeybindList.Parse("LeftShift + R, R");
    }
}

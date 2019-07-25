using System;

namespace RestInPeace.Enums {
    [Flags]
    public enum DebugLevels {        
        None = 0,
        FinalResultOnly=0x1,
        Basic=0x2,
        Files=0x4,
        Functions=0x8,
        All = 0x10
    }
}
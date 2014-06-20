using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.SimpleF3DEX2.CombinerEmulation
{
    internal interface ICombiner
    {
        string GetName();
        string GetDescription();

        void Attach(F3DEX2Interpreter interpreter);
        void Reset();
        void Bind(ulong mux);
    }
}

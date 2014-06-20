using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SceneNavi.SimpleF3DEX2.CombinerEmulation
{
    internal class CombinerException : Exception
    {
        public CombinerException(string message) : base(message) { }
        public CombinerException(string message, Exception innerException) : base(message, innerException) { }
    }
}

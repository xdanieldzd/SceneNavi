using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SceneNavi.SimpleF3DEX2.CombinerEmulation
{
    internal class ArbCombineManager
    {
        List<ArbCombineProgram> fragcache;

        public ArbCombineManager()
        {
            fragcache = new List<ArbCombineProgram>();

            ResetFragmentCache();
        }

        public void BindCombiner(uint m0, uint m1, bool tex)
        {
            GL.Enable((EnableCap)All.FragmentProgram);
            foreach (ArbCombineProgram frag in fragcache)
            {
                if (frag.Mux0 == m0 && frag.Mux1 == m1 && frag.Textured == tex)
                {
                    GL.Arb.BindProgram(AssemblyProgramTargetArb.FragmentProgram, frag.GLID);
                    return;
                }
            }

            ArbCombineProgram newcached = new ArbCombineProgram(m0, m1, tex);
            fragcache.Add(newcached);
        }

        public void ResetFragmentCache()
        {
            if (fragcache != null)
            {
                foreach (ArbCombineProgram fc in fragcache) if (GL.Arb.IsProgram(fc.GLID)) { int glid = fc.GLID; GL.Arb.DeleteProgram(1, ref glid); }
                fragcache.Clear();
            }

            foreach (uint[] knownMux in KnownCombinerMuxes.Muxes)
            {
                BindCombiner(knownMux[0], knownMux[1], true);
                BindCombiner(knownMux[0], knownMux[1], false);
            }
        }
    }
}

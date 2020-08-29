using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // Client::Graphics::Render::Material
    public class RenderMaterialOffsets
    {
        public const int MTRL = 0x10;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct RenderMaterial
    {
        [FieldOffset(RenderMaterialOffsets.MTRL)] public MaterialResourceHandle* MTRL;
    }
}

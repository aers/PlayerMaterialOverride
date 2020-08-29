using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // Client::Graphics::Render::Model
    public class RenderModelOffsets
    {
        public const int MDL = 0x30;
        public const int MaterialArray = 0x90;
        public const int MaterialCount = 0x98;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct RenderModel
    {
        [FieldOffset(RenderModelOffsets.MDL)] public ModelResourceHandle* MDL;
        [FieldOffset(RenderModelOffsets.MaterialArray)] public RenderMaterial** MaterialArray;
        [FieldOffset(RenderModelOffsets.MaterialCount)] public int MaterialCount;
    }
}

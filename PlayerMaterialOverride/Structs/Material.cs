using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // Game object Material which represents the material loaded from a mtrl file
    public class MaterialOffsets
    {
        public const int MTRL = 0x10;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Material
    {
        [FieldOffset(MaterialOffsets.MTRL)] public FileMTRL* MTRL;
    }
}

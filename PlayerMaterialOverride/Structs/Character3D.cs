using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PlayerMaterialOverride.Structs
{
    // Character3D
    // size - 0xA80
    // ctor - E8 ? ? ? ? 48 8B F8 48 85 C0 74 28 48 8D 55 D7 
    public class Character3DOffsets
    {
        public const int SlotCount = 0x98;
        public const int ModelArray = 0xA8;
        public const int MaterialArray = 0x2E8;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Character3D
    {
        [FieldOffset(Character3DOffsets.SlotCount)] public int SlotCount;
        [FieldOffset(Character3DOffsets.ModelArray)] public Model** ModelArray;
        [FieldOffset(Character3DOffsets.MaterialArray)] public Material** MaterialArray;
    }
}

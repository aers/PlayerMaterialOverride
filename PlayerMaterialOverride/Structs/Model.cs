using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // Game object Model which represents the model loaded from a mdl file
    public class ModelOffsets
    {
        public const int MDL = 0x30;
        public const int MaterialArray = 0x90;
        public const int MaterialCount = 0x98;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Model
    {
        [FieldOffset(ModelOffsets.MDL)] public FileMDL* MDL;
        [FieldOffset(ModelOffsets.MaterialArray)] public Material** MaterialArray;
        [FieldOffset(ModelOffsets.MaterialCount)] public int MaterialCount;
    }
}

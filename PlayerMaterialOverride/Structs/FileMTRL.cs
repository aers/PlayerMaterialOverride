using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // FileMTRL - representation of "mtrl" file
    public class FileMTRLOffsets
    {
        public const int FileNameString = 0x48;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct FileMTRL
    {
        [FieldOffset(FileMDLOffsets.FileNameString)] public char* FileNameString;
    }
}

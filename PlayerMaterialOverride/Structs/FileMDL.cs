using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // FileMDL - representation of "mdl" file
    public class FileMDLOffsets
    {
        public const int FileNameString = 0x48;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct FileMDL
    {
        [FieldOffset(FileMDLOffsets.FileNameString)] public char* FileNameString;
    }
}

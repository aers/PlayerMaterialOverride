using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // Client::System::Resource::Handle::ModelResourceHandle
    public class ModelResourceHandleOffsets
    {
        public const int FileNameString = 0x48;
    }
    
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ModelResourceHandle
    {
        [FieldOffset(ModelResourceHandleOffsets.FileNameString)] public char* FileNameString;
    }
}

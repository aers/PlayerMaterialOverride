using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PlayerMaterialOverride.Structs
{
    // Client::System::Resource::Handle::MaterialResourceHandle
    public class MaterialResourceHandleOffsets
    {
        public const int FileNameString = 0x48;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct MaterialResourceHandle
    {
        [FieldOffset(ModelResourceHandleOffsets.FileNameString)] public char* FileNameString;
    }
}

using System.Runtime.InteropServices;

namespace Castle.Core.CastleKit.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InteropError
    {
        public int Code;
        public string LocalizedDescription;
        public string TaskId;
    }
}

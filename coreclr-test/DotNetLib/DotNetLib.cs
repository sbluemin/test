using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DotNetLib;
public static class Lib
{
    static IntPtr nativeHandle;

    [DllImport("__Internal")]
    public static extern void FromManaged();

    [DllImport("__Internal")]
    public static extern void HelloWorld();

    public delegate void CustomEntryPointDelegate(IntPtr handle);
    public static void CustomEntryPoint(IntPtr handle)
    {
        Console.WriteLine(handle);
        nativeHandle = handle;

        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnResolveDllImport);

        Console.WriteLine($"Hello, from unmanaged!");

        FromManaged();
        HelloWorld();
    }

    public static IntPtr OnResolveDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        Console.WriteLine("Called OnResolveDllImport");
        if (libraryName == "__Internal")
        {
            return nativeHandle;
        }

        // Insert additional resolution logic as needed
        Console.WriteLine("Can not resolve __Internal");
        return IntPtr.Zero;
    }
}

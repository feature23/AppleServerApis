// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

// This is a hack required to use C#9 or higher language features (like records and init-only properties)
// in a netstandard2.0 project. See link below for details:
// https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809#TPIN-N1249582
internal static class IsExternalInit
{   
}
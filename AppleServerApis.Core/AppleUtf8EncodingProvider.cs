using System.Text;

namespace AppleServerApis.Core;

internal class AppleUtf8EncodingProvider : EncodingProvider
{
    public override Encoding? GetEncoding(int codepage) =>
        CodePagesEncodingProvider.Instance.GetEncoding(codepage);

    public override Encoding? GetEncoding(string name)
    {
        return name == "utf8" 
            ? Encoding.UTF8 
            : CodePagesEncodingProvider.Instance.GetEncoding(name);
    }
}

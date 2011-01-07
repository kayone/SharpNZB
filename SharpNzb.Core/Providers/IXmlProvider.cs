using System.Xml.Linq;

namespace SharpNzb.Core.Providers
{
    public interface IXmlProvider
    {
        //Create XML Responses for:
        //Queue, History, Version, APIKey?, Scripts, Categories

        XElement Queue(int start, int count);
        XElement History(int start, int count);
        XElement Version();
        XElement Scripts();
        XElement Categories();
    }
}

using SharpNzb.Core.Model.Nzb;

namespace SharpNzb.Core.Providers
{
    public interface INntpConnectionProvider
    {
        bool Connect(string hostname, int port, bool ssl, string username, string password);
        void Disconnect();
        void GetArticle(NzbSegmentModel segment);
        long Speed { get; }
        long SpeedLimit { get; set; }
        event ArticleFinishedHandler ArticleFinished;
    }
}

namespace SharpNzb.Core.Providers
{
    public interface IConfigProvider
    {
        string GetValue(string key, object defaultValue, bool makePermanent);
        void SetValue(string key, string value);
    }
}

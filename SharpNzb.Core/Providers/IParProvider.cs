namespace SharpNzb.Core.Providers
{
    public interface IParProvider
    {
        //Define methods to make up the ParProvider
        bool Verify(string fileName);
        bool Repair(string fileName);
    }
}

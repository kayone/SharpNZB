using System;

namespace SharpNzb.Core.Providers
{
    public class DecompressProvider : IDecompressProvider
    {
        #region IDecompressProvider Members

        public bool Unrar(string path)
        {
            throw new NotImplementedException("Decompress - Unrar");
        }

        #endregion
    }
}

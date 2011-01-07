namespace SharpNzb.Core.Model.Nzb
{
    public enum NzbStatus
    {
        Queued = 0,
        Paused = 1,
        Deleted = 2,
        Downloading = 3,
        Downloaded = 4,
        Verifying = 5,
        Repairing = 6,
        Unpacking = 7,
        PostProcessing = 8,
        Completed = 9,
    }

    public enum NzbFileStatus
    {
        Queued = 0,
        Deleted = 1,
        Downloading = 2,
        Downloaded = 3,
        DecodeQueued = 4,
        Decoding = 5,
        Decoded = 6,
        DecodeFailed = 7
    }

    public enum NzbSegmentStatus
    {
        Queued = 0,
        Downloading = 1,
        Downloaded = 2,
        Decoding = 3,
        Decoded = 4
    }

    public enum ImportStatus
    {
        Ok = 0,
        Failed = 1,
        Invalid = 2,
        Rejected =3
    }

    public enum PostProcessingStatus
    {
        Ok = 0,
        VerificationFailed = 1,
        UnpackFailed = 2,
        FailedVerificationAndUnpack = 3
    }
}

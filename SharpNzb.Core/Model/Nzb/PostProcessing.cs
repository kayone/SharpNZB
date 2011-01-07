namespace SharpNzb.Core.Model.Nzb
{
    public enum PostProcessing
    {
        Download = 0, //Downloads only
        Repair = 1, //Download and Repair
        Unpack = 2, //Download, Repair and Unpack
        Delete = 3, //Download, Repair, Unpack and Delete
        Default = -100 //Use the default PostProcessing Level
    }
}

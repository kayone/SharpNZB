using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpNzb.Core.Helper
{
    public class FileSizeFormatHelper
    {
        public enum Unit : long
        {
            B = 1,
            KB = 1024,
            MB = KB * 1024,
            GB = MB * 1024,
            TB = GB * 1024
        }

        private string _pattern = @"(?<Number>\d+\.\d+|\d+)";

        // Define other methods and classes here

        public string Format(object value)
        {
            //Convert from Bytes (Default) to the best match		
            decimal size;
            Unit suffix;

            try
            {
                var match = Regex.Match(value.ToString(), _pattern);
                size = Convert.ToDecimal(match.Groups["Number"].Value);
            }

            catch
            {
                throw new NotFiniteNumberException("Value supplied is not a number");
                return null;
            }

            if (size >= (long)Unit.TB)
            {
                size /= (long)Unit.TB;
                suffix = Unit.TB;
            }

            else if (size >= (long)Unit.GB)
            {
                size /= (long)Unit.GB;
                suffix = Unit.GB;
            }

            else if (size >= (long)Unit.MB)
            {
                size /= (long)Unit.MB;
                suffix = Unit.MB;
            }

            else if (size >= (long)Unit.KB)
            {
                size /= (long)Unit.KB;
                suffix = Unit.KB;
            }

            else
            {
                suffix = Unit.B;
            }

            //return String.Format("{0:N" + precision + "}{1}", size, suffix);
            return String.Format("{0:0.##}{1}", size, suffix);
        }

        public string Format(object value, Unit inputFormat, Unit outputFormat)
        {
            if (inputFormat == outputFormat)
                return value.ToString() + inputFormat;

            decimal size;
            Unit suffix = Unit.B;
            string precision = "2";

            try
            {
                var match = Regex.Match(value.ToString(), _pattern);
                size = Convert.ToDecimal(match.Groups["Number"].Value);
            }

            catch
            {
                throw new NotFiniteNumberException("Value supplied is not a number");
                return null;
            }
            //Ensure we are dealing with Bytes as the source (Convert as required)
            size = size * (long)inputFormat;

            if (outputFormat == Unit.B)
            {
                size /= (long)Unit.B;
                suffix = Unit.B;
            }

            if (outputFormat == Unit.KB)
            {
                size /= (long)Unit.KB;
                suffix = Unit.KB;
            }

            if (outputFormat == Unit.MB)
            {
                size /= (long)Unit.MB;
                suffix = Unit.MB;
            }

            if (outputFormat == Unit.GB)
            {
                size /= (long)Unit.GB;
                suffix = Unit.GB;
            }

            if (outputFormat == Unit.TB)
            {
                size /= (long)Unit.TB;
                suffix = Unit.TB;
            }

            return String.Format("{0:0.##}{1}", size, suffix);
        }
    }
}

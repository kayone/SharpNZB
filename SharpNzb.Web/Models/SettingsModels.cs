using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SharpNzb.Web.Models
{
    public class SettingsModel
    {
        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Temporary Download Folder")]
        [Description("Location to store unprocessed downloads.")]
        public String TempDir
        {
            get;
            set;
        }

        [Required]
        [DataType(DataType.Text)]
        [DisplayName("Completed Download Folder")]
        public String CompleteDir
        {
            get;
            set;
        }
    }
}
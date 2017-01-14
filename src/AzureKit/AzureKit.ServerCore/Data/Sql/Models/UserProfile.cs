using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureKit.Data.Sql.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(254)]
        public string ContactEmail { get; set; }
        public bool NotificationEmailsEnabled { get; set; }
    }
}

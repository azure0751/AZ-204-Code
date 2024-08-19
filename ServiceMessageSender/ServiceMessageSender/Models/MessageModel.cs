using System.ComponentModel.DataAnnotations;

namespace ServiceMessageSender.Models
{
    public class MessageModel
    {
        
        [Display(Name = "Message Content")]
        public string MessageContent { get; set; }

        [Required]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; }

        [Required]
        [Display(Name = "Number of Messages")]
        public int MessageCount { get; set; }
    }
}

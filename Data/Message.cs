using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace HostedServiceSample.Data
{
    public class Message
    {
        public int Id { get; set; }

        public string Text { get; set; }

        [BindNever]
        [Display(Name = "Pig Latin Translation")]
        public string ProcessedText { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace HostedServiceSample.Data
{
    public class Word
    {
        public int Id { get; set; }

        [Display(Name = "Word")]
        public string Text { get; set; }
        
        public int Count { get; set; }
    }
}

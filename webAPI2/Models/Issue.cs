using System.ComponentModel.DataAnnotations;

namespace webAPI2.Models
{


    public class Issue
    {
        public string Id { get; set; }
        [Required]
        //Specifies that these properties must have a value
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        public Priority Priority { get; set; }

        public IssueType IssueType { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Completed { get; set; }
    }
    public enum Priority { 
    Low, Medium, High
    }

    public enum IssueType { Feature , Bug , Documentation}
}

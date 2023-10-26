using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _07_NguyenDinhSon_Assignment_03.Models
{
    public class Posts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostID { get; set; }
        public int AuthorID { get; set; }
        public int CategoryID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set;}
        public string Title { get; set; }
        public string Content { get; set; }
        public Boolean PublishStatus { get; set; }

        public virtual AppUsers? AppUsers { get; set; }
        public virtual PostCategories? PostCategories { get; set; }
    }
}

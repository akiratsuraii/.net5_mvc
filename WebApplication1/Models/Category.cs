using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("test")] //cshtml的asp-for會顯示出這邊的名稱
        [Range(1,99,ErrorMessage ="must lesser then 5 words")]
        public string Name { get; set; }
    }
}

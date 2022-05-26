using System.ComponentModel;

namespace WebApplication1.Models
{
    public class Category
    {
        public int Id { get; set; }
        [DisplayName("test")] //cshtml的asp-for會顯示出這邊的名稱
        public string Name { get; set; }
    }
}

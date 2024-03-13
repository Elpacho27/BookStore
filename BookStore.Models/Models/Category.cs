using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [MaxLength(30)]
    public string Name { get; set; }
    [DisplayName("Display Name")]
    [Range(1,30)]

    public int DisplayOrder { get; set; }

}

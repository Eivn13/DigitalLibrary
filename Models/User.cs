using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Digital_library.Models;

public class User
{
    public int Id { get; set;}
    
    [Required]
    [MaxLength(10)]
    public String? Name { get; set;}

    [Required]
    [MaxLength(20)]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public String? Email { get; set;}

    [JsonIgnore]
    public ICollection<Book>? BorrowedBooks { get; set;}
}
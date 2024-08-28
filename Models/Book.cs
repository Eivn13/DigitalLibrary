using System.ComponentModel.DataAnnotations;
namespace Digital_library.Models;

public class Book
{
    public int Id { get; set;}
    
    [Required]
    [MaxLength(20)]
    public string? Name { get; set;}

    public User? BorrowedBy { get; set;}

    public DateTime? Until { get; set;}
}
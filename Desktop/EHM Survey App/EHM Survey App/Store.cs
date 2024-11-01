using System.ComponentModel.DataAnnotations;

public class Store
{
    [Key] // Primary key olarak belirtin
    public int StoreId { get; set; }
    
    [Required(ErrorMessage = "Mağaza kodu gerekli")]
    public string StoreCode { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Mağaza adı gerekli")]
    public string StoreName { get; set; } = string.Empty;
}

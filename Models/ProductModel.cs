using System.ComponentModel.DataAnnotations.Schema;

namespace projektdotnet.Models; 

public class ProductModel {
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int Price { get; set; }

    //Gör bildtillägg möjligt
    public string? ImageName { get; set; }
    
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    //kopplar till kategori 
    public int CategoryModelId { get; set; }
    public CategoryModel? Category { get; set; }
}
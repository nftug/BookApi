using System.ComponentModel.DataAnnotations;

namespace BookApi.Infrastructure.DataModels.Intermediates;

public class BookAuthorDataModel
{
    [Key] public int ID { get; set; }
    public int BookID { get; set; }
    public int AuthorID { get; set; }
}

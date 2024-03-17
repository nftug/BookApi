namespace BookApi.Infrastructure.DataModels.Intermediates;

public class BookAuthorDataModel
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }
    public int Order { get; set; }

    public virtual BookDataModel Book { get; set; } = null!;
    public virtual AuthorDataModel Author { get; set; } = null!;
}

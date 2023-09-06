namespace BookShop.Entities
{
    public class AuthorAddress
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }


        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}

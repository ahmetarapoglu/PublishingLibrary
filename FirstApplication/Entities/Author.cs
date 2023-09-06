namespace BookShop.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public decimal TotalPayment { get; set; }
        
        public AuthorAddress AuthorAddress { get; set; }
        public AuthorBiography AuthorBiography { get; set; }

        public List<BookAuthor> BookAuthors { get; set; }

        public List<AuthorPayment> AuthorPayments { get; set; }


    }
}

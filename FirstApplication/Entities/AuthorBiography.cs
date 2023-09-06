namespace BookShop.Entities
{
    public class AuthorBiography
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NativeLanguage { get; set; }
        public string Education { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

    }
}

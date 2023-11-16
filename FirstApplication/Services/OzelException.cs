namespace BookShop.Services
{
    public class OzelException : Exception
    {

        public List<Error> Errors;

        public OzelException(Error error)
        {
            this.Errors = new List<Error>() { error };
        }

        public OzelException(List<Error> errors)
        {
            this.Errors = errors;
        }

    }
}

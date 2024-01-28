using System.ComponentModel;

namespace BookShop.Models.RequestModels
{
    public class VersionRequest : DataTableRequest
    {
        [DefaultValue(null)]
        public int BookId { get; set; }
    }
}

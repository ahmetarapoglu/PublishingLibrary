using System.ComponentModel;

namespace BookShop.Models.RequestModels
{
    public class DataTableRequest
    {
        [DefaultValue(1)]
        public int CurrentPage { get; set; }
        [DefaultValue(10)]
        public int PageSize { get; set; }
        [DefaultValue("")]
        public string Search { get; set; }

        public int Skip => (CurrentPage - 1 ) * PageSize;
        public int Take => PageSize;
    }
}

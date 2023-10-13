using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.RequestModels
{
    public class DataTableRequest
    {
        [DefaultValue(1)]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be between 1 and the maximum allowed value.")]
        public int Current { get; set; }
        [DefaultValue(10)]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be between 1 and the maximum allowed value.")]
        public int PageSize { get; set; }
        [DefaultValue("")]
        public string Search { get; set; }

        public int Skip => (Current - 1 ) * PageSize;
        public int Take => PageSize;
    }
}

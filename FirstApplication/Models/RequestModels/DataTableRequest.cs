using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.RequestModels
{

    public class DataTableRequest
    {
        [DefaultValue(1)]
        //[Range(1, int.MaxValue, ErrorMessage = "The value must be between 1 and the maximum allowed value.")]
        public int Current { get; set; }
        [DefaultValue(100)]
        //[Range(1, int.MaxValue, ErrorMessage = "The value must be between 1 and the maximum allowed value.")]
        public int PageSize { get; set; }
        [DefaultValue("")]
        public string? Search { get; set; }
        [DefaultValue("id")]
        public string? Order { get; set; }
        [DefaultValue("ascend")]
        public string? SortDir { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public int Skip => (Current - 1 ) * PageSize;
        public int Take => PageSize;
    }
}

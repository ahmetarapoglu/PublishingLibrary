using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace BookShop.Models.BranchModels
{
    public class BranchModel
    {
        [Required(ErrorMessage = "BranchName is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters")]
        public string BranchName { get; set; }
        [Required(ErrorMessage = "BranchAddress is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "BranchAddress must be between 3 and 50 characters")]
        public string BranchAddress { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }
    }
}


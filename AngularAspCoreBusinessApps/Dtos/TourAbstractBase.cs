using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AngularAspCoreBusinessApps.Dtos
{
    public abstract class TourAbstractBase : IValidatableObject
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title is too long.")]
        public string Title { get; set; }

        [MaxLength(2000, ErrorMessage = "Description is too long.")]
        public virtual string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The start date is required.")]
        public DateTimeOffset StartDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "The end date is required.")]
        public DateTimeOffset EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!(StartDate < EndDate))
            {
                yield return new ValidationResult(
                    "The start date should be smaller than the end date.",
                    new[] { "Tour" });
            }
        }
    }
}

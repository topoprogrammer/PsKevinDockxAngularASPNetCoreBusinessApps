using System.Collections.Generic;

namespace AngularAspCoreBusinessApps.Dtos
{
    public class TourWithManagerAndShowsForCreation : TourWithManagerForCreation
    {
        public ICollection<ShowForCreation> Shows { get; set; }
            = new List<ShowForCreation>();
    }
}

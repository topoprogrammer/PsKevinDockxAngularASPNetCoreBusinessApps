using System.Collections.Generic;

namespace AngularAspCoreBusinessApps.Dtos
{
    public class TourWithShowsForCreation : TourForCreation
    {
        public ICollection<ShowForCreation> Shows { get; set; }
          = new List<ShowForCreation>();
    }
}

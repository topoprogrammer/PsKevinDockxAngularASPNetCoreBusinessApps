using System.Collections.Generic;

namespace AngularAspCoreBusinessApps.Dtos
{
    public class TourWithEstimatedProfitsAndShows : TourWithEstimatedProfits
    {
        public ICollection<Show> Shows { get; set; }
              = new List<Show>();
    }
}

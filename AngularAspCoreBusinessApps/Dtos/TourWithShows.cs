using System.Collections.Generic;

namespace AngularAspCoreBusinessApps.Dtos
{
    public class TourWithShows : Tour
    {
        public ICollection<Show> Shows { get; set; }
            = new List<Show>();
    }
}

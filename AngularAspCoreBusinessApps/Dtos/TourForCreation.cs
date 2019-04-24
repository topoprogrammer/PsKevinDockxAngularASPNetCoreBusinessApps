using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularAspCoreBusinessApps.Dtos
{
    public class TourForCreation : TourAbstractBase
    {
        public Guid BandId { get; set; }
    }
}

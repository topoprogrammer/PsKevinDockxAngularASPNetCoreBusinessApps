using System;

namespace AngularAspCoreBusinessApps.Dtos
{
    public class Tour : TourAbstractBase
    {
        public Guid TourId { get; set; }
        public string Band { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularAspCoreBusinessApps.Dtos
{
    public class TourWithManagerForCreation : TourForCreation
    {
        public string ManagerId { get; set; }
    }
}

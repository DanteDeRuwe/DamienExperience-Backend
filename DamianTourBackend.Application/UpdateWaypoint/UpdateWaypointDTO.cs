using System.Collections.Generic;

namespace DamianTourBackend.Application.UpdateWaypoint
{
    public class UpdateWaypointDTO
    {
        public string TourName { get; set; }
        public IEnumerable<WaypointDTO> Dtos { get; set; }
    }
}

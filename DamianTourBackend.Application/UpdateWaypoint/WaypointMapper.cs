using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.UpdateWaypoint
{
    public static class WaypointMapper
    {
        public static void UpdateWaypoint(this WaypointDTO model, ref Waypoint waypoint)
        {
            waypoint.Longitude = model.Longitude;
            waypoint.Latitude = model.Latitude;
            waypoint.LanguagesText = model.LanguagesText;
        }

        public static Waypoint MapToWaypoint(this WaypointDTO model) =>
            new Waypoint
            {
                Longitude = model.Longitude,
                Latitude = model.Latitude,
                LanguagesText = model.LanguagesText
            };

        public static WaypointDTO MapToWaypointDTO(this Waypoint waypoint) =>
            new WaypointDTO
            {
                Longitude = waypoint.Longitude,
                Latitude = waypoint.Latitude,
                LanguagesText = waypoint.LanguagesText
            };

        public static ICollection<Waypoint> MapToWaypoints(this ICollection<WaypointDTO> waypoints)
        {
            return waypoints.Select(dto => MapToWaypoint(dto)).ToList();
        }

        public static ICollection<WaypointDTO> MapToWaypointDTOs(this ICollection<Waypoint> waypoints)
        {
            if (waypoints == null) return null;
            return waypoints.Select(waypoint => MapToWaypointDTO(waypoint)).ToList();
        }
    }
}

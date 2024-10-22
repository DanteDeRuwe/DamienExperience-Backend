﻿using DamianTourBackend.Application.UpdateWaypoint;
using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Application.UpdateRoute
{
    public class RouteDTO
    {
        //public Guid TourId { get; set; }
        public string TourName { get; set; }
        public DateTime Date { get; set; }
        public int DistanceInMeters { get; set; }
        public string LineColor { get; set; }
        public List<double[]> Coordinates { get; set; }
        public Dictionary<string, string> Info { get; set; }
        public ICollection<WaypointDTO> Waypoints { get; set; }
    } 
}

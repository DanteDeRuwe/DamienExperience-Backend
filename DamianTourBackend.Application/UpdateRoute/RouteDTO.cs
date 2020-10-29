using DamianTourBackend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Application.UpdateRoute
{
    public class RouteDTO
    {
        public string TourName { get; set; }
        public int DistanceInMeters { get; set; }
        public List<double[]> Coordinates { get; set; }
    } 
}

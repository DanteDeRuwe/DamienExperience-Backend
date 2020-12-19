using System;
using System.Collections.Generic;
using System.Text;

namespace DamianTourBackend.Application.UpdateWalk
{
    public class WalkDTO
    {
        public Guid ID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LineColor { get; set; }
        public List<double[]> Coordinates { get; set; }
    }
}

using System.Collections.Generic;

namespace DamianTourBackend.Core.Entities
{
    public class Path
    {
        public List<double[]> Coordinates { get; set; }

        public Path()
        {
            Coordinates = new List<double[]>();
        }
    }
}
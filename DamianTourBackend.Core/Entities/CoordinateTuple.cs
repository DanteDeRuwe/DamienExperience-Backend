namespace DamianTourBackend.Core.Entities
{
    public class CoordinateTuple
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public CoordinateTuple(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}

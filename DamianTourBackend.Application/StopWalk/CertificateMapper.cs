using System;
using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.StopWalk
{
    public class CertificateMapper
    {
        public static CertificateDTO DTOFrom(User user, Walk walk, Route route)
        {
            return new CertificateDTO() {
                Id = walk.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Distance = (route.DistanceInMeters / 1000).ToString() + " KM",
                Date = DateTime.Now.ToString(),
            };
        }
    }
}
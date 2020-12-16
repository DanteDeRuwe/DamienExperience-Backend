using System;

namespace DamianTourBackend.Api.Helpers
{
    public static class DateCheckHelper
    {
        public static bool CheckEqualsDate(DateTime datetime1, DateTime datetime2)
        {
            var routeDate = datetime1.Date;
            var now = datetime2.Date;
            if (routeDate.Year != now.Year) return false;
            if (routeDate.Month != now.Month) return false;
            if (routeDate.Day != now.Day) return false;
            return true;
        }

        public static bool CheckAfterOrEqualsToday(DateTime datetime1)
        {
            return datetime1.Date >= DateTime.Now.Date;
        }

        public static bool CheckBeforeToday(DateTime datetime1)
        {
            return datetime1.Date < DateTime.Now.Date;
        }

    }
}

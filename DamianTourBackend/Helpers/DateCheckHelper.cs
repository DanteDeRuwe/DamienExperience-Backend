﻿using System;

namespace DamianTourBackend.Api.Helpers
{
    public static class DateCheckHelper
    {
        public static bool CheckEqualsDate(DateTime datetime1, DateTime datetime2)
        {
            var date1 = datetime1.Date;
            var date2 = datetime2.Date;
            if (date1.Year != date2.Year) return false;
            if (date1.Month != date2.Month) return false;
            if (date1.Day != date2.Day) return false;
            return true;
        }

        public static bool CheckGreaterThenOrEqualsDate(DateTime datetime1)
        {
            return datetime1.Date < DateTime.Now.Date;
        }

    }
}

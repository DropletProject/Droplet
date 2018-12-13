using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Utility.Data
{
    public class DateTimePeriod
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public DateTimePeriod(DateTime startTime, DateTime endTime)
        {
            if (startTime > endTime)
                throw new Exception("Endtime should great than starttime.");

            StartTime = startTime;
            EndTime = endTime;
        }
    }
}

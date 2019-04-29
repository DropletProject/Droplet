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

        public bool Contains(DateTime dateTime, CompareOption compareOption = CompareOption.SemiClose)
        {
            if (compareOption == CompareOption.SemiClose && dateTime >= StartTime && dateTime < EndTime)
            {
                return true;
            }

            if (compareOption == CompareOption.SemiOpen && dateTime > StartTime && dateTime <= EndTime)
            {
                return true;
            }

            if (compareOption == CompareOption.OpenClose && dateTime >= StartTime && dateTime <= EndTime)
            {
                return true;
            }

            if (compareOption == CompareOption.SemiOpenClose && dateTime > StartTime && dateTime < EndTime)
            {
                return true;
            }

            return false;
        }
    }

    public enum CompareOption
    {
        SemiOpenClose,
        SemiOpen,
        SemiClose,
        OpenClose
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RingBell
{
    internal class Utility
    {
        public static Lesson GenerateFirstLesson(Shift shift, int duration)
        {
            return new Lesson(shift.StartHour, shift.StartMinutes, duration);
        }
    }
}

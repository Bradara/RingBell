using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RingBell
{
    internal class Lesson
    {
        private int duration;
        private string timeStart;
        private string timeEnd;

        public Lesson() { }
        public Lesson(int hourStart, int minutesStart, int hourEnd, int minutesEnd, int @break)
        {
            HourStart = hourStart;
            MinutesStart = minutesStart;
            TimeStart = hourStart + ":" + minutesStart.ToString("00");
            HourEnd = hourEnd;
            MinutesEnd = minutesEnd;
            TimeEnd = hourEnd + ":" + minutesEnd.ToString("00");
            Break = @break;
            duration = (HourEnd * 60 + MinutesEnd) - (HourStart * 60 + MinutesStart);
        }

        public Lesson(int hourStart, int minutesStart, int duration)
        {
            HourStart = hourStart;
            MinutesStart = minutesStart;
            Duration = duration;
            SetEndTimes();
            TimeStart = hourStart + ":" + minutesStart.ToString("00");
            TimeEnd = HourEnd + ":" + MinutesEnd.ToString("00");
        }

        private void SetEndTimes()
        {
            MinutesEnd = MinutesStart + Duration;
            if(MinutesEnd >= 60) 
            {
                MinutesEnd -= 60;
                HourEnd = HourStart + 1;
            }
            else
            {
                HourEnd = HourStart;
            }
        }

        public int HourStart { get; set; }
        public int MinutesStart { get; set; }
        public int HourEnd { get; set; }
        public int MinutesEnd { get; set; }
        public int Break { get; set; }

        public int Id { get; set; }
        public string TimeStart
        {
            get => timeStart;
            set
            {
                timeStart = value;
                string[] tokens = value.Split(":");
                HourStart = int.Parse(tokens[0]);
                MinutesStart = int.Parse(tokens[1]);
            } 
        }

        public string TimeEnd
        {
            get => timeEnd;
            set
            {
                timeEnd = value;
                string[] tokens = value.Split(":");
                HourEnd = int.Parse(tokens[0]);
                MinutesEnd = int.Parse(tokens[1]);
            }
        }

        public int Duration {
            get => duration;
            set => duration = value;
        }

        public static Lesson NextLesson(Lesson lesson, int @break, int duration)
        {
            int newLessonStartMin = lesson.MinutesEnd + @break;
            int newLessonStartHour = lesson.HourEnd;
            if(newLessonStartMin >= 60)
            {
                newLessonStartMin -= 60;
                newLessonStartHour++;
            }

            Lesson l = new Lesson(newLessonStartHour, newLessonStartMin, duration);
            return l;
        }


    }
}

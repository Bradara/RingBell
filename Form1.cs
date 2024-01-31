using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;


namespace RingBell
{
    public partial class Form1 : Form
    {
        private WMPLib.WindowsMediaPlayer player;
        private System.Windows.Forms.Timer clockTimer;
        private System.Windows.Forms.Timer ringTimer;       
        private Shift[]? shifts;
        private int[] lessonsDurations;
        private int[] breaks;
        private Lesson[] lessonsFirstShift = new Lesson[14];
        public Form1()
        {
            InitializeComponent();
            player = new WMPLib.WindowsMediaPlayer();

            clockTimer = new System.Windows.Forms.Timer();
            ringTimer = new System.Windows.Forms.Timer();

            lessonsFirstShift = DataAccess.GetLessons();
        }



         private void UpdateBreaks()
        {
            ComboBox[] cboxs = this.Controls.OfType<ComboBox>().ToArray();
            for (int i = 0; i < cboxs.Length; i++)
            {
                int nameLength = cboxs[i].Name.Length;
                int lessonIndex = int.Parse(cboxs[i].Name.Substring(nameLength-1));
                cboxs[i].Text = lessonsFirstShift[lessonIndex-1].Break.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            player.URL = ".\\TempleOfTheKing_cut.mp3";
            player.controls.stop();
            clockTimer.Interval = 1000;
            clockTimer.Tick += new EventHandler(Timer_tick);
            clockTimer.Start();            
            ringTimer.Interval = 1000;
            ringTimer.Start();
            btnStartAutoRing_Click(sender, e);
            UpdateLessonsTimes();
            UpdateBreaks();
        }

        private void Timer_tick(object? sender, EventArgs e)
        {
            lblClock.Text = DateTime.Now.Hour + " : " + DateTime.Now.Minute.ToString("00")
                + " : " + DateTime.Now.Second.ToString("00");

        }

        private void btnGenerateTimeTable1_Click(object sender, EventArgs e)
        {
            string firstLessonStart = tboxLesson1Start1.Text;
            string firstLessonEnd = tboxLesson1End1.Text;
            string firstLessonBreak = cbox1break1.Text;

            if (firstLessonStart == "" || firstLessonEnd == "" || firstLessonBreak == "")
            {
                MessageBox.Show("Въведете данни за начало, край и междучасие за първи час.\nКакто и всички междучасия!!!");
            }
            else
            {
                int[] firstLessonTimesStart = tboxLesson1Start1.Text
                    .Split(":", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                int[] firstLessonTimesEnd = tboxLesson1End1.Text
                    .Split(":", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                int firstBreak = int.Parse(cbox1break1.Text);
                lessonsFirstShift[0] = new Lesson(firstLessonTimesStart[0], firstLessonTimesStart[1], firstLessonTimesEnd[0], firstLessonTimesEnd[1], firstBreak);

                GenerateLessonsFirstShift();
                UpdateLessonsTimes();

            }

        }

        private void UpdateLessonsTimes()
        {
            var tboxStart = this.Controls.OfType<TextBox>()
                .Where(x => x.Name.StartsWith("tboxLesson1Start") || x.Name.StartsWith("tboxLesson2Start"))
                .OrderBy(x => x.Name).ToArray();
            var tboxEnd = this.Controls.OfType<TextBox>()
                .Where(x => x.Name.StartsWith("tboxLesson1End") || x.Name.StartsWith("tboxLesson2End"))
                .OrderBy(x => x.Name).ToArray();

            for (int i = 0; i < lessonsFirstShift.Length; i++)
            {
                Lesson l = lessonsFirstShift[i];
                tboxStart[i].Text = l.TimeStart.ToString();
                tboxEnd[i].Text = l.TimeEnd.ToString();                
            }
        }

        private void GenerateLessonsFirstShift()
        {
            int lessonduration = lessonsFirstShift[0].Duration;

            for (int i = 1; i < 7; i++)
            {
                Lesson prevLesson = lessonsFirstShift[i - 1];
                Lesson newLesson = Lesson.NextLesson(prevLesson, prevLesson.Break, lessonduration);

                switch (i)
                {
                    case 1: newLesson.Break = int.Parse(cbox1break2.Text); break;
                    case 2: newLesson.Break = int.Parse(cbox1break3.Text); break;
                    case 3: newLesson.Break = int.Parse(cbox1break4.Text); break;
                    case 4: newLesson.Break = int.Parse(cbox1break5.Text); break;
                    case 5: newLesson.Break = int.Parse(cbox1break6.Text); break;
                    case 6: newLesson.Break = int.Parse(cbox1break7.Text); break;
                    default: return;
                }

                lessonsFirstShift[i] = newLesson;
            }
        }

        private async void chooseButton_Click(object sender, EventArgs e)
        {
            player.controls.play();
            await Task.Delay(25000);
            player.controls.stop();
        }

        private void CheckForRinging(object? sender, EventArgs e)
        {
            string dayOfWeek = DateTime.Now.DayOfWeek.ToString();
            int hours = DateTime.Now.Hour;
            int minutes = DateTime.Now.Minute;
            List<Lesson> lesson = lessonsFirstShift.Where(x => (x.HourEnd == hours && x.MinutesEnd == minutes) || (x.HourStart == hours && x.MinutesStart == minutes)).ToList();
            var daysOfWeek1Shift = this.Controls.OfType<CheckBox>()
                .Where(x => x.Name.StartsWith("cbox1") && x.Checked)
                .Select(x => x.Name.Substring(5)).ToArray();
            var daysOfWeek2Shift = this.Controls.OfType<CheckBox>()
                .Where(x => x.Name.StartsWith("cbox2") && x.Checked)
                .Select(x => x.Name.Substring(5)).ToArray();

            string status = player.status.Substring(0, 4);

            if (lesson.Count == 1 && status != "Play")
            {                
                int id = lesson[0].Id;
                if (id < 8 && !daysOfWeek1Shift.Contains(dayOfWeek))
                    return;
                if (id > 7 && !daysOfWeek2Shift.Contains(dayOfWeek))
                    return;

                player.controls.play();

                ringTimer.Interval = 15000;
            }
            else if(lesson.Count == 1 && status == "Play")
            {
                player.controls.stop();
                ringTimer.Interval = 45000;
            }
            else
            {
                player.controls.stop();

                ringTimer.Interval = 1000;
            }

        }

        private void btnStartAutoRing_Click(object sender, EventArgs e)
        {
            ringTimer.Tick += new EventHandler(CheckForRinging);

            ringTimer.Start();
        }
    }
}

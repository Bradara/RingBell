using System.Data;


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
        private Lesson[] lessons = new Lesson[14];
        public Form1()
        {
            InitializeComponent();
            player = new WMPLib.WindowsMediaPlayer();

            clockTimer = new System.Windows.Forms.Timer();
            ringTimer = new System.Windows.Forms.Timer();

            lessons = DataAccess.GetLessons();
        }



        private void UpdateBreaks()
        {
            ComboBox[] cboxs = this.Controls.OfType<ComboBox>().ToArray();
            for (int i = 0; i < cboxs.Length; i++)
            {
                int nameLength = cboxs[i].Name.Length;
                int lessonIndex = int.Parse(cboxs[i].Name.Substring(nameLength - 1));
                cboxs[i].Text = lessons[lessonIndex - 1].Break.ToString();
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
                lessons[0] = new Lesson(firstLessonTimesStart[0], firstLessonTimesStart[1], firstLessonTimesEnd[0], firstLessonTimesEnd[1], firstBreak);

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

            for (int i = 0; i < lessons.Length; i++)
            {
                Lesson l = lessons[i];
                tboxStart[i].Text = l.TimeStart.ToString();
                tboxEnd[i].Text = l.TimeEnd.ToString();
            }
        }

        private void GenerateLessonsFirstShift()
        {
            int lessonduration = lessons[0].Duration;

            for (int i = 1; i < 7; i++)
            {
                Lesson prevLesson = lessons[i - 1];
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

                lessons[i] = newLesson;
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
            List<Lesson> lesson = lessons.Where(x => (x.HourEnd == hours && x.MinutesEnd == minutes) || (x.HourStart == hours && x.MinutesStart == minutes)).ToList();
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
                try
                {
                HIDRelay.OpenRelay();
                }
                catch (Exception ex)
                {
                    lblInfo.Text = ex.Message;
                }

                ringTimer.Interval = 15000;
            }
            else if (lesson.Count == 1 && status == "Play")
            {
                player.controls.stop();
                try
                {
                    HIDRelay.CloseRelay();
                }
                catch (Exception ex)
                {
                    lblInfo.Text = ex.Message;
                }
                
                ringTimer.Interval = 45000;
            }
            else
            {
                player.controls.stop();
                try
                {
                    HIDRelay.CloseRelay();
                }
                catch (Exception ex)
                {
                    lblInfo.Text = ex.Message;
                }

                ringTimer.Interval = 1000;
            }

        }

        private void btnStartAutoRing_Click(object sender, EventArgs e)
        {
            ringTimer.Tick += new EventHandler(CheckForRinging);

            ringTimer.Start();
        }

        private void btnGenerateTimeTable2_Click(object sender, EventArgs e)
        {
            string firstLessonStart = tboxLesson2Start1.Text;
            string firstLessonEnd = tboxLesson2End1.Text;
            string firstLessonBreak = cbox2break1.Text;

            if (firstLessonStart == "" || firstLessonEnd == "" || firstLessonBreak == "")
            {
                MessageBox.Show("Въведете данни за начало, край и междучасие за първи час.\nКакто и всички междучасия!!!");
            }
            else
            {
                int[] firstLessonTimesStart = tboxLesson2Start1.Text
                    .Split(":", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                int[] firstLessonTimesEnd = tboxLesson2End1.Text
                    .Split(":", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                int firstBreak = int.Parse(cbox2break1.Text);
                lessons[7] = new Lesson(firstLessonTimesStart[0], firstLessonTimesStart[1], firstLessonTimesEnd[0], firstLessonTimesEnd[1], firstBreak);

                GenerateLessonsSecondShift();
                UpdateLessonsTimes();
            }
        }

        private void GenerateLessonsSecondShift()
        {
            int lessonduration = lessons[7].Duration;

            for (int i = 8; i < 14; i++)
            {
                Lesson prevLesson = lessons[i - 1];
                Lesson newLesson = Lesson.NextLesson(prevLesson, prevLesson.Break, lessonduration);

                switch (i)
                {
                    case 8: newLesson.Break = int.Parse(cbox2break2.Text); break;
                    case 9: newLesson.Break = int.Parse(cbox2break3.Text); break;
                    case 10: newLesson.Break = int.Parse(cbox2break4.Text); break;
                    case 11: newLesson.Break = int.Parse(cbox2break5.Text); break;
                    case 12: newLesson.Break = int.Parse(cbox2break6.Text); break;
                    case 13: newLesson.Break = int.Parse(cbox2break7.Text); break;
                    default: return;
                }

                lessons[i] = newLesson;
            }
        }
    }
}

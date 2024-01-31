using Dapper;
using Microsoft.Data.Sqlite;

namespace RingBell
{
    internal class DataAccess
    {
        private static string connString = "Data Source=RingBell.db";

        public static Shift[] GetShifts()
        {
            using var conn = new SqliteConnection(connString);
            Shift[] result = conn.Query<Shift>("SELECT * FROM Shifts").ToArray();
            //Console.WriteLine(result);
            return result;
        }

        public static int[] GetDurations()
        {
            using var conn = new SqliteConnection(connString);
            int[] result = conn.Query<int>("SELECT Duration FROM Lessons_duration").ToArray();
            return result;
        }

        internal static int[]? GetBreaks()
        {
            using var conn = new SqliteConnection(connString);
            int[] result = conn.Query<int>("SELECT Duration FROM Breaks").ToArray();
            return result;
        }

        internal static Lesson[] GetLessons()
        {
            using var conn = new SqliteConnection(connString);
            Lesson[] result = conn.Query<Lesson>("SELECT * FROM Lessons").OrderBy(x => x.Id).ToArray();
            return result;
        }
    }
}

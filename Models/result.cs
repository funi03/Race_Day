namespace Race_Day.Models
{
    public class result
    {
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public int FinishPosition { get; set; }
        public TimeSpan FinishTime { get; set; }
        public string ResultStatus { get; set; } = "Finished"; // Finished, Disqualified, Did Not Finish

        public EventEnrolment? Enrolment { get; set; }
    }
}

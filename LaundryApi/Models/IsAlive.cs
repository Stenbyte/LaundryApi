namespace LaundryApi.Models
{
    public class IsAliveResponse
    {
        public string Status { get; set; } = "";
        public string Commit { get; set; } = "";
        public string CommitDate { get; set; } = "";
        public string BuildTime { get; set; } = "";
    }
}
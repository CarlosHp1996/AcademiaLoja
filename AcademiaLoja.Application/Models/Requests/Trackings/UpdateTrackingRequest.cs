namespace AcademiaLoja.Application.Models.Requests.Trackings
{
    public class UpdateTrackingRequest
    {
        public string Status { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public DateTime EventDate { get; set; }
        public string TrackingNumber { get; set; }
    }
}

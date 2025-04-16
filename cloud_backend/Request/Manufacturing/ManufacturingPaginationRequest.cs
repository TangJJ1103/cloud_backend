namespace cloud_backend.Request.Manufacturing
{
    public class ManufacturingPaginationRequest
    {
        public int offset { get; set; }
        public int currentIndex { get; set; }
        public int? status { get; set; }
        public string? searchTerm { get; set; }
    }
}

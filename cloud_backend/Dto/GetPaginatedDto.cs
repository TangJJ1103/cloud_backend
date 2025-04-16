namespace cloud_backend.Dto
{
    public class GetPaginatedDto<T>
    {
        public List<T> data { get; set; }
        public int total { get; set; }
    }
}

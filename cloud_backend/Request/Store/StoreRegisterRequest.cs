namespace cloud_backend.Request.Store
{
    public class StoreRegisterRequest
    {
        public string username { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string? address { get; set; }
        public string contactNumber { get; set; }
        public bool isActive { get; set; }
    }
}

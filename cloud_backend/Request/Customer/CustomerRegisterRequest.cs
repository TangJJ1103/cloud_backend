namespace cloud_backend.Request.Customer
{
    public class CustomerRegisterRequest
    {
        public string username { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string? address { get; set; }
        public string contactNumber { get; set; }
    }
}

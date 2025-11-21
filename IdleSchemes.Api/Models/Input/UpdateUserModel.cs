namespace IdleSchemes.Api.Models.Input {
    public class UpdateUserModel {
        public string? Name { get; set; }
        public bool RemoveEmail { get; set; } = false;
        public string? Email { get; set; }
        public bool RemovePhone { get; set; } = false;
        public string? Phone { get; set; }
        public string? Region { get; set; }
        public string? TimeZone { get; set; }
        public string? Password { get; set; }
    }
}

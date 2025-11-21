namespace IdleSchemes.Api.Models {
    public class RegistrationInfoModel {
        public RegistrationInfoModel() {
        }
        public string Id { get; set; } = "";
        public string Secret { get; set; } = "";
        public DateTime Started { get; set; }
    }
}

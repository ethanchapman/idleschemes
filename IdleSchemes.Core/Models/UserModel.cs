using IdleSchemes.Data.Models;

namespace IdleSchemes.Core.Models {
    public class UserModel {

        public UserModel() {
        }

        public UserModel(User user) {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Phone = user.Phone;
        }

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Email { get; set; }
        public string? Phone { get; set; }

    }
}

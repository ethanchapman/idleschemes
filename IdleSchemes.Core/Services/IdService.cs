using System.Security.Cryptography;

namespace IdleSchemes.Core.Services {
    public class IdService {

        private const string ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        public string GenerateId() {
            return new string(Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Select(c => ALPHABET.Contains(c) ? c : RandomNumberGenerator.GetItems<char>(ALPHABET,1)[0])
                .ToArray());
        }

        public string GenerateSecret(int length) {
            return RandomNumberGenerator.GetString(ALPHABET, length);
        }

    }
}

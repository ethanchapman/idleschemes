using System.Diagnostics.CodeAnalysis;
using IdleSchemes.Data;
using IdleSchemes.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Core.Services {
    public class UserService {

        private readonly IdleDbContext _dbContext;
        private readonly IdService _idService;
        private readonly TimeService _timeService;

        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UserService(IdleDbContext dbContext, IdService idService, TimeService timeService) {
            _dbContext = dbContext;
            _idService = idService;
            _timeService = timeService;
        }

        public async Task<User?> TryGetUserAsync(string? email = null, string? phone = null) {
            List<User> users;
            string? normalizedPhone;
            if (!string.IsNullOrEmpty(email) && IsPhoneNumber(phone, out normalizedPhone)) {
                users = await QueryUsers()
                    .Where(u => u.Email == email || u.Phone == normalizedPhone)
                    .ToListAsync();
            } else if (!string.IsNullOrEmpty(email)) {
                users = await QueryUsers()
                    .Where(u => u.Email == email)
                    .ToListAsync();
            } else if (IsPhoneNumber(phone, out normalizedPhone)) {
                users = await QueryUsers()
                    .Where(u => u.Phone == normalizedPhone)
                    .ToListAsync();
            } else {
                return null;
            }
            if (users.Count() > 1) {
                throw new Exception($"Multiple users found for '{email}'/'{phone}'");
            }
            return users.FirstOrDefault();
        }

        public async Task<User> GetOrCreateUserAsync(string? email = null, string? phone = null, string? name = null, Region? region = null) {
            User? user = await TryGetUserAsync(email, phone);
            if (user is null) {
                if (string.IsNullOrEmpty(email) && !IsPhoneNumber(phone, out _)) {
                    throw new Exception("Must provide an email or a phone");
                }
                user = _dbContext.Users.Add(new User {
                    Id = _idService.GenerateId(),
                    Name = name ?? "New User",
                    Region = region,
                    Created = _timeService.GetNow(),
                }).Entity;
            }
            if (string.IsNullOrEmpty(user.Email)) {
                user.Email = email;
            }
            if(IsPhoneNumber(phone, out var normalized)) {
                user.Phone = normalized;
            }
            if(user.Region is null) {
                user.Region = region;
            }
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public void SetPassword(User user, string? password) {
            if (password is null) {
                user.Password = null;
                return;
            }
            user.Password = _passwordHasher.HashPassword(user, password);
        }

        public bool IsPasswordValid(User user, string? password) {
            if (user.Password is null || password is null) {
                return false;
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.SuccessRehashNeeded) {
                SetPassword(user, password);
            }
            return result != PasswordVerificationResult.Failed;
        }

        private IQueryable<User> QueryUsers() {
            return _dbContext.Users
                .Include(u => u.Region);
        }

        public bool IsPhoneNumber(string? phone, [MaybeNullWhen(false)] out string? normalized) {
            if (string.IsNullOrEmpty(phone)) {
                normalized = null;
                return false;
            }
            var containsValidCharacters = phone.All(c => "1234567890.+-_ ()".Contains(c));
            if (!containsValidCharacters) {
                normalized = null;
                return false;
            }
            normalized = string.Join("", phone.Where(c => "1234567890+".Contains(c)));
            if (!normalized.StartsWith("+")) {
                if(normalized.Length == 10) {
                    normalized = "+1" + normalized;
                } else {
                    normalized = "+" + normalized;
                }
            }
            return true;
        }

    }
}

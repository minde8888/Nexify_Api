﻿using Microsoft.AspNetCore.Identity;

namespace Nexify.Domain.Entities.Auth
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Roles { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool AcceptTerms { get; set; }
        public string VerificationToken { get; set; }
        public DateTime? Verified { get; set; }
        public bool IsVerified => Verified.HasValue || PasswordReset.HasValue;
        public string ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime? PasswordReset { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
        public bool OwnsToken(string token)
        {
            return RefreshTokens.Find(x => x.Token == token) != null;
        }
    }
}

﻿using Microsoft.AspNetCore.Identity;

namespace FitnessApp1.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public List<Comment> Comments { get; set; }
        public bool IsRememberMe { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}

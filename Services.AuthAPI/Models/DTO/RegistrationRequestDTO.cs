﻿namespace Services.AuthAPI.Models.DTO
{
    public class RegistrationRequestDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; } 
        public string RoleName { get; set; }
    }
}

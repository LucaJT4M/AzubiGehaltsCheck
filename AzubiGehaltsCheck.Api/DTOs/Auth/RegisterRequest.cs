using System.ComponentModel.DataAnnotations;

namespace AzubiGehaltsCheck.Api.DTOs.Auth;

public record RegisterRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(10)] string Password
);
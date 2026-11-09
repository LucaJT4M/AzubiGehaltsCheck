using System.ComponentModel.DataAnnotations;

namespace AzubiGehaltsCheck.Api.DTOs.Auth;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);
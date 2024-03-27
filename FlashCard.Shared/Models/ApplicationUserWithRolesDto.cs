using System.ComponentModel.DataAnnotations;

namespace FlashCard.Shared.Models;

public class ApplicationUserWithRolesDto : ApplicationUserDto
{
    public List<string>? Roles { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Digital_Library.Domain.Entities
{
    public class User : Entity
    {
        [StringLength(100)]
        public string Fullname { get; set; } = null!;

        [StringLength(100)]
        public string Login { get; set; } = null!;

        [StringLength(256)]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string Salt { get; set; } = null!;

        [StringLength(250)]
        public string? Photo { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}

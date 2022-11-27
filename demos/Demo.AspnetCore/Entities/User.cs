using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.AspnetCore.Entities
{
    [Table(nameof(User))]
    public class User
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Dominio.Entidades
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;
        [Required]
        [StringLength(255)]
        public String Email { get; set; } = default!;
        [Required]
        [StringLength(50)]
        public String Senha { get; set; } = default!;
        [Required]
        [StringLength(50)]
        public String Perfil { get; set; } = default!;

    }
}
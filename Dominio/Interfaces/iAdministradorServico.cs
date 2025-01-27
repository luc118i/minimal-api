using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Infraestutura.Interfaces
{
    public interface iAdministradorServico
    {
        Administrador? Login(LoginDTO loginDTO);

        Administrador? Incluir(Administrador administrador);

        Administrador? BuscaPorId(int id);
        List<Administrador> Todos(int? pagina);

    }
}
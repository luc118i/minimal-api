using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestutura.Db;
using minimal_api.Infraestutura.Interfaces;

namespace minimal_api.Dominio.Serviços
{
    public class AdministradorServico : iAdministradorServico
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }
        //buscar por id
        public Administrador? BuscaPorId(int id)
        {
            return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
        }
        //criar usuario
        public Administrador? Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();

            return administrador;
        }
        //login
        public Administrador? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Administradores
                .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha)
                .FirstOrDefault();
            return adm;
        }
        //buscar por todos
        public List<Administrador> Todos(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();

            int itensPorPaginas = 10;
            if (pagina != null)
            {
                query = query.Skip((int)(pagina - 1) * itensPorPaginas).Take(itensPorPaginas);
            }

            return query.ToList();
        }

        // Método para excluir um administrador
        public bool Excluir(int id)
        {
            var administrador = _contexto.Administradores.FirstOrDefault(a => a.Id == id);
            if (administrador == null)
            {
                return false;
            }

            _contexto.Administradores.Remove(administrador);
            _contexto.SaveChanges();
            return true;
        }
        // Método para editar um administrador
        public Administrador? Editar(Administrador administradorAtualizado)
        {
            var administradorExistente = _contexto.Administradores.FirstOrDefault(a => a.Id == administradorAtualizado.Id);
            if (administradorExistente == null)
            {
                return null;
            }

            administradorExistente.Email = administradorAtualizado.Email;
            administradorExistente.Senha = administradorAtualizado.Senha;
            administradorExistente.Perfil = administradorAtualizado.Perfil;

            _contexto.Administradores.Update(administradorExistente);
            _contexto.SaveChanges();

            return administradorExistente;
        }
    }
}
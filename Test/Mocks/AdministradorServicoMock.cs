using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestutura.Interfaces;

namespace Test.Mocks
{
    public class AdministradorServicoMock : iAdministradorServico
    {
        private static List<Administrador> administradores = new List<Administrador>(){
            new Administrador{
                Id = 1,
                Email = "teste@teste.com",
                Senha = "senha",
                Perfil = "Adm"
            },
            new Administrador{
                Id = 2,
                Email = "editor@teste.com",
                Senha = "senha",
                Perfil = "Editor"
            }

        };

        public Administrador? BuscaPorId(int id)
        {
            return administradores.Find(a => a.Id == id);
        }

        public Administrador? Editar(Administrador administrador)
        {
            var index = administradores.FindIndex(a => a.Id == administrador.Id);
            if (index != -1)
            {
                administradores[index] = administrador;
                return administrador;
            }
            return null;
        }

        public bool Excluir(int id)
        {
            return administradores.RemoveAll(a => a.Id == id) > 0;
        }

        public Administrador? Incluir(Administrador administrador)
        {
            administrador.Id = administradores.Count() + 1;
            administradores.Add(administrador);
            return administrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {
            return administradores.FirstOrDefault(a =>
            a.Email == loginDTO.Email &&
            a.Senha == loginDTO.Senha);
        }

        public List<Administrador> Todos(int? pagina)
        {
            return administradores;
        }
    }
}
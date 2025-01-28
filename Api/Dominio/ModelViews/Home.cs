using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Dominio.ModelViews
{
    public struct Home
    {
        public string Apresentaçao { get => "Bem-vindo a API de veiculo - Minimal-API"; }
        public string Documentação { get => "/swagger"; }
    }
}
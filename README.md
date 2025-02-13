Essa API é um sistema que gerencia administradores e veículos. Ela permite que administradores façam login e obtenham um token de acesso (JWT), garantindo segurança. Com esse token, eles podem:

- Criar, listar, editar e excluir administradores e veículos.
- Controlar permissões para que apenas usuários autorizados possam acessar certas funções.
- Usar um banco de dados SQL Server para armazenar as informações.

Resumindo, é uma API segura para gerenciar usuários e veículos, com autenticação e autorização.

# API de Gerenciamento de Administradores e Veículos

Esta API foi desenvolvida utilizando **C# com ASP.NET Core** e segue o modelo de uma **Minimal API**. A API implementa **autenticação e autorização baseadas em JWT (JSON Web Token)**, garantindo controle de acesso seguro.

## 📌 Objetivo da API
Gerenciar **administradores e veículos**, permitindo operações CRUD (Create, Read, Update, Delete).

## 🚀 Funcionalidades Principais

### ✅ Autenticação e Autorização
- Implementação de **JWT** para autenticação de usuários.
- Proteção de rotas com diferentes **perfis de acesso** (Administrador, Editor).

### ✅ Gerenciamento de Administradores
- Login de administradores com geração de **token JWT**.
- Cadastro, listagem, edição e exclusão de administradores.

### ✅ Gerenciamento de Veículos
- Cadastro, listagem, edição e exclusão de veículos.
- Validação de dados (exemplo: ano mínimo permitido para veículos).

### ✅ Banco de Dados
- Utilização do **Entity Framework Core** para comunicação com um **banco de dados SQL Server**.

### ✅ Documentação da API
- **Swagger (OpenAPI)** configurado para facilitar o uso e testes da API.

## 🛠️ Tecnologias Utilizadas
- **C# e ASP.NET Core** (Minimal API)
- **JWT (JSON Web Token)** para autenticação
- **Entity Framework Core** para acesso ao banco de dados
- **SQL Server** como banco de dados
- **Swagger** para documentação

## 🔒 Segurança e Boas Práticas
Esta API foi projetada seguindo **boas práticas de segurança e organização**, garantindo escalabilidade e facilidade de manutenção. Ideal para integrar um sistema de gestão de veículos com usuários administrativos.

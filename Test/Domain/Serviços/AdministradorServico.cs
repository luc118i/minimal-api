using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Testing.Platform.Builder;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Serviços;
using minimal_api.Infraestutura.Db;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class AdministradoresServicoTest
{
    [TestInitialize]
    public void LimparDadosDeTeste()
    {
        var context = CriarContextoDeTest();
        context.Database.ExecuteSqlRaw("DELETE FROM Administradores");  // Limpar a tabela
        context.SaveChanges();
    }


    private DbContexto CriarContextoDeTest()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
        var configuration = builder.Build();

        return new DbContexto(configuration);
    }

    [TestMethod]
    public void TestarBuscaPorId()
    {
        //Arrange
        var context = CriarContextoDeTest();
        context.Database.Migrate();


        var adm = new Administrador();

        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        adm.Email = "teste@teste.com";
        adm.Senha = "senha";
        adm.Perfil = "Adm";
        var administrador = new AdministradorServico(context);


        //Act
        administrador.Incluir(adm);
        var admDoBanco = administrador.BuscaPorId(adm.Id);




        //Assert
        var todosAdministradores = administrador.Todos(1);
        Assert.IsNotNull(admDoBanco);
        Assert.AreEqual(1, admDoBanco.Id);


    }

    [TestMethod]
    public void TestarEditarAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTest();
        context.Database.Migrate();

        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "senha",
            Perfil = "Adm"
        };

        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var administradorServico = new AdministradorServico(context);

        // Incluir o administrador
        var administradorIncluido = administradorServico.Incluir(adm);

        // Verificar se o administrador foi incluído corretamente e se ID foi atribuído
        Assert.IsNotNull(administradorIncluido);
        Assert.AreNotEqual(0, administradorIncluido.Id);  // Verifique se o ID foi atribuído após a inclusão

        // Modificar o administrador
        adm.Email = "novoemail@teste.com";
        adm.Senha = "novasenha";
        adm.Perfil = "SuperAdm";

        // Act
        var administradorEditado = administradorServico.Editar(adm);

        // Assert
        Assert.IsNotNull(administradorEditado);
        Assert.AreEqual("novoemail@teste.com", administradorEditado.Email);
        Assert.AreEqual("novasenha", administradorEditado.Senha);
        Assert.AreEqual("SuperAdm", administradorEditado.Perfil);
    }



    [TestMethod]
    public void TestarExcluirAdministrador()
    {
        // Arrange
        var context = CriarContextoDeTest();
        context.Database.Migrate();


        var adm = new Administrador
        {
            Email = "teste@teste.com",
            Senha = "senha",
            Perfil = "Adm"
        };

        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        var administrador = new AdministradorServico(context);

        // Act
        administrador.Incluir(adm);
        var excluido = administrador.Excluir(adm.Id);

        var admDoBanco = administrador.BuscaPorId(adm.Id);

        // Assert
        Assert.IsTrue(excluido);
        Assert.IsNull(admDoBanco);
    }


}
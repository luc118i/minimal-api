using minimal_api.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class AdministradoresTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        //Arrange
        var adm = new Administrador();

        //Act
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "senha";
        adm.Perfil = "Adm";


        //Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("senha", adm.Senha);
        Assert.AreEqual("Adm", adm.Perfil);
    }
}
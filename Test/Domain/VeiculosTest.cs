using minimal_api.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public sealed class VeiculosTest
{
    [TestMethod]
    public void TestarGetSetPropriedades()
    {
        //Arrange
        var veiculo = new Veiculo();

        //Act
        veiculo.Id = 1;
        veiculo.Nome = "Gol";
        veiculo.Ano = 2009;
        veiculo.Marca = "Volkswagen";


        //Assert
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("Gol", veiculo.Nome);
        Assert.AreEqual(2009, veiculo.Ano);
        Assert.AreEqual("Volkswagen", veiculo.Marca);
    }
}
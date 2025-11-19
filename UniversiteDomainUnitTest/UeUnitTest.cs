using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTest;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateUeUseCase_Success()
    {
        // Arrange
        string numeroUe = "UE001";
        string intitule = "Programmation Orientée Objet";

        // On crée l'UE qui doit être ajoutée en base
        Ue ueSansId = new Ue { NumeroUe = numeroUe, Intitule = intitule };

        // Créons le mock du repository
        var mockUeRepository = new Mock<IUeRepository>();

        // Simulation de GetAllAsync - aucune UE n'existe avec ce numéro
        var reponseGetAll = new List<Ue>();
        mockUeRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(reponseGetAll);

        // Simulation de AddAsync - on simule l'ajout
        mockUeRepository.Setup(repo => repo.AddAsync(It.IsAny<Ue>())).Returns(Task.CompletedTask);

        // Créons une fausse factory qui retourne ce repository
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUeRepository.Object);
        mockFactory.Setup(facto => facto.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);
        var ueTestee = await useCase.ExecuteAsync(ueSansId);

        // Assert
        Assert.That(ueTestee.NumeroUe, Is.EqualTo(numeroUe));
        Assert.That(ueTestee.Intitule, Is.EqualTo(intitule));
        
        // Vérification que les méthodes ont bien été appelées
        mockUeRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        mockUeRepository.Verify(repo => repo.AddAsync(It.IsAny<Ue>()), Times.Once);
        mockFactory.Verify(facto => facto.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public void CreateUeUseCase_InvalidIntitule_ThrowsException()
    {
        // Arrange
        string numeroUe = "UE002";
        string intituleTropCourt = "POO"; // Seulement 3 caractères

        Ue ueAvecIntituleInvalide = new Ue { NumeroUe = numeroUe, Intitule = intituleTropCourt };

        // Créons le mock du repository
        var mockUeRepository = new Mock<IUeRepository>();
        
        // Créons une fausse factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUeRepository.Object);

        // Act & Assert
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);
        
        var exception = Assert.ThrowsAsync<InvalidIntituleUeException>(
            async () => await useCase.ExecuteAsync(ueAvecIntituleInvalide)
        );
        
        Assert.That(exception.Message, Does.Contain("plus de 3 caractères"));
    }

    [Test]
    public void CreateUeUseCase_DuplicateNumeroUe_ThrowsException()
    {
        // Arrange
        string numeroUe = "UE003";
        string intitule = "Base de données";

        Ue nouvelleUe = new Ue { NumeroUe = numeroUe, Intitule = intitule };
        
        // Une UE existe déjà avec ce numéro
        Ue ueExistante = new Ue { Id = 1, NumeroUe = numeroUe, Intitule = "Autre intitulé" };

        // Créons le mock du repository
        var mockUeRepository = new Mock<IUeRepository>();
        
        // Simulation de GetAllAsync - retourne une UE avec le même numéro
        var reponseGetAll = new List<Ue> { ueExistante };
        mockUeRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(reponseGetAll);

        // Créons une fausse factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUeRepository.Object);

        // Act & Assert
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);
        
        var exception = Assert.ThrowsAsync<DuplicateNumeroUeException>(
            async () => await useCase.ExecuteAsync(nouvelleUe)
        );
        
        Assert.That(exception.Message, Does.Contain("déjà affecté"));
    }

    [Test]
    public async Task CreateUeUseCase_WithParameters_Success()
    {
        // Arrange
        string numeroUe = "UE004";
        string intitule = "Développement Web";

        // Créons le mock du repository
        var mockUeRepository = new Mock<IUeRepository>();

        // Simulation de GetAllAsync - aucune UE n'existe
        var reponseGetAll = new List<Ue>();
        mockUeRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(reponseGetAll);

        // Simulation de AddAsync
        mockUeRepository.Setup(repo => repo.AddAsync(It.IsAny<Ue>())).Returns(Task.CompletedTask);

        // Créons une fausse factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto => facto.UeRepository()).Returns(mockUeRepository.Object);
        mockFactory.Setup(facto => facto.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);
        var ueTestee = await useCase.ExecuteAsync(numeroUe, intitule);

        // Assert
        Assert.That(ueTestee.NumeroUe, Is.EqualTo(numeroUe));
        Assert.That(ueTestee.Intitule, Is.EqualTo(intitule));
    }
}


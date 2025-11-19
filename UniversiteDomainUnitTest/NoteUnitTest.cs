using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Create;

namespace UniversiteDomainUnitTest;

public class NoteUnitTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task CreateNoteUseCase_Success()
    {
        // Arrange
        long etudiantId = 1;
        long ueId = 2;
        float valeur = 15.5f;

        // Créer l'étudiant
        Etudiant etudiant = new Etudiant 
        { 
            Id = etudiantId, 
            NumEtud = "E001", 
            Nom = "Dupont", 
            Prenom = "Jean",
            Email = "jean.dupont@test.fr"
        };

        // Créer l'UE
        Ue ue = new Ue 
        { 
            Id = ueId, 
            NumeroUe = "UE001", 
            Intitule = "Programmation" 
        };

        // Créer le parcours avec l'étudiant inscrit et l'UE enseignée
        Parcours parcours = new Parcours
        {
            Id = 3,
            NomParcours = "Licence Info",
            AnneeFormation = 2,
            Inscrits = new List<Etudiant> { etudiant },
            UesEnseignees = new List<Ue> { ue }
        };

        // Mock des repositories
        var mockEtudiantRepo = new Mock<IEtudiantRepository>();
        var mockUeRepo = new Mock<IUeRepository>();
        var mockParcoursRepo = new Mock<IParcoursRepository>();
        var mockNoteRepo = new Mock<INoteRepository>();

        // Setup - L'étudiant existe
        mockEtudiantRepo
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });

        // Setup - L'UE existe
        mockUeRepo
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });

        // Setup - Le parcours existe avec l'étudiant inscrit
        mockParcoursRepo
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });

        // Setup - Aucune note n'existe déjà
        mockNoteRepo
            .Setup(repo => repo.GetByIdAsync(etudiantId, ueId))
            .ReturnsAsync((Note?)null);

        // Setup - Ajout de la note
        mockNoteRepo
            .Setup(repo => repo.AddAsync(It.IsAny<Note>()))
            .Returns(Task.CompletedTask);

        // Mock de la factory
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiantRepo.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUeRepo.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcoursRepo.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNoteRepo.Object);
        mockFactory.Setup(f => f.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        CreateNoteUseCase useCase = new CreateNoteUseCase(mockFactory.Object);
        var noteCreee = await useCase.ExecuteAsync(etudiantId, ueId, valeur);

        // Assert
        Assert.That(noteCreee, Is.Not.Null);
        Assert.That(noteCreee.EtudiantId, Is.EqualTo(etudiantId));
        Assert.That(noteCreee.UeId, Is.EqualTo(ueId));
        Assert.That(noteCreee.Valeur, Is.EqualTo(valeur));

        // Vérifications des appels
        mockEtudiantRepo.Verify(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()), Times.Once);
        mockUeRepo.Verify(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()), Times.Once);
        mockParcoursRepo.Verify(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()), Times.Once);
        mockNoteRepo.Verify(repo => repo.AddAsync(It.IsAny<Note>()), Times.Once);
        mockFactory.Verify(f => f.SaveChangesAsync(), Times.Once);
    }

}


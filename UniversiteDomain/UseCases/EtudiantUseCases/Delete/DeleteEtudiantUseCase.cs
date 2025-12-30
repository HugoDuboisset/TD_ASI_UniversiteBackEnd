using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<int> ExecuteAsync(long idEtudiant)
    {
        await CheckBusinessRules(idEtudiant);
        var repo = repositoryFactory.EtudiantRepository();
        int result = await repo.DeleteAsync(idEtudiant);
        await repositoryFactory.SaveChangesAsync();
        return result;
    }

    private async Task CheckBusinessRules(long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        var repo = repositoryFactory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(repo);

        // Vérifier que l'étudiant existe
        var etudiant = await repo.FindAsync(idEtudiant);
        if (etudiant == null)
        {
            throw new EtudiantNotFoundException($"L'étudiant avec l'ID {idEtudiant} n'existe pas");
        }
    }

    public bool IsAuthorized(string role)
    {
        // Seuls les responsables peuvent supprimer des étudiants
        return role.Equals(Roles.Responsable);
    }
}

using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(long id, string nomParcours, int anneeFormation)
    {
        var p = new Parcours { Id = id, NomParcours = nomParcours, AnneeFormation = anneeFormation };
        return await ExecuteAsync(p);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        var repo = repositoryFactory.ParcoursRepository();
        Parcours p = await repo.CreateAsync(parcours);
        await repositoryFactory.SaveChangesAsync();
        return p;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var repo = repositoryFactory.ParcoursRepository();
        ArgumentNullException.ThrowIfNull(repo);

        List<Parcours> existe = await repo.FindByConditionAsync(p => p.NomParcours.Equals(parcours.NomParcours));

        if (existe is { Count: > 0 }) throw new DuplicateNomParcoursException(parcours.NomParcours + " - ce code de parcours est déjà affecté à un parcours");
        if (parcours.NomParcours.Length < 2) throw new InvalidNomParcoursException(parcours.NomParcours + " incorrect - Le nom d'un parcours doit contenir au moins 2 caractères");
    }
}

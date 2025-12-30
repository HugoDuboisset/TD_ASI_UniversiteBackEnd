using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        var ue = new Ue { NumeroUe = numeroUe, Intitule = intitule };
        return await ExecuteAsync(ue);
    }

    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        var repo = repositoryFactory.UeRepository();
        Ue createdUe = await repo.CreateAsync(ue);
        await repositoryFactory.SaveChangesAsync();
        return createdUe;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var repo = repositoryFactory.UeRepository();
        ArgumentNullException.ThrowIfNull(repo, nameof(repo));

        if (ue.Intitule.Length <= 3)
            throw new InvalidIntituleUeException(ue.Intitule + " - L'intitulé d'une UE doit contenir plus de 3 caractères");

        var ues = await repo.FindByConditionAsync(u => u.NumeroUe.Equals(ue.NumeroUe));

        if (ues.Count > 0)
            throw new DuplicateNumeroUeException(ue.NumeroUe + " - Ce numéro d'UE est déjà affecté à une UE");
    }
}


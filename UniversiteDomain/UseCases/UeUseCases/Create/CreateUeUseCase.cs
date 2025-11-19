using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

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
        await repo.AddAsync(ue);
        await repositoryFactory.SaveChangesAsync();
        return ue;
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var repo = repositoryFactory.UeRepository();
        ArgumentNullException.ThrowIfNull(repo, nameof(repo));

        // Vérification que l'intitulé contient plus de 3 caractères
        if (ue.Intitule.Length <= 3)
            throw new InvalidIntituleUeException(ue.Intitule + " - L'intitulé d'une UE doit contenir plus de 3 caractères");

        // Vérification qu'aucune UE n'a déjà le même numéro
        var ues = await repo.GetAllAsync();
        var existeDeja = ues.Any(u => u.NumeroUe.Equals(ue.NumeroUe));
        
        if (existeDeja)
            throw new DuplicateNumeroUeException(ue.NumeroUe + " - Ce numéro d'UE est déjà affecté à une UE");
    }
}


using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{

    public async Task<Note> ExecuteAsync(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        return await ExecuteAsync(note.EtudiantId, note.UeId, note.Valeur);
    }

    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        await CheckBusinessRules(etudiantId, ueId, valeur);

        // Vérifier si la note existe déjà
        var noteExistante = await repositoryFactory.NoteRepository()
            .GetByIdAsync(etudiantId, ueId);

        Note note = new Note
        {
            EtudiantId = etudiantId,
            UeId = ueId,
            Valeur = valeur
        };

        if (noteExistante == null)
        {
            // Créer une nouvelle note
            await repositoryFactory.NoteRepository().CreateAsync(note);
        }
        else
        {
            // Mettre à jour la note existante
            await repositoryFactory.NoteRepository().UpdateAsync(note);
        }

        await repositoryFactory.SaveChangesAsync();
        return note;
    }

    private async Task CheckBusinessRules(long etudiantId, long ueId, float valeur)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);

        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());

        if (valeur < 0 || valeur > 20)
        {
            throw new InvalidNoteValeurException(
                $"La note doit être comprise entre 0 et 20. Valeur reçue : {valeur}");
        }

        var etudiants = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Id.Equals(etudiantId));

        if (etudiants == null || etudiants.Count == 0)
        {
            throw new EtudiantNotFoundException($"L'étudiant avec l'ID {etudiantId} n'existe pas");
        }

        var ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.Id.Equals(ueId));

        if (ues == null || ues.Count == 0)
        {
            throw new UeNotFoundException(ueId.ToString());
        }

        var tousLesParcours = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.Inscrits != null && p.Inscrits.Any(e => e.Id == etudiantId));

        if (tousLesParcours == null || tousLesParcours.Count == 0)
        {
            throw new UeNotInParcoursException(
                $"L'étudiant {etudiantId} n'est inscrit dans aucun parcours");
        }

        bool ueInParcours = false;
        foreach (var parcours in tousLesParcours)
        {
            if (parcours.UesEnseignees != null &&
                parcours.UesEnseignees.Any(u => u.Id == ueId))
            {
                ueInParcours = true;
                break;
            }
        }

        if (!ueInParcours)
        {
            throw new UeNotInParcoursException(
                $"L'UE {ueId} ne fait pas partie du parcours de l'étudiant {etudiantId}");
        }
    }
}


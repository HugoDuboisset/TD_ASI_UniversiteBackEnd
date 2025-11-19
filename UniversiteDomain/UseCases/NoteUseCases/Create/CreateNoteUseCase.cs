using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    /// <summary>
    /// Crée ou met à jour une note pour un étudiant dans une UE
    /// </summary>
    /// <param name="note">La note à créer</param>
    /// <returns>La note créée ou mise à jour</returns>
    public async Task<Note> ExecuteAsync(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        return await ExecuteAsync(note.EtudiantId, note.UeId, note.Valeur);
    }

    /// <summary>
    /// Crée ou met à jour une note pour un étudiant dans une UE
    /// </summary>
    /// <param name="etudiantId">L'ID de l'étudiant</param>
    /// <param name="ueId">L'ID de l'UE</param>
    /// <param name="valeur">La valeur de la note (entre 0 et 20)</param>
    /// <returns>La note créée ou mise à jour</returns>
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
            await repositoryFactory.NoteRepository().AddAsync(note);
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
        // Vérification des paramètres
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(etudiantId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(ueId);

        // Vérifions la connexion aux datasources
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());

        // Règle 1 : Une note est comprise entre 0 et 20
        if (valeur < 0 || valeur > 20)
        {
            throw new InvalidNoteValeurException(
                $"La note doit être comprise entre 0 et 20. Valeur reçue : {valeur}");
        }

        // Vérifier que l'étudiant existe
        var etudiants = await repositoryFactory.EtudiantRepository()
            .FindByConditionAsync(e => e.Id.Equals(etudiantId));
        
        if (etudiants == null || etudiants.Count == 0)
        {
            throw new EtudiantNotFoundException($"L'étudiant avec l'ID {etudiantId} n'existe pas");
        }

        // Vérifier que l'UE existe
        var ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.Id.Equals(ueId));
        
        if (ues == null || ues.Count == 0)
        {
            throw new UeNotFoundException(ueId.ToString());
        }

        // Règle 2 : Un étudiant ne peut avoir une note que dans une UE du parcours dans lequel il est inscrit
        // Trouver le parcours de l'étudiant
        var tousLesParcours = await repositoryFactory.ParcoursRepository()
            .FindByConditionAsync(p => p.Inscrits != null && p.Inscrits.Any(e => e.Id == etudiantId));

        if (tousLesParcours == null || tousLesParcours.Count == 0)
        {
            throw new UeNotInParcoursException(
                $"L'étudiant {etudiantId} n'est inscrit dans aucun parcours");
        }

        // Vérifier que l'UE fait partie du parcours de l'étudiant
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


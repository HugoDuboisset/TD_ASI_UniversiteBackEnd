using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.GenerateTemplate;

public class GenerateNotesTemplateUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<List<NoteCsvDto>> ExecuteAsync(long ueId)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var ue = await repositoryFactory.UeRepository().FindAsync(ueId);
        if (ue == null)
        {
            throw new UeNotFoundException(ueId.ToString());
        }

        var parcours = await repositoryFactory.ParcoursRepository()
            .GetParcoursWithStudentsByUeAsync(ueId);

        var notesExistantes = await repositoryFactory.NoteRepository()
            .GetNotesByUeAsync(ueId);

        var notesByEtudiant = notesExistantes.ToDictionary(n => n.EtudiantId, n => n.Valeur);

        var csvData = new List<NoteCsvDto>();
        foreach (var p in parcours)
        {
            if (p.Inscrits == null) continue;
            foreach (var etudiant in p.Inscrits)
            {
                    var noteValue = notesByEtudiant.ContainsKey(etudiant.Id)
                    ? notesByEtudiant[etudiant.Id].ToString(System.Globalization.CultureInfo.InvariantCulture)
                    : string.Empty;

                csvData.Add(new NoteCsvDto
                {
                    NumEtud = etudiant.NumEtud,
                    Nom = etudiant.Nom,
                    Prenom = etudiant.Prenom,
                    NumeroUe = ue.NumeroUe,
                    IntituleUe = ue.Intitule,
                    Note = noteValue
                });
            }
        }
        return csvData;
    }

    

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}

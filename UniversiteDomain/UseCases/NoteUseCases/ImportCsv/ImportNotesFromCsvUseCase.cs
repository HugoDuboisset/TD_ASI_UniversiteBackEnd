using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Create;

namespace UniversiteDomain.UseCases.NoteUseCases.ImportCsv;

public class ImportNotesFromCsvUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<ImportResult> ExecuteAsync(List<NoteCsvDto> csvData)
    {
        ArgumentNullException.ThrowIfNull(csvData);
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        var errors = new List<CsvImportError>();

        await ValidateCsvDataAsync(csvData, errors);

        if (errors.Count > 0)
        {
            throw new CsvImportException(errors);
        }

        var notesCreated = 0;
        var notesUpdated = 0;
        var createNoteUseCase = new CreateNoteUseCase(repositoryFactory);

        var premiereLigne = csvData[0];
        var ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.NumeroUe == premiereLigne.NumeroUe);
        var ue = ues.FirstOrDefault();

        if (ue == null)
        {
            throw new Exception("UE introuvable"); 
        }

        var notesExistantes = await repositoryFactory.NoteRepository()
            .GetNotesByUeAsync(ue.Id);
        var notesExistantesSet = notesExistantes
            .Select(n => (n.EtudiantId, n.UeId))
            .ToHashSet();

        for (int i = 0; i < csvData.Count; i++)
        {
            var ligne = csvData[i];
            var noteValue = ligne.GetNoteValue();
            if (noteValue == null) continue;
            var etudiants = await repositoryFactory.EtudiantRepository()
                .FindByConditionAsync(e => e.NumEtud == ligne.NumEtud);
            var etudiant = etudiants.FirstOrDefault();

            if (etudiant == null) continue;

            var noteExistait = notesExistantesSet.Contains((etudiant.Id, ue.Id));
            await createNoteUseCase.ExecuteAsync(etudiant.Id, ue.Id, noteValue.Value);

            if (noteExistait)
                notesUpdated++;
            else
                notesCreated++;
        }

        return new ImportResult
        {
            NotesCreated = notesCreated,
            NotesUpdated = notesUpdated,
            TotalImported = notesCreated + notesUpdated
        };
    }

    private async Task ValidateCsvDataAsync(List<NoteCsvDto> csvData, List<CsvImportError> errors)
    {
        if (csvData.Count == 0)
        {
            errors.Add(new CsvImportError(0, "Le fichier CSV est vide"));
            return;
        }

        var premiereLigne = csvData[0];
        var numeroUeAttendu = premiereLigne.NumeroUe;
        var intituleUeAttendu = premiereLigne.IntituleUe;
        var ues = await repositoryFactory.UeRepository()
            .FindByConditionAsync(u => u.NumeroUe == numeroUeAttendu);
        var ue = ues.FirstOrDefault();

        if (ue == null)
        {
            errors.Add(new CsvImportError(1, $"L'UE '{numeroUeAttendu}' n'existe pas"));
        }

        var touslEsEtudiants = await repositoryFactory.EtudiantRepository().FindAllAsync();
        var etudiantsDict = touslEsEtudiants.ToDictionary(e => e.NumEtud, e => e);

        List<Parcours> parcoursAvecUe = new List<Parcours>();
        if (ue != null)
        {
            parcoursAvecUe = await repositoryFactory.ParcoursRepository()
                .GetParcoursWithStudentsByUeAsync(ue.Id);
        }

        var etudiantsQuiSuiventUe = new HashSet<long>();
        foreach (var parcours in parcoursAvecUe)
        {
            if (parcours.Inscrits != null)
            {
                foreach (var etudiant in parcours.Inscrits)
                {
                    etudiantsQuiSuiventUe.Add(etudiant.Id);
                }
            }
        }

        for (int i = 0; i < csvData.Count; i++)
        {
            var ligne = csvData[i];
            var numeroLigne = i + 2; 

            if (ligne.NumeroUe != numeroUeAttendu)
            {
                errors.Add(new CsvImportError(numeroLigne,
                    $"Incohérence : NumeroUe '{ligne.NumeroUe}' différent de '{numeroUeAttendu}'"));
            }

            if (ligne.IntituleUe != intituleUeAttendu)
            {
                errors.Add(new CsvImportError(numeroLigne,
                    $"Incohérence : IntituleUe différent de la première ligne"));
            }

            var noteValue = ligne.GetNoteValue();
            if (noteValue == null && !string.IsNullOrWhiteSpace(ligne.Note))
            {
                errors.Add(new CsvImportError(numeroLigne,
                    $"Format de note invalide : '{ligne.Note}'"));
                continue;
            }

            if (noteValue == null) continue;

            if (noteValue.Value < 0 || noteValue.Value > 20)
            {
                errors.Add(new CsvImportError(numeroLigne,
                    $"La note doit être comprise entre 0 et 20. Valeur reçue : {noteValue.Value}"));
            }

            if (!etudiantsDict.ContainsKey(ligne.NumEtud))
            {
                errors.Add(new CsvImportError(numeroLigne,
                    $"L'étudiant '{ligne.NumEtud}' n'existe pas"));
                continue;
            }

            var etudiant = etudiantsDict[ligne.NumEtud];

            if (etudiant.Nom != ligne.Nom || etudiant.Prenom != ligne.Prenom)
            {
                errors.Add(new CsvImportError(numeroLigne,
                    $"Les informations de l'étudiant '{ligne.NumEtud}' ne correspondent pas : " +
                    $"attendu '{etudiant.Nom} {etudiant.Prenom}', trouvé '{ligne.Nom} {ligne.Prenom}'"));
            }
            if (ue != null && !etudiantsQuiSuiventUe.Contains(etudiant.Id))
            {
                errors.Add(new CsvImportError(numeroLigne,
                    $"L'étudiant '{ligne.NumEtud}' ne suit pas l'UE '{numeroUeAttendu}'"));
            }
        }
    }

    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite);
    }
}

public class ImportResult
{
    public int NotesCreated { get; set; }
    public int NotesUpdated { get; set; }
    public int TotalImported { get; set; }
}

using System.Globalization;
using System.Security.Claims;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.UseCases.NoteUseCases.GenerateTemplate;
using UniversiteDomain.UseCases.NoteUseCases.ImportCsv;
using UniversiteDomain.UseCases.SecurityUseCases.Create;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    // GET: api/note/template/5
    [HttpGet("template/{ueId}")]
    public async Task<IActionResult> GetNotesTemplate(long ueId)
    {
        // Authentification et autorisation
        string role = "";
        string email = "";
        IUniversiteUser user = null;

        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception)
        {
            return Unauthorized();
        }

        var generateUc = new GenerateNotesTemplateUseCase(repositoryFactory);

        // Vérifier que l'utilisateur a le rôle Scolarite
        if (!generateUc.IsAuthorized(role))
        {
            return Unauthorized();
        }

        // Générer les données CSV
        List<NoteCsvDto> csvData;
        try
        {
            csvData = await generateUc.ExecuteAsync(ueId);
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }

        // Créer le fichier CSV
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
        using var csvWriter = new CsvWriter(streamWriter, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ","
        });

        csvWriter.WriteRecords(csvData);
        streamWriter.Flush();

        var csvBytes = memoryStream.ToArray();
        var fileName = $"notes_ue_{ueId}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

        return File(csvBytes, "text/csv", fileName);
    }

    // POST: api/note/import
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportNotes(IFormFile file)
    {
        // Authentification et autorisation
        string role = "";
        string email = "";
        IUniversiteUser user = null;

        try
        {
            CheckSecu(out role, out email, out user);
        }
        catch (Exception)
        {
            return Unauthorized();
        }

        var importUc = new ImportNotesFromCsvUseCase(repositoryFactory);

        // Vérifier que l'utilisateur a le rôle Scolarite
        if (!importUc.IsAuthorized(role))
        {
            return Unauthorized();
        }

        // Vérifier qu'un fichier a été uploadé
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "Aucun fichier n'a été uploadé" });
        }

        // Vérifier que c'est bien un fichier CSV
        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { error = "Le fichier doit être au format CSV" });
        }

        // Lire le fichier CSV
        List<NoteCsvDto> csvData;
        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            });

            csvData = csv.GetRecords<NoteCsvDto>().ToList();
        }
        catch (Exception e)
        {
            return BadRequest(new { error = $"Erreur lors de la lecture du fichier CSV : {e.Message}" });
        }

        // Importer les notes
        ImportResult result;
        try
        {
            result = await importUc.ExecuteAsync(csvData);
        }
        catch (CsvImportException e)
        {
            // Retourner les erreurs de validation
            return BadRequest(new { errors = e.Errors });
        }
        catch (Exception e)
        {
            ModelState.AddModelError(nameof(e), e.Message);
            return ValidationProblem();
        }

        return Ok(new
        {
            message = $"{result.TotalImported} note(s) importée(s) avec succès",
            notesCreated = result.NotesCreated,
            notesUpdated = result.NotesUpdated,
            totalImported = result.TotalImported
        });
    }

    private void CheckSecu(out string role, out string email, out IUniversiteUser user)
    {
        role = "";
        // Récupération des informations de connexion dans la requête http entrante
        ClaimsPrincipal claims = HttpContext.User;
        // Faisons nos tests pour savoir si la personne est bien connectée
        if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
        // Récupérons le email de la personne connectée
        if (claims.FindFirst(ClaimTypes.Email) == null) throw new UnauthorizedAccessException();
        email = claims.FindFirst(ClaimTypes.Email).Value;
        if (email == null) throw new UnauthorizedAccessException();
        // Vérifions qu'il est bien associé à un utilisateur référencé
        user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
        if (user == null) throw new UnauthorizedAccessException();
        // Vérifions qu'un rôle a bien été défini
        if (claims.FindFirst(ClaimTypes.Role) == null) throw new UnauthorizedAccessException();
        // Récupérons le rôle de l'utilisateur
        var ident = claims.Identities.FirstOrDefault();
        if (ident == null) throw new UnauthorizedAccessException();
        role = ident.FindFirst(ClaimTypes.Role).Value;
        if (role == null) throw new UnauthorizedAccessException();
        // Vérifions que le user a bien le role envoyé via http
        bool isInRole = new IsInRoleUseCase(repositoryFactory).ExecuteAsync(email, role).Result;
        if (!isInRole) throw new UnauthorizedAccessException();
        // Si tout est passé sans renvoyer d'exception, le user est authentifié et connecté
    }
}

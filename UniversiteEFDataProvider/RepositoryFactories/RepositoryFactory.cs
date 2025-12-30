using Microsoft.AspNetCore.Identity;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;

namespace UniversiteEFDataProvider.RepositoryFactories;

public class RepositoryFactory(UniversiteDbContext context, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager) : IRepositoryFactory
{
    private IParcoursRepository? _parcours;
    private IEtudiantRepository? _etudiants;
    private IUeRepository? _ues;
    private INoteRepository? _notes;
    private IUniversiteUserRepository? _universiteUserRepository;
    private IUniversiteRoleRepository? _universiteRoleRepository;

    public IParcoursRepository ParcoursRepository()
    {
        if (_parcours == null)
        {
            _parcours = new ParcoursRepository(context ?? throw new InvalidOperationException());
        }
        return _parcours;
    }

    public IEtudiantRepository EtudiantRepository()
    {
        if (_etudiants == null)
        {
            _etudiants = new EtudiantRepository(context ?? throw new InvalidOperationException());
        }
        return _etudiants;
    }

    public IUeRepository UeRepository()
    {
        if (_ues == null)
        {
            _ues = new UeRepository(context ?? throw new InvalidOperationException());
        }
        return _ues;
    }

    public INoteRepository NoteRepository()
    {
        if (_notes == null)
        {
            _notes = new NoteRepository(context ?? throw new InvalidOperationException());
        }
        return _notes;

    }

    public IUniversiteUserRepository UniversiteUserRepository()
    {
        if (_universiteUserRepository == null)
        {
            _universiteUserRepository = new UniversiteUserRepository(context ?? throw new InvalidOperationException(), userManager, roleManager);
        }
        return _universiteUserRepository;
    }

    public IUniversiteRoleRepository UniversiteRoleRepository()
    {
        if (_universiteRoleRepository == null)
        {
            _universiteRoleRepository = new UniversiteRoleRepository(context ?? throw new InvalidOperationException(), roleManager);
        }
        return _universiteRoleRepository;
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task EnsureCreatedAsync()
    {
        await context.Database.EnsureCreatedAsync();
    }

    public async Task EnsureDeletedAsync()
    {
        await context.Database.EnsureDeletedAsync();
    }
}
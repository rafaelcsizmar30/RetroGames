using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> hasher)
    {
        _repositorio = repositorio;
        _hasher = hasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await _repositorio.Usuario.BuscarPorEmailAsync(email.Trim());
        if (usuario == null || !usuario.Ativo)
        {
            return null;
        }

        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        return resultado == PasswordVerificationResult.Failed ? null : usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string email, string senha)
    {
        if (await _repositorio.Usuario.ExisteAdministradorAsync(PerfilIds.Administrador))
        {
            return;
        }

        var admin = new Usuario
        {
            Nome = PerfilNomes.Administrador,
            Email = email.Trim(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow,
            IdPerfil = PerfilIds.Administrador,
        };
        admin.SenhaHash = _hasher.HashPassword(admin, senha);

        await _repositorio.Usuario.InserirAsync(admin);
    }
}
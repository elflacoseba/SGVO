namespace SGVO.Domain.Entities;

/// <summary>
/// Representa un postulante a una vacante.
/// </summary>
public class Postulante
{
    public ulong Id { get; private set; }
    public ulong? PersonaId { get; private set; }
    public string Nombre { get; private set; } = null!;
    public string Apellido { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Telefono { get; private set; }
    public string Origen { get; private set; } = null!;
    public string? CurriculumUrl { get; private set; }
    public bool Activo { get; private set; }
    public DateTime CreadoEn { get; private set; }
    public DateTime? EliminadoEn { get; private set; }
    public ulong? EliminadoPor { get; private set; }

    // Constructor privado para EF Core
    private Postulante() { }

    /// <summary>
    /// Constructor para crear un nuevo postulante.
    /// </summary>
    public Postulante(string nombre, string apellido, string email, string origen, ulong? personaId = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido es obligatorio.", nameof(apellido));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.", nameof(email));

        if (string.IsNullOrWhiteSpace(origen))
            throw new ArgumentException("El origen es obligatorio.", nameof(origen));

        if (nombre.Length > 150)
            throw new ArgumentException("El nombre no puede exceder 150 caracteres.", nameof(nombre));

        if (apellido.Length > 150)
            throw new ArgumentException("El apellido no puede exceder 150 caracteres.", nameof(apellido));

        if (email.Length > 200)
            throw new ArgumentException("El email no puede exceder 200 caracteres.", nameof(email));

        if (origen.Length > 50)
            throw new ArgumentException("El origen no puede exceder 50 caracteres.", nameof(origen));

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        Email = email.Trim().ToLowerInvariant();
        Origen = origen.Trim();
        PersonaId = personaId;
        Activo = true;
        CreadoEn = DateTime.UtcNow;
    }

    /// <summary>
    /// Actualiza los datos del postulante.
    /// </summary>
    public void Actualizar(string nombre, string apellido, string email, string? telefono, string? curriculumUrl)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido es obligatorio.", nameof(apellido));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.", nameof(email));

        if (nombre.Length > 150)
            throw new ArgumentException("El nombre no puede exceder 150 caracteres.", nameof(nombre));

        if (apellido.Length > 150)
            throw new ArgumentException("El apellido no puede exceder 150 caracteres.", nameof(apellido));

        if (email.Length > 200)
            throw new ArgumentException("El email no puede exceder 200 caracteres.", nameof(email));

        if (telefono?.Length > 50)
            throw new ArgumentException("El teléfono no puede exceder 50 caracteres.", nameof(telefono));

        if (curriculumUrl?.Length > 500)
            throw new ArgumentException("La URL del currículum no puede exceder 500 caracteres.", nameof(curriculumUrl));

        Nombre = nombre.Trim();
        Apellido = apellido.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefono = telefono?.Trim();
        CurriculumUrl = curriculumUrl?.Trim();
    }

    /// <summary>
    /// Marca el postulante como eliminado (soft delete).
    /// </summary>
    public void Eliminar(ulong eliminadoPor)
    {
        EliminadoEn = DateTime.UtcNow;
        EliminadoPor = eliminadoPor;
        Activo = false;
    }

    /// <summary>
    /// Reactiva el postulante.
    /// </summary>
    public void Reactivar()
    {
        EliminadoEn = null;
        EliminadoPor = null;
        Activo = true;
    }
}

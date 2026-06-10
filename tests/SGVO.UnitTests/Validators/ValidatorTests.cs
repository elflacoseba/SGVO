using FluentValidation;
using FluentValidation.TestHelper;
using SGVO.Application.Validators;

namespace SGVO.UnitTests.Validators;

/// <summary>
/// Modelo de prueba para validar el BaseValidator.
/// </summary>
public class TestRequest
{
    public string Nombre { get; set; } = string.Empty;
    public int Edad { get; set; }
}

/// <summary>
/// Validador de prueba para TestRequest.
/// </summary>
public class TestRequestValidator : BaseValidator<TestRequest>
{
    public TestRequestValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Edad).GreaterThan(0);
    }
}

/// <summary>
/// Tests unitarios para validadores.
/// </summary>
public class ValidatorTests
{
    private readonly TestRequestValidator _validator = new();

    [Fact]
    public void Validar_NombreVacio_Debe_Fallar()
    {
        var request = new TestRequest { Nombre = "", Edad = 25 };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Validar_EdadCero_Debe_Fallar()
    {
        var request = new TestRequest { Nombre = "Juan", Edad = 0 };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Edad);
    }

    [Fact]
    public void Validar_RequestCorrecto_Debe_Pasar()
    {
        var request = new TestRequest { Nombre = "Juan", Edad = 25 };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}

using FluentValidation;

namespace microUsuarios.API.Model.Request.Validators
{
    public class IniciarSesionValidator : AbstractValidator<IniciarSesionRequest>
    {
        public IniciarSesionValidator()
        {

            RuleFor(x => x.Correo)
               .Must(m => m != String.Empty)
               .WithMessage("Debe ingresar su Correo electronico.");

            RuleFor(x => x.Contrasenia)
               .Must(m => m != String.Empty)
               .WithMessage("Debe ingresar su Contraseña.");
        }
    }
}

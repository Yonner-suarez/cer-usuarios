using FluentValidation;

namespace microUsuarios.API.Model.Request.Validators
{
    public class AgregarUsuarioValidator : AbstractValidator<AgregarUsuarioRequest>
    {
        public AgregarUsuarioValidator()
        {
            RuleFor(x => x.NroDocumento)
               .Must(m => m != 0)
               .WithMessage("Debe ingresar una número de documento válido.");

            RuleFor(x => x.Nombre)
               .Must(m => m != String.Empty)
               .WithMessage("Debe ingresar su Nombre.");


            RuleFor(x => x.Correo)
            .NotEmpty()
            .WithMessage("Debe ingresar su Correo electrónico.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .WithMessage("Debe ingresar un Correo electrónico válido.");



            RuleFor(x => x.Contrasenia)
               .Must(m => m != String.Empty)
               .WithMessage("Debe ingresar su Contraseña.");
        }
    }
}

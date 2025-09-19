using FluentValidation;

namespace microUsuarios.API.Model.Request.Validators
{
    public class AgregarUsuarioValidator : AbstractValidator<AgregarUsuarioRequest>
    {
        public AgregarUsuarioValidator()
        {
            RuleFor(x => x.dni)
               .Must(m => m != 0)
               .WithMessage("Debe ingresar una marca válida.");
        }
    }
}

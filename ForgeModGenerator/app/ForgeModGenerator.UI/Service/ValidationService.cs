using ForgeModGenerator.Model;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ForgeModGenerator.Service
{
    public interface IValidationService<T>
    {
        bool IsValid(T obj);
    }

    public class NameValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            return new ValidationResult(Regex.IsMatch(value as string, "^[a-z]{3,21}$"), "It's not valid");
            //return ValidationResult.ValidResult;
        }
    }

    public class ModValidationService : IValidationService<Mod>
    {
        protected readonly string lowerMatch = "^[a-z]{3,21}$"; // full lowercase letters, length limit 3-21
        protected readonly string nameMatch = "^[A-Z]+[A-z]{2,20}$"; // first upper letter, next letters case dont matter, length limit 3-21

        public bool IsValid(Mod mod)
        {
            return IsValidName(mod.ModInfo.Name) && IsValidOrganization(mod.Organization) && IsValidModid(mod.ModInfo.Modid);
        }

        public bool IsValidName(string name)
        {
            return Regex.IsMatch(name, nameMatch);
        }

        public bool IsValidOrganization(string organization)
        {
            return Regex.IsMatch(organization, lowerMatch);
        }

        public bool IsValidModid(string modid)
        {
            return Regex.IsMatch(modid, lowerMatch);
        }

        public Match ValidateName(string name)
        {
            return Regex.Match(name, nameMatch);
        }

        public Match ValidateOrganization(string organization)
        {
            return Regex.Match(organization, lowerMatch);
        }

        public Match ValidateModid(string modid)
        {
            return Regex.Match(modid, lowerMatch);
        }
    }
}

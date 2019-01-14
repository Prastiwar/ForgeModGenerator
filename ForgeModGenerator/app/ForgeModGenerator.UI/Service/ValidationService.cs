using ForgeModGenerator.Model;
using System.Text.RegularExpressions;

namespace ForgeModGenerator.Service
{
    public interface IValidationService<T>
    {
        bool Validate(T obj);
    }

    public class ModValidationService : IValidationService<Mod>
    {
        protected readonly string lowerMatch = "^[a-z]{3,21}$"; // full lowercase letters, length limit 3-21
        protected readonly string nameMatch = "^[A-Z]+[A-z]{2,20}$"; // first upper letter, next letters case dont matter, length limit 3-21

        public bool Validate(Mod mod)
        {
            return ValidateName(mod) && ValidateOrganization(mod) && ValidateModid(mod);
        }

        public bool ValidateName(Mod mod)
        {
            return Regex.IsMatch(mod.ModInfo.Name, nameMatch);
        }

        public bool ValidateOrganization(Mod mod)
        {
            return Regex.IsMatch(mod.Organization, lowerMatch);
        }

        public bool ValidateModid(Mod mod)
        {
            return Regex.IsMatch(mod.ModInfo.Modid, lowerMatch);
        }
    }
}

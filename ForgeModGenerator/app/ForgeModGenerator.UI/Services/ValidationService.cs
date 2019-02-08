using ForgeModGenerator.ModGenerator.Models;
using System.Text.RegularExpressions;

namespace ForgeModGenerator.Services
{
    public interface IValidationService<T>
    {
        bool IsValid(T obj);
    }

    public class ModValidationService : IValidationService<Mod>
    {
        protected readonly string lowerMatch = "^[a-z]{3,21}$"; // full lowercase letters, length limit 3-21
        protected readonly string nameMatch = "^[A-Z]+[A-z]{2,20}$"; // first upper letter, next letters case dont matter, length limit 3-21

        public bool IsValid(Mod mod)
        {
            if (mod == null || mod.ForgeVersion == null)
            {
                return false;
            }
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

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
        public bool Validate(Mod obj)
        {
            string lowerMatch = "^[a-z]{3,21}$"; // full lowercase, length limit 3-21
            string nameMatch = "^[A-Z]+[A-z]{2,20}$"; // first upper, next doesnt matter, length limit 3-21
            return Regex.IsMatch(obj.Organization, lowerMatch)
                && Regex.IsMatch(obj.ModInfo.Modid, lowerMatch)
                && Regex.IsMatch(obj.ModInfo.Name, nameMatch);
        }
    }
}

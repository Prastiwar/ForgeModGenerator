using System.Threading.Tasks;

namespace ForgeModGenerator
{
    public interface IChoiceForm<TItem>
    {
        object OpenChoices();
        Task<object> OpenChoicesAsync();
    }
}

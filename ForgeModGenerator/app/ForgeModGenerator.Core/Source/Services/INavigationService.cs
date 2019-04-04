using System;

namespace ForgeModGenerator.Services
{
    public interface INavigationService
    {
        bool NavigateTo(string name);
        bool NavigateTo(Uri uri);
    }
}

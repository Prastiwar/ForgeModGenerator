using ForgeModGenerator.Models;

namespace ForgeModGenerator.MaterialGenerator.Models
{
    public class Material : ObservableModel
    {
        public override bool CopyValues(object fromCopy)
        {
            if (fromCopy is Material fromModel)
            {
                Name = fromModel.Name;
            }
            return false;
        }

        public override object DeepClone()
        {
            Material item = new Material();
            item.CopyValues(this);
            return item;
        }
    }
}

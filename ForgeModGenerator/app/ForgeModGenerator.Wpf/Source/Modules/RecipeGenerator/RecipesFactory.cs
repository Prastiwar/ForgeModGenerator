//using ForgeModGenerator.RecipeGenerator.Models;
//using ForgeModGenerator.Serialization;
//using ForgeModGenerator.Utility;
//using System.Collections.Generic;
//using System.IO;

//namespace ForgeModGenerator.RecipeGenerator
//{
//    public class RecipesFactory : WpfFoldersFactory<ObservableFolder<Recipe>, Recipe>
//    {
//        public RecipesFactory(ISerializer<Recipe> serializer) => Serializer = serializer;

//        protected ISerializer<Recipe> Serializer { get; }

//        /// <inheritdoc/>
//        public override ObservableFolder<Recipe> Create(string path, IEnumerable<string> filePaths)
//        {
//            ObservableFolder<Recipe> folder = null;
//            if (path != null)
//            {
//                folder = ReflectionHelper.CreateInstance<ObservableFolder<Recipe>>(path);
//                if (filePaths != null)
//                {
//                    foreach (string filePath in filePaths)
//                    {
//                        string content = File.ReadAllText(filePath);
//                        Recipe recipe = Serializer.Deserialize(content);
//                        recipe.SetInfo(filePath);
//                        folder.Add(recipe);
//                    }
//                }
//            }
//            else
//            {
//                folder = ReflectionHelper.CreateInstance<ObservableFolder<Recipe>>(true);
//                if (filePaths != null)
//                {
//                    folder.AddRange(filePaths);
//                }
//            }
//            return folder;
//        }
//    }
//}

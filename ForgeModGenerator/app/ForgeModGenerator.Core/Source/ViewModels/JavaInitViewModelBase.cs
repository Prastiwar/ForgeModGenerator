using ForgeModGenerator.Models;
using ForgeModGenerator.Utility;
using ForgeModGenerator.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ForgeModGenerator.ViewModels
{
    /// <summary> Inits TModel from java field from init class </summary>
    public abstract class JavaInitViewModelBase<TModel> : SimpleInitViewModelBase<TModel>
        where TModel : ObservableDirtyObject, IValidable
    {
        public JavaInitViewModelBase(GeneratorContext<TModel> context) : base(context) { }

        /// <summary> Returns content from parantheses block => forString(content) </summary>
        protected string GetParenthesesContentFor(string fromString, string forString, int startLookingForStringIndex = 0)
        {
            int startIndex = fromString.IndexOf(forString, startLookingForStringIndex);
            if (startIndex < 0)
            {
                return "";
            }
            fromString = fromString.Substring(startIndex);
            string content = fromString.GetParenthesesContent();
            return content;
        }

        /// <summary> Returns content from parantheses block => forString(content) </summary>
        protected string[] SplitParenthesesContentFor(string fromString, string forString, int startLookingForStringIndex = 0)
        {
            string content = GetParenthesesContentFor(fromString, forString, startLookingForStringIndex);
            return content == null ? Array.Empty<string>() : content.SplitTrim(',');
        }

        protected abstract TModel ParseModelFromJavaField(string line);

        protected override IEnumerable<TModel> FindModelsFromPath(string path)
        {
            if (!File.Exists(path))
            {
                return Enumerable.Empty<TModel>();
            }
            List<TModel> models = new List<TModel>(8);
            IEnumerable<string> items = File.ReadLines(path).Where(x => x.Trim().StartsWith("public static final"));
            bool firstIsArray = true;
            foreach (string item in items)
            {
                if (!firstIsArray)
                {
                    string line = item.Trim();
                    if (TryParseModelFromJavaField(line, out TModel model))
                    {
                        models.Add(model);
                    }
                    else
                    {
                        Log.Warning($"Couldn't parse model of type {typeof(TModel)} from java field line {line}");
                    }
                }
                else
                {
                    firstIsArray = false;
                    continue;
                }
            }
            return models;
        }

        protected bool TryParseModelFromJavaField(string line, out TModel model)
        {
            try
            {
                model = ParseModelFromJavaField(line);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                model = null;
                return false;
            }
            return model != null;
        }
    }
}

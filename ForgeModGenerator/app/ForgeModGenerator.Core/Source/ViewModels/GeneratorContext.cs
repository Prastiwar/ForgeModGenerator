using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;

namespace ForgeModGenerator.ViewModels
{
    public class GeneratorContext<TItem>
    {
        public GeneratorContext(ISessionContextService sessionContext,
                                IUniqueValidator<TItem> validator,
                                ICodeGenerationService codeGenerationService,
                                IEditorFormFactory<TItem> editorFormFactory)
        {
            SessionContext = sessionContext;
            Validator = validator;
            CodeGenerationService = codeGenerationService;
            EditorFormFactory = editorFormFactory;
        }

        public ISessionContextService SessionContext { get; }
        public IUniqueValidator<TItem> Validator { get; }
        public ICodeGenerationService CodeGenerationService { get; }
        public IEditorFormFactory<TItem> EditorFormFactory { get; }
    }
}

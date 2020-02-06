using ForgeModGenerator.Services;
using ForgeModGenerator.Validation;

namespace ForgeModGenerator.ViewModels
{
    public class GeneratorContext<TItem>
    {
        public GeneratorContext(ISessionContextService sessionContext,
                                IUniqueValidator<TItem> validator,
                                ICodeGenerationService codeGenerationService,
                                IEditorFormFactory<TItem> editorFormFactory,
                                IFileSystem fileSystem,
                                IDialogService dialogService)
        {
            SessionContext = sessionContext;
            Validator = validator;
            CodeGenerationService = codeGenerationService;
            EditorFormFactory = editorFormFactory;
            FileSystem = fileSystem;
            DialogService = dialogService;
        }

        public ISessionContextService SessionContext { get; }
        public IUniqueValidator<TItem> Validator { get; }
        public ICodeGenerationService CodeGenerationService { get; }
        public IEditorFormFactory<TItem> EditorFormFactory { get; }
        public IFileSystem FileSystem { get; }
        public IDialogService DialogService { get; }
    }
}

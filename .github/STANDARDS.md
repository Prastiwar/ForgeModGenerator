Official C# standards (convention)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) mostly covers convention used in this project.

If non-auto property are used, private fields should be just before property
```cs
private bool isFileUpdateAvailable;
public bool IsFileUpdateAvailable {
    get => isFileUpdateAvailable;
    set => Set(ref isFileUpdateAvailable, value);
}

private ICommand editFileCommand;
public ICommand EditFileCommand => editFileCommand ?? (editFileCommand = new RelayCommand<TFile>(FileEditor.OpenItemEditor));
```

```cs
private bool shouldDoIt;
public bool ShouldDoIt {
    get {
        if (CheckSomething())
        {
            shouldDoIt = false;
        }
        return shouldDoIt;
    }
    set {
        UpdateSomething(shouldDoIt);
        shouldDoIt = value;
    }
}
```

ALWAYS include brackets
```cs
public void SomeFunction()
{
    bool shouldExecute = false;
    if (shouldExecute)
    {
        return;
    }
    if (true)
    {
        DoOnIf();
    }
    else
    {
        DoOnElse();
    }
}
```
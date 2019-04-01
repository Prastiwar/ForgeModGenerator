namespace ForgeModGenerator
{
    public interface IFileSystem
    {
        bool MoveDirectoriesAndFiles(string from, string destination);
        bool MoveDirectoriesAndFiles(string from, string destination, bool overwrite);

        bool MoveDirectory(string from, string destination);
        bool MoveDirectory(string from, string destination, bool overwrite);

        bool CopyDirectory(string from, string destination);
        bool CopyDirectory(string from, string destination, bool overwrite);

        bool DeleteDirectory(string path, bool recycle);

        bool RenameDirectory(string name, string newName);

        bool CreateDirectory(string path);

        bool MoveFile(string from, string destination);
        bool MoveFile(string from, string destination, bool overwrite);

        bool CopyFile(string from, string destination);
        bool CopyFile(string from, string destination, bool overwrite);

        bool DeleteFile(string path, bool recycle);

        bool RenameFile(string name, string newName);
    }
}

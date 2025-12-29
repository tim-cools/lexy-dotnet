namespace Lexy.Compiler.Infrastructure;

public interface IFileSystem {

    string[] ReadAllLines(string fileName);

    string GetFileName(string fullFileName);
    string GetDirectoryName(string fileName);
    string GetFullPath(string directoryName);

    string Combine(string fullPath, string fileName);

    bool FileExists(string fileName);
    bool DirectoryExists(string absoluteFolder);

    bool IsPathRooted(string folder);

    string[] GetDirectoryFiles(string folder, string[] extensions);
    string[] GetDirectories(string folder);

    string LogFolders();
}
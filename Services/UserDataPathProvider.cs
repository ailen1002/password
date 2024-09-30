using System;
using System.IO;

namespace password.Services;

public class UserDataPathProvider
{
    public string UserFilePath { get; private set; }

    public UserDataPathProvider(bool useUserDirectory)
    {
        if (useUserDirectory)
        {
            var userFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "password", "UserData");
            Directory.CreateDirectory(userFolder);
            UserFilePath = Path.Combine(userFolder, "user.json");
        }
        else
        {
            var programFolder = Path.Combine(AppContext.BaseDirectory, "UserData");
            Directory.CreateDirectory(programFolder);
            UserFilePath = Path.Combine(programFolder, "user.json");

        }
    }
}
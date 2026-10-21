using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AnatomyReleaseBuilder
{
    const string OutputPath = "Builds/GuateGeeksAnatomiaVR-release-0.1.7.apk";
    const string ReleaseVersion = "0.1.7";
    const int ReleaseCode = 8;

    [MenuItem("GuateGeeks/Anatomy Lab/Build Quest Release")]
    public static void BuildQuestRelease()
    {
        var keystorePath = Environment.GetEnvironmentVariable("BODYVR_KEYSTORE_PATH");
        if (string.IsNullOrWhiteSpace(keystorePath))
            keystorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Documents/GuateGeeks/BodyVR-signing/bodyvr-release.keystore");
        if (!File.Exists(keystorePath))
            throw new FileNotFoundException("No se encontró la clave de firma de BodyVR. Define BODYVR_KEYSTORE_PATH.", keystorePath);

        var password = ReadSigningPassword();
        var previousKeystore = PlayerSettings.Android.keystoreName;
        var previousAlias = PlayerSettings.Android.keyaliasName;
        var previousCustomSigning = PlayerSettings.Android.useCustomKeystore;
        try
        {
            AnatomyProjectBuilder.ConfigureQuestIcon();
            PlayerSettings.bundleVersion = ReleaseVersion;
            PlayerSettings.Android.bundleVersionCode = ReleaseCode;
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = keystorePath;
            PlayerSettings.Android.keystorePass = password;
            PlayerSettings.Android.keyaliasName = "bodyvr";
            PlayerSettings.Android.keyaliasPass = password;
            // Unity puede recompilar durante BuildPlayer; persistir la versión antes evita
            // que el manifiesto Android vuelva a tomar el código anterior.
            AssetDatabase.SaveAssets();

            Directory.CreateDirectory("Builds");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[]
                {
                    "Assets/AnatomyLab/Scenes/01_Bootstrap.unity",
                    "Assets/AnatomyLab/Scenes/02_AnatomyLab.unity"
                },
                locationPathName = OutputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            });
            File.WriteAllText("Builds/release-result.txt",
                report.summary.result + "\nBytes: " + report.summary.totalSize +
                "\nTime: " + report.summary.totalTime);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("No se pudo generar la versión de publicación de Quest");
            UnityEngine.Debug.Log("[AnatomyLab] Versión de publicación creada: " + OutputPath);
        }
        finally
        {
            PlayerSettings.Android.keystorePass = string.Empty;
            PlayerSettings.Android.keyaliasPass = string.Empty;
            PlayerSettings.Android.keystoreName = previousKeystore;
            PlayerSettings.Android.keyaliasName = previousAlias;
            PlayerSettings.Android.useCustomKeystore = previousCustomSigning;
        }
    }

    static string ReadSigningPassword()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("BODYVR_RELEASE_PASSWORD");
        if (!string.IsNullOrWhiteSpace(fromEnvironment)) return fromEnvironment;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/usr/bin/security",
                Arguments = "find-generic-password -a bodyvr-release -s com.guategeeks.anatomylabvr.signing -w",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };
        process.Start();
        var password = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();
        if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(password))
            throw new Exception("No se pudo leer la contraseña de firma desde el llavero de macOS");
        return password;
    }
}

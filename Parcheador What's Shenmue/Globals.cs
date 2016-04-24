using System.IO;
using System.Windows.Forms;

namespace Parcheador_Whats_Shenmue
{
    static class Globals
    {
        //public const bool DebugMode = true;

        /// <summary>This patcher's path.</summary>
        public static string AppPath;

        /// <summary>Temporary path.</summary>
        public static string TempPath;

        /// <summary>Utils path inside temporary path.</summary>
        public static string UtilsPath;

        /// <summary>Patch path inside temporary path.</summary>
        public static string PatchPath;

        /// <summary>Common path path inside temporary path.</summary>
        //static string PatchPathCommon;

        /// <summary>Each folder from the patch corresponding to each GDI, extracted to temporary files.</summary>
        public static string[] PatchPaths;

#if !ExternalPatch
        public static string TempZipName;
#endif

        /// <summary>Temporary folder where data files for each GDI are located.</summary>
        public static string[] TempGDIPaths;

        /// <summary>Temporary folder where IP.BIN files for each GDI are stored.</summary>
        public static string TempIPPath;

        /// <summary>Temporary folder where dummy and processed tracks are located.</summary>
        public static string TempTracksPath;

        /// <summary>Path where the translated ISO will be saved.</summary>
        public static string FinalISOPath;

        /// <summary>Translated ISO names.</summary>
        public static string[] FinalISO;

        public static void InitGlobals()
        {
            AppPath = Path.GetDirectoryName(Application.ExecutablePath);

#if ExternalPatch
            TempPath = Path.Combine(AppPath, PatchInfo.InternalPatchName);

            UtilsPath = Path.Combine(AppPath, PatchInfo.UtilsFolderName);
            PatchPath = Path.Combine(AppPath, PatchInfo.PatchFolderName + Localization.VarPatchFolderSuffix);
#else
            TempPath = Path.Combine(Path.GetTempPath(), PatchInfo.InternalPatchName);
            TempZipName = Path.Combine(Path.GetTempPath(), PatchInfo.InternalPatchName + ".zip");

            UtilsPath = Path.Combine(TempPath, PatchInfo.UtilsFolderName);
            PatchPath = Path.Combine(TempPath, PatchInfo.PatchFolderName + Localization.VarPatchFolderSuffix);
#endif

            FinalISOPath = Path.Combine(AppPath, PatchInfo.TranslatedISOPathName + Localization.VarPatchISOPathNameSuffix);

            TempGDIPaths = new string[GameInfo.NumberOfDiscs];
            PatchPaths = new string[GameInfo.NumberOfDiscs];
            FinalISO = new string[GameInfo.NumberOfDiscs];

            //PatchPathCommon = Path.Combine(PatchPath, "common");

            TempIPPath = Path.Combine(TempPath, "IPs");
            TempTracksPath = Path.Combine(TempPath, "Tracks");

            for (int n = 0; n < GameInfo.NumberOfDiscs; n++)
            {
                TempGDIPaths[n] = Path.Combine(TempPath, "GDI" + (n + 1));
                PatchPaths[n] = Path.Combine(PatchPath, (n + 1).ToString());
                FinalISO[n] = Path.Combine(FinalISOPath, PatchInfo.TranslatedISOName + Localization.VarPatchISONameSuffix);
            }
        }
    }
}

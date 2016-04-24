using System.IO;
using System.Threading;

namespace WSPatcher
{
	partial class Patcher
	{
		//string[] _GDIFiles;
		//string[] _GDIFolders;
		int _TotalNumberOfDiscs;
		int CurrentProgress;

		StreamWriter LogStream;

		struct GDI
		{
			public string FileName;
			public string Path;
			public string[] Tracks;
			public TrackFormat TrackFormat;
		}
		GDI[] _GDI;
		enum TrackFormat
		{
			BIN,
			ISO
		};

		#region Events
		public delegate void StatusChangedHandler(string Status, int Percentage);
		public event StatusChangedHandler StatusChanged;
		public delegate void ProcessFinishedHandler(bool ProcessFinishedSuccessfully);
		public event ProcessFinishedHandler ProcessFinished;
		public delegate void MessageHandler(string Message, MessageType MessageType);
		public event MessageHandler OutputMessage;

		public enum MessageType
		{
			Information,
			Warning,
			Error
		};
		#endregion

		public enum ImageFormat { GDI_Emulators, MDF_MDS_Consoles };
		ImageFormat _SelectedImageFormat;

		public Patcher(string[] GDIFiles, int TotalNumberOfDiscs, ImageFormat SelectedImageFormat)
		{
			_GDI = new GDI[TotalNumberOfDiscs];
			_TotalNumberOfDiscs = TotalNumberOfDiscs;
			_SelectedImageFormat = SelectedImageFormat;

			for (int n = 0; n < _TotalNumberOfDiscs; n++)
			{
				_GDI[n].FileName = GDIFiles[n];
			}
		}

		public void Start()
		{
			Thread t = new Thread(_Start);
			t.Name = "Patching Process";
			t.Start();
		}

		void _Start()
		{
			CurrentProgress = 1;

			using (LogStream = new StreamWriter(Path.Combine(Globals.AppPath, "log.txt")))
			{
				StatusChanged(Localization.StatusCheckingFiles, CurrentProgress++);
				if (!CheckFiles())
				{
					ProcessFinished(false);
					return;
				}

				if (!DeleteTempFiles())
				{
					ProcessFinished(false);
					return;
				}

#if !ExternalPatch
				StatusChanged(Localization.StatusExtractingFiles, CurrentProgress++);
				UncompressRequiredFiles();
#endif

				if (!ExtractGDIs())
				{
					ProcessFinished(false);
					return;
				}

				StatusChanged(Localization.StatusPatchingXDelta, CurrentProgress++);
				if (!PatchXDeltas())
				{
					ProcessFinished(false);
					return;
				}

				ReplaceFiles();

				if (!ProcessAFS())
				{
					ProcessFinished(false);
					return;
				}

				GenerateDummyFile(Path.Combine(Globals.TempGDIPaths[0], "0.0"), 304 * 1024 * 1024); //304Mb

				ProcessTracks();

				GenerateFinalISOs();
			}

			ProcessFinished(true);
		}
	}
}

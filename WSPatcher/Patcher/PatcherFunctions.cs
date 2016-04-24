using System;
using System.IO;
using System.Text;
using System.Threading;

namespace WSPatcher
{
	partial class Patcher
	{
		bool CheckFiles()
		{
			int NullDiscs = 0;

			for (int n = 0; n < _TotalNumberOfDiscs; n++)
			{
				//Comprobar si los GDIs introducidos existen, o si se han dejado en blanco, marcar como null.
				//Así, en un juego de múltiples discos, no es obligatorio parchearlos todos siempre.
				if (_GDI[n].FileName.Length == 0)
				{
					_GDI[n].FileName = null;
					NullDiscs++;
				}
				else
				{
					if (File.Exists(_GDI[n].FileName))
					{
						_GDI[n].Path = Path.GetDirectoryName(_GDI[n].FileName);
					}
					else
					{
						OutputMessage(Localization.MsgBoxFileNotFound + _GDI[n].FileName, MessageType.Error);
						return false;
					}
				}

				//Si todos los GDIs son null, no se puede parchear nada.
				if (NullDiscs == _TotalNumberOfDiscs)
				{
					OutputMessage(Localization.MsgBoxNoDisc, MessageType.Error);
					return false;
				}

				//Comprobar si existen los tracks de los discos
				if (_GDI[n].FileName != null)
				{
					_GDI[n].Tracks = ReadTracksNames(_GDI[n].FileName);

					if (_GDI[n].Tracks[0] == "Error")
					{
						OutputMessage(String.Format(Localization.MsgBoxInvalidGDI, n + 1), MessageType.Error);
						return false;
					}

					for (int m = 0; m < _GDI[n].Tracks.Length; m++)
					{
						if (!File.Exists(_GDI[n].Tracks[m]))
						{
							OutputMessage(String.Format(Localization.MsgBoxTrackNotFound, _GDI[n].Tracks[m], _GDI[n].Tracks.Length, _GDI[n].FileName), MessageType.Error);
							return false;
						}
					}

					int BaseOffset = 0;

					if (Path.GetExtension(_GDI[n].Tracks[0]).ToLower() == ".bin")
					{
						BaseOffset = 0x10;
						_GDI[n].TrackFormat = TrackFormat.BIN;
					}
					else
						_GDI[n].TrackFormat = TrackFormat.ISO;

					//Comprobar si el GDI es del juego correcto y no de otro, y si se ha introducido el cd correcto
					byte[] GameCode = Encoding.ASCII.GetBytes(GameInfo.GameCode);
					byte[] ReadCode = new byte[GameCode.Length];
					byte[] CDNumber = new byte[1];

					using (FileStream fs = new FileStream(_GDI[n].Tracks[0], FileMode.Open, FileAccess.Read))
					{
						fs.Seek(BaseOffset + 0x2B, SeekOrigin.Begin);
						fs.Read(CDNumber, 0, CDNumber.Length);
						fs.Seek(BaseOffset + 0x40, SeekOrigin.Begin);
						fs.Read(ReadCode, 0, ReadCode.Length);
					}
					if (!Utils.ByteArrayCompare(ReadCode, GameCode))
					{
						OutputMessage(String.Format(Localization.MsgBoxGDIWrongOrder, n + 1, GameInfo.GameName, GameInfo.GameRegion), MessageType.Error);
						return false;
					}
					if (CDNumber[0] != 0x31 + n)
					{
						OutputMessage(String.Format(Localization.MsgBoxGDIWrongOrder2, CDNumber[0] - 0x30, GameInfo.GameName, n + 1), MessageType.Error);
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>Delete temporary files and folders if they exist.</summary>
		bool DeleteTempFiles()
		{
			if (Directory.Exists(Globals.TempPath))
			{
				Directory.Delete(Globals.TempPath, true);

				//Check if TempPath has already been deleted, since it can cause exceptions sometimes otherwise.
				//The reason is that program flow will not be blocked while deleting the folder. So sometimes
				//it will try to recreate the folder while still deleting it.
				bool HasBeenDeleted = false;
				for (int n = 0; n < 10; n++)
				{
					if (Directory.Exists(Globals.TempPath))
					{
						Thread.Sleep(250);
					}
					else
					{
						HasBeenDeleted = true;
						break;
					}
				}
				if (!HasBeenDeleted)
				{
					OutputMessage(Localization.MsgBoxDeleteTempError, MessageType.Error);
					return false;
				}
			}

			//Create the directories again for the next patching process
			for (int n = 0; n < _TotalNumberOfDiscs; n++)
			{
				Directory.CreateDirectory(Globals.TempGDIPaths[n]);
			}
			Directory.CreateDirectory(Globals.TempIPPath);
			Directory.CreateDirectory(Globals.TempTracksPath);

			return true;
		}

#if !ExternalPatch
		void UncompressRequiredFiles()
		{
			Utils.DecompressLZMA(PatchInfo.InternalDataName, Globals.TempZipName);
			Utils.DecompressZIP(Globals.TempZipName, Path.GetTempPath());

			if (File.Exists(Globals.TempZipName)) File.Delete(Globals.TempZipName);
		}
#endif

		/// <summary>Extract GDI files to temporary folder.</summary>
		bool ExtractGDIs()
		{
			for (int n = 0; n < _TotalNumberOfDiscs; n++)
			{
				if (_GDI[n].FileName != null)
				{
					//Convert BIN files to ISO. If they are already in ISO format, then do nothing.
					string TempISOFile2 = Path.Combine(Globals.TempGDIPaths[n], Path.GetFileNameWithoutExtension(_GDI[n].Tracks[2]) + ".iso");
					string TempISOFile8 = Path.Combine(Globals.TempGDIPaths[n], Path.GetFileNameWithoutExtension(_GDI[n].Tracks[8]) + ".iso");

					//TODO: Comprobar que va bien con tracks en formato BIN
					if (_GDI[n].TrackFormat == TrackFormat.BIN)
					{
						StatusChanged(String.Format(Localization.StatusConvertingTracksToISO, n + 1), CurrentProgress++);

						if (StartProcess("bin2iso.exe", "\"" + _GDI[n].Tracks[2] + "\" \"" + TempISOFile2 + "\"", null, true) != 0) return false;
						if (StartProcess("bin2iso.exe", "\"" + _GDI[n].Tracks[8] + "\" \"" + TempISOFile8 + "\"", null, true) != 0) return false;

						if (!File.Exists(TempISOFile2) || !File.Exists(TempISOFile8))
						{
							OutputMessage(String.Format(Localization.MsgBoxConvertingTracksError, n + 1), MessageType.Error);
							return false;
						}
					}

					//Extract GDIs
					StatusChanged(String.Format(Localization.StatusExtractingGDI, n + 1), CurrentProgress++);

					int LBA = ReadLBA(_GDI[n].FileName);

					if (_GDI[n].TrackFormat == TrackFormat.BIN)
					{
						if (StartProcess("extract.exe", "\"" + TempISOFile2 + "\" \"" + TempISOFile8 + "\" " + LBA.ToString(), Globals.TempGDIPaths[n], false) != 0) return false;
						File.Delete(TempISOFile2);
						File.Delete(TempISOFile8);
					}
					else
					{
						if (StartProcess("extract.exe", "\"" + _GDI[n].Tracks[2] + "\" \"" + _GDI[n].Tracks[8] + "\" " + LBA.ToString(), Globals.TempGDIPaths[n], false) != 0) return false;
					}

					//File.Move(Path.Combine(Globals.TempGDIPaths[n], "IP.BIN"), Path.Combine(Globals.TempIPPath, "IP" + (n + 1) + ".BIN"));
					File.Delete(Path.Combine(Globals.TempGDIPaths[n], "IP.BIN"));
				}
			}

			return true;
		}

		bool PatchXDeltas()
		{
			if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/SIMMDATA.BIN"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/SIMMDATA.BIN.DELTA"))) return false;
			if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/EPLG/MODELS.PKF"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/EPLG/MODELS.PKF.DELTA"))) return false;
			if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/GSEN/MODELS2.PKF"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/GSEN/MODELS2.PKF.DELTA"))) return false;
			if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/GSEN/MODELS2.PKS"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/GSEN/MODELS2.PKS.DELTA"))) return false;
			if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/STREAM/HUMANS.AFS"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/STREAM/HUMANS.AFS.DELTA"))) return false;
			if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/TIKN/MAP01.MT5"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/TIKN/MAP01.MT5.DELTA"))) return false;
			if (Localization.SelectedLanguage == Localization.Languages.Spanish)
			{
				if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/TIKN/MAP02.MT5"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/TIKN/MAP02.MT5.DELTA"))) return false;
			}
			if (!ApplyXDelta(0, Path.Combine(Globals.TempGDIPaths[0], "SCENE/80/YUK1/MAP09.MT5"), Path.Combine(Globals.PatchPaths[0], "XDeltas/SCENE/80/YUK1/MAP09.MT5.DELTA"))) return false;

			return true;
		}

		void ReplaceFiles()
		{
			File.Copy(Path.Combine(Globals.PatchPaths[0], "0GDTEX.bmp"), Path.Combine(Globals.TempGDIPaths[0], "0GDTEX.bmp"));
			File.Copy(Path.Combine(Globals.PatchPaths[0], "AUTORUN.INF"), Path.Combine(Globals.TempGDIPaths[0], "AUTORUN.INF"));

			if (_SelectedImageFormat == ImageFormat.MDF_MDS_Consoles)
				File.Copy(Path.Combine(Globals.PatchPaths[0], "1ST_READ.BIN"), Path.Combine(Globals.TempGDIPaths[0], "1ST_READ.BIN"), true);
			else
				File.Copy(Path.Combine(Globals.PatchPaths[0], "1ST_READ.GDI.BIN"), Path.Combine(Globals.TempGDIPaths[0], "1ST_READ.BIN"), true);

			if (_SelectedImageFormat == ImageFormat.MDF_MDS_Consoles)
				File.Copy(Path.Combine(Globals.PatchPaths[0], "IP.BIN"), Path.Combine(Globals.TempIPPath, "IP1.BIN"));
			else
				File.Copy(Path.Combine(Globals.PatchPaths[0], "IP.GDI.BIN"), Path.Combine(Globals.TempIPPath, "IP1.BIN"));

			Utils.CopyFolder(Path.Combine(Globals.PatchPaths[0], "FONT"), Path.Combine(Globals.TempGDIPaths[0], "FONT"), true);
			Utils.CopyFolder(Path.Combine(Globals.PatchPaths[0], "MODEL"), Path.Combine(Globals.TempGDIPaths[0], "MODEL"), true);
			Utils.CopyFolder(Path.Combine(Globals.PatchPaths[0], "SPRITE"), Path.Combine(Globals.TempGDIPaths[0], "SPRITE"), true);
			Utils.CopyFolder(Path.Combine(Globals.PatchPaths[0], "TITLE"), Path.Combine(Globals.TempGDIPaths[0], "TITLE"), true);
		}

		bool ProcessAFS()
		{
			AFS Afs = new AFS();

			for (int n = 0; n < _TotalNumberOfDiscs; n++)
			{
				if (_GDI[n].FileName != null)
				{
					StatusChanged(String.Format(Localization.StatusExtractingAFS, n + 1), CurrentProgress++);

					string[] AFSFiles = Directory.GetFiles(Path.Combine(Globals.TempGDIPaths[n], "SCENE/80/STREAM"), "*.afs");

					//Extraer AFS

					for (int i = 0; i < AFSFiles.Length; i++)
					{
						string Folder = Path.Combine(Path.GetDirectoryName(AFSFiles[i]), Path.GetFileNameWithoutExtension(AFSFiles[i]));
						if (!Afs.ExtractAFS(AFSFiles[i], Folder, Folder + ".list"))
						{
							OutputMessage(Localization.MsgBoxAFSExtractingError + AFSFiles[i], MessageType.Error);
							return false;
						}
					}

					//Reemplazar los SRF extraidos por los del parche
					StatusChanged(String.Format(Localization.StatusReplacingSRF, n + 1), CurrentProgress++);
					Utils.CopyFolder(Path.Combine(Globals.PatchPaths[n], "SCENE/80/STREAM"), Path.Combine(Globals.TempGDIPaths[n], "SCENE/80/STREAM"), true);

					//Recrear los AFS
					StatusChanged(String.Format(Localization.StatusRegeneratingAFS, n + 1), CurrentProgress++);

					for (int i = 0; i < AFSFiles.Length; i++)
					{
						string Folder = Path.Combine(Path.GetDirectoryName(AFSFiles[i]), Path.GetFileNameWithoutExtension(AFSFiles[i]));

						Afs.CreateAFS(Folder, AFSFiles[i] + "2", Folder + ".list");
						File.Delete(Folder + ".list");
						Directory.Delete(Folder, true);

						//Regenerar IDX salvo para el archivo HUMAN.AFS
						if (!AFSFiles[i].EndsWith("HUMANS.AFS"))
						{
							if (StartProcess("idxmaker.exe", "-1 \"" + AFSFiles[i] + "2\" \"" + Folder + ".IDX2\" \"" + AFSFiles[i] + "\" \"" + Folder + ".IDX\"", null, true) != 0) return false;
							if (File.Exists(Folder + ".IDX2"))
							{
								File.Delete(Folder + ".IDX");
								File.Move(Folder + ".IDX2", Folder + ".IDX");
							}
						}

						File.Delete(AFSFiles[i]);
						File.Move(AFSFiles[i] + "2", AFSFiles[i]);
					}
				}
			}

			return true;
		}

		void ProcessTracks()
		{
			if (_SelectedImageFormat == ImageFormat.MDF_MDS_Consoles)
			{
				GenerateDummyFile(Path.Combine(Globals.TempTracksPath, "track01.raw"), 1427744);
				GenerateDummyFile(Path.Combine(Globals.TempTracksPath, "track02.raw"), 2025840);
				GenerateDummyFile(Path.Combine(Globals.TempTracksPath, "track03.raw"), 1033888);

				TrimFile(_GDI[0].Tracks[3], Path.Combine(Globals.TempTracksPath, "track04.raw"), 1024, 32);
				TrimFile(_GDI[0].Tracks[4], Path.Combine(Globals.TempTracksPath, "track05.raw"), 1024, 32);
				TrimFile(_GDI[0].Tracks[5], Path.Combine(Globals.TempTracksPath, "track06.raw"), 1024, 32);
				TrimFile(_GDI[0].Tracks[6], Path.Combine(Globals.TempTracksPath, "track07.raw"), 2048, 32);
				TrimFile(_GDI[0].Tracks[7], Path.Combine(Globals.TempTracksPath, "track08.raw"), 2048, 32);

				GenerateDummyFile(Path.Combine(Globals.TempTracksPath, "track09.raw"), 1073440);
			}
		}

		bool GenerateFinalISOs()
		{
			if (!Directory.Exists(Globals.FinalISOPath)) Directory.CreateDirectory(Globals.FinalISOPath);

			string[] num = { "45000" };

			for (int n = 0; n < _TotalNumberOfDiscs; n++)
			{
				if (_GDI[n].FileName != null)
				{
					if (_SelectedImageFormat == ImageFormat.GDI_Emulators)
					{
						if (!Directory.Exists(Globals.FinalISO[n])) Directory.CreateDirectory(Globals.FinalISO[n]);

						string CDDATracks = string.Format("-cdda \"{0}\" \"{1}\" \"{2}\" \"{3}\" \"{4}\"", _GDI[n].Tracks[3], _GDI[n].Tracks[4], _GDI[n].Tracks[5], _GDI[n].Tracks[6], _GDI[n].Tracks[7]);

						//buildgdi.exe -data (CARPETA CON ARCHIVOS MODIFICADOS) -ip IP.BIN -V (NOMBRE INTERNO DE LA IMAGEN) -cdda (TRACKS DE AUDIO A PARTIR DEL 4) -output (CARPETA CON EL RESULTADO, DEBE EXISTIR DE ANTEMANO) -raw 2352
						if (StartProcess("buildgdi.exe", String.Format("-data \"{0}\" -ip \"{1}.BIN\" -V WHATS_SHENMUE {2} -output \"{3}\" -raw 2352", Globals.TempGDIPaths[n], Path.Combine(Globals.TempIPPath, "IP" + (n + 1)), CDDATracks, Globals.FinalISO[n]), null, true) != 0) return false;

						File.Copy(_GDI[n].Tracks[0], Path.Combine(Globals.FinalISO[n], Path.GetFileName(_GDI[n].Tracks[0])), true);
						File.Copy(_GDI[n].Tracks[1], Path.Combine(Globals.FinalISO[n], Path.GetFileName(_GDI[n].Tracks[1])), true);
						File.Copy(_GDI[n].Tracks[3], Path.Combine(Globals.FinalISO[n], Path.GetFileName(_GDI[n].Tracks[3])), true);
						File.Copy(_GDI[n].Tracks[4], Path.Combine(Globals.FinalISO[n], Path.GetFileName(_GDI[n].Tracks[4])), true);
						File.Copy(_GDI[n].Tracks[5], Path.Combine(Globals.FinalISO[n], Path.GetFileName(_GDI[n].Tracks[5])), true);
						File.Copy(_GDI[n].Tracks[6], Path.Combine(Globals.FinalISO[n], Path.GetFileName(_GDI[n].Tracks[6])), true);
						File.Copy(_GDI[n].Tracks[7], Path.Combine(Globals.FinalISO[n], Path.GetFileName(_GDI[n].Tracks[7])), true);

						string[] OriginalGDI = File.ReadAllLines(_GDI[n].FileName);
						string NewGDITracks = File.ReadAllText(Path.Combine(Globals.UtilsPath, "GDIText.txt"));
						using (StreamWriter sw = new StreamWriter(Path.Combine(Globals.FinalISO[n], Path.GetFileNameWithoutExtension(Globals.FinalISO[n]) + ".gdi"), false, System.Text.Encoding.ASCII))
						{
							for (int i = 0; i < 3; i++) sw.WriteLine(OriginalGDI[i]);
							sw.Write(NewGDITracks);
						}
					}
					else
					{
						StatusChanged(String.Format(Localization.StatusBuildingFinalISO, n + 1), CurrentProgress++);

						//mkisofs -C 0,45000 -V WHATS_SHENMUE -G IP.BIN -sort sorttxt1.txt -duplicates-once -l -o test01.ISO GDI1
						if (StartProcess("mkisofs.exe", String.Format("-C 0,{0} -V WHATS_SHENMUE -G \"{1}\" -sort \"{2}\" -duplicates-once -l -o \"{3}.iso\" \"{4}\"", num[n], Path.Combine(Globals.TempIPPath, "IP" + (n + 1) + ".BIN"), Path.Combine(Globals.PatchPath, "sorttxt" + (n + 1) + ".txt"), Globals.FinalISO[n], Globals.TempGDIPaths[n]), null, false) != 0) return false;

						//mds4dc -c WhatsShenmueCDDA.MDS test01.ISO track01EDIT.raw track02EDIT.raw track03EDIT.raw track04EDIT.raw track05EDIT.raw track06EDIT.raw track07EDIT.raw track08EDIT.raw track09EDIT.raw
						if (StartProcess("mds4dc.exe", String.Format("-c \"{0}.mds\" \"{0}.iso\" \"{1}\" \"{2}\" \"{3}\" \"{4}\" \"{5}\" \"{6}\" \"{7}\" \"{8}\" \"{9}\"", Globals.FinalISO[n], Path.Combine(Globals.TempTracksPath, "track01.raw"), Path.Combine(Globals.TempTracksPath, "track02.raw"), Path.Combine(Globals.TempTracksPath, "track03.raw"), Path.Combine(Globals.TempTracksPath, "track04.raw"), Path.Combine(Globals.TempTracksPath, "track05.raw"), Path.Combine(Globals.TempTracksPath, "track06.raw"), Path.Combine(Globals.TempTracksPath, "track07.raw"), Path.Combine(Globals.TempTracksPath, "track08.raw"), Path.Combine(Globals.TempTracksPath, "track09.raw")), null, false) != 0) return false;

#if !DEBUG
						File.Delete(Globals.FinalISO[n] + ".iso");
#endif
					}
				}
			}

			return true;
		}
	}
}
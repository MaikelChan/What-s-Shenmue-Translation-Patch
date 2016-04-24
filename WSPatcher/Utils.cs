using System;
using System.IO;
using System.IO.Compression;

namespace WSPatcher
{
	class Utils
	{
		/// <summary>
		/// Copy a folder and subfolders to another folder
		/// </summary>
		/// <param name="SourceFolder">Source folder.</param>
		/// <param name="DestinationFolder">Destination folder.</param>
		/// <param name="Overwrite">Overwrite existing files in the destination folder and subfolders.</param>
		public static void CopyFolder(string SourceFolder, string DestinationFolder, bool Overwrite)
		{
			if (!Directory.Exists(DestinationFolder)) Directory.CreateDirectory(DestinationFolder);
			string[] files = Directory.GetFiles(SourceFolder);

			foreach (string file in files)
			{
				string name = Path.GetFileName(file);
				string dest = Path.Combine(DestinationFolder, name);
				File.Copy(file, dest, Overwrite);
			}

			string[] folders = Directory.GetDirectories(SourceFolder);

			foreach (string folder in folders)
			{
				string name = Path.GetFileName(folder);
				string dest = Path.Combine(DestinationFolder, name);
				CopyFolder(folder, dest, Overwrite);
			}
		}

		/// <summary>
		/// Decompress a ZIP file
		/// </summary>
		public static void DecompressZIP(string SourceFile, string OutputDirectory)
		{
			ZipFile.ExtractToDirectory(SourceFile, OutputDirectory);
		}

		/// <summary>
		/// Decompress a LZMA file
		/// </summary>
		public static void DecompressLZMA(string SourceFile, string OutputFile)
		{
			using (Stream input = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(SourceFile))
			{
				using (FileStream output = new FileStream(OutputFile, FileMode.Create))
				{
					SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();

					byte[] properties = new byte[5];
					if (input.Read(properties, 0, 5) != 5)
						throw (new Exception("input .lzma is too short"));
					decoder.SetDecoderProperties(properties);

					long outSize = 0;
					for (int i = 0; i < 8; i++)
					{
						int v = input.ReadByte();
						if (v < 0)
							throw (new Exception("Can't Read 1"));
						outSize |= ((long)(byte)v) << (8 * i);
					}
					long compressedSize = input.Length - input.Position;

					decoder.Code(input, output, compressedSize, outSize, null);
				}
			}
		}

		/// <summary>
		/// Compare two byte arrays.
		/// </summary>
		public static bool ByteArrayCompare(byte[] a1, byte[] a2)
		{
			if (a1.Length != a2.Length)
				return false;

			for (int i = 0; i < a1.Length; i++)
				if (a1[i] != a2[i])
					return false;

			return true;
		}

		/// <summary>
		/// Gets if OS is Windows 7 or superior
		/// </summary>
		public static bool IsWindows7OrSuperior()
		{
			if (Environment.OSVersion.Version.Major == 6)
			{
				if (Environment.OSVersion.Version.Minor >= 1) return true;
			}
			else if (Environment.OSVersion.Version.Major > 6)
			{
				return true;
			}

			return false;
		}
	}
}

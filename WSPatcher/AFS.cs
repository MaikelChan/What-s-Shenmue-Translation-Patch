using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WSPatcher
{
    class AFS
    {
        public void CreateAFS(string inputDirectory, string outputFile, string FilesList = "")
        {
            string[] inputFiles;

            if (FilesList == "")
            {
                inputFiles = Directory.GetFiles(inputDirectory);
            }
            else
            {
                inputFiles = File.ReadAllLines(FilesList);
            }

            FileStream fs1 = new FileStream(outputFile, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs1);

            byte[] header = { 0x41, 0x46, 0x53, 0x00 }; //AFS

            fs1.Write(header, 0, header.Length);
            bw.Write((uint)inputFiles.Length);

            //Generate TOC and FileNameDirectory

            TableOfContents[] TOC = new TableOfContents[inputFiles.Length];
            FileNameDirectory[] FND = new FileNameDirectory[inputFiles.Length];

            UInt32 CurrentOffset = Pad((uint)(8 + (8 * inputFiles.Length) + 8), 0x800);  //Header + TOC + FND Offset and size

            for (int n = 0; n < inputFiles.Length; n++)
            {
                FileInfo f = new FileInfo(inputFiles[n]);

                //TOC
                TOC[n].FileSize = (uint)f.Length;
                TOC[n].Offset = CurrentOffset;

                CurrentOffset += TOC[n].FileSize;
                CurrentOffset = Pad(CurrentOffset, 0x800);

                //FileNameDirectory
                FND[n].FileName = Path.GetFileName(inputFiles[n]);
                FND[n].Year = (UInt16)f.LastWriteTime.Year;
                FND[n].Month = (UInt16)f.LastWriteTime.Month;
                FND[n].Day = (UInt16)f.LastWriteTime.Day;
                FND[n].Hour = (UInt16)f.LastWriteTime.Hour;
                FND[n].Minute = (UInt16)f.LastWriteTime.Minute;
                FND[n].Second = (UInt16)f.LastWriteTime.Second;
                FND[n].FileSize = (uint)f.Length;
            }

            //Write TOC to file
            for (int n = 0; n < inputFiles.Length; n++)
            {
                bw.Write(TOC[n].Offset);
                bw.Write(TOC[n].FileSize);
            }

            //Write Filename Directory Offset and Size
            UInt32 FNDOffset = CurrentOffset;
            UInt32 FNDSize = (uint)(inputFiles.Length * 0x30);
            fs1.Seek(TOC[0].Offset - 8, SeekOrigin.Begin);
            bw.Write(FNDOffset);
            bw.Write(FNDSize);

            //Write files data to file
            for (int n = 0; n < inputFiles.Length; n++)
            {
                byte[] data = File.ReadAllBytes(inputFiles[n]);
                fs1.Seek(TOC[n].Offset, SeekOrigin.Begin);
                fs1.Write(data, 0, data.Length);
            }

            //Write Filename Directory
            fs1.Seek(FNDOffset, SeekOrigin.Begin);
            for (int n = 0; n < inputFiles.Length; n++)
            {
                byte[] name = Encoding.Default.GetBytes(FND[n].FileName);
                fs1.Write(name, 0, name.Length);
                fs1.Seek(0x20 - name.Length, SeekOrigin.Current);

                bw.Write(FND[n].Year);
                bw.Write(FND[n].Month);
                bw.Write(FND[n].Day);
                bw.Write(FND[n].Hour);
                bw.Write(FND[n].Minute);
                bw.Write(FND[n].Second);
                bw.Write(FND[n].FileSize);
            }

            //Pad final 0s
            long currentPosition = fs1.Position;
            long EOF = Pad((uint)fs1.Position, 0x800);
            for (long n = currentPosition; n < EOF; n++) bw.Write((byte)0);

            bw.Close();
            fs1.Close();
        }

        public bool ExtractAFS(string inputFile, string outputDirectory, string FilesList = "")
        {
            bool ThereIsFileNameTable = true;

            FileStream fs1 = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs1);

            if (br.ReadUInt32() != 0x00534641) //Header different than AFS
            {
                br.Close();
                fs1.Close();
                return false;
            }

            UInt32 NumberOfFiles = br.ReadUInt32();

            TableOfContents[] TOC = new TableOfContents[NumberOfFiles];
            FileNameDirectory[] FND = new FileNameDirectory[NumberOfFiles];

            //Read TOC
            for (int n = 0; n < NumberOfFiles; n++)
            {
                TOC[n].Offset = br.ReadUInt32();
                TOC[n].FileSize = br.ReadUInt32();
            }

            //Read Filename Directory Offset and Size
            UInt32 FNDOffset = 0;
            UInt32 FNDSize = 0;
            while (fs1.Position < TOC[0].Offset && FNDOffset == 0)
            {
                //fs1.Seek(TOC[0].Offset - 8, SeekOrigin.Begin);
                FNDOffset = br.ReadUInt32();
                FNDSize = br.ReadUInt32();
            }
            if (FNDOffset == 0) ThereIsFileNameTable = false;

            string[] FileName = new string[NumberOfFiles];

            if (ThereIsFileNameTable)
            {
                //Read FND
                fs1.Seek(FNDOffset, SeekOrigin.Begin);

                for (int n = 0; n < NumberOfFiles; n++)
                {
                    byte[] name = new byte[32];
                    fs1.Read(name, 0, name.Length);
                    FileName[n] = Encoding.Default.GetString(name).Replace("\0", "");

                    FND[n].Year = br.ReadUInt16();
                    FND[n].Month = br.ReadUInt16();
                    FND[n].Day = br.ReadUInt16();
                    FND[n].Hour = br.ReadUInt16();
                    FND[n].Minute = br.ReadUInt16();
                    FND[n].Second = br.ReadUInt16();
                    FND[n].FileSize = br.ReadUInt32();
                }
            }
            else
            {
                string IDX = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile)) + ".idx";

                if (File.Exists(IDX))
                {
                    FileName = ReadIDX(IDX);
                }
                else
                {
                    for (int n = 0; n < NumberOfFiles; n++)
                    {
                        FileName[n] = n.ToString("00000000");
                    }
                }
            }

            //Extract files
            if (outputDirectory.EndsWith(@"\") == false) outputDirectory += @"\";
            if (Directory.Exists(outputDirectory) == false) Directory.CreateDirectory(outputDirectory);

            String[] filelist = new String[NumberOfFiles];

            for (int n = 0; n < NumberOfFiles; n++)
            {
                byte[] filedata = new byte[TOC[n].FileSize];
                fs1.Seek(TOC[n].Offset, SeekOrigin.Begin);
                fs1.Read(filedata, 0, filedata.Length);

                string outputFile = outputDirectory + FileName[n];

                File.WriteAllBytes(outputFile, filedata);

                if (ThereIsFileNameTable)
                {
                    System.DateTime date = new System.DateTime(FND[n].Year, FND[n].Month, FND[n].Day, FND[n].Hour, FND[n].Minute, FND[n].Second);
                    File.SetLastWriteTime(outputFile, date);
                }

                filelist[n] = outputFile; //Save the list of files in order to have the original order
            }

            if (FilesList != "") File.WriteAllLines(FilesList, filelist);

            br.Close();
            fs1.Close();

            return true;
        }

        public string[] ReadIDX(string IDXFile)
        {
            FileStream fs1 = new FileStream(IDXFile, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs1);

            br.BaseStream.Seek(0x4, SeekOrigin.Begin);
            int SubIndexOffset = br.ReadInt32();

            br.BaseStream.Seek(0xa, SeekOrigin.Begin);
            UInt16 SubIndexCount = br.ReadUInt16();

            br.BaseStream.Seek(SubIndexOffset + 4, SeekOrigin.Begin);
            int IndexOffset = br.ReadInt32() + SubIndexOffset;

            br.BaseStream.Seek(IndexOffset + 4, SeekOrigin.Begin);
            int IndexCount = br.ReadInt32() / 12;

            string[] IndexNames = new string[IndexCount];
            int[] IndexSubIndexCount = new int[IndexCount];

            //Read Index
            for (int i = 0; i < IndexCount; i++)
            {
                byte[] data = new byte[8];
                fs1.Read(data, 0, 8);

                IndexNames[i] = Encoding.Default.GetString(data);

                UInt16 A = br.ReadUInt16();
                UInt16 B = br.ReadUInt16();

                IndexSubIndexCount[i] = B - A - i;
                if (IndexSubIndexCount[i] < 0) IndexSubIndexCount[i] = 0;
            }

            br.BaseStream.Seek(SubIndexOffset + 8, SeekOrigin.Begin);

            List<string> FullNames = new List<string>();

            //Read Sub Index
            for (int i = 0; i < IndexSubIndexCount.Length; i++)
            {
                for (int j = 0; j < IndexSubIndexCount[i]; j++)
                {
                    byte[] data = new byte[4];
                    fs1.Read(data, 0, 4);

                    FullNames.Add(IndexNames[i].TrimStart("_".ToCharArray()) + Encoding.Default.GetString(data) + ".ahx");

                    fs1.Seek(4, SeekOrigin.Current);
                }

                if (IndexSubIndexCount[i] > 0) FullNames.Add(IndexNames[i].TrimStart("_".ToCharArray()) + ".SRF");
            }

            br.Close();
            fs1.Close();

            //File.WriteAllLines(@"C:\Users\Miguel\Desktop\lol.txt", FullNames.ToArray(), Encoding.Default);
            return FullNames.ToArray();
        }

        uint Pad(uint value, uint padBytes)
        {
            if ((value % padBytes) != 0) return value + (padBytes - (value % padBytes));
            else return value;
        }

        public struct TableOfContents
        {
            public UInt32 Offset;
            public UInt32 FileSize;
        }

        public struct FileNameDirectory
        {
            public string FileName;
            public UInt16 Year;
            public UInt16 Month;
            public UInt16 Day;
            public UInt16 Hour;
            public UInt16 Minute;
            public UInt16 Second;
            public UInt32 FileSize;
        }
    }
}

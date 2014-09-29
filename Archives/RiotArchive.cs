using ItzWarty;
using System;
using System.IO;

// Formerly RAFArchive from 8 May 2011 ~~ItzWarty
namespace Dargon.IO.RADS.Archives
{
    public class RiotArchive
    {
        private FileStream dataFileStream = null;
        private RAFDirectoryFile directoryFile = null;
        private string rafPath = "";
       private readonly string datpaht;

       public RiotArchive(string rafPath, string datpaht)
        {
            this.rafPath = rafPath;
           this.datpaht = datpaht;
           //dataFileStream = new FileStream(rafPath+".dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            this.directoryFile = new RAFDirectoryFile(this, rafPath); //Doesn't take file handle
        }

        public string RAFFilePath
        {
            get
            {
                return rafPath;
            }
        }

        public string DatFilePath
        {
           get { return datpaht; }
        }

        /// <summary>
        /// Gets a FileStream of our RAF data file
        /// </summary>
        /// <returns></returns>
        //public FileStream GetDataFileContentStream()
        //{
        //    return this.dataFileStream;
        //}
        /// <summary>
        /// Gets the RAFDirectoryFile wrapper
        /// </summary>
        /// <returns></returns>
        public RAFDirectoryFile GetDirectoryFile() { return this.directoryFile; }


        /// <summary>
        /// Packs the /dump/ child directory of the given directory.
        /// Outputs to the given ostream.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="ostream"></param>
        /// <returns></returns>
        public static bool Pack(string directory, StreamWriter ostream)
        {
            //Environment.CurrentDirectory = directory;
            //Environment.CurrentDirectory = @"C:\Riot Games\League of Legends\RADS\projects\lol_game_client\filearchives\0.0.0.29\";
            ostream.WriteLine("Experimental RAF Packer by ItzWarty @ ItzWarty.com April 29 2011 3:37pm pst");
            ostream.WriteLine("Only use-able after RAF Dumper, for now.");
            //Pack whatever dump folder is sitting next to us, write to pack folder
            if (Directory.Exists(Environment.CurrentDirectory + @"\dump\"))
            {
                RAFPacker packer = new RAFPacker(ostream);
                return packer.PackRAF(
                    Environment.CurrentDirectory + @"\dump\",
                    Environment.CurrentDirectory + @"\pack\"
                );
            }
            else
            {
                ostream.WriteLine("Error"); //Very helpful error message
                return false;
            }
            //else
            //    MessageBox.Show("Place RAFDUMP executable next to 'dump' folder.  Upon run, the 'pack' folder will be created w/ raf+raf.dat");
        }
        /// <summary>
        /// Creates the given directory and all directories leading up to it.
        /// </summary>
        private static void PrepareDirectory(string path)
        {
            path = path.Replace("/", "\\");
            String[] dirs = path.Split("\\");
            for (int i = 1; i < dirs.Length; i++)
            {
                String dirPath = String.Join("\\", dirs.SubArray(0, i)) + "\\";
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                //ostream.WriteLine(dirPath);
            }
        }
        /// <summary>
        /// Returns what i'm calling the ID of an archive, though it's probably related to LoL versioning.
        /// IE: 0.0.0.25, 0.0.0.26
        /// </summary>
        /// <returns></returns>
        public string GetID()
        {
            return new FileInfo(this.rafPath).Directory.Name;
        }

    }
}

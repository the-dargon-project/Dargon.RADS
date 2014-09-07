using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ItzWarty.Database
{
    public class DatabaseClient
    {
        /// <summary>
        /// Root is the root database [the location of the database folder]
        /// </summary>
        public string root;

        /// <summary>
        /// Folderstack is pushed into whenever you enter, popped when you go up, and cleared when you exit...
        /// </summary>
        public List<string> folderStack = new List<string>();

        /// <summary>
        /// When we jump to root, sometimes we want to define our root to actually be a subdatabase of our root database, so we push to here
        /// When we reset, we will copy all contents from here to the folderstack again.
        /// </summary>
        public List<string> defaultFolderStack = new List<string>();

        public string currentDBPath
        {
            get
            {
                if (folderStack.Count == 0)
                    return dbName;
                else
                {
                    string final = dbName;
                    for (int i = 0; i < folderStack.Count; i++) final += "." + folderStack[i];
                    return final;
                }
            }
        }
        public string currentFolderLocation
        {
            get
            {
                string final = root + "\\";
                for (int i = 0; i < folderStack.Count; i++) final += "\\" + folderStack[i];
                return final;
            }
        }
        public string dbName;
        /// <summary>
        /// Sets the DB Root, if it doesnt exist, create the DB Root
        /// </summary>
        /// <param name="DBNAME"></param>
        public DatabaseClient(string DBNAME)
        {
            this.root = Directory.GetCurrentDirectory() + "\\" + DBNAME;
            this.dbName = DBNAME;
            this.defaultFolderStack.Clear(); //We want to be at root, so we shouldn't be inside any subfolder...
            this.ReturnToRoot();
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
                Console.WriteLine("Database \"" + dbName + "\" created");
            }
            ReturnToRoot();
        }

        public void ReturnToRoot()
        {
            this.folderStack.Clear();
            this.folderStack = new List<string>(this.defaultFolderStack.ToArray());
        }
        public void ReturnToSuperParent()
        {
            this.folderStack.Clear();
            this.defaultFolderStack.Clear();
        }
        public enum DBResponse
        {
            InvalidName,
            Exists,
            doesNotExist,
            Selected,
            Created,
            moved,
            Deleted
        }
        public string[] GetExistingDBNames()
        {
            string[] toreturn = Directory.GetDirectories(currentFolderLocation);
            string dirToReplaceToNothing = "";
            ReturnToRoot();
            SelectDB("Channels");
            dirToReplaceToNothing = currentFolderLocation;
            for (int i = 0; i < toreturn.Length; i++)
                toreturn[i] = toreturn[i].Replace(dirToReplaceToNothing, "");
            return toreturn;
        }
        public System.Collections.ArrayList dirList = new System.Collections.ArrayList();
        public string[] OutputListofDBs()
        {
            dirList.Clear();
            //Console.WriteLine(currentFolderLocation);
            foreach (string dirName in Directory.GetDirectories(currentFolderLocation))
            {
                SubOutputListofDBs(dirName);
            }
            string[] final = new string[dirList.Count];
            for (int i = 0; i < dirList.Count; i++)
            {
                //Console.WriteLine((string)dirList[i]);
                final[i] = ((string)(dirList[i]));
            }
            return final;
        }
        public void SubOutputListofDBs(string curDir)
        {
            //Console.WriteLine(curDir.Replace(currentFolderLocation, ""));
            dirList.Add(curDir.Replace(currentFolderLocation, ""));
            foreach (string dirName in Directory.GetDirectories(curDir))
            {
                SubOutputListofDBs(dirName);
            }
        }
        public bool IsWinNameOkay(string val)
        {
            //NTFS does not allow / ? < > \ : * |
            if (val.Replace("/", "!") != val) return false;
            if (val.Replace("?", "!") != val) return false;
            if (val.Replace("<", "!") != val) return false;
            if (val.Replace(">", "!") != val) return false;
            if (val.Replace("\\", "!") != val) return false;
            if (val.Replace(":", "!") != val) return false;
            if (val.Replace("*", "!") != val) return false;
            if (val.Replace("|", "!") != val) return false;
            //FAT doesnt allow the above and ^
            if (val.Replace("^", "!") != val) return false;
            //illegal folder names under windows
            if (val.ToLowerInvariant() == "com1") return false;
            if (val.ToLowerInvariant() == "com2") return false;
            if (val.ToLowerInvariant() == "com3") return false;
            if (val.ToLowerInvariant() == "com4") return false;
            if (val.ToLowerInvariant() == "com5") return false;
            if (val.ToLowerInvariant() == "com6") return false;
            if (val.ToLowerInvariant() == "com7") return false;
            if (val.ToLowerInvariant() == "com8") return false;
            if (val.ToLowerInvariant() == "com9") return false;
            if (val.ToLowerInvariant() == "lpt1") return false;
            if (val.ToLowerInvariant() == "lpt2") return false;
            if (val.ToLowerInvariant() == "lpt3") return false;
            if (val.ToLowerInvariant() == "lpt4") return false;
            if (val.ToLowerInvariant() == "lpt5") return false;
            if (val.ToLowerInvariant() == "lpt6") return false;
            if (val.ToLowerInvariant() == "lpt7") return false;
            if (val.ToLowerInvariant() == "lpt8") return false;
            if (val.ToLowerInvariant() == "lpt9") return false;
            if (val.ToLowerInvariant() == "con") return false;
            if (val.ToLowerInvariant() == "nul") return false;
            if (val.ToLowerInvariant() == "prn") return false;

            //YOu cant have a period or a space at the end of a file name
            char[] a_tDB = val.ToCharArray();
            if (a_tDB[a_tDB.Length - 1] == ' ') return false;
            if (a_tDB[a_tDB.Length - 1] == '.') return false;
            //we shold be fine o_O
            return true;
        }

        #region dbAction
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldDBNAME"></param>
        /// <param name="newDBNAME"></param>
        /// <returns>Invalid Name, exists, does not exist, moved</returns>
        public DBResponse RenameDB(string oldDBNAME, string newDBNAME)
        {
            if (!IsWinNameOkay(newDBNAME))
                return DBResponse.InvalidName;
            if (!IsWinNameOkay(oldDBNAME))
                return DBResponse.InvalidName;
            if (!Directory.Exists(currentFolderLocation+"\\"+oldDBNAME)) return DBResponse.doesNotExist;
            if (Directory.Exists(currentFolderLocation + "\\" + newDBNAME)) return DBResponse.Exists;
            Directory.Move(currentFolderLocation + "\\" + oldDBNAME, currentFolderLocation + "\\" + newDBNAME);
            return DBResponse.moved;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetDB"></param>
        /// <returns>DBResponses: Exists, doesNotExist</returns>
        public DBResponse ExistDB(string targetDB)
        {
            if (Directory.Exists(currentFolderLocation + "\\"+targetDB)) return DBResponse.Exists;
            return DBResponse.doesNotExist;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetDB"></param>
        /// <returns>Exists, Created, InvalidName</returns>
        public DBResponse CreateDB(string targetDB)
        {
            if (ExistDB(targetDB) == DBResponse.Exists) return DBResponse.Exists;
            if (!IsWinNameOkay(targetDB)) return DBResponse.InvalidName;
            //create the DB
            Directory.CreateDirectory(currentFolderLocation + "\\" + targetDB.Trim());
            //dummy file for zip archives
            //File.WriteAllLines(currentFolderLocation + "\\" + targetDB.Trim() + "\\Dummy", new string[] { "dummy" });
            Console.WriteLine("Database \"" + currentDBPath + "." + targetDB + "\" created");
            return DBResponse.Created;
        }
        //DB = folder
        //Value = file
        public DBResponse SelectDB(string targetDB)
        {
            return SelectDB(targetDB, false);
        }

        public DatabaseClient SelectDB2(string targetDB)
        {
            DatabaseClient result = this.Clone();
            DBResponse response = result.SelectDB(targetDB);
            if (response == DBResponse.doesNotExist) return null;
            else if (response == DBResponse.Selected)
                return result;
            else
                return null;
        }

        public DatabaseClient SelectDB2(string targetDB, bool autocreate)
        {
            DatabaseClient result = this.Clone();
            DBResponse response = result.SelectDB(targetDB, autocreate);
            if (response == DBResponse.doesNotExist) return null;
            else if (response == DBResponse.Selected)
                return result;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetDB"></param>
        /// <returns>DBResponses: Selected, doesNotExist</returns>
        public DBResponse SelectDB(string targetDB, bool autocreate)
        {
            if (autocreate)
            {
                if (ExistDB(targetDB) == DBResponse.doesNotExist)
                    CreateDB(targetDB);
            }
            if (Directory.Exists(currentFolderLocation+"\\"+ targetDB))
            {
                folderStack.Add(targetDB);
                return DBResponse.Selected;
            }
            else
            {
                Console.WriteLine(currentFolderLocation +"\\"+ targetDB + "  Doesntexist");
                return DBResponse.doesNotExist;
            }
        }
        public DBResponse DeleteDB(string targetDB)
        {
            if (ExistDB(targetDB) == DBResponse.doesNotExist) return DBResponse.doesNotExist;
            if (!IsWinNameOkay(targetDB)) return DBResponse.InvalidName;
            Directory.Delete(currentFolderLocation + "\\" + targetDB.Trim());
            return DBResponse.Deleted;
        }
        #endregion
        #region valueAction
        public bool ValueExists(string valueName)
        {
            string loc = currentFolderLocation + "\\" + valueName;
            if (!File.Exists(loc)) return false;
            return true;
        }
        public string GetValue(string valueName)
        {
            string loc = currentFolderLocation + "\\" + valueName;
            if (!File.Exists(loc)) throw new Exception("Value " + valueName + " does NOT exist in" + loc);
            return File.ReadAllText(loc);
        }

        /// <summary>
        /// Stores the value as the (string)content 
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="valueContent"></param>
        public void SetValue(string valueName, object valueContent)
        {
            if (!IsWinNameOkay(valueName)) throw new Exception(valueName + "is an INVALID windows name at " + currentFolderLocation);
            string loc = currentFolderLocation + "\\" + valueName;
            if (!File.Exists(loc))
                File.Delete(loc);
            //start anew
            FileStream myFS = File.Create(loc);
            byte[] myBytes = Encoding.ASCII.GetBytes(valueContent.ToString());
            myFS.Write(myBytes, 0, myBytes.Length);
            myFS.Close();

            Console.WriteLine(currentDBPath + "." + valueName + " now set to " + valueContent);
        }

        public bool GetBool(string valueName)
        {
            return GetValue(valueName) == "1";
        }
        public void SetBool(string valueName, bool value)
        {
            SetValue(valueName, (value) ? "1" : "0");
        }

        public int GetInt(string intName)
        {
            return int.Parse(GetValue(intName));
        }
        public void SetInt(string intName, int value)
        {
            SetValue(intName, value.ToString());
        }
        #endregion
        public bool FileExists(string fileName)
        {
            string loc = currentFolderLocation + "\\" + fileName;
            return File.Exists(loc);
        }
        public void WriteFile(string fileName, byte[] data)
        {
            string loc = currentFolderLocation + "\\" + fileName;
            FileStream fs = File.Open(loc, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None); //While we are writing, you can't do anything with it, sorry.
            if (File.Exists(loc))
            {
                File.Delete(loc);
            }
        }
        public DatabaseClient Clone()
        {
            DatabaseClient copy = new DatabaseClient(this.dbName);
            copy.defaultFolderStack = new List<string>(this.defaultFolderStack.ToArray());
            copy.folderStack = new List<string>(this.folderStack.ToArray());
            return copy;
        }

        public void SetCurrentDBAsRoot()
        {
            defaultFolderStack.Clear();
            for (int i = 0; i < folderStack.Count; i++) defaultFolderStack.Add(folderStack[i]);
        }

        public void CreateTable(string tableName, string[] columns)
        {
            wDBTable.CreateTable(tableName, this);
        }
        public wDBTable GetTable(string name)
        {
            return new wDBTable(name, this);
        }
        public string[] ListDatabases()
        {
            string[] childDirs = Directory.GetDirectories(currentFolderLocation);
            for (int i = 0; i < childDirs.Length; i++)
                childDirs[i] = childDirs[i].Replace(currentFolderLocation, "");
            return childDirs;
        }
    }
}

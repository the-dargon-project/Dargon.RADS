//From: http://www.codeproject.com/Articles/1966/An-INI-file-handling-class-using-C
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ItzWarty.ThirdParty.Ini
{
    /// <summary>
    /// Create a New INI file to store or load data
    /// </summary>
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        /// <summary>
        /// INIFile Constructor.
        /// </summary>
        /// <PARAM name="INIPath"></PARAM>
        public IniFile(string INIPath)
        {
            path = INIPath;
        }
        /// <summary>
        /// Write Data to the INI File
        /// </summary>
        /// <PARAM name="Section">Section name</PARAM>
        /// <PARAM name="Key">Key Name</PARAM>
        /// <PARAM name="Value">Value Name</PARAM>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// Read Data Value From the Ini File
        /// </summary>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, this.path);
            return temp.ToString();
        }

        /// <summary>
        /// Gets the given property string of the property name section/key
        /// </summary>
        public string GetPropertyString(string propertyName, string defaultValue)
        {
            //Split property name by /
            var propertyNameParts = propertyName.Split("/");

            //Section is the first part of the property name, which defaults to application if that index doesn't exist
            string section = propertyNameParts.Count() >= 2 ? propertyNameParts.First() : "Application";

            //Key is the last part of the property name
            string key = propertyNameParts.Last();

            //Read the value from the ini file, it might be null/empty here
            string result = IniReadValue(section, key);

            //If it's null or empty, we set it to the default value
            if (string.IsNullOrEmpty(result))
            {
                SetPropertyString(propertyName, result = defaultValue);
            }
            return result;
        }

        /// <summary>
        /// Gets the given property integer of property name section/key
        /// </summary>
        public int GetPropertyInteger(string propertyName, int defaultValue)
        {
            //Gets the string representation of the property
            string resultString = GetPropertyString(propertyName, defaultValue.ToString());
            int result;

            if (!int.TryParse(resultString, out result))
            {
                SetPropertyString(propertyName, (result = defaultValue).ToString());
            }
            return result;
        }

        /// <summary>
        /// Sets the value of the given property with the given value.
        /// </summary>
        public bool GetPropertyBoolean(string propertyName, bool defaultValue)
        {
            return GetPropertyInteger(propertyName, defaultValue ? 1 : 0) == 1;
        }

        /// <summary>
        /// Sets the value of the given property with the given value.
        /// </summary>
        public void SetPropertyString(string propertyName, string value)
        {
            //Split property name by /
            var propertyNameParts = propertyName.Split("/");

            //Section is the first part of the property name, which defaults to application if that index doesn't exist
            string section = propertyNameParts.Count() >= 2 ? propertyNameParts.First() : "Application";

            //Key is the last part of the property name
            string key = propertyNameParts.Last();

            IniWriteValue(section, key, value);
        }

        /// <summary>
        /// Sets the value of the given property with the given value.
        /// </summary>
        public void SetPropertyInteger(string propertyName, int value)
        {
            SetPropertyString(propertyName, value.ToString());
        }

        /// <summary>
        /// Sets the value of the given property with the given value.
        /// </summary>
        public void SetPropertyBoolean(string propertyName, bool value)
        {
            SetPropertyString(propertyName, value ? "0" : "1");
        }
    }
}
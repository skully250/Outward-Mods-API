using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace OModAPI
{
    public class ConfigHelper
    {
        // Default path is Outward/Config
        private string basePath = Path.Combine(Directory.GetCurrentDirectory(), "BepInEx\\config");
        private string configName;
        // Default mode is read only
        private ConfigModes mode = ConfigModes.ReadOnly;
        private XmlDocument configDoc;
        private string xmlConfigDefault;
        private string configDirectory = "\\BepInEx\\config";
        private FileSystemWatcher directoryWatcher = new FileSystemWatcher();
        private FileSystemWatcher fileWatcher = new FileSystemWatcher();

        #region Constructors
        public ConfigHelper() { }

        public ConfigHelper(ConfigModes mode)
        {
            this.mode = mode;
        }

        public ConfigHelper(ConfigModes mode, string configName)
        {
            this.mode = mode;
            this.configName = configName;
        }
        public ConfigHelper(string configName)
        {
            this.configName = configName;
        }

        public ConfigHelper(string configName, string basePath)
        {
            this.configName = configName;
            this.basePath = basePath;
        }

        public ConfigHelper(ConfigModes mode, string configName, string basePath)
        {
            this.mode = mode;
            this.basePath = basePath;
            this.configName = configName;
        }

        #endregion

        public void Init()
        {
            // Create the config directory if it doesn't exist
            if (!Directory.Exists(basePath))
            {
                // File system operations are slow, so we can't guarantee that the directory will be created 
                // before we try to create the file

                directoryWatcher.Path = Directory.GetCurrentDirectory();
                directoryWatcher.Created += watcherCreated;
                directoryWatcher.EnableRaisingEvents = true;

                Directory.CreateDirectory(basePath);
                return;
            }

            configDoc = new XmlDocument();

            // Handle if the config file we're looking for doesn't actually exist
            if (!File.Exists(FullPath))
            {
                // Check if we allow creation
                if (mode == ConfigModes.CreateIfMissing)
                {
                    fileWatcher.Path = basePath;
                    fileWatcher.Created += watcherCreated;
                    fileWatcher.EnableRaisingEvents = true;
                    CreateDefault();
                    return;
                }
                else
                    throw new IOException(string.Format("Config file {0} doesn't exist, and ConfigMode doesn't permit creation.", FullPath));
            }
            try
            {
                configDoc.Load(FullPath);
            }
            catch (FileNotFoundException e)
            {
                throw new FileNotFoundException("Can't find config file: " + FullPath);
            }
            catch (XmlException e)
            {
                // Do nothing, which is bad practice
            }
        }

        private void watcherCreated(object sender, FileSystemEventArgs e)
        {
            // Wait for the directory we want to create to actually be created
            if ((configDirectory.Contains(e.Name) || e.Name.Contains(configDirectory) || e.Name.Equals(configDirectory)) ||
                configName.Contains(e.Name) || e.Name.Contains(configName) || e.Name.Equals(configName))
                // When that happens, re-call Init, since it will skip the creating this time
                Init();
        }

        public float ReadFloat(string xpath)
        {
            return float.Parse(ReadString(xpath));
        }

        public int ReadInt(string xpath)
        {
            return int.Parse(ReadString(xpath));
        }

        public string ReadString(string xpath)
        {
            XmlNode curNode = ReadNode(xpath);
            if (curNode == null)
                return null;
            return ReadNode(xpath).InnerText;
        }

        public XmlNode ReadNode(string xpath)
        {
            // If Init() hasn't been called by the time we try to read a value, call it
            if (configDoc == null)
                Init();
            return configDoc.SelectSingleNode(xpath);
        }

        public void WriteValue(string xpath, string value)
        {
            WriteToXml(xpath, value);
        }

        public void WriteToXml(string xpath, string value)
        {
            if (configDoc == null)
                Init();

            // Break up the path into parts
            string[] paths;
            if (xpath.Contains("/"))
                paths = xpath.Split('/');
            else
                paths = new string[] { xpath };

            XmlNode curNode = configDoc;
            XPathNavigator nav = configDoc.CreateNavigator();
            foreach (string s in paths)
            {
                // Skip blank paths
                if (s.Equals(""))
                    continue;

                // Try to move down the hierarchy, if we can't then create the nodes
                bool moveSuccess = nav.MoveToChild(s, "");
                if (!moveSuccess)
                {
                    XmlWriter pages = nav.AppendChild();
                    pages.WriteElementString(s, "");
                    pages.Close();
                    // Actually move to the new node
                    nav.MoveToChild(s, "");
                }
            }
            // Set the value at the end
            nav.SetValue(value);

            // Save the resulting changes
            configDoc.Save(FullPath);
        }

        /// <summary>
        /// Write the default xml to the config file, if specified.
        /// This allows easy storing of defaults, so users don't have to go look up 
        /// what the config file looks like.
        /// </summary>
        /// <param name="locBasePath"></param>
        /// <param name="configName"></param>
        private void CreateDefault()
        {
            try
            {
                // This is okay to do because we never get here unless the config file doesn't exist
                File.WriteAllText(FullPath, xmlConfigDefault);
            }
            catch (Exception e)
            {
                // This is bad practice, but I want to make sure the exception gets passed up to the individual mod
                // so people don't think it's the API's fault.
                throw e;
            }
        }

        // Accessors
        public string XMLDefaultConfig
        {
            get { return xmlConfigDefault; }
            set { this.xmlConfigDefault = value; }
        }
        public string FullPath
        {
            get { return Path.Combine(basePath, configName); }
        }
        public ConfigModes Mode
        {
            get { return mode; }
        }
        /// <summary>
        /// Expose the actual XmlDocument to allow for people to implement their own methods if need be.
        /// </summary>
        public XmlDocument ConfigDoc
        {
            get { return configDoc; }
        }

        public enum ConfigModes
        {
            ReadOnly,
            CreateIfMissing
        }
    }
}

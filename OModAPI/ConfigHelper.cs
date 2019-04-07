using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace OModAPI
{
    public class ConfigHelper
    {
        // Default path is Outward/Config
        private string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        private string configName;
        // Default mode is read only
        private ConfigModes mode = ConfigModes.ReadOnly;
        private XmlDocument configDoc;
        private string xmlConfigDefault;
        private string configDirectory = "\\Config";


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
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = Directory.GetCurrentDirectory();
                watcher.Created += watcherCreated;
                watcher.EnableRaisingEvents = true;

                Directory.CreateDirectory(basePath);
                return;
            }

            configDoc = new XmlDocument();

            // Handle if the config file we're looking for doesn't actually exist
            if (!File.Exists(FullPath))
            {
                // Check if we allow creation
                if (mode == ConfigModes.CreateIfMissing)
                    CreateDefault();
                throw new IOException(string.Format("Config file {0} doesn't exist, and ConfigMode doesn't permit creation.", FullPath));
            }
            try
            {
                configDoc.Load(FullPath);
            }
            catch(FileNotFoundException e)
            {
                throw new FileNotFoundException("Can't find config file: " + FullPath);
            }
        }

        private void watcherCreated(object sender, FileSystemEventArgs e)
        {
            // Wait for the directory we want to create to actually be created
            if (configDirectory.Contains(e.Name) || e.Name.Contains(configDirectory) || e.Name.Equals(configDirectory))
                // When that happens, re-call Init, since it will skip the creating this time
                Init();
        }

        public float ReadFloat(string xpath)
        {
            float res;
            float.TryParse(ReadString(xpath), out res);
            return res;
        }

        public int ReadInt(string xpath)
        {
            int res;
            int.TryParse(ReadString(xpath), out res);
            return res;
        }

        public string ReadString(string xpath)
        {
            return ReadNode(xpath).InnerText;
        }

        public XmlNode ReadNode(string xpath)
        {
            // If Init() hasn't been called by the time we try to read a value, call it
            if (configDoc == null)
                Init();
            return configDoc.SelectSingleNode(xpath);
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
            catch(Exception e)
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

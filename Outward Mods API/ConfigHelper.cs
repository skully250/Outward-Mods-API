using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace OModAPI
{
    class ConfigHelper
    {
        // Default path is Outward/Config
        private static string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Config");
        private string configName;
        // Default mode is read only
        private ConfigModes mode = ConfigModes.ReadOnly;
        private XmlDocument configDoc;
        private string xmlConfigDefault;


        #region Constructors
        public ConfigHelper() {}

        public ConfigHelper(ConfigModes mode)
        {
            this.mode = mode;
        }

        public ConfigHelper(ConfigModes mode, string configName)
        {
            this.mode = mode;
            init(basePath, configName);
        }
        public ConfigHelper(string configName)
        {
            init(basePath, configName);
        }

        public ConfigHelper(string configName, string locBasePath)
        {
            init(locBasePath, configName);
        }

        public ConfigHelper(ConfigModes mode, string configName, string locBasePath)
        {
            this.mode = mode;
            init(locBasePath, configName);
        }

        #endregion

        private void init(string locBasePath, string configName)
        {
            // Create the config directory if it doesn't exist
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Config"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Config");

            this.configName = configName;
            configDoc = new XmlDocument();

            // Handle if the config file we're looking for doesn't actually exist
            if (!File.Exists(FullPath))
            {
                // Check if we allow creation
                if (mode != ConfigModes.CreateIfMissing)
                    throw new IOException(string.Format("Config file {0} doesn't exist, and ConfigMode doesn't permit creation.", FullPath));

                CreateDefault(locBasePath, configName);
            }
            configDoc.Load(FullPath);
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
            if (configDoc == null)
                return null;
            return configDoc.SelectSingleNode(xpath);
        }

        private void CreateDefault(string locBasePath, string configName)
        {
            // TODO: This method
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
            set { this.mode = value; }
        }

        public enum ConfigModes
        {
            ReadOnly,
            CreateIfMissing
        }
    }
}

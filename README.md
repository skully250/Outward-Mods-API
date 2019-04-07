# Outward-Mods-API


## ConfigHelper Example
```csharp
public void Initialize()
{
	// Read config file
	ConfigHelper configHelper = new ConfigHelper(ConfigHelper.ConfigModes.CreateIfMissing, "FileNameHere.xml");
	configHelper.XMLDefaultConfig = "<config><baseSneakSpeed>0.7</baseSneakSpeed><stealthTrainingBonus>1.3</stealthTrainigBonus></config>";

	Debug.Log("Trying to load " + configHelper.FullPath);

	float baseSneakSpeed = configHelper.ReadFloat("/config/baseSneakSpeed");
	float stealthTrainingBonus = configHelper.ReadFloat("/config/stealthTrainingBonus");
}
```

`XMLDefaultConfig` is optional, if it and `ConfigModes.CreateIfMissing` is set then the API will automatically create the config file in "Outward\Config\<Filename>" and populate it with whatever string you provide.
# Outward-Mods-API

## User Installation
Download the API from the Nexus or Github Releases, unzip the file and place the .dll file into "Outward\Mods\". Mods will now be able to use functions provided.

## Developer Installation
Download the API from the Nexus or Github Releases, unzip the file and place it somewhere. Then add a Reference to it in Visual Studio. 
Add `using OModAPI` to the files you want to use the API in.

## ConfigHelper Example
```csharp
public void Initialize()
{
	// Read config file
	ConfigHelper configHelper = new ConfigHelper(ConfigHelper.ConfigModes.CreateIfMissing, "FileNameHere.xml");
	configHelper.XMLDefaultConfig = "<config><baseSneakSpeed>0.7</baseSneakSpeed><stealthTrainingBonus>1.3</stealthTrainingBonus></config>";

	Debug.Log("Trying to load " + configHelper.FullPath);

	float baseSneakSpeed = configHelper.ReadFloat("/config/baseSneakSpeed");
	float stealthTrainingBonus = configHelper.ReadFloat("/config/stealthTrainingBonus");
}
```

`XMLDefaultConfig` is optional, if it and `ConfigModes.CreateIfMissing` is set then the API will automatically create the config file in "Outward\Config\<Filename>" and populate it with whatever string you provide.

## ReflectionTools Example
```csharp
// These can be run once, for example in Initialize(), since their values don't change
// Variable
FieldInfo m_autoRun = ReflectionTools.GetField(typeof(LocalCharacterControl), "m_autoRun");
// Method
MethodInfo StopAutoRun = ReflectionTools.GetMethod(typeof(LocalCharacterControl), "StopAutoRun");

// Using the reflected values has to be done in a method where an instance to the class exists (in this example, self)
public void detectMovementInputs(On.LocalCharacterControl.orig_DetectMovementInputs orig, LocalCharacterControl self)
{
	// Reading variable
	if ((bool)m_autoRun.GetValue(self))
	
	// Setting variable
	m_autoRun.SetValue(self, false);
	
	// Calling a method with no parameters
	StopAutoRun.Invoke(self, null);
	
	// Calling a method with parameters
	StopAutoRun.Invoke(self, new object[] { param1, param2 ...});
}
```

## OLogger Example
```csharp

//[Check the Logger folder for documentation](../OModAPI/Logger)

```
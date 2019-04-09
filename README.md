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
	
	configHelper.WriteValue("/config/test", "write value 1");

	for(int i = 0; i < 10; ++i)
		configHelper.WriteValue("/config/loopValues/val_" + i, i.ToString());
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

//All of these functions are called from the static class OLogger

//Here is the main Log function:
OLogger.Log(object _obj, string _color = "ffffffff", string _panel = "Default")

//_obj is the object you want to display, it will be turned into a string in the log function.
//_color is the color that the text will be, which by default will be white.
//_panel is the panel that the text will be output to.

//Other functions:
//This isn't actually needed as .Log will create a panel, however, this give you control over the writeToDisk/enabledOnCreation
OLogger.CreateLog(Rect _rect, string _panel = "Default", bool _writeToDisk = true, bool _enabledOnCreation = true);

//_writeToDisk is whether you want to output any text in this panel to a file in "mods/Debug/'PanelName'.txt"
//treat writing to disk in an update loop the same as calling Debug.Log.

OLogger.SetUIPanelEnabled(string _panel, bool _enabled); //this will set the panel "_panel" to "enabled"
OLogger.SetPanelWriteToDisk(string _panel, bool _writeToDisk) //this will set writeToDisk to "_writeToDisk"

OLogger.ClearUIPanel(string _panel); //this will clear the text in the "_panel" panel
OLogger.DestroyUIPanel(string _panel); //this will destroy the "_panel" panel;

OLogger.Warning(object _obj, string _panel = "Default"); //this will output yellow text to the "_panel" panel
OLogger.Error(object _obj, string _panel = "Default"); //this will output red text to the "_panel" panel

//Example Turn Unity Debug Into OLogger Debug:
public void Update()
{
	
	//First setup "Default Unity Compiler" panel at location: (X:400,Y:400)
	//	size: (W:400,H:400) and have it log to file (mods/Debug/"PanelName".txt) and be enabled on start
	OLogger.CreateLog(new Rect(400, 400, 400, 400), "Default Unity Compiler", true, true);
	
	//If you want to also debug Unity's stack trace then call this
	OLogger.CreateLog(new Rect(400, 400, 400, 400), "Default Unity Stack Trace", true, true);

	//Add ignores to OLogger ignore list (will filter out from Unity's Debug calls)
	OLogger.ignoreList.AddToIgnore("Internal", "Failed to create agent"
								  , "is registered with more than one LODGroup"
								  , "No AudioManager"); 
	//ignores can also be removed by calling OLogger.ignoreList.RemoveFromIgnore()

	//Finally hook OLogger onto logMessageReceived to receive Unity's Debug calls
	Application.logMessageReceived += OLogger.DebugMethodHook;

}
```
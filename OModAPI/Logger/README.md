# Outward-Mods-API

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
//_enabledOnCreation is whether you want to have the debug panel show on creation

OLogger.SetUIPanelEnabled(string _panel, bool _enabled); //this will set the panel "_panel" to "enabled"
OLogger.SetPanelWriteToDisk(string _panel, bool _writeToDisk); //this will set writeToDisk to "_writeToDisk"

OLogger.ClearUIPanel(string _panel); //this will clear the text in the "_panel" panel
OLogger.DestroyUIPanel(string _panel); //this will destroy the "_panel" panel;

OLogger.Warning(object _obj, string _panel = "Default"); //this will output yellow text to the "_panel" panel
OLogger.Error(object _obj, string _panel = "Default"); //this will output red text to the "_panel" panel

OLogger.ignoreList.AddToIgnore(params string[] _toAdd); //this will add strings to ignore from Unity's debug
OLogger.ignoreList.RemoveFromIgnore(params string[] _toRemove); //this will remove strings ^

OLogger.maxSetupTextTime //this will only allow setting up text to take x milliseconds before returning
//default value is 3, lower this if game is stuttering whilst debug logging

//Example To Turn Unity Debug Into OLogger Debug:
public void Start()
{
	
	//First setup "Default Unity Compiler" panel at location: (X:400,Y:400)
	//	size: (W:400,H:400) and have it log to file (mods/Debug/"PanelName".txt) and be enabled on start
	OLogger.CreateLog(new Rect(400, 400, 400, 400), "Default Unity Compiler", true, true);

	//Add ignores to OLogger ignore list (will filter out from Unity's Debug calls)
	OLogger.ignoreList.AddToIgnore("Internal", "Failed to create agent"
						, "is registered with more than one LODGroup"
						, "No AudioManager"); 
	//ignores can also be removed by calling OLogger.ignoreList.RemoveFromIgnore()

	//Finally hook OLogger onto logMessageReceived to receive Unity's Debug calls
	Application.logMessageReceived += OLogger.DebugMethodHook;
	
	//Now you can just call Debug.Log and it will come through as a OLogger Log

}

//Example To Debug On Key Press F
public void Update()
{

	if (Input.GetKeyDown(KeyCode.F))
	{
		OLogger.Log("You Have Pressed F");
	}

}
```
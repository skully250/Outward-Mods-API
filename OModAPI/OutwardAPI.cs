using Partiality.Modloader;
using UnityEngine;

namespace OModAPI
{
    public class ModAPI : PartialityMod
    {
        public ModAPI()
        {
            this.ModID = "";
            this.Version = "0110";
            this.author = "Faedar, Elec0, JoshF67";
        }

        public static APILoad APILoader;

        public override void OnEnable()
        {
            base.OnEnable();
            APILoad.api = this;
            GameObject obj = new GameObject();
            APILoader = obj.AddComponent<APILoad>();
            APILoader.Initialise();

            //Used to set the parent of all debug boxes to obj
            OLogger.SetupObject(obj);
        }
    }
}

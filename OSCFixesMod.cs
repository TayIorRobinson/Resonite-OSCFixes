using System.Reflection;
using ResoniteModLoader;
using FrooxEngine;
using HarmonyLib;
using Rug.Osc;
#if DEBUG
using ResoniteHotReloadLib;
#endif

    
namespace OSCFixes;



public class OSCFixesMod : ResoniteMod {
    internal const string VERSION_CONSTANT = "1.0.0"; 
    public override string Name => "OSCFixes";
    public override string Author => "U-TaylorRobinson";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://robins.one";
    private const string harmonyId = "one.robins.OSCFixes";
    public override void OnEngineInit() {
        Msg("Initialising!");
        #if DEBUG
        HotReloader.RegisterForHotReload(this);
        #endif 
        Init();
    }


    static void OnHotReload(ResoniteMod mod)
    {
        Msg("Hot reloading!");
        Init();
    }


    private static void Init() {
        Msg("Init called");
        Harmony harmony = new Harmony(harmonyId);
        harmony.PatchAll();
    }
    
    static void BeforeHotReload() {
        Msg("Unloading!");
        Harmony harmony = new Harmony(harmonyId);
        harmony.UnpatchAll(harmonyId);
    }
    [HarmonyPatch]
    class OSCPatch {
        static MethodBase TargetMethod() {
            Type oscReciever = typeof(OSC_Receiver);
            Msg("OSC Reciever Class " + oscReciever);
            return oscReciever.GetMethod("ReceiveData", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        
        static bool Prefix(
            ref OSC_Receiver __instance,
            OscReceiver receiver
        ) {
            var atEntry = receiver.State;
            Msg("Entering OSC_Reciever#RecieveData with state " + atEntry + " on " +__instance.ReferenceID);
            while (receiver.State != OscSocketState.Connected) {
                Msg("Waiting for OSC Connected on " + __instance.ReferenceID);
                Thread.Sleep(10);
            }
            return true;
        }
        static void Postfix(ref OSC_Receiver __instance)
        {
            Msg("Leaving OSC_Reciever#RecieveData on " + __instance.ReferenceID);
        }
    }
}

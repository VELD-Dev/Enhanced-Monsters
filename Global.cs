global using System;
global using System.Collections;
global using System.Collections.Generic;
global using System.Configuration;
global using System.Diagnostics;
global using System.IO;
global using System.Linq;
global using System.Reflection;
global using System.Net;
global using System.Numerics;
global using System.Threading;
global using System.Text;
global using System.Runtime.Serialization.Formatters.Binary;
global using System.Xml;
global using System.Xml.Linq;

global using Newtonsoft;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Converters;
global using Newtonsoft.Json.Linq;
global using Newtonsoft.Json.Serialization;
global using Newtonsoft.Json.Utilities;

global using BepInEx;
global using BepInEx.Configuration;
global using BepInEx.Logging;
global using HarmonyLib;
global using HarmonyLib.Tools;

global using LethalConfig;
global using LethalConfig.ConfigItems;
global using LethalConfig.ConfigItems.Options;

global using EnhancedMonsters;
global using EnhancedMonsters.Config;
global using EnhancedMonsters.Monobehaviours;
global using EnhancedMonsters.Patches;
global using EnhancedMonsters.Utils;

global using StaticNetcodeLib;

global using Unity;
global using Unity.Collections;
global using Unity.Netcode;
global using Unity.Networking;
global using Unity.Networking.Transport;
global using Unity.Networking.Transport.Utilities;
global using UnityEngine;
global using UnityEngine.Networking;
global using UnityEngine.Networking.PlayerConnection;

global using Zeekerss;
global using Zeekerss.Core;
global using Zeekerss.Core.Singletons;

global using GameNetcodeStuff;
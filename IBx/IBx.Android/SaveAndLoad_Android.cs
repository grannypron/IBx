﻿using Android;
using Android.Content.Res;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Analytics;
using IBx.Droid;
using Newtonsoft.Json;
using Plugin.SimpleAudioPlayer;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveAndLoad_Android))]
namespace IBx.Droid
{
    public class SaveAndLoad_Android : ISaveAndLoad
    {
        public string TrackingId = "UA-60615839-5";
        private static GoogleAnalytics GAInstance;
        private static Tracker GATracker;
        public Context thisContext;
        int numOfTrackerEventHitsInThisSession = 0;
        public ISimpleAudioPlayer soundPlayer;
        public ISimpleAudioPlayer areaMusicPlayer;
        public ISimpleAudioPlayer areaAmbientSoundsPlayer;

        #region Instantiation ...
        private static SaveAndLoad_Android thisRef;
        public SaveAndLoad_Android()
        {
            // no code req'd
        }

        public static SaveAndLoad_Android GetGASInstance()
        {
            if (thisRef == null)
                // it's ok, we can call this constructor
                thisRef = new SaveAndLoad_Android();
            return thisRef;
        }
        #endregion

        public void Initialize_NativeGAS(Context AppContext = null)
        {
            thisContext = AppContext;
            GAInstance = GoogleAnalytics.GetInstance(AppContext.ApplicationContext);
            GAInstance.SetLocalDispatchPeriod(10);

            GATracker = GAInstance.NewTracker(TrackingId);
            GATracker.EnableExceptionReporting(true);
            GATracker.EnableAdvertisingIdCollection(true);
        }

        public string ConvertFullPath(string fullPath, string replaceWith)
        {
            string convertedFullPath = "";
            convertedFullPath = fullPath.Replace("\\", replaceWith);
            return convertedFullPath;
        }

        public bool AllowReadWriteExternal()
        {
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            {
                return true;
            }
            if (Android.App.Application.Context.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateUserFolders()
        {            
            if (AllowReadWriteExternal())
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                string convertedFullPath = sdCard.AbsolutePath + "/IBx/modules";
                string path = ConvertFullPath(convertedFullPath, "\\");
                Directory.CreateDirectory(path);
                convertedFullPath = sdCard.AbsolutePath + "/IBx/saves";
                path = ConvertFullPath(convertedFullPath, "\\");
                Directory.CreateDirectory(path);
                convertedFullPath = sdCard.AbsolutePath + "/IBx/user";
                path = ConvertFullPath(convertedFullPath, "\\");
                Directory.CreateDirectory(path);
            }
            else
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string convertedFullPath = documents + "/IBx/saves";
                string path = ConvertFullPath(convertedFullPath, "\\");
                Directory.CreateDirectory(path);
            }
        }

        public void SaveText(string fullPath, string text)
        {
            /*string storageFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string convertedFullPath = storageFolder + ConvertFullPath(fullPath, "/");
            string dir = Path.GetDirectoryName(convertedFullPath);
            Directory.CreateDirectory(dir);
            var path = ConvertFullPath(fullPath, "/");
            using (StreamWriter sw = File.CreateText(storageFolder + path))
            {
                sw.Write(text);
            }*/

            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (AllowReadWriteExternal())
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                root = sdCard.AbsolutePath;
            }
            //Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            string convertedFullPath = root + "/IBx" + ConvertFullPath(fullPath, "/");
            string path = ConvertFullPath(fullPath, "\\");
            string dir = Path.GetDirectoryName(convertedFullPath);
            Directory.CreateDirectory(dir);
            using (StreamWriter sw = File.CreateText(convertedFullPath))
            {
                sw.Write(text);
            }
        }
        
        public string LoadStringFromUserFolder(string fullPath)
        {
            string text = "";
            //check in app module folderr first
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.Droid.Assets" + ConvertFullPath(fullPath, "."));
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
                return text;
            }

            //check in user module folder next
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (AllowReadWriteExternal())
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                root = sdCard.AbsolutePath;
            }
            //Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            string filePath = root + "/IBx" + ConvertFullPath(fullPath, "/");
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return text;
        }
        public string LoadStringFromAssetFolder(string fullPath)
        {
            string text = "";
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.Droid.Assets" + ConvertFullPath(fullPath, "."));
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }
            return text;
        }
        public string LoadStringFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            string text = "";
            //check in module folder first
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (AllowReadWriteExternal())
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                root = sdCard.AbsolutePath;
            }
            string filePath = root + "/IBx" + ConvertFullPath(userFolderpath, "/");
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.Droid.Assets" + ConvertFullPath(assetFolderpath, "."));
            if (stream != null)
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }
            return text;
        }
        public string[] LoadStringLinesFromEitherFolder(string assetFolderpath, string userFolderpath)
        {
            string[] lines;
            //check in module folder first
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            string filePath = sdCard.AbsolutePath + "/IBx" + ConvertFullPath(userFolderpath, "/");
            if (File.Exists(filePath))
            {
                return File.ReadAllLines(filePath);
            }
            //check in Assests folder last
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("IBx.Droid.Assets" + ConvertFullPath(assetFolderpath, "."));
            using (var reader = new System.IO.StreamReader(stream))
            {
                List<string> linesArray = new List<string>();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    linesArray.Add(line);
                }
                lines = linesArray.ToArray();
            }
            return lines;
        }

        public string GetModuleFileString(string modFilename)
        {
            //asset module
            if (modFilename.StartsWith("IBx."))
            {
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(modFilename);
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            else
            {
                string modFolder = Path.GetFileNameWithoutExtension(modFilename);

                string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (AllowReadWriteExternal())
                {
                    Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                    root = sdCard.AbsolutePath;
                }
                string filePath = root + "/IBx/modules/" + modFolder + "/" + modFilename;
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }                
                //try asset area            
                string modFilenameNoExtension = modFilename.Replace(".mod", "");
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.modules." + modFilenameNoExtension + "." + modFilename);
                if (stream != null)
                {
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return "";
        }
                
        public SKBitmap LoadBitmap(string filename, Module mdl)
        {
            //SKBitmap bm = null;
            try
            {
                //MODULE'S GRAPHICS FOLDER
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                string documents = Path.Combine(sdCard.AbsolutePath, "IBx");

                //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var modulesDir = Path.Combine(documents, "modules");
                var modFolder = Path.Combine(modulesDir, mdl.moduleName);
                var modGraphicsFolder = Path.Combine(modFolder, "graphics");
                var filePath = Path.Combine(modGraphicsFolder, filename);

                if (File.Exists(filePath))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath);
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".png"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".png");
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".jpg"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".jpg");
                    if (bm != null) { return bm; }
                }
                modGraphicsFolder = Path.Combine(modFolder, "pctokens");
                filePath = Path.Combine(modGraphicsFolder, filename);
                if (File.Exists(filePath))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath);
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".png"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".png");
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".jpg"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".jpg");
                    if (bm != null) { return bm; }
                }
                modGraphicsFolder = Path.Combine(modFolder, "portraits");
                filePath = Path.Combine(modGraphicsFolder, filename);
                if (File.Exists(filePath))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath);
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".png"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".png");
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".jpg"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".jpg");
                    if (bm != null) { return bm; }
                }
                modGraphicsFolder = Path.Combine(modFolder, "tiles");
                filePath = Path.Combine(modGraphicsFolder, filename);
                if (File.Exists(filePath))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath);
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".png"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".png");
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".jpg"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".jpg");
                    if (bm != null) { return bm; }
                }
                modGraphicsFolder = Path.Combine(modFolder, "ui");
                filePath = Path.Combine(modGraphicsFolder, filename);
                if (File.Exists(filePath))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath);
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".png"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".png");
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".jpg"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".jpg");
                    if (bm != null) { return bm; }
                }




                //string storageFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                /*Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                string storageFolder = sdCard.AbsolutePath + "/IBx";
                if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + mdl.currentArea.sourceBitmapName + "/" + filename + ".png")))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + mdl.currentArea.sourceBitmapName + "/" + filename + ".png");
                }
                else if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + mdl.currentArea.sourceBitmapName + "/" + filename + ".PNG")))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + mdl.currentArea.sourceBitmapName + "/" + filename + ".PNG");
                }
                else if ((mdl.currentArea.sourceBitmapName != "") && (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + mdl.currentArea.sourceBitmapName + "/" + filename + ".jpg")))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + mdl.currentArea.sourceBitmapName + "/" + filename + ".jpg");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/tiles/" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/tiles/" + filename + ".png");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/tiles/" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/tiles/" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/tiles/" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/tiles/" + filename);
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename + ".png");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename + ".jpg"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename + ".jpg");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/graphics/" + filename);
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/ui/" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/ui/" + filename + ".png");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/ui/" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/ui/" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/ui/" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/ui/" + filename);
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/pctokens/" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/pctokens/" + filename + ".png");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/pctokens/" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/pctokens/" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/pctokens/" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/pctokens/" + filename);
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/portraits/" + filename + ".png"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/portraits/" + filename + ".png");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/portraits/" + filename + ".PNG"))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/portraits/" + filename + ".PNG");
                }
                else if (File.Exists(storageFolder + "/modules/" + mdl.moduleName + "/portraits/" + filename))
                {
                    bm = SKBitmap.Decode(storageFolder + "/modules/" + mdl.moduleName + "/portraits/" + filename);
                }
                //STOP here if already found bitmap
                if (bm != null)
                {
                    return bm;
                }*/



                //USER FOLDER
                documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var userDir = Path.Combine(documents, "user");
                filePath = Path.Combine(userDir, filename);

                if (File.Exists(filePath))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath);
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".png"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".png");
                    if (bm != null) { return bm; }
                }
                else if (File.Exists(filePath + ".jpg"))
                {
                    SKBitmap bm = SKBitmap.Decode(filePath + ".jpg");
                    if (bm != null) { return bm; }
                }



                //If not found then try in Asset folder
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.graphics." + filename);
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.graphics." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.graphics." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.tiles." + filename);
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.tiles." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.tiles." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.ui." + filename);
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.ui." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.ui." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.portraits." + filename);
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.portraits." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.portraits." + filename + ".PNG");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.portraits." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.pctokens." + filename);
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.pctokens." + filename + ".png");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.pctokens." + filename + ".PNG");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.pctokens." + filename + ".jpg");
                }
                if (stream == null)
                {
                    stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.graphics.ui_missingtexture.png");
                }
                SKManagedStream skStream = new SKManagedStream(stream);
                return SKBitmap.Decode(skStream);
            }
            catch (Exception ex)
            {
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.graphics.ui_missingtexture.png");
                SKManagedStream skStream = new SKManagedStream(stream);
                return SKBitmap.Decode(skStream);
            }
        }
        
        public List<string> GetAllFilesWithExtensionFromUserFolder(string folderpath, string extension)
        {
            List<string> list = new List<string>();
            //FROM ASSETS
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains(ConvertFullPath(folderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2]);
                }
            }

            //search in external folder
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (AllowReadWriteExternal())
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                root = sdCard.AbsolutePath;
            }
            //Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File directory = new Java.IO.File(root + "/IBx" + ConvertFullPath(folderpath, "/"));
            //directory.Mkdirs();
            if (directory.Exists())
            {
                foreach (Java.IO.File f in directory.ListFiles())
                {
                    if (f.Name.EndsWith(extension))
                    {
                        string[] split = f.Name.Split('.');
                        list.Add(split[split.Length - 2]);
                    }
                }
            }
            return list;
        }
        public List<string> GetAllFilesWithExtensionFromAssetFolder(string folderpath, string extension)
        {
            List<string> list = new List<string>();
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains(ConvertFullPath(folderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2]);
                }
            }
            return list;
        }
        public List<string> GetAllFilesWithExtensionFromBothFolders(string assetFolderpath, string userFolderpath, string extension)
        {
            List<string> list = new List<string>();
            string uppercase = extension.ToUpper();
            string lowercase = extension.ToLower();

            //search in external folder
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (AllowReadWriteExternal())
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                root = sdCard.AbsolutePath;
            }
            //Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File directory = new Java.IO.File(root + "/IBx" + ConvertFullPath(userFolderpath, "/"));
            //directory.Mkdirs();
            if (directory.Exists())
            {
                foreach (Java.IO.File f in directory.ListFiles())
                {
                    if (f.Name.EndsWith(extension))
                    {
                        string[] split = f.Name.Split('.');
                        if (!list.Contains(split[split.Length - 2] + "." + split[split.Length - 1]))
                        {
                            list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                        }
                    }
                    else if (f.Name.EndsWith(uppercase))
                    {
                        string[] split = f.Name.Split('.');
                        if (!list.Contains(split[split.Length - 2] + "." + split[split.Length - 1]))
                        {
                            list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                        }
                    }
                    else if (f.Name.EndsWith(lowercase))
                    {
                        string[] split = f.Name.Split('.');
                        if (!list.Contains(split[split.Length - 2] + "." + split[split.Length - 1]))
                        {
                            list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                        }
                    }
                }
            }
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            //module folder in app 
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains("IBx.Droid.Assets" + ConvertFullPath(userFolderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                else if ((res.Contains("IBx.Droid.Assets" + ConvertFullPath(userFolderpath, "."))) && (res.EndsWith(uppercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                else if ((res.Contains("IBx.Droid.Assets" + ConvertFullPath(userFolderpath, "."))) && (res.EndsWith(lowercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
            }
            //from main asset folder
            foreach (var res in assembly.GetManifestResourceNames())
            {
                if ((res.Contains("IBx.Droid.Assets" + ConvertFullPath(assetFolderpath, "."))) && (res.EndsWith(extension)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                if ((res.Contains("IBx.Droid.Assets" + ConvertFullPath(assetFolderpath, "."))) && (res.EndsWith(uppercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
                if ((res.Contains("IBx.Droid.Assets" + ConvertFullPath(assetFolderpath, "."))) && (res.EndsWith(lowercase)))
                {
                    string[] split = res.Split('.');
                    list.Add(split[split.Length - 2] + "." + split[split.Length - 1]);
                }
            }
            return list;
        }
        public List<string> GetAllModuleFiles(bool userOnly)
        {
            List<string> list = new List<string>();

            //search in assets
            if (!userOnly)
            {
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    if ((res.EndsWith(".mod")) && (!res.EndsWith("NewModule.mod")))
                    {
                        list.Add(res);
                    }
                }
            }
                        
            //search in external folder
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (AllowReadWriteExternal())
            {
                Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
                root = sdCard.AbsolutePath;
            }
            //Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            Java.IO.File directory = new Java.IO.File(root + "/IBbasic/modules");
            if (directory.Exists())
            {
                foreach (Java.IO.File d in directory.ListFiles())
                {
                    if (d.IsDirectory)
                    {
                        Java.IO.File modDirectory = new Java.IO.File(directory.Path + "/" + d.Name);
                        foreach (Java.IO.File f in modDirectory.ListFiles())
                        {
                            try
                            {
                                if (f.Name.EndsWith(".mod"))
                                {
                                    list.Add(f.Name);
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }

            return list;
        }

        public void TrackAppEvent(string Category, string EventAction, string EventLabel)
        {
            try
            {
                if (numOfTrackerEventHitsInThisSession > 300)
                {
                    GATracker.Send(new HitBuilders.EventBuilder().SetNewSession().Build());
                    numOfTrackerEventHitsInThisSession = 0;
                }
                else
                {
                    numOfTrackerEventHitsInThisSession++;
                }
                HitBuilders.EventBuilder builder = new HitBuilders.EventBuilder();
                builder.SetCategory("An_" + Category);
                builder.SetAction("An_" + EventAction);
                builder.SetLabel("An_" + EventLabel);
                GATracker.Send(builder.Build());
            }
            catch { }
        }


        string CreatePathToFile(string filename)
        {
            var docsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(docsPath, filename);
        }
        public string GetFileNameFromResource(string res)
        {
            string filename = "";
            List<string> parts = res.Split('.').ToList();
            filename = parts[parts.Count - 2] + "." + parts[parts.Count - 1];
            return filename;
        }

        Stream GetStreamFromFile(GameView gv, string filename)
        {
            //MODULE'S GRAPHICS FOLDER
            Java.IO.File sdCard = Android.OS.Environment.ExternalStorageDirectory;
            string documents = Path.Combine(sdCard.AbsolutePath, "IBx");

            //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var modulesDir = Path.Combine(documents, "modules");
            var modFolder = Path.Combine(modulesDir, gv.mod.moduleName);
            var modSoundFolder = Path.Combine(modFolder, "sounds");
            var filePath = Path.Combine(modSoundFolder, filename);

            if (File.Exists(filePath))
            {
                return File.OpenRead(filePath);
            }
            else if (File.Exists(filePath + ".wav"))
            {
                return File.OpenRead(filePath + ".wav");
            }
            else if (File.Exists(filePath + ".mp3"))
            {
                return File.OpenRead(filePath + ".mp3");
            }
            modSoundFolder = Path.Combine(modFolder, "music");
            filePath = Path.Combine(modSoundFolder, filename);
            if (File.Exists(filePath))
            {
                return File.OpenRead(filePath);
            }
            else if (File.Exists(filePath + ".wav"))
            {
                return File.OpenRead(filePath + ".wav");
            }
            else if (File.Exists(filePath + ".mp3"))
            {
                return File.OpenRead(filePath + ".mp3");
            }

            //DEFAULT ASSETS
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.modules." + gv.mod.moduleName + ".sounds." + filename);
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.modules." + gv.mod.moduleName + ".sounds." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.modules." + gv.mod.moduleName + ".sounds." + filename + ".mp3");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.modules." + gv.mod.moduleName + ".music." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.modules." + gv.mod.moduleName + ".music." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.modules." + gv.mod.moduleName + ".music." + filename + ".mp3");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.sounds." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.sounds." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.sounds." + filename + ".mp3");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.music." + filename);
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.music." + filename + ".wav");
            }
            if (stream == null)
            {
                stream = assembly.GetManifestResourceStream("IBx.Droid.Assets.music." + filename + ".mp3");
            }
            return stream;
        }
        public void PlaySound(GameView gv, string filenameNoExtension)
        {
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!gv.mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                if (soundPlayer == null)
                {
                    soundPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                }
                try
                {
                    soundPlayer.Loop = false;
                    soundPlayer.Load(GetStreamFromFile(gv, filenameNoExtension));
                    soundPlayer.Play();
                }
                catch (Exception ex)
                {
                    if (gv.mod.debugMode) //SD_20131102
                    {
                        gv.cc.addLogText("<yl>failed to play sound" + filenameNoExtension + "</yl><BR>");
                    }
                }
            }
        }
        public void PlayAreaMusic(GameView gv, string filenameNoExtension)
        {
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!gv.mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                if (areaMusicPlayer == null)
                {
                    areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                    areaMusicPlayer.PlaybackEnded += AreaMusicPlayer_PlaybackEnded;
                }
                if (!areaMusicPlayer.IsPlaying)
                {
                    try
                    {
                        areaMusicPlayer.Loop = true;
                        areaMusicPlayer.Load(GetStreamFromFile(gv, filenameNoExtension));
                        areaMusicPlayer.Play();
                    }
                    catch (Exception ex)
                    {
                        if (gv.mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<yl>failed to play area music" + filenameNoExtension + "</yl><BR>");
                        }
                    }
                }
            }
        }

        private void AreaMusicPlayer_PlaybackEnded(object sender, EventArgs e)
        {
            RestartAreaMusicIfEnded();
        }

        public void PlayAreaAmbientSounds(GameView gv, string filenameNoExtension)
        {
            if ((filenameNoExtension.Equals("none")) || (filenameNoExtension.Equals("")) || (!gv.mod.playSoundFx))
            {
                //play nothing
                return;
            }
            else
            {
                if (areaAmbientSoundsPlayer == null)
                {
                    areaAmbientSoundsPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                }
                if (!areaAmbientSoundsPlayer.IsPlaying)
                {
                    try
                    {
                        areaAmbientSoundsPlayer.Loop = true;
                        areaAmbientSoundsPlayer.Load(GetStreamFromFile(gv, filenameNoExtension));
                        areaAmbientSoundsPlayer.Play();
                    }
                    catch (Exception ex)
                    {
                        if (gv.mod.debugMode) //SD_20131102
                        {
                            gv.cc.addLogText("<yl>failed to play area ambient sounds" + filenameNoExtension + "</yl><BR>");
                        }
                    }
                }
            }
        }
        public void RestartAreaMusicIfEnded(GameView gv)
        {
            //restart area music
            if (areaMusicPlayer == null)
            {
                areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if ((!areaMusicPlayer.IsPlaying) && (gv.mod.playSoundFx))
                {
                    try
                    {
                        areaMusicPlayer.Play();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            //restart area ambient sounds
            if (areaAmbientSoundsPlayer == null)
            {
                areaAmbientSoundsPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if ((!areaAmbientSoundsPlayer.IsPlaying) && (gv.mod.playSoundFx))
                {
                    try
                    {
                        areaAmbientSoundsPlayer.Play();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void RestartAreaMusicIfEnded()
        {
            //restart area music
            if (areaMusicPlayer == null)
            {
                areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if (!areaMusicPlayer.IsPlaying)
                {
                    try
                    {
                        areaMusicPlayer.Play();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }

            //restart area ambient sounds
            if (areaAmbientSoundsPlayer == null)
            {
                areaAmbientSoundsPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if (!areaAmbientSoundsPlayer.IsPlaying)
                {
                    try
                    {
                        areaAmbientSoundsPlayer.Play();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void StopAreaMusic()
        {
            if (areaMusicPlayer == null)
            {
                areaMusicPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            }
            try
            {
                if (areaMusicPlayer.IsPlaying)
                {
                    areaMusicPlayer.Stop();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void PauseAreaMusic()
        {
            //playerAreaMusic.Pause();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueQuark;

namespace BuddyApp.AutomatedTest
{
    public sealed class ModuleManager
    {
        /*
        **  All modules available for test.
        */
        public enum MODULES
        {
            E_MOTION,
            E_CAMERA,
            E_VOCAL,
            //E_GUI,
            E_NB_MODULE,
        }

        public Dictionary<MODULES, AModuleTest> Modules = null;
        public string FileLog;

        // Singleton design pattern
        private static readonly ModuleManager mInstance = new ModuleManager();

        // Singleton design pattern
        static ModuleManager()
        {
        }

        // Singleton design pattern
        private ModuleManager()
        {
        }

        // Singleton design pattern
        public static ModuleManager GetInstance()
        {
            return mInstance;
        }

        public void Initialize()
        {
            // Create the dictionary
            Modules = new Dictionary<MODULES, AModuleTest>();
        }

        public void ForeachModulesDo(System.Action<KeyValuePair<ModuleManager.MODULES, AModuleTest>> iFunction)
        {
            if (Modules == null || (Modules != null & Modules.Count == 0))
                return;
            foreach (KeyValuePair<ModuleManager.MODULES, AModuleTest> lModule in Modules)
            {
                iFunction(lModule);
            }
        }

        public void ClearModulesData(bool iClearSelectedTest = true)
        {
            foreach (KeyValuePair<ModuleManager.MODULES, AModuleTest> lModule in Modules)
            {
                lModule.Value.Mode = AModuleTest.TestMode.M_MANUAL;
                if (iClearSelectedTest)
                    lModule.Value.ClearSelectedTest();
                lModule.Value.ClearError();
            }
        }
    }
}

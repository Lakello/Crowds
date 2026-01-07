namespace _GameResources.Scripts.Tools
{
    using System;
    using Editor;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class EnumConfigurator
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _namespace;
        [SerializeField]
        private string[] _members;

        [Button]
        private void Generate()
        {
            EnumGenerator.GenerateEnum(_members, _name, _namespace);
        }
    }
}
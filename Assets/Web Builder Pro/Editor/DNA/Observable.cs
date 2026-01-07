using System;
using UnityEditor;
using UnityEngine;

namespace Anvil.WebBuilderPro
{
    internal abstract class ObservableBase
    {
        public abstract void ForceNotify();
        public abstract void UnBind();
    }

    internal abstract class Observable<T> : ObservableBase
    {
        public abstract event Action<T> OnValueChanged;
        public abstract void ForceNotify(T newValue);
    }

    [Serializable]
    internal class ObservableBool : Observable<bool>
    {
        public ObservableBool(bool value = default)
        {
            _value = value;
        }
        
        [SerializeField]
        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                if (value.Equals(_value)) return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
        
        public override event Action<bool> OnValueChanged;
        
        // Notifies all subscribers regardless of the value
        public override void ForceNotify(bool newValue)
        {
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }
        
        public override void ForceNotify()
        {
            OnValueChanged?.Invoke(_value);
        }

        public override void UnBind()
        {
            OnValueChanged = null;
        }
    }

    [CustomPropertyDrawer(typeof(ObservableBool))]
    internal sealed class ObservableBoolDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var value = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(position, value, label);
            EditorGUI.EndProperty();
        }
    }
    
    
    [Serializable]
    internal sealed class ObservableString : Observable<string>
    {
        public ObservableString(string value = default)
        {
            _value = value;
        }
        
        [SerializeField]
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (value.Equals(_value)) return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
        public override event Action<string> OnValueChanged;
        
        // Notifies all subscribers regardless of the value
        public override void ForceNotify(string newValue)
        {
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }
        
        public override void ForceNotify()
        {
            OnValueChanged?.Invoke(_value);
        }

        public override void UnBind()
        {
            OnValueChanged = null;
        }
    }
    
    [CustomPropertyDrawer(typeof(ObservableString))]
    internal sealed class ObservableStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var value = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(position, value, label);
            EditorGUI.EndProperty();
        }
    }

    
    [Serializable]
    internal sealed class ObservableInt : Observable<int>
    {
        public ObservableInt(int value = default)
        {
            _value = value;
        }
        
        [SerializeField]
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }
        public override event Action<int> OnValueChanged;
        
        // Notifies all subscribers regardless of the value
        public override void ForceNotify(int newValue)
        {
            _value = newValue;
            OnValueChanged?.Invoke(_value);
        }
        
        public override void ForceNotify()
        {
            OnValueChanged?.Invoke(_value);
        }

        public override void UnBind()
        {
            OnValueChanged = null;
        }
    }
    
    [CustomPropertyDrawer(typeof(ObservableInt))]
    internal sealed class ObservableIntDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var value = property.FindPropertyRelative("_value");
            EditorGUI.PropertyField(position, value, label);
            EditorGUI.EndProperty();
        }
    }
    
}
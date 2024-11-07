using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JG.UPM.PropertyRefs.Editor
{
    /// <summary>
    /// Drawer for the PropertyRef Class.
    /// </summary>
    /// <seealso cref="JG.UPM.PropertyRefs.PropertyRef"/>
    [CustomPropertyDrawer(typeof(PropertyRef))]
    internal class PropertyRefDrawer : PropertyDrawer
    {
        private readonly PropertyRef _reference = new PropertyRef();

        private Component[] _components;

        private PropertyInfo[] _propertiesInfo;

        private readonly List<string> _dropdownValues = new List<string>();

        private readonly List<string> _dropdownDisplay = new List<string>();
        

        /// <inheritdoc cref="PropertyDrawer.OnGUI"/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            //  Getting the properties references.
            var componentProperty = property.FindPropertyRelative("component");
            var namePropProperty = property.FindPropertyRelative("property");

            _reference.SetComponent(componentProperty.objectReferenceValue);
            _reference.SetProperty(namePropProperty.stringValue);

            if (_reference.Component == null)
            {
                SelectObject(position);
                StoreData(property);
                return;
            }

            if (_reference.Component as GameObject)
            {
                SelectComponent(position);
                StoreData(property);
                return;
            }

            if (PropertyRef.Registry.ContainsComponent(_reference.Component))
            {
                if (string.IsNullOrEmpty(_reference.Property))
                {
                    SelectProperty(position);
                    StoreData(property);
                }
                else if (!PropertyRef.Registry.ContainsProperty(_reference.Component, _reference.Property))
                {
                    RegistryProperty(position);
                    StoreData(property);
                }
                else
                {
                    DrawReference(position, property);
                    StoreData(property);
                }
            }
            else
            {
                RegistryComponent(position);
                StoreData(property);
            }

        }

        private void SelectObject(Rect position)
        {
            // Draw Dropdown.
            EditorGUI.BeginChangeCheck();
            var objectAttempt = EditorGUI.ObjectField(
                position: position,
                label: "Select Object",
                obj: _reference.Component,
                objType: typeof(Object),
                allowSceneObjects: true
            );
            if (!EditorGUI.EndChangeCheck()) return;

            // Set Object.
            _reference.SetComponent(objectAttempt);
        }

        private void SelectComponent(Rect position)
        {
            // Set up dropdown.
            _components = (_reference.Component as GameObject)?.GetComponents<Component>();

            // Fill Dropdown.
            _dropdownDisplay.Clear();
            _dropdownDisplay.Add("Select Component...");
            if (_components != null)
            {
                foreach (var component in _components)
                {
                    _dropdownDisplay.Add(ObjectNames.GetClassName(component));
                }
            }

            _dropdownDisplay.Add("Remove Reference...");

            // Draw Dropdown.
            var componentIndex = 0;
            EditorGUI.BeginChangeCheck();
            componentIndex = EditorGUI.Popup(
                position,
                "Component",
                componentIndex,
                _dropdownDisplay.ToArray()
            );
            if (!EditorGUI.EndChangeCheck()) return;

            // Delete Option.
            if (componentIndex == _dropdownDisplay.Count - 1)
            {
                _reference.SetComponent(null);
                return;
            }

            // Select Component.
            if (componentIndex == 0 || _components == null) return;
            _reference.SetComponent(_components[componentIndex - 1]);
        }

        private void SelectProperty(Rect position)
        {
            // Dropdown Setup
            _propertiesInfo = ReflectionUtils.GetAvailableProperties(_reference.Component);

            // Fill Dropdown.
            _dropdownValues.Clear();
            _dropdownDisplay.Clear();
            _dropdownDisplay.Add("Select Property...");
            foreach (var propertyInfo in _propertiesInfo)
            {
                if (!ReflectionUtils.IsSupportedProperty(propertyInfo)) { continue; }

                _dropdownValues.Add(propertyInfo.Name);
                _dropdownDisplay.Add(ObjectNames.NicifyVariableName(propertyInfo.Name));
            }

            _dropdownDisplay.Add("Remove Reference...");

            // Draw Dropdown.
            EditorGUI.BeginChangeCheck();
            var propertyIndex = EditorGUI.Popup(
                position,
                "Property",
                selectedIndex: 0,
                _dropdownDisplay.ToArray()
            );
            if (!EditorGUI.EndChangeCheck()) return;

            // Delete Option.
            if (propertyIndex == _dropdownDisplay.Count - 1)
            {
                _reference.SetComponent(null);
                return;
            }

            // Set Property.
            _reference.SetProperty(
                propertyIndex != 0
                    ? _dropdownValues[propertyIndex - 1]
                    : string.Empty
            );
        }

        private void RegistryProperty(Rect position)
        {
            var propertyName = _reference.Property;
            
            var rectMessage = position;
            rectMessage.width = EditorGUIUtility.labelWidth;
            var rectButtonCancel = position;
            rectButtonCancel.width =
                (position.width - rectMessage.width) * 0.5f;
            rectButtonCancel.x = position.x + rectMessage.width;
            var rectButtonOk = rectButtonCancel;
            rectButtonOk.x += rectButtonCancel.width;
            rectMessage.width -= 5;

            EditorGUI.LabelField(
                rectMessage,
                new GUIContent(
                    "Property Not Found!",
                    // ReSharper disable once StringLiteralTypo
                    EditorGUIUtility.IconContent("console.warnicon.sml").image,
                    $"The Property {propertyName} is not registered yet."
                )
            );

            EditorGUI.BeginChangeCheck();
            GUI.Button(rectButtonCancel, "Cancel");
            if (EditorGUI.EndChangeCheck())
            {
                _reference.SetProperty(string.Empty);
            }

            EditorGUI.BeginChangeCheck();
            GUI.Button(rectButtonOk, "Register");
            if (EditorGUI.EndChangeCheck())
            {
                var type = _reference.Component.GetType();
                var propertyInfo = type.GetProperty(propertyName);
                RegistryUtils.RegisterProperty(_reference.Component.GetType(), propertyInfo);
            }
        }
        
        private void RegistryComponent(Rect position)
        {
            var type = _reference.Component.GetType();
            
            var rectMessage = position;
            rectMessage.width = EditorGUIUtility.labelWidth;
            var rectButtonCancel = position;
            rectButtonCancel.width =
                (position.width - rectMessage.width) * 0.5f;
            rectButtonCancel.x = position.x + rectMessage.width;
            var rectButtonOk = rectButtonCancel;
            rectButtonOk.x += rectButtonCancel.width;
            rectMessage.width -= 5;

            EditorGUI.LabelField(
                rectMessage,
                new GUIContent(
                    "Component Not Found!",
                    // ReSharper disable once StringLiteralTypo
                    EditorGUIUtility.IconContent("console.warnicon.sml").image,
                    $"The Object {type.Name} is not registered yet."
                )
            );

            EditorGUI.BeginChangeCheck();
            GUI.Button(rectButtonCancel, "Cancel");
            if (EditorGUI.EndChangeCheck())
            {
                _reference.SetComponent(null);
            }

            EditorGUI.BeginChangeCheck();
            GUI.Button(rectButtonOk, "Register");
            if (EditorGUI.EndChangeCheck())
            {
                RegistryUtils.RegisterComponent(_reference.Component.GetType());
            }
        }


        private void DrawReference(Rect position, SerializedProperty reference)
        {
            var propertyPosition = position;
            propertyPosition.width -= 30;
            var rectButtonRemove = position;
            rectButtonRemove.width = 25;
            rectButtonRemove.x = position.x + propertyPosition.width + 5;

            if (GUI.Button(
                    rectButtonRemove,
                    new GUIContent(
                        EditorGUIUtility.IconContent("d_TreeEditor.Trash").image,
                        "Reset this reference."
                    )
                ))
            {
                _reference.SetComponent(null);
                return;
            }

            var referenceName = reference.name;
            var isArray = reference.propertyPath.Contains("Array.data");
            string nameLabel;

            if (!isArray)
            {
                nameLabel = ObjectNames.NicifyVariableName(referenceName);
            }

            else
            {
                var owner = GetGameObjectFromComponent(_reference.Component);
                var ownerLabel = string.Empty;
                if ((object)owner != null)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    ownerLabel = owner.name + " : ";
                }

                nameLabel = ownerLabel + ObjectNames.NicifyVariableName(_reference.Property);
            }
            
            var label = new GUIContent(nameLabel);

            EditorGUI.BeginChangeCheck();

            var type = _reference.GetPropertyType();
            if (type == null)
            {
                EditorGUI.LabelField(
                    position,
                    label,
                    new GUIContent("Not Supported Yet")
                );
                return;
            }

            if (type == typeof(bool))
            {
                _reference.SetValue(
                    EditorGUI.Toggle(
                        propertyPosition,
                        label,
                        (bool)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(int))
            {
                _reference.SetValue(
                    EditorGUI.IntField(
                        propertyPosition,
                        label,
                        (int)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(float))
            {
                _reference.SetValue(
                    EditorGUI.FloatField(
                        propertyPosition,
                        label,
                        (float)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(double))
            {
                _reference.SetValue(
                    EditorGUI.DoubleField(
                        propertyPosition,
                        label,
                        (double)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(Vector2))
            {
                _reference.SetValue(
                    EditorGUI.Vector2Field(
                        propertyPosition,
                        label,
                        (Vector2)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(Vector3))
            {
                _reference.SetValue(
                    EditorGUI.Vector3Field(
                        propertyPosition,
                        label,
                        (Vector3)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(Vector4))
            {
                _reference.SetValue(
                    EditorGUI.Vector4Field(
                        propertyPosition,
                        label,
                        (Vector4)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(Quaternion))
            {
                var quaternion = (Quaternion)_reference.GetValue();
                var aux = EditorGUI.Vector4Field(
                    propertyPosition,
                    label,
                    new Vector4(
                        quaternion.x, 
                        quaternion.y, 
                        quaternion.z, 
                        quaternion.w
                    )
                );
                _reference.SetValue(
                    new Quaternion(aux.x, aux.y, aux.z, aux.w)
                );
            }

            if (type == typeof(Color))
            {
                _reference.SetValue(
                    EditorGUI.ColorField(
                        propertyPosition,
                        label,
                        (Color)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(string))
            {
                _reference.SetValue(
                    EditorGUI.TextField(
                        propertyPosition,
                        label,
                        (string)_reference.GetValue()
                    )
                );
            }

            if (type == typeof(AnimationCurve))
            {
                _reference.SetValue(
                    EditorGUI.CurveField(
                        propertyPosition,
                        label,
                        _reference.GetValue() as AnimationCurve
                    )
                );
            }

            if (type == typeof(Object))
            {
                var component = _reference.GetValue() as Object;
                _reference.SetValue(
                    EditorGUI.ObjectField(
                        propertyPosition,
                        label,
                        component,
                        component != null
                            ? component.GetType()
                            : typeof(Object),
                        true
                    )
                );
            }

            if (type.IsSubclassOf(typeof(Object)))
            {
                _reference.SetValue(
                    EditorGUI.ObjectField(
                        propertyPosition,
                        label,
                        _reference.GetValue() as Object,
                        type,
                        true
                    )
                );
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_reference.Component);
            }
        }

        private void StoreData(SerializedProperty property)
        {
            property.FindPropertyRelative("component").objectReferenceValue =
                _reference.Component;
            property.FindPropertyRelative("property").stringValue =
                _reference.Property;
        }

        private GameObject GetGameObjectFromComponent(Object component)
        {
            if (component == null) return null;
            var unityComponent = _reference.Component as Component;
            return (object)unityComponent == null ? null : unityComponent.gameObject;
        }
    }
}
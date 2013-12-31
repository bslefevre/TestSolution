using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    /// <summary>
    /// Source: http://stackoverflow.com/questions/1398109/multiple-bindingsource-components-necessary-for-just-one-data-source
    /// </summary>
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            var customDataBinding = new CustomDataBinding();
            customDataBinding.SetValues("Prop1_DING", "Prop2_DING");
            var propertyDescriptorCollection = customDataBinding.GetProperties();

            foreach (CustomDataBinding.MyPropertyDescriptor myPropertyDescriptor in propertyDescriptorCollection)
            {
                myPropertyDescriptor.SetValue(customDataBinding, "Test2");
            }

            bindingSource1.DataSource = customDataBinding;
        }
    }

    public class CustomDataBinding : Component, ICustomTypeDescriptor, INotifyPropertyChanged
    {
        private String _Property1;
        private String _Property2;

        public class MyPropertyDescriptor : PropertyDescriptor
        {
            private String _Name;

            public MyPropertyDescriptor(String name)
                : base(name, null)
            {
                _Name = name;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return typeof(CustomDataBinding); }
            }

            public override object GetValue(object component)
            {
                var source = (CustomDataBinding)component;
                switch (_Name)
                {
                    case "Property1":
                        return source._Property1;
                        break;

                    case "Property2":
                        return source._Property2;
                        break;

                    default:
                        return null;
                }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return typeof(String); }
            }

            public override void ResetValue(object component)
            {
                SetValue(component, _Name);
            }

            public override void SetValue(object component, object value)
            {
                var source = (CustomDataBinding)component;
                switch (_Name)
                {
                    case "Property1":
                        source._Property1 = Convert.ToString(value);
                        Debug.WriteLine("Property1 changed to " + value);
                        break;

                    case "Property2":
                        source._Property2 = Convert.ToString(value);
                        Debug.WriteLine("Property2 changed to " + value);
                        break;

                    default:
                        return;
                }
                source.OnPropertyChanged(_Name);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }

        public CustomDataBinding()
        {
            _Property1 = "Property1";
            _Property2 = "Property2";
        }

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return new AttributeCollection(null);
        }

        public string GetClassName()
        {
            return null;
        }

        public string GetComponentName()
        {
            return null;
        }

        public TypeConverter GetConverter()
        {
            return null;
        }

        public EventDescriptor GetDefaultEvent()
        {
            return null;
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public object GetEditor(Type editorBaseType)
        {
            return null;
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return new EventDescriptorCollection(null);
        }

        public EventDescriptorCollection GetEvents()
        {
            return new EventDescriptorCollection(null);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(new PropertyDescriptor[] {
            new MyPropertyDescriptor("Property1"),
            new MyPropertyDescriptor("Property2"),
            new MyPropertyDescriptor("Test"), });
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return GetProperties(null);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void SetValues(String p1, String p2)
        {
            _Property1 = p1;
            _Property2 = p2;

            OnPropertyChanged("Property1");
            OnPropertyChanged("Property2");
        }

        #endregion
    }
}

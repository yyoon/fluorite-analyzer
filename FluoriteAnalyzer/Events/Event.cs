using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace FluoriteAnalyzer.Events
{
    public enum EventType
    {
        Command,
        DocumentChange,
        Annotation,
        Dummy
    }

    public abstract class Event
    {
        protected Dictionary<string, string> _dict;
        protected XmlElement _xmlElement;

        public Event(int timestamp)
        {
            Timestamp = timestamp;
        }

        public Event(XmlElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("commandElement is null");
            }

            _xmlElement = element;

            FillInDictionary();

            ID = int.Parse(GetPropertyValueFromDict("__id", false, "-1"));
            TypeString = GetPropertyValueFromDict("_type");
            Timestamp = long.Parse(GetPropertyValueFromDict("timestamp"));
            RepeatCount = int.Parse(GetPropertyValueFromDict("repeat", false, "1"));
        }

        public int ID { get; internal set; }
        public string TypeString { get; private set; }
        public long Timestamp { get; private set; }
        public int RepeatCount { get; private set; }

        public abstract EventType EventType { get; }

        public virtual string TypeOrCommandString
        {
            get { return TypeString; }
        }

        public virtual string ParameterStringPlain
        {
            get { return "{" + string.Join(", ", _dict.Select(x => x.Key + "=\"" + x.Value + "\"")) + "}"; }
        }

        public virtual string ParameterStringComplex
        {
            get
            {
                return string.Join(Environment.NewLine, _dict.Select(x => "[" + x.Key + "] = " + GetParameterValue(x)));
            }
        }

        private string GetParameterValue(KeyValuePair<string, string> pair)
        {
            switch (pair.Key)
            {
                case "text":
                case "snapshot":
                case "insertedText":
                case "deletedText":
                    return Environment.NewLine + pair.Value;

                default:
                    return pair.Value;
            }
        }

        public static Event CreateEventFromXmlElement(XmlElement element)
        {
            string typeString = (element.Name == "Command" || element.Name == "DocumentChange")
                                    ? element.Attributes["_type"].Value
                                    : element.Name;

            // For backward compatibility
            if (typeString.StartsWith("Macro"))
            {
                typeString = typeString.Substring(5);
            }

            Type type = Type.GetType("FluoriteAnalyzer.Events." + typeString);

            try
            {
                if (!type.IsSubclassOf(typeof (Event)))
                {
                    throw new Exception("class \"" + type.Name + "\" is not a subclass of class \"Event\"");
                }

                ConstructorInfo cinfo = type.GetConstructor(new[] {typeof (XmlElement)});
                return (Event) (cinfo.Invoke(new object[] {element}));
            }
            catch (Exception e)
            {
                throw new Exception("Exception thrown while creating an event object for\n" + element.OuterXml, e);
            }
        }

        public bool ContainsString(string str)
        {
            // If the class name contains the string, return true
            if (GetType().Name.Contains(str))
            {
                return true;
            }

            // If any of the key-value pairs contains the string, return true
            foreach (var kvp in _dict)
            {
                if (kvp.Key.Contains(str) || kvp.Value.Contains(str))
                {
                    return true;
                }
            }

            return false;
        }

        private void FillInDictionary()
        {
            _dict = new Dictionary<string, string>();

            foreach (XmlAttribute attr in _xmlElement.Attributes)
            {
                _dict.Add(attr.Name, attr.Value);
            }

            foreach (XmlElement child in _xmlElement.ChildNodes)
            {
                if (child.ChildNodes.Count == 1 && child.FirstChild.NodeType == XmlNodeType.CDATA)
                {
                    _dict.Add(child.Name, child.FirstChild.Value);
                }
            }
        }

        protected string GetPropertyValueFromDict(string propertyName)
        {
            return GetPropertyValueFromDict(propertyName, true);
        }

        protected string GetPropertyValueFromDict(string propertyName, bool throwExceptionWhenMissing)
        {
            return GetPropertyValueFromDict(propertyName, throwExceptionWhenMissing, null);
        }

        protected string GetPropertyValueFromDict(string propertyName, bool throwExceptionWhenMissing,
                                                  string defaultValue)
        {
            if (!_dict.ContainsKey(propertyName))
            {
                if (throwExceptionWhenMissing)
                {
                    throw new ArgumentException(GetType().Name + " without " + propertyName);
                }
                else
                {
                    return defaultValue;
                }
            }

            return _dict[propertyName];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using FluoriteAnalyzer.Commons;

namespace FluoriteAnalyzer.Events
{
    [Serializable]
    public enum EventType
    {
        Command,
        DocumentChange,
        Annotation,
        Dummy
    }

    [Serializable]
    public abstract class Event
    {
        protected Dictionary<string, string> _dict;

        // Holds the XmlElement children that are not in the standard format.
        [NonSerialized]
        protected List<XmlElement> _others;

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

            FillInDictionary(element);

            ID = int.Parse(GetPropertyValueFromDict("__id", false, "-1"));
            TypeString = GetPropertyValueFromDict("_type");
            Timestamp = long.Parse(GetPropertyValueFromDict("timestamp"));
            RepeatCount = int.Parse(GetPropertyValueFromDict("repeat", false, "1"));

            string timestamp2Value = GetPropertyValueFromDict("timestamp2", false);
            if (timestamp2Value != null) { Timestamp2 = long.Parse(timestamp2Value); }
            else { Timestamp2 = null; }
        }

        public int ID { get; internal set; }
        public string TypeString { get; private set; }
        public long Timestamp { get; private set; }
        public long? Timestamp2 { get; set; }

        public long EndTimestamp { get { return Timestamp2 ?? Timestamp; } }

        public int RepeatCount { get; private set; }

        public abstract EventType EventType { get; }

        public string LogFilePath { get; internal set; }

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

        public static XmlElement FindCorrespondingXmlElementFromXmlDocument(XmlDocument doc, Event anEvent)
        {
            if (doc == null || anEvent == null) { throw new ArgumentNullException(); }

            return FindCorrespondingXmlElementFromXmlElements(doc.DocumentElement.ChildNodes.OfType<XmlElement>(), anEvent);
        }

        public static XmlElement FindCorrespondingXmlElementFromXmlElements(IEnumerable<XmlElement> elements, Event anEvent)
        {
            if (elements == null || anEvent == null) { throw new ArgumentNullException(); }

            return elements.FirstOrDefault(x => x.Attributes["__id"].Value == anEvent.ID.ToString());
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

        private void FillInDictionary(XmlElement element)
        {
            _dict = new Dictionary<string, string>();
            _others = new List<XmlElement>();

            foreach (XmlAttribute attr in element.Attributes)
            {
                _dict.Add(attr.Name, attr.Value);
            }

            foreach (XmlElement child in element.ChildNodes)
            {
                if (child.ChildNodes.Count == 1 && child.FirstChild.NodeType == XmlNodeType.CDATA)
                {
                    _dict.Add(child.Name, child.FirstChild.Value);
                }
                else if (child.ChildNodes.Count > 1 && child.FirstChild.NodeType == XmlNodeType.CDATA)
                {
                    string data = string.Empty;
                    XmlNode curDataNode = child.FirstChild;
                    while (curDataNode != null)
                    {
                        data += curDataNode.Value;
                        if (curDataNode.NextSibling != null && curDataNode.NextSibling.NodeType == XmlNodeType.CDATA)
                        {
                            curDataNode = curDataNode.NextSibling;
                        }
                        else
                        {
                            curDataNode = null;
                        }
                    }

                    _dict.Add(child.Name, data);
                }
                else
                {
                    _others.Add(child);
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
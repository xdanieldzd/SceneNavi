using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Collections;
using System.ComponentModel;

namespace SceneNavi
{
    public class XMLHashTableReader
    {
        public Hashtable Names { get; private set; }
        public Type KeyType { get; private set; }
        public Type ValueType { get; private set; }

        public XMLHashTableReader(string defdir, string fn)
        {
            Names = new Hashtable();

            string path = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), defdir);
            if (!Directory.Exists(path)) return;

            string file = Path.Combine(path, fn);
            if (!File.Exists(file)) return;

            /* Load XDocument */
            XDocument xdoc = XDocument.Load(Path.Combine(path, fn));

            /* Fetch Key- and ValueType */
            KeyType = Type.GetType((string)xdoc.Root.Attribute("KeyType"));
            var keyconv = TypeDescriptor.GetConverter(KeyType);

            ValueType = Type.GetType((string)xdoc.Root.Attribute("ValueType"));
            var valconv = TypeDescriptor.GetConverter(ValueType);

            /* Parse elements */
            foreach (XElement element in xdoc.Root.Elements())
            {
                /* Convert values & add to list */
                Names.Add(keyconv.ConvertFrom(element.Attribute("Key").Value), valconv.ConvertFrom(element.Value));
            }
        }
    }
}

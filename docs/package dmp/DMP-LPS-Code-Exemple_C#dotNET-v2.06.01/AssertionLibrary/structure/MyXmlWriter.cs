using System.IO;

namespace TLSi_AssertionLibrary.structure
{
    class MyXmlWriter : System.Xml.XmlTextWriter
    {
        private static bool Premier = true;
        public MyXmlWriter(TextWriter fileName)
            : base(fileName)
        { }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {

            if (ns == "urn:oasis:names:tc:SAML:2.0:assertion")
            {



                base.WriteStartElement("saml2", localName, ns);
                if (Premier)
                {
                    base.WriteAttributeString("xmlns", "samlp", null, "urn:oasis:names:tc:SAML:2.0:protocol");
                    base.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    base.WriteAttributeString("xsi", "schemaLocation", null, "urn:oasis:names:tc:SAML:2.0:assertion http://docs.oasis-open.org/security/saml/v2.0/saml-schema-assertion-2.0.xsd");
                    Premier = false;

                }


            }
            else
            {
                base.WriteStartElement(prefix, localName, ns);
            }
        }

    }
}

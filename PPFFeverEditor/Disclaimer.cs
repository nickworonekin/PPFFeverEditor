using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Windows.Forms;

namespace PPFFeverEditor
{
    public class Disclaimer
    {
        public static void Display()
        {
            // Display the disclaimer
            bool displayDisclaimer = true;

            // Does a settings.xml exist?
            if (File.Exists("settings.xml"))
            {
                try
                {
                    // Load settings.xml and see if we already displayed and the user
                    // agreed to the disclaimer already
                    XmlDocument xml = new XmlDocument();
                    xml.Load("settings.xml");
                    XmlNode node = xml.SelectSingleNode("/settings");
                    displayDisclaimer = (node["disclaimer"].InnerText != "1");
                }
                catch
                {
                }
            }

            // Should we still display the disclaimer?
            if (displayDisclaimer)
            {
                DialogResult result = MessageBox.Show("I will not use this program to cheat online.",
                    "Disclaimer", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                // The user agreed to it. Now write a settings.xml file and remember that they agreed to it.
                if (result == DialogResult.OK)
                {
                    using (XmlTextWriter writer = new XmlTextWriter("settings.xml", Encoding.ASCII))
                    {
                        writer.WriteStartDocument();
                        writer.WriteWhitespace("\n");
                        writer.WriteStartElement("settings");
                        writer.WriteWhitespace("\n\t");
                        writer.WriteElementString("disclaimer", "1");
                        writer.WriteWhitespace("\n");
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                }
                else
                    Environment.Exit(0);
            }
        }
    }
    }
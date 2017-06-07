using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Project.Performance.Model;

namespace Project.Performance.Utility
{
    public class XMLHelper
    {
        public static string SaveStringToFile(string models, string fileName, string userName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = userName + Guid.NewGuid().ToString();
            }

            string folder = Path.Combine(BaseConfig.SaveFilePath, userName);
            DirectoryInfo di = new DirectoryInfo(folder);
            if (!di.Exists)
            {
                di.Create();
            }

            string filePath = Path.Combine(folder, fileName + ".xml");
            if (File.Exists(filePath))
            {
                throw new ArgumentException("File " + fileName + "Already Exists.");
            }

            using (StreamWriter sr = new StreamWriter(filePath, true, Encoding.ASCII))
            {
                sr.Write(models);
                sr.Close();
            }
            return filePath;
        }

        public static XMLModel GetModelFromXML(string xml)
        {
            XMLModel xmlModel;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XMLModel));
                StringReader sr = new StringReader(xml);
                xmlModel = (XMLModel)serializer.Deserialize(sr);
            }
            catch (Exception)
            {
                throw new FormatException("The document is of incorrect format, please check before saving it.");
            }

            if (!CheckXMLModelTaskCount(xmlModel))
            {
                throw new FormatException("No children node or wrong children node for root node.");
            }

            return xmlModel;
        }

        public static bool ValidateXMLFormat(string xml)
        {
            XMLModel xmlModel = GetModelFromXML(xml);
            if (xmlModel == null)
            {
                return false;
            }
            return true;
        }

        public static string FormatXML(Stream xmlStream)
        {
            string result = string.Empty;

            try
            {
                // Load the XmlDocument with the XML.
                XmlDocument document = new XmlDocument();
                document.Load(xmlStream);

                // Write the XML into a formatting XmlTextWriter
                MemoryStream mStream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
                writer.Formatting = System.Xml.Formatting.Indented;
                document.WriteContentTo(writer);

                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);
                result = sReader.ReadToEnd();

                mStream.Close();
                writer.Close();
            }
            catch (XmlException e)
            {
                throw e;
            }
            return result;
        }

        public static string ReadModelToString(XMLModel model)
        {
            XmlSerializerNamespaces xns = new XmlSerializerNamespaces();
            XmlSerializer xml = new XmlSerializer(typeof(XMLModel));
            xns.Add(string.Empty, string.Empty);
            using (StringWriter textWriter = new StringWriter())
            {
                xml.Serialize(textWriter, model, xns);
                string modelStr = textWriter.ToString();
                modelStr = modelStr.Substring(modelStr.IndexOf(">") + 1, modelStr.Length - modelStr.IndexOf(">") - 1);
                textWriter.Close();
                return modelStr;
            }
        }

        public static XMLModel ReadConfigFileToModels(string filePath)
        {
            XMLModel xmlModel;

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(XMLModel));

                    xmlModel = (XMLModel)serializer.Deserialize(reader);
                }
                catch (Exception)
                {
                    throw new FormatException("The document is of incorrect format, please check before saving it.");
                }

                if (!CheckXMLModelTaskCount(xmlModel))
                {
                    throw new FormatException("No children node or wrong children node for root node.");
                }

                return xmlModel;
            }
        }

        private static bool CheckXMLModelTaskCount(XMLModel model)
        {
            if (model != null)
            {
                if (model.CustomFieldModelList.Count != 0 || model.LookupTableModelList.Count != 0 || model.ProjectModelList.Count != 0 || model.ResourcePoolModelList.Count != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

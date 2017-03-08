using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace ArcadeGame
{
    abstract class Maps
    {
        private static Dictionary<string, Map> maps_;
        public static Dictionary<string, Map> Maps_
        {
            get { return maps_; }
            set { maps_ = value; }
        }

        private static string GetStringAttr(XmlNode node, string attr)
        {
            string res = null;
            if (node.SelectSingleNode("//" + attr) != null)
                res = ((XmlElement)node.SelectSingleNode("//" + attr)).GetAttribute("value");
            return res == null ? res : res.Trim();
        }

        private static int GetIntAttr(XmlNode node, string attr)
        {
            int res = 0;
            if (node.SelectSingleNode("//" + attr) != null)
                res = Convert.ToInt32(((XmlElement)node.SelectSingleNode("//" + attr)).GetAttribute("value"));
            return res;
        }

        public static void Initialize(string save = null)
        {
            if (maps_ == null)
                maps_ = new Dictionary<string, Map>();
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load("../../../../LightContent/database/maps.xml");
            XmlNodeList nodeList = xmlDocument.SelectNodes("//map");
            for (int k = 0; k < nodeList.Count; k++)
            {
                string name = "",
                       next_map = "";
                int gravity = 1;
                Vector2 position = new Vector2(1);
                int width = 30,
                    height = 20;
                List<string> content = null;
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(nodeList[k].OuterXml);
                if (xml.SelectSingleNode("//name") != null)
                    name = xml.SelectSingleNode("//name/text()").Value;
                if (save != null && name != save)
                    continue;
                /*if (maps_.Keys.Contains(name))
                    continue;*/
                if (xml.SelectSingleNode("//position") != null)
                {
                    string[] pos = xml.SelectSingleNode("//position/text()").Value.Split(' ');
                    position = new Vector2(Convert.ToInt16(pos[0]), Convert.ToInt16(pos[1]));
                }
                if (xml.SelectSingleNode("//width") != null)
                    width = Convert.ToInt32(xml.SelectSingleNode("//width/text()").Value);
                if (xml.SelectSingleNode("//height") != null)
                    height = Convert.ToInt32(xml.SelectSingleNode("//height/text()").Value);
                if (xml.SelectSingleNode("//gravity") != null)
                    gravity = Convert.ToInt32(xml.SelectSingleNode("//gravity/text()").Value);
                bool read_only = false;
                if (xml.SelectSingleNode("//read-only") != null)
                    read_only = true;
                bool tutorial = false;
                if (xml.SelectSingleNode("//tutorial") != null)
                    tutorial = true;
                bool official = false;
                if (xml.SelectSingleNode("//official") != null)
                    official = true;
                if (xml.SelectSingleNode("//next-map") != null)
                    next_map = xml.SelectSingleNode("//next-map/text()").Value;
                Cell[,] cells = new Cell[width, height];
                if (xml.SelectSingleNode("//content") != null)
                    content = xml.SelectSingleNode("//content/text()").Value.Split('\n').ToList<string>();
                Dictionary<int, string> messages = new Dictionary<int, string>();
                if (xml.SelectSingleNode("//messages") != null)
                {
                    XmlDocument xmlMessages = new XmlDocument();
                    xmlMessages.LoadXml(xml.SelectSingleNode("//messages").OuterXml);
                    foreach (XmlNode node in xmlMessages.SelectNodes("//message"))
                    {
                        int index = Convert.ToInt16(((XmlElement)node).GetAttribute("value"));
                        string message = ((XmlElement)node).InnerText.Trim();
                        messages[index] = message;
                    }
                }
                for (int j = 0; j < height; j++)
                {
                    content[j] = content[j].Replace("\r", "");
                    content[j] = Regex.Replace(content[j], @"^\ +", "");
                    content[j] = Regex.Replace(content[j], @"\ +$", "");
                    if (Regex.IsMatch(content[j], @"^\ *$"))
                    {
                        content.Remove(content[j]);
                        j--;
                    }
                }
                for (int j = 0; j < height; j++)
                {
                    List<String> line = content[j].Split(' ').ToList<string>();
                    for (int i = 0; i < width; i++)
                    {
                        int[] cell = { -1, -1, -1 };
                        int n = 0;
                        int message_index = -1;
                        foreach (string str in line[i].Split('/'))
                        {
                            if (str.StartsWith("m"))
                            {
                                cell[n++] = -2;
                                try
                                {
                                    message_index = Convert.ToInt16(str.Substring(1));
                                }
                                catch
                                {
                                    message_index = -1;
                                }
                            }
                            else
                                cell[n++] = (Convert.ToInt16(str));
                        }
                        if (message_index == -1)
                            cells[i, j] = new Cell(cell, i, j);
                        else
                            cells[i, j] = new Cell(cell, i, j, message_index);
                    }
                }
                maps_[name] = new Map(name, width, height, cells, position, gravity, next_map, read_only, tutorial, official, messages);
            }
        }

        public static Dictionary<string, Tuple<string, bool, bool>> GetNamesAndScores()
        {
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load("../../../../LightContent/database/scores.xml");
            }
            catch
            {
                Console.WriteLine("Unable to save: cannot load XML.");
                return null;
            }
            XmlNode root = xmlDocument.DocumentElement;
            XmlNode node = xmlDocument.SelectSingleNode("//scores");
            Dictionary<string, Tuple<string, bool, bool>> result = new Dictionary<string, Tuple<string, bool, bool>>();
            foreach (String name in maps_.Keys)
            {
                node = root.SelectSingleNode("descendant::score[name='" + name + "']");
                bool official = maps_[name].Official_;
                bool tutorial = maps_[name].Tutorial_;
                if (node != null)
                {
                    XmlNode score = node.SelectSingleNode("//best-score");
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml("<score>" + node.InnerXml + "</score>");
                    XmlNode best = doc.SelectSingleNode("//best-score");
                    result[name] = best == null ?
                        new Tuple<string, bool, bool>("", tutorial, official) :
                        new Tuple<string, bool, bool>((" — " + best.InnerText.Substring(3, Math.Min(9, best.InnerText.Length - 3))), tutorial, official);
                }
                else
                {
                    result[name] = new Tuple<string, bool, bool>("", tutorial, official);
                }
            }
            return result;
        }
    }
}

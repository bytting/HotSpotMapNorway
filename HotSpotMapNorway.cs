using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Xml;

namespace HotSpotMapNorway
{
    public class HotSpotMapNorway : ImageMap
    {                
        public HotSpotMapNorway() {}                

        protected override void OnInit(EventArgs e)
        {
            CurrentDepth = 0;
            CurrentCounty = String.Empty;
            CurrentCommunity = String.Empty;
            ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway.Fylker.jpg");
            loadCountiesHotSpots();
            HotSpotMode = HotSpotMode.PostBack;
            base.OnInit(e);
        }

        protected override void OnClick(ImageMapEventArgs e)
        {            
            string[] args = e.PostBackValue.Split(':');
            CurrentDepth = Convert.ToInt32(args[0]);
            CurrentCounty = String.Empty;
            CurrentCommunity = String.Empty;

            if (CurrentDepth == 0)
            {                                
                ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway.Fylker.jpg");
                loadCountiesHotSpots();
            }
            else if (CurrentDepth == 1)
            {
                CurrentCounty = args[1];
                ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway." + CurrentCounty + ".jpg");
                loadCountyHotSpots();                

                // The back hotspot
                PolygonHotSpot phs = new PolygonHotSpot();
                phs.PostBackValue = "0";
                phs.Coordinates = "0,0 20,0 20,20 0,20";
                HotSpots.Add(phs);
            }
            else
            {
                CurrentCounty = args[1];
                CurrentCommunity = args[2];

                ImageUrl = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway." + CurrentCounty + ".jpg");
                loadCountyHotSpots();                

                // The back hotspot
                PolygonHotSpot phs = new PolygonHotSpot();
                phs.PostBackValue = "1:" + CurrentCounty;
                phs.Coordinates = "0,0 20,0 20,20 0,20";
                HotSpots.Add(phs);
            }

            base.OnClick(e);
        }        

        protected void loadCountyHotSpots()
        {            
            HotSpots.Clear();            

            string hotSpotsXml = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway.HotSpots.xml");
            hotSpotsXml = GetAbsolutePath(hotSpotsXml);

            XmlDocument hotSpots = new XmlDocument();
            hotSpots.Load(hotSpotsXml);
            
            XmlNode countyNode = hotSpots.SelectSingleNode("/counties/county[@name=\"" + CurrentCounty + "\"]");
            XmlNodeList nodes = countyNode.SelectNodes("community");

            foreach (XmlElement node in nodes)
            {
                PolygonHotSpot phs = new PolygonHotSpot();
                phs.PostBackValue = "2:" + CurrentCounty + ":" + node.GetAttribute("name");
                phs.Coordinates = node.FirstChild.InnerText;
                HotSpots.Add(phs);            
            }                        
        }        

        protected void loadCountiesHotSpots()
        {
            HotSpots.Clear();            

            string hotSpotsXml = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway.HotSpots.xml");
            hotSpotsXml = GetAbsolutePath(hotSpotsXml);

            XmlDocument hotSpots = new XmlDocument();
            hotSpots.Load(hotSpotsXml);            

            XmlNodeList nodes = hotSpots.SelectNodes("//county");
            foreach (XmlElement node in nodes)
            {                
                PolygonHotSpot phs = new PolygonHotSpot();
                phs.PostBackValue = "1:" + node.GetAttribute("name");
                phs.Coordinates = node.FirstChild.InnerText;
                HotSpots.Add(phs);
            }
        }                

        string GetAbsolutePath(string relativePath)
        {
            string absolutePath = Page.Request.Url.AbsoluteUri;
            string basePath = absolutePath.Remove(absolutePath.LastIndexOf("/"));
            return basePath + relativePath;
        }

        public int CurrentDepth
        {
            get { return (ViewState["CurrentDepth"] != null) ? Convert.ToInt32(ViewState["CurrentDepth"]) : -1; }
            set { ViewState["CurrentDepth"] = value; }
        }

        public string CurrentCounty
        {
            get { return (ViewState["CurrentCounty"] != null) ? Convert.ToString(ViewState["CurrentCounty"]) : String.Empty; }
            set { ViewState["CurrentCounty"] = value; }
        }

        public string CurrentCommunity
        {
            get { return (ViewState["CurrentCommunity"] != null) ? Convert.ToString(ViewState["CurrentCommunity"]) : String.Empty; }
            set { ViewState["CurrentCommunity"] = value; }
        }        

        public void GetCounties(ref ArrayList counties)
        {            
            string hotSpotsXml = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway.HotSpots.xml");
            hotSpotsXml = GetAbsolutePath(hotSpotsXml);

            counties.Clear();

            XmlDocument hotSpots = new XmlDocument();
            hotSpots.Load(hotSpotsXml);

            XmlNodeList nodes = hotSpots.SelectNodes("//county");
            foreach (XmlElement node in nodes)            
                counties.Add(node.GetAttribute("name"));                                
        }

        public void GetCommunities(string county, ref ArrayList communities)
        {
            string hotSpotsXml = Page.ClientScript.GetWebResourceUrl(this.GetType(), "HotSpotMapNorway.HotSpots.xml");
            hotSpotsXml = GetAbsolutePath(hotSpotsXml);

            communities.Clear();

            XmlDocument hotSpots = new XmlDocument();
            hotSpots.Load(hotSpotsXml);

            XmlNode countyNode = hotSpots.SelectSingleNode("/counties/county[@name=\"" + county + "\"]");
            XmlNodeList nodes = countyNode.SelectNodes("community");
            
            foreach (XmlElement node in nodes)            
                communities.Add(node.GetAttribute("name"));                            
        }
    }        
}
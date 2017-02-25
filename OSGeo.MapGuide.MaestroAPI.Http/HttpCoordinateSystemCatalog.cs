﻿#region Disclaimer / License

// Copyright (C) 2010, Jackie Ng
// https://github.com/jumpinjackie/mapguide-maestro
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

#endregion Disclaimer / License

using OSGeo.MapGuide.MaestroAPI.CoordinateSystem;
using OSGeo.MapGuide.MaestroAPI.Http;
using System.Xml;

namespace OSGeo.MapGuide.MaestroAPI
{
    public class HttpCoordinateSystemCatalog : CoordinateSystemCatalog
    {
        private IHttpRequest m_con;
        private RequestBuilder m_req;
        private CoordinateSystemCategory[] m_categories;
        private string m_coordLib = null;

        internal HttpCoordinateSystemCatalog(IHttpRequest con, RequestBuilder req)
        {
            m_con = con;
            m_req = req;
        }

        internal RequestBuilder RequestBuilder => m_req;

        public override CoordinateSystemCategory[] Categories
        {
            get
            {
                if (m_categories == null)
                {
                    string req = m_req.EnumerateCategories();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(m_con.OpenReadSync(req));
                    XmlNodeList lst = doc.SelectNodes("StringCollection/Item"); //NOXLATE
                    CoordinateSystemCategory[] data = new CoordinateSystemCategory[lst.Count];
                    for (int i = 0; i < lst.Count; i++)
                        data[i] = new HttpCoordinateSystemCategory(this, lst[i].InnerText);
                    m_categories = data;
                }

                return m_categories;
            }
        }

        public override string LibraryName
        {
            get
            {
                if (m_coordLib == null)
                {
                    string req = m_req.GetBaseLibrary();
                    m_coordLib = System.Text.Encoding.UTF8.GetString(m_con.DownloadDataSync(req));
                }
                return m_coordLib;
            }
        }

        public override bool IsValid(string wkt)
        {
            string req = m_req.IsValidCoordSys(wkt);
            return System.Text.Encoding.UTF8.GetString(m_con.DownloadDataSync(req)).ToLower().Trim().Equals("true");
        }

        public override string ConvertWktToCoordinateSystemCode(string wkt)
        {
            string req = m_req.ConvertWktToCoordinateSystemCode(wkt);
            return System.Text.Encoding.UTF8.GetString(m_con.DownloadDataSync(req)).Trim('\0');
        }

        public override string ConvertCoordinateSystemCodeToWkt(string coordcode)
        {
            string req = m_req.ConvertCoordinateSystemCodeToWkt(coordcode);
            return System.Text.Encoding.UTF8.GetString(m_con.DownloadDataSync(req)).Trim('\0');
        }

        public override string ConvertWktToEpsgCode(string wkt)
        {
            string req = m_req.ConvertWktToEpsgCode(wkt);
            return System.Text.Encoding.UTF8.GetString(m_con.DownloadDataSync(req)).Trim('\0');
        }

        public override string ConvertEpsgCodeToWkt(string epsg)
        {
            string req = m_req.ConvertEpsgCodeToWkt(epsg);
            return System.Text.Encoding.UTF8.GetString(m_con.DownloadDataSync(req)).Trim('\0');
        }

        public override bool IsLoaded => m_categories != null;

        public override CoordinateSystemDefinitionBase[] EnumerateCoordinateSystems(string category)
        {
            CoordinateSystemCategory cat = null;
            foreach (CoordinateSystemCategory csc in this.Categories)
            {
                if (csc.Name == category)
                {
                    cat = csc;
                    break;
                }
            }

            if (cat == null)
                return new CoordinateSystemDefinitionBase[0];

            string req = this.RequestBuilder.EnumerateCoordinateSystems(category);
            XmlDocument doc = new XmlDocument();
            doc.Load(m_con.OpenReadSync(req));
            XmlNodeList lst = doc.SelectNodes("BatchPropertyCollection/PropertyCollection");
            CoordinateSystemDefinitionBase[] data = new CoordinateSystemDefinitionBase[lst.Count];
            for (int i = 0; i < lst.Count; i++)
                data[i] = new HttpCoordinateSystemDefinition(cat, lst[i]);

            return data;
        }

        public override CoordinateSystemDefinitionBase CreateEmptyCoordinateSystem() => new HttpCoordinateSystemDefinition();
    }
}
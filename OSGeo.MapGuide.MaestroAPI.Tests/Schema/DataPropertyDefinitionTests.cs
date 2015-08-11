﻿#region Disclaimer / License

// Copyright (C) 2014, Jackie Ng
// http://trac.osgeo.org/mapguide/wiki/maestro, jumpinjackie@gmail.com
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.MapGuide.MaestroAPI.Schema;
using NUnit.Framework;

namespace OSGeo.MapGuide.MaestroAPI.Schema.Tests
{
    [TestFixture]
    public class DataPropertyDefinitionTests
    {
        [Test]
        public void DataPropertyDefinitionTest()
        {
            var prop = new DataPropertyDefinition("Foo", "Bar");
            Assert.AreEqual("Foo", prop.Name);
            Assert.AreEqual("Bar", prop.Description);
            Assert.AreEqual(DataPropertyType.String, prop.DataType);
        }

        [Test]
        public void IsNumericTypeTest()
        {
            var prop = new DataPropertyDefinition("Foo", "Bar");
            Assert.AreEqual("Foo", prop.Name);
            Assert.AreEqual("Bar", prop.Description);
            Assert.AreEqual(DataPropertyType.String, prop.DataType);

            foreach (DataPropertyType dt in Enum.GetValues(typeof(DataPropertyType)))
            {
                prop.DataType = dt;
                if (dt == DataPropertyType.Blob ||
                    dt == DataPropertyType.Boolean ||
                    dt == DataPropertyType.Clob ||
                    dt == DataPropertyType.DateTime ||
                    dt == DataPropertyType.String)
                {
                    Assert.False(prop.IsNumericType());
                }
                else
                {
                    Assert.True(prop.IsNumericType());
                }
            }
        }
    }
}

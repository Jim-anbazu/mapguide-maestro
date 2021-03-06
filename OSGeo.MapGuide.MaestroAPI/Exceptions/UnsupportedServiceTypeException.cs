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

using OSGeo.MapGuide.MaestroAPI.Services;
using System;

namespace OSGeo.MapGuide.MaestroAPI.Exceptions
{
    /// <summary>
    /// Exception that is thrown when an attempt is made to get a service which is not
    /// supported by the connection
    /// </summary>
    [global::System.Serializable]
    public class UnsupportedServiceTypeException : MaestroException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedServiceTypeException"/> class.
        /// </summary>
        /// <param name="st">The st.</param>
        public UnsupportedServiceTypeException(ServiceType st)
        {
            this.ServiceType = st;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedServiceTypeException"/> class.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="message">The message.</param>
        public UnsupportedServiceTypeException(ServiceType st, string message)
            : base(message)
        {
            this.ServiceType = st;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedServiceTypeException"/> class.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UnsupportedServiceTypeException(ServiceType st, string message, Exception inner)
            : base(message, inner)
        {
            this.ServiceType = st;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedServiceTypeException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        protected UnsupportedServiceTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>The type of the service.</value>
        public ServiceType ServiceType { get; }
    }
}
﻿using System;
using System.Runtime.Serialization;

namespace VirtualBicycle
{
    /// <summary>
    /// Thrown when a graphics device cannot be created.
    /// </summary>
    [Serializable]
    public class DeviceCreationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCreationException"/> class.
        /// </summary>
        public DeviceCreationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCreationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DeviceCreationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCreationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DeviceCreationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceCreationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected DeviceCreationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

using System;

namespace GsKit
{
    namespace Exceptions
    {
        public class ResourceTypeMismatchException : Exception
        {
            public ResourceTypeMismatchException(Resources.AbstractResource resource, Type expectedType) : base(
                "Resource with id \"" +
                resource.ResourceID +
                "\"is not of type: " +
                expectedType)
            {
            }

            public ResourceTypeMismatchException(string message) : base(message)
            {
            }

            public ResourceTypeMismatchException(string message, Exception exception) : base(message, exception)
            {
            }
        }
    }
}
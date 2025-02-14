using System;

namespace GsKit
{
    namespace Exceptions
    {
        public class DuplicateResourceIDException : Exception
        {
            public DuplicateResourceIDException(Resources.AbstractResource resource1,
                Resources.AbstractResource resource2) : base("Duplicate resource IDs found: " +
                                                            $"\"{resource1.ResourceID}\" " +
                                                            $"between:\n{resource1}\n{resource2}")
            {
            }

            public DuplicateResourceIDException(string message) : base(message)
            {
            }

            public DuplicateResourceIDException(string message, Exception exception) : base(message, exception)
            {
            }
        }
    }
}
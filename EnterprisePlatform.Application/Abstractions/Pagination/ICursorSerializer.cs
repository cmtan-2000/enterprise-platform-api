using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterprisePlatform.Application.Abstractions.Pagination
{
    public interface ICursorSerializer
    {
        string Encode<T>(T value);

        T Decode<T>(string cursor);
    }
}
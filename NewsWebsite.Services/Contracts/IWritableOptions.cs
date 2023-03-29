using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsWebsite.Services.Contracts
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        T Value { get;}
        void Update(Action<T> applyChanges);
    }
}

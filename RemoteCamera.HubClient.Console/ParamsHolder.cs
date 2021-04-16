using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteCamera.HubClient.Console
{
    public class ParamsHolder
    {
        private IEnumerable<string> _parameters;

        public string this[int idx]
            => _parameters.Count() <= idx ? null : _parameters.ElementAt(idx);

        public void Update(string input)
        {
            _parameters = input
                .ToUpperInvariant()
                .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .AsEnumerable();
        }

        public bool Any()
        {
            return _parameters.Any();
        }

    }
}

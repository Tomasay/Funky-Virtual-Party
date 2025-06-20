using Glitch9.Editor.Collections;
using System.Collections.Generic;

namespace Glitch9.Editor.IMGUI
{
    public class TreeViewExpandedState
    {
        private readonly EPrefsDictionary<string, HashSet<int>> _expandedStates;

        public TreeViewExpandedState(string prefsKey)
        {
            _expandedStates = new EPrefsDictionary<string, HashSet<int>>(prefsKey);
        }

        public void Update(string key, HashSet<int> expandedState)
        {
            _expandedStates.AddOrUpdate(key, expandedState);
        }

        public bool TryGetValue(string key, out HashSet<int> expandedState)
        {
            return _expandedStates.TryGetValue(key, out expandedState);
        }
    }
}
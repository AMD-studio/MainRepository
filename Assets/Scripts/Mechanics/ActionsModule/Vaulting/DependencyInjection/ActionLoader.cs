using UnityEngine;
using System;

namespace Climbing.DependencyInjection
{
    public interface IActionLoader
    {
        public Action LoadAction(string actionPath);
    }

    public class ActionLoader : IActionLoader
    {
        public Action LoadAction(string actionPath) => Resources.Load<Action>(actionPath);
    }
}

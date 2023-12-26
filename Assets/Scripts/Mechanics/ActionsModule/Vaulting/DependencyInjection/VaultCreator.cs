using System;
using System.Reflection;

namespace Climbing.DependencyInjection
{
    // Interface for VaultActionCreators
    public interface IVaultActionCreator
    {
        public VaultAction CreateAction();
    }

    public class VaultCreator<T> : IVaultActionCreator where T : VaultAction
    {
        private readonly ThirdPersonController _controller;
        private readonly IActionLoader _actionLoader;

        private readonly Type _actionType;

        private readonly Func<ThirdPersonController, Action, VaultAction> _actionCreatorWithInfo;
        private readonly Func<ThirdPersonController, VaultAction> _actionCreatorWithoutInfo;

        private readonly string _actionPath;

        public VaultCreator(ThirdPersonController controller, IActionLoader actionLoader = null, string actionPath = null)
        {
            _controller = controller;
            _actionLoader = actionLoader;
            _actionPath = actionPath;
            _actionType = typeof(T);

            _actionCreatorWithInfo = CreateActionWithInfoCreator();
            _actionCreatorWithoutInfo = CreateActionWithoutInfoCreator();
        }

        public VaultAction CreateAction()
        {
            if (!string.IsNullOrEmpty(_actionPath) && _actionLoader != null)
            {
                Action actionInfo = _actionLoader.LoadAction(_actionPath);
                return _actionCreatorWithInfo(_controller, actionInfo);
            }

            return _actionCreatorWithoutInfo(_controller);
        }

        private Func<ThirdPersonController, Action, VaultAction> CreateActionWithInfoCreator()
        {
            ConstructorInfo ctor = _actionType.GetConstructor(new[] { typeof(ThirdPersonController), typeof(Action) });
            return (controller, actionInfo) => (VaultAction)ctor.Invoke(new object[] { controller, actionInfo });
        }

        private Func<ThirdPersonController, VaultAction> CreateActionWithoutInfoCreator()
        {
            ConstructorInfo ctor = _actionType.GetConstructor(new[] { typeof(ThirdPersonController), typeof(Action) });
            return (controller) => (VaultAction)ctor.Invoke(new object[] { controller, null });
        }
    }

}
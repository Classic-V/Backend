using System.Reflection;
using Autofac;
using Backend.Utils.Interfaces;
using Backend.Utils.Models;

namespace Backend.Utils
{
	public class ModuleContainer : IDisposable
	{
		private readonly Type[] _loadedTypes = Assembly.GetExecutingAssembly().GetTypes();

		private IContainer _container = null!;
		private ILifetimeScope _scope = null!;

		private readonly List<Type> _modules = new();
		private readonly List<Type> _controllers = new();
		private readonly List<Type> _services = new();
		private readonly List<Type> _items = new();

		internal async Task RegisterTypes()
		{
			var builder = new ContainerBuilder();

			await LoadTypes();

			builder.RegisterType<Application>().As<IApplication>();

			foreach (var item in _items)
			{
				builder.RegisterType(item)
					.AsImplementedInterfaces()
					.AsSelf()
					.SingleInstance();
			}

			foreach (var module in _modules)
			{
				builder.RegisterType(module)
					.AsImplementedInterfaces()
					.AsSelf()
					.SingleInstance();
			}

			foreach (var controller in _controllers)
			{
				builder.RegisterType(controller)
					.AsImplementedInterfaces()
					.AsSelf()
					.SingleInstance();
			}

			foreach (var service in _services)
			{
				builder.RegisterType(service)
					.AsImplementedInterfaces()
					.AsSelf()
					.SingleInstance();
			}

			_container = builder.Build();
		}

		internal Task ResolveTypes()
		{
			_scope = _container.BeginLifetimeScope();

			foreach (var item in _items) _scope.Resolve(item);
			foreach (var module in _modules) _scope.Resolve(module);
			foreach (var controller in _controllers) _scope.Resolve(controller);
			foreach (var service in _services) _scope.Resolve(service);

			return Task.CompletedTask;
		}

		internal T Resolve<T>() where T : notnull
		{
			return _scope.Resolve<T>();
		}

		private Task LoadTypes()
		{
			foreach (var type in _loadedTypes)
			{
				if (IsModuleType(type)) _modules.Add(type);
				if (IsControllerType(type)) _controllers.Add(type);
				if (IsServiceType(type)) _services.Add(type);
				if (IsItemType(type)) _items.Add(type);
			}

			return Task.CompletedTask;
		}

		private bool IsModuleType(Type type)
		{
			if (type.Namespace == null) return false;
			return type.Namespace.StartsWith("Backend.Modules") &&
				type.BaseType != null &&
				(type.BaseType == typeof(ModuleBase) || type.BaseType.IsGenericType) &&
				!type.Name.StartsWith("<") &&
				(!type.Namespace.StartsWith("Backend.Modules.Chat.Dev") || Resource.DevMode);
		}

		private bool IsControllerType(Type type)
		{
			if (type.Namespace == null) return false;
			return type.Namespace.StartsWith("Backend.Controllers")
				   && type.BaseType != null
				   && !type.Name.StartsWith("<");
		}

		private bool IsServiceType(Type type)
		{
			if (type.Namespace == null) return false;
			return type.Namespace.StartsWith("Backend.Services")
				   && type.BaseType != null
				   && !type.Name.StartsWith("<");
		}

		private bool IsItemType(Type type)
		{
			if (type.Namespace == null) return false;
			return type.Namespace.StartsWith("Backend.Utils.Models.Inventory.Items")
				   && type.BaseType != null
				   && !type.Name.StartsWith("<");
		}

		public void Dispose()
		{
			_container.Dispose();
			_scope.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
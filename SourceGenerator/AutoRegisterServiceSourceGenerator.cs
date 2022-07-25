using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerator
{

    [Generator]
    public class AutoRegisterServiceSourceGenerator : ISourceGenerator
    {

        private readonly Dictionary<string, string> _actions = new Dictionary<string, string>
        {
            { "Abstractions.ISingletonDependencyResolver", "AddSingleton" },
            { "Abstractions.ITransientDependencyResolver", "AddTransient" },
            { "Abstractions.IScopedDependencyResolver", "AddScoped" }
        };

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {

            var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

            var stringBuilder = new StringBuilder("namespace " + mainMethod.ContainingNamespace.ToDisplayString() + "\n"
                                            + "{\n"
                                            + "    using System;\n"
                                            + "    using System.Threading.Tasks;\n"
                                            + "    using Microsoft.Extensions.DependencyInjection;\n");
            var namespaces = new List<string>();

            string defaultPath = typeof(object).Assembly.Location.Replace("mscorlib", "{0}");

            var references = new List<MetadataReference>()
            {
                { MetadataReference.CreateFromFile(string.Format(defaultPath, "System.Threading.Tasks")) }
            };

            string dependencyResolverName = "Abstractions.IDependencyResolver";
            var types = GetAllTypes(context.Compilation);
            var neededTypes = new Dictionary<INamedTypeSymbol, string>();
            foreach (var type in types)
            {
                if (!type.Interfaces.IsEmpty && type.TypeKind == TypeKind.Class)
                {
                    if (type.AllInterfaces.All(i => i.ToDisplayString() != dependencyResolverName))
                        continue;

                    var @interface = type.Interfaces.FirstOrDefault(i => i.Name == $"I{type.Name}");
                    if (@interface == null)
                        continue;

                    string interfaceNamespace = @interface.ContainingNamespace.ToString();
                    if (string.IsNullOrEmpty(interfaceNamespace))
                        continue;

                    string classNamespace = type.ContainingNamespace.ToString();
                    namespaces.Add(type.ContainingNamespace.ToString());

                    if (classNamespace != interfaceNamespace)
                        namespaces.Add(interfaceNamespace);

                    neededTypes.Add(type, _actions.Values.First());
                    foreach (var name in _actions.Keys)
                    {
                        if (type.AllInterfaces.Any(i => i.ToDisplayString() == name))
                        {
                            neededTypes[type] = _actions[name];
                            break;
                        }
                    }

                }

            }

            namespaces.Distinct().OrderBy(n => n.ToString()).ToList().ForEach(n => stringBuilder.Append($"    using {n};\n"));

            stringBuilder.Append(
                "    /// <summary>\n" +
                "    /// Service registrator class.\n" +
                "    /// </summary>\n" +
                "    public partial class " + mainMethod.ContainingType.Name + " \n" +
                "    {\n" +
                "        /// <summary>\n" +
                "        /// Register dependency injection instances.\n" +
                "        /// </summary>\n" +
                "        /// <param name=\"services\">Startup services.</param>\n" +
                "        /// <returns>The given <see cref=\"IServiceCollection\"/> instance.</returns>\n" +
                "        static partial void RegisterService(IServiceCollection services)\n" +
                "        {\n");

            foreach (var type in neededTypes.Keys)
            {
                stringBuilder.Append($"            services.{neededTypes[type]}<I{type.Name}, {type.Name}>();");
                stringBuilder.AppendLine();
            }

            stringBuilder.Append("            //return services;\n" +
                "        }\n" +
                "    }\n" +
                "}\n");

            context.Compilation.AddReferences(references);

            context.AddSource("ServicesRegistrator", SourceText.From(stringBuilder.ToString(), Encoding.UTF8));
        }

        IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation) =>
            GetAllTypes(compilation.GlobalNamespace);

        IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
                foreach (var nestedType in GetNestedTypes(type))
                    yield return nestedType;

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
                foreach (var type in GetAllTypes(nestedNamespace))
                    yield return type;
        }

        IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
        {
            yield return type;
            foreach (var nestedType in type.GetTypeMembers()
                .SelectMany(nestedType => GetNestedTypes(nestedType)))
                yield return nestedType;
        }

    }
}

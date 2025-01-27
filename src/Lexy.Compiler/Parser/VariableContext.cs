using System;
using System.Collections.Generic;
using Lexy.Compiler.Language;
using Lexy.Compiler.Language.VariableTypes;

namespace Lexy.Compiler.Parser;

public class VariableContext : IVariableContext
{
    private readonly IParserLogger logger;
    private readonly RootNodeList rootNodes;
    private readonly IVariableContext parentContext;
    private readonly IDictionary<string, VariableEntry> variables = new Dictionary<string, VariableEntry>();

    public VariableContext(RootNodeList rootNodes, IParserLogger logger, IVariableContext parentContext)
    {
        this.rootNodes = rootNodes ?? throw new ArgumentNullException(nameof(rootNodes));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.parentContext = parentContext;
    }

    public void AddVariable(string name, VariableType type, VariableSource source)
    {
        if (Contains(name)) return;

        var entry = new VariableEntry(type, source);
        variables.Add(name, entry);
    }

    public void RegisterVariableAndVerifyUnique(SourceReference reference, string name, VariableType type,
        VariableSource source)
    {
        if (Contains(name))
        {
            logger.Fail(reference, $"Duplicated variable name: '{name}'");
            return;
        }

        var entry = new VariableEntry(type, source);
        variables.Add(name, entry);
    }

    public bool Contains(string name)
    {
        return variables.ContainsKey(name) || parentContext != null && parentContext.Contains(name);
    }

    public bool Contains(VariablePath path, IValidationContext context)
    {
        var parent = GetVariable(path.ParentIdentifier);
        if (parent == null) return false;

        return !path.HasChildIdentifiers ||
               ContainChild(parent.VariableType, path.ChildrenReference(), context);
    }

    public VariableReference CreateVariableReference(SourceReference reference, VariablePath path, IValidationContext validationContext) {
        var rootVariableType = rootNodes.GetType(path.ParentIdentifier);
        if (rootVariableType != null) {
            return this.CreateTypeVariableReference(reference, path, rootVariableType, validationContext);
        }
        return CreateVariableReferenceFromRegisteredVariables(path, reference, validationContext);
    }

    private VariableReference CreateVariableReferenceFromRegisteredVariables(VariablePath path,
        SourceReference reference, IValidationContext validationContext) {
        var variable = GetVariable(path.ParentIdentifier);
        if (variable == null) {
            logger.Fail(reference, $"Unknown variable name: '{path.FullPath()}'");
            return null;
        }
        var variableType = GetVariableType(path, validationContext);
        if (variableType == null) {
            logger.Fail(reference, $"Unknown variable name: '{path.FullPath()}'");
            return null;
        }
        return new VariableReference(path, null, variableType, variable.VariableSource);
    }

    private VariableReference CreateTypeVariableReference(SourceReference reference, VariablePath path,
        TypeWithMembers rootVariableType, IValidationContext validationContext) {
        if (path.Parts == 1) {
            return new VariableReference(path, rootVariableType, rootVariableType, VariableSource.Type);
        }
        var parentIdentifier = path.ParentIdentifier;
        if (path.Parts > 2) {
            logger.Fail(reference, $"Invalid member access '{path}'. Variable '{parentIdentifier}' not found.");
            return null;
        }
        var typeWithMembers = rootVariableType as ITypeWithMembers;
        if (typeWithMembers == null) {
            logger.Fail(reference, $"Invalid member access '{path}'. Variable '{parentIdentifier}' not found.");
            return null;
        }
        var member = path.LastPart();
        var memberType = typeWithMembers.MemberType(member, validationContext);
        if (memberType == null) {
            logger.Fail(reference,
                $"Invalid member access '{path}'. Member '{member}' not found on '{parentIdentifier}'.");
        }
        return new VariableReference(path, rootVariableType, memberType, VariableSource.Type);
    }

    public VariableType GetVariableType(string name)
    {
        return variables.TryGetValue(name, out var value)
            ? value.VariableType
            : parentContext?.GetVariableType(name);
    }

    public VariableType GetVariableType(VariablePath path, IValidationContext context)
    {
        if (path == null) throw new ArgumentNullException(nameof(path));

        var parent = GetVariableType(path.ParentIdentifier);
        return parent == null || !path.HasChildIdentifiers
            ? parent
            : GetVariableType(parent, path.ChildrenReference(), context);
    }

    public VariableEntry GetVariable(string name)
    {
        return variables.TryGetValue(name, out var value)
            ? value
            : parentContext?.GetVariable(name);
    }

    private bool ContainChild(VariableType parentType, VariablePath path, IValidationContext context)
    {
        var typeWithMembers = parentType as ITypeWithMembers;

        var memberVariableType = typeWithMembers?.MemberType(path.ParentIdentifier, context);
        if (memberVariableType == null) return false;

        return !path.HasChildIdentifiers
               || ContainChild(memberVariableType, path.ChildrenReference(), context);
    }

    private VariableType GetVariableType(VariableType parentType, VariablePath path,
        IValidationContext context)
    {
        if (parentType is not ITypeWithMembers typeWithMembers) return null;

        var memberVariableType = typeWithMembers.MemberType(path.ParentIdentifier, context);
        if (memberVariableType == null) return null;

        return !path.HasChildIdentifiers
            ? memberVariableType
            : GetVariableType(memberVariableType, path.ChildrenReference(), context);
    }
}
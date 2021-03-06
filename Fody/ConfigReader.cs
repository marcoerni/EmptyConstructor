﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public XElement Config { get; set; }
    public List<string> IncludeNamespaces = new List<string>();
    public List<string> ExcludeNamespaces = new List<string>();

    public void ReadConfig()
    {
        if (Config == null)
        {
            return;
        }

        ReadVisibility();
        ReadExcludes();
        ReadIncludes();

        if (IncludeNamespaces.Any() && ExcludeNamespaces.Any())
        {
            throw new WeavingException("Either configure IncludeNamespaces OR ExcludeNamespaces, not both.");
        }
    }


    void ReadVisibility()
    {
        var visibilityAttribute = Config.Attribute("Visibility");
        if (visibilityAttribute != null)
        {
            if (visibilityAttribute.Value == "public")
            {
                Visibility = MethodAttributes.Public;
                return;
            }
            if (visibilityAttribute.Value == "family")
            {
                Visibility = MethodAttributes.Family;
                return;
            }
            var message = $"Could not convert '{visibilityAttribute.Value}' to a visibility. Only 'public' or 'family' are allowed.";
            throw new WeavingException(message);
        }
    }
    void ReadExcludes()
    {
        var excludeNamespacesAttribute = Config.Attribute("ExcludeNamespaces");
        if (excludeNamespacesAttribute != null)
        {
            foreach (var item in excludeNamespacesAttribute.Value.Split('|').NonEmpty())
            {
                ExcludeNamespaces.Add(item);
            }
        }

        var excludeNamespacesElement = Config.Element("ExcludeNamespaces");
        if (excludeNamespacesElement != null)
        {
            foreach (var item in excludeNamespacesElement.Value
                                                         .Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                                                         .NonEmpty())
            {
                ExcludeNamespaces.Add(item);
            }
        }
    }

    void ReadIncludes()
    {
        var includeNamespacesAttribute = Config.Attribute("IncludeNamespaces");
        if (includeNamespacesAttribute != null)
        {
            foreach (var item in includeNamespacesAttribute.Value.Split('|').NonEmpty())
            {
                IncludeNamespaces.Add(item);
            }
        }

        var includeNamespacesElement = Config.Element("IncludeNamespaces");
        if (includeNamespacesElement != null)
        {
            foreach (var item in includeNamespacesElement.Value
                                                         .Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries)
                                                         .NonEmpty())
            {
                IncludeNamespaces.Add(item);
            }
        }
    }
}
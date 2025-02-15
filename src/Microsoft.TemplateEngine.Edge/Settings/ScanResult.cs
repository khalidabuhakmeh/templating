// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.TemplateEngine.Abstractions;

namespace Microsoft.TemplateEngine.Edge.Settings
{
    /// <summary>
    /// Returned by <see cref="Scanner.Scan"/>.
    /// </summary>
    public class ScanResult
    {
        public static readonly ScanResult Empty = new(
            string.Empty,
            Array.Empty<ITemplate>(),
            Array.Empty<ILocalizationLocator>(),
            Array.Empty<(string AssemblyPath, Type InterfaceType, IIdentifiedComponent Instance)>()
            );

        public ScanResult(
            string mountPointUri,
            IReadOnlyList<ITemplate> templates,
            IReadOnlyList<ILocalizationLocator> localizations,
            IReadOnlyList<(string AssemblyPath, Type InterfaceType, IIdentifiedComponent Instance)> components
            )
        {
            MountPointUri = mountPointUri;
            Templates = templates;
            Localizations = localizations;
            Components = components;
        }

        /// <summary>
        /// Uri of mountpoint that was scanned.
        /// </summary>
        public string MountPointUri { get; }

        /// <summary>
        /// All components found inside mountpoint.
        /// AssemblyPath is full path inside <see cref="Abstractions.Mount.IMountPoint"/>.
        /// InterfaceType is type of interface that is implemented by component.
        /// Instance is object that implements InterfaceType.
        /// </summary>
        public IReadOnlyList<(string AssemblyPath, Type InterfaceType, IIdentifiedComponent Instance)> Components { get; }

        /// <summary>
        /// All template localizations found inside mountpoint.
        /// </summary>
        public IReadOnlyList<ILocalizationLocator> Localizations { get; }

        /// <summary>
        /// All templates found inside mountpoint.
        /// </summary>
        public IReadOnlyList<ITemplate> Templates { get; }
    }
}

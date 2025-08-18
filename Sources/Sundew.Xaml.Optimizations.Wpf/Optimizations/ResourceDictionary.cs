// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDictionary.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SystemResourceDictionary = System.Windows.ResourceDictionary;

/// <summary>
/// A ResourceDictionary that ensures that a source is only loaded once and otherwise retrieved from a cache.
/// </summary>
/// <seealso cref="SystemResourceDictionary" />
public sealed class ResourceDictionary : SystemResourceDictionary
{
    private static readonly object LockObject = new object();
    private static readonly ConcurrentDictionary<Uri, Entry> ResourceDictionaries = new ConcurrentDictionary<Uri, Entry>();
    private Entry? entry;

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    public new Uri? Source
    {
        get => this.entry?.SourceResourceDictionary.Source;

        set
        {
            if (this.Source == value)
            {
                return;
            }

            if (this.Source != null)
            {
                if (this.entry != null)
                {
                    this.MergedDictionaries.Remove(this.entry.SourceResourceDictionary);
                    lock (LockObject)
                    {
                        this.entry.ReferencingResourceDictionaries.Remove(this);
                        if (this.IsFirstSourceReference)
                        {
                            var newFirstSource = this.entry.ReferencingResourceDictionaries.FirstOrDefault();
                            if (newFirstSource != null)
                            {
                                this.entry.FirstReferencingResourceDictionary = newFirstSource;
                            }
                            else
                            {
                                ResourceDictionaries.TryRemove(this.Source, out _);
                            }
                        }
                    }
                }
            }

            if (value != null)
            {
                this.entry = ResourceDictionaries.AddOrUpdate(
                    value,
                    uri =>
                    {
                        var actualResourceDictionary = new SystemResourceDictionary { Source = value };
                        this.MergedDictionaries.Add(actualResourceDictionary);
                        var newEntry = new Entry(actualResourceDictionary, this);
                        return newEntry;
                    }, (uri, existingEntry) =>
                    {
                        lock (LockObject)
                        {
                            existingEntry.ReferencingResourceDictionaries.Add(this);
                        }

                        this.MergedDictionaries.Add(existingEntry.SourceResourceDictionary);
                        return existingEntry;
                    });
            }
        }
    }

    internal static IDictionary CachedDictionaries => ResourceDictionaries;

    internal bool IsFirstSourceReference => ReferenceEquals(this.entry?.FirstReferencingResourceDictionary, this);

    /// <summary>
    /// Tries to remove a resources by <see cref="Uri"/> from the cache, if there are no references.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns><c>true</c>, if the item could be removed, otherwise <c>false</c>.</returns>
    public static bool TryRemoveFromCache(Uri source)
    {
        if (ResourceDictionaries.TryGetValue(source, out var entry))
        {
            lock (LockObject)
            {
                if (entry.ReferencingResourceDictionaries.Count == 0)
                {
                    return ResourceDictionaries.TryRemove(source, out _);
                }

                var onlyReferencingResourceDictionary = entry.ReferencingResourceDictionaries.SingleOrDefault();
                if (ReferenceEquals(onlyReferencingResourceDictionary, entry.FirstReferencingResourceDictionary))
                {
                    return ResourceDictionaries.TryRemove(source, out _);
                }
            }
        }

        return false;
    }

    private class Entry
    {
        public Entry(SystemResourceDictionary sourceResourceDictionary, ResourceDictionary firstReferencingResourceDictionary)
        {
            this.SourceResourceDictionary = sourceResourceDictionary;
            this.FirstReferencingResourceDictionary = firstReferencingResourceDictionary;
            this.ReferencingResourceDictionaries = new List<ResourceDictionary> { firstReferencingResourceDictionary };
        }

        public SystemResourceDictionary SourceResourceDictionary { get; }

        public ResourceDictionary FirstReferencingResourceDictionary { get; set; }

        public List<ResourceDictionary> ReferencingResourceDictionaries { get; }
    }
}
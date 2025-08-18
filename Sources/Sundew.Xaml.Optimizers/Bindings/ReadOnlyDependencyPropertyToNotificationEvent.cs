// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyDependencyPropertyToNotificationEvent.cs" company="Sundews">
// Copyright (c) Sundews. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sundew.Xaml.Optimizations.Bindings;

/// <summary>Contains information on how a readonly dependency property can be notified by an event.</summary>
public class ReadOnlyDependencyPropertyToNotificationEvent
{
    /// <summary>Initializes a new instance of the <see cref="ReadOnlyDependencyPropertyToNotificationEvent"/> class.</summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="namespaceQualifiedDelegate">The namespace qualified delegate.</param>
    public ReadOnlyDependencyPropertyToNotificationEvent(string eventName, string namespaceQualifiedDelegate)
    {
        this.EventName = eventName;
        this.NamespaceQualifiedDelegate = namespaceQualifiedDelegate;
    }

    /// <summary>Gets the name of the event.</summary>
    /// <value>The name of the event.</value>
    public string EventName { get; }

    /// <summary>Gets the namespace qualified delegate.</summary>
    /// <value>The namespace qualified delegate.</value>
    public string NamespaceQualifiedDelegate { get; }
}
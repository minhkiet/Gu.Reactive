namespace Gu.Reactive.PropertyPathStuff
{
    using System.Reflection;

    /// <summary>
    /// A wrapper around a delegate created from a <see cref="PropertyInfo.GetMethod"/>
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The property type.</typeparam>
    public abstract class Getter<TSource, TValue> : IGetter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Getter{TSource, TValue}"/> class.
        /// </summary>
        protected Getter(PropertyInfo property)
        {
            this.Property = property;
        }

        /// <summary>
        /// The <see cref="PropertyInfo"/> that this instance is for.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        object IGetter.GetValue(object source) => this.GetValue((TSource)source);

        /// <summary>
        /// Get the value of the property for <paramref name="source"/>
        /// </summary>
        public abstract TValue GetValue(TSource source);

        /// <inheritdoc/>
        public override string ToString() => $"{this.Property.ReflectedType?.Name}.{this.Property.Name}";
    }
}
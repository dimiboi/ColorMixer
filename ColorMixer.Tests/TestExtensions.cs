using FluentAssertions;
using System.ComponentModel;

namespace ColorMixer.Tests
{
    public static class TestExtensions
    {
        // Gets a property using reflection
        public static object GetProperty<T>(this T obj, string property)
            => obj.GetType()
                  .GetProperty(property)
                  .GetValue(obj, null);

        // Sets a property using reflection
        public static void SetProperty<T>(this T obj, string property, object value)
            => obj.GetType()
                  .GetProperty(property)
                  .SetValue(obj, value);

        public static void ShouldSetProperty<TClass, TProp>(this TClass obj, string property,
                                                                 TProp initial, TProp expected)
            where TClass : class, INotifyPropertyChanged
        {
            // Arrange

            var raised = default(string);

            obj.SetProperty(property, initial);
            obj.PropertyChanged += (s, e) => raised = e.PropertyName;

            // Act

            obj.SetProperty(property, expected);

            // Assert

            obj.GetProperty(property).Should().Be(expected);
            raised.Should().Be(property);
        }
    }
}
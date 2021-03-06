using System.Collections.Generic;
using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests
{
    public class LocalizedModelsDiscoveryTests
    {
        private readonly IEnumerable<DiscoveredResource> _properties;

        public LocalizedModelsDiscoveryTests()
        {
            var types = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>().ToList();

            Assert.NotEmpty(types);

            _properties = types.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false));
        }

        [Fact]
        public void SingleLevel_ScalarProperties_NoAttribute()
        {
            var simpleProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty");
            Assert.NotNull(simpleProperty);

            var ignoredProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.IgnoredProperty");
            Assert.Null(ignoredProperty);

            Assert.Equal("SampleProperty", simpleProperty.Translation);

            var simplePropertyWithDefaultValue = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty2");
            Assert.NotNull(simplePropertyWithDefaultValue);
            Assert.Equal("This is Display value", simplePropertyWithDefaultValue.Translation);

            var simplePropertyRequired = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-Required");
            Assert.NotNull(simplePropertyRequired);
            Assert.Equal("SampleProperty-Required", simplePropertyRequired.Translation);

            var simplePropertyStringLength = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.SampleProperty-StringLength");
            Assert.NotNull(simplePropertyStringLength);

            var subProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SubViewModel.AnotherProperty");
            Assert.NotNull(subProperty);

            var includedSubProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.ComplexIncludedProperty");
            Assert.NotNull(includedSubProperty);

            var nonExistingEmailDataTypeResource = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.Email-DataTypeEmailAddress");
            Assert.NotNull(nonExistingEmailDataTypeResource);

            var nullable = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.NullableInt");
            Assert.NotNull(nullable);
        }

        [Fact]
        public void EnumType_CheckDiscovered_Found()
        {
            var enumProperty = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.DocumentEntity.Status");
            Assert.NotNull(enumProperty);
        }

        [Fact]
        public void PropertyWithAttributes_DisplayDescription_Discovered()
        {
            var resource = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.PropertyWithDescription");
            Assert.NotNull(resource);

            var propertyWithDescriptionResource = _properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.SampleViewModel.PropertyWithDescription-Description");
            Assert.NotNull(propertyWithDescriptionResource);
        }
    }
}

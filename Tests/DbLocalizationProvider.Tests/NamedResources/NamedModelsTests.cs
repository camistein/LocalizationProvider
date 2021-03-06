using System.Linq;
using DbLocalizationProvider.Sync;
using Xunit;

namespace DbLocalizationProvider.Tests.NamedResources
{
    public class NamedModelsTests
    {
        [Fact]
        public void MultipleAttributesForSingleProperty_NoPrefix()
        {
            var model = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ModelWithNamedProperties)}");

            var properties = model.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false)).ToList();

            var nonexistingProperty = properties.FirstOrDefault(p => p.Key == "DbLocalizationProvider.Tests.NamedResources.ModelWithNamedProperties.PageHeader");
            Assert.Null(nonexistingProperty);

            var namedProperty = properties.FirstOrDefault(p => p.Key == "/this/is/xpath/key");
            Assert.NotNull(namedProperty);
            Assert.Equal("This is page header", namedProperty.Translation);

            var anotherNamedProperty = properties.FirstOrDefault(p => p.Key == "/this/is/another/xpath/key");
            Assert.NotNull(anotherNamedProperty);

            var resourceKeyOnComplexProperty = properties.FirstOrDefault(p => p.Key == "/this/is/complex/type");
            Assert.NotNull(resourceKeyOnComplexProperty);

            var propertyWithDisplayName = properties.FirstOrDefault(p => p.Key == "/simple/property/with/display/name");
            Assert.NotNull(propertyWithDisplayName);
            Assert.Equal("This is simple property", propertyWithDisplayName.Translation);
        }

        [Fact]
        public void SingleAttributeForSingleProperty_WithClassPrefix()
        {
            var model = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ModelWithNamedPropertiesWithPrefix)}");

            var properties = model.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false)).ToList();

            var name = "/contenttypes/modelwithnamedpropertieswithprefix/properties/pageheader/name";
            var headerProperty = properties.FirstOrDefault(p => p.Key == name);

            Assert.NotNull(headerProperty);
            Assert.Equal("This is page header", headerProperty.Translation);
        }

        [Fact]
        public void MultipleAttributeForSingleProperty_WithClassPrefix()
        {
            var model = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ModelWithNamedPropertiesWithPrefix)}");

            var properties = model.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false)).ToList();

            var firstResource = properties.FirstOrDefault(p => p.Key == "/contenttypes/modelwithnamedpropertieswithprefix/resource1");

            Assert.NotNull(firstResource);
            Assert.Equal("1st resource", firstResource.Translation);

            var secondResource = properties.FirstOrDefault(p => p.Key == "/contenttypes/modelwithnamedpropertieswithprefix/resource2");

            Assert.NotNull(secondResource);
            Assert.Equal("2nd resource", secondResource.Translation);
        }

        [Fact]
        public void ResourceAttributeToClass_WithClassPrefix()
        {
            var model = TypeDiscoveryHelper.GetTypesWithAttribute<LocalizedModelAttribute>()
                                           .Where(t => t.FullName == $"DbLocalizationProvider.Tests.NamedResources.{nameof(ModelWithNamedPropertiesWithPrefixAndKeyOnClass)}");

            var properties = model.SelectMany(t => TypeDiscoveryHelper.GetAllProperties(t, contextAwareScanning: false)).ToList();

            var firstResource = properties.FirstOrDefault(p => p.Key == "/contenttypes/modelwithnamedpropertieswithprefixandkeyonclass/name");
            Assert.NotNull(firstResource);

            var secondResource = properties.FirstOrDefault(p => p.Key == "/contenttypes/modelwithnamedpropertieswithprefixandkeyonclass/description");
            Assert.NotNull(secondResource);

            var thirdResource = properties.FirstOrDefault(p => p.Key == "/contenttypes/modelwithnamedpropertieswithprefixandkeyonclass/properties/pageheader/caption");
            Assert.NotNull(thirdResource);
        }
    }
}

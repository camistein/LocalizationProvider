﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DbLocalizationProvider.Queries;

namespace DbLocalizationProvider
{
    public class LocalizationProvider
    {
        public static LocalizationProvider Current { get; } = new LocalizationProvider();

        public virtual string GetString(string resourceKey)
        {
            return GetString(resourceKey, CultureInfo.CurrentUICulture);
        }

        public virtual string GetString(string resourceKey, CultureInfo culture)
        {
            return GetStringByCulture(resourceKey, culture, null);
        }

        public virtual string GetStringByCulture(Expression<Func<object>> resource, CultureInfo culture, params object[] formatArguments)
        {
            if(resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            var resourceKey = ExpressionHelper.GetFullMemberName(resource);
            return GetStringByCulture(resourceKey, culture, formatArguments);
        }

        public virtual string GetStringByCulture(string resourceKey, CultureInfo culture, params object[] formatArguments)
        {
            var q = new GetTranslation.Query(resourceKey, culture);
            var resourceValue = q.Execute();

            if(resourceValue == null)
                return null;

            try
            {
                return Format(resourceValue, formatArguments);
            }
            catch (Exception)
            {
                // TODO: log
            }

            return resourceValue;
        }

        internal static string Format(string message, params object[] formatArguments)
        {
            if(formatArguments == null || !formatArguments.Any())
            {
                return message;
            }

            // check if first element is not scalar - format with named placeholders
            var first = formatArguments.First();
            return !first.GetType().IsSimpleType()
                       ? FormatWithAnonymousObject(message, first)
                       : string.Format(message, formatArguments);
        }

        private static string FormatWithAnonymousObject(string message, object model)
        {
            var type = model.GetType();
            if(type == typeof(string))
            {
                return string.Format(message, model);
            }

            var placeHolders = Regex.Matches(message, "{.*?}").Cast<Match>().Select(m => m.Value).ToList();

            if(!placeHolders.Any())
            {
                return message;
            }

            var placeholderMap = new Dictionary<string, object>();
            var properties = type.GetProperties();

            foreach (var placeHolder in placeHolders)
            {
                var propertyInfo = properties.FirstOrDefault(p => p.Name == placeHolder.Trim('{', '}'));

                // property found - extract value and add to the map
                var val = propertyInfo?.GetValue(model);
                if(val != null)
                {
                    placeholderMap.Add(placeHolder, val);
                }
            }

            return placeholderMap.Aggregate(message, (current, pair) => current.Replace(pair.Key, pair.Value.ToString()));
        }
    }
}

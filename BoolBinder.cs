﻿using System;
using System.Web.Mvc;

namespace UserPlus.App.Models
{
    public class BoolModelBinder : IModelBinder
    {
        public BoolModelBinder() { }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Throw if null
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext");

            // Get value
            bool? valueAttempt = GetBool(bindingContext);

            // Return value
            return valueAttempt;
        }

        private bool? GetBool(ModelBindingContext bindingContext)
        {
            // Get value
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            // Return null
            if (valueResult == null)
            {
                if (bindingContext.ModelType == typeof(bool))
                    return false;
                else
                    return null;
            }

            // Get lower case value
            var lowerCaseValue = valueResult.AttemptedValue == null ? "" : valueResult.AttemptedValue.ToLower();

            // Check value
            if (lowerCaseValue == "on" || lowerCaseValue == "true")
                return true;
            else
                return false;
        }
    }
}
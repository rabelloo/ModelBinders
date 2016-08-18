using System;
using System.Web.Mvc;

namespace UserPlus.App.Models
{
    public class DateAndTimeModelBinder : IModelBinder
    {
        public DateAndTimeModelBinder() { }
 
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }
 
            // Try to get whole datetime
            DateTime? dateTimeAttempt = GetA<DateTime>(bindingContext, "-DateTime");

            // Return UTC Date when it has value
            if (dateTimeAttempt != null)
                return dateTimeAttempt.Value.ToUtcDate();
 
            // Default to "" when unset
            if (string.IsNullOrEmpty(this.DateSuffix))
                this.DateSuffix = "-Date";

            // Default to "time" when unset
            if (string.IsNullOrEmpty(this.TimeSuffix))
                this.TimeSuffix = "-Time";
 
            // Get Date and Time separately
            DateTime? dateAttempt = GetA<DateTime>(bindingContext, this.DateSuffix);
            DateTime? timeAttempt = GetA<DateTime>(bindingContext, this.TimeSuffix);
 
            // Return combined UTC Date when both have value
            if (dateAttempt != null && timeAttempt != null)
            {
                return new DateTime(
                                dateAttempt.Value.Year,
                                dateAttempt.Value.Month,
                                dateAttempt.Value.Day,
                                timeAttempt.Value.Hour,
                                timeAttempt.Value.Minute,
                                timeAttempt.Value.Second
                            )
                            // Convert to UTC!
                            .ToUtcDate();
            }

            // Return UTC Date or Time when only one has value
            if (dateAttempt != null)
                return dateAttempt.Value.ToUtcDate();
            if (timeAttempt != null)
                return timeAttempt.Value.ToUtcDate();

            // Return null when nothing else works
            return null;
        }
 
        private Nullable<T> GetA<T>(ModelBindingContext bindingContext, string suffix) where T : struct
        {
            if (string.IsNullOrEmpty(suffix))
                return null;

            // Try it with the prefix
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + suffix);

            // Try without the prefix if null
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
                valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            // If still null return null
            if (valueResult == null)
                return null;

            // Return value
            return (Nullable<T>)valueResult.ConvertTo(typeof(T), System.Globalization.CultureInfo.CurrentUICulture);
        }

        public string DateSuffix { get; set; }
        public string TimeSuffix { get; set; }
    }
 
    public class DateAndTimeAttribute : CustomModelBinderAttribute
    {
        private IModelBinder _binder;
 
        // The user cares about a full date structure and full
        // time structure, or one or the other.
        public DateAndTimeAttribute(string date, string time)
        {
            _binder = new DateAndTimeModelBinder
            {
                DateSuffix = date,
                TimeSuffix = time
            };
        }
 
        public override IModelBinder GetBinder() { return _binder; }
    }
}